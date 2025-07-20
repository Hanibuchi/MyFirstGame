using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading;
using MyGame;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SerializeManager))]
[JsonObject(MemberSerialization.OptIn)]
public class AreaManager : MonoBehaviour, IAreaManager, ISerializableComponent
{
    public string AreaDirectoryPath => Path.Combine(_terrainManager.TerrainDataDirectoryPath, AreaName);
    string AreaDataPath => Path.Combine(AreaDirectoryPath, AreaName);


    [JsonProperty]
    public bool IsVisited { get; private set; }

    [SerializeField] float HiringCostMean;
    [SerializeField] float HiringCostStdDev;


    public float GetHireAmount()
    {
        return Random.Randoms[RandomName.HireAmount.ToString()].NormalDistribution() * HiringCostStdDev + HiringCostMean;
    }


    [JsonProperty]
    [SerializeField] string _areaName = "DefaultArea"; // このAreaManagerが管理するエリア名
    public string AreaName => _areaName;

    private TerrainManager _terrainManager;
    private Dictionary<Vector2Int, IChunkManager> _activeChunkManagersInArea = new Dictionary<Vector2Int, IChunkManager>();

    public void Init(TerrainManager terrainManager)
    {
        _terrainManager = terrainManager;
        _terrainManager.RegisterAreaManager(_areaName, this);
    }

    public virtual JObject GetChunkData(Vector2Int chunkPos)
    {
        string chunkDataPath = ChunkManager.GetChunkFilePath(chunkPos);
        var chunkDataString = EditFile.ReadAndDecompressJson(chunkDataPath);
        JObject chunkData = null;
        if (chunkDataString != "")
            chunkData = EditFile.JsonToJObject(chunkDataString);
        else
        {
            List<JObject> subChunkDatas = new();
            GetSubChunkDatas(subChunkDatas, chunkPos);
            var cellSize = TerrainManager.Instance.TerrainTilemap.cellSize;
            chunkData = ChunkDataEditor.MergeSubChunkDatas(subChunkDatas, cellSize.x, cellSize.y);
        }
        Debug.Log($"chunkData: {chunkDataString} in AreaManager");

        return chunkData;
    }

    [SerializeField] List<RandomSubChunkDataEntry> _randomSubChunkDataEntries = new();
    void GetSubChunkDatas(List<JObject> subChunkDatas, Vector2Int chunkPos)
    {
        subChunkDatas ??= new();
        subChunkDatas.Clear();
        Vector2Int subChunkDiv = TerrainManager.Instance.SubChunkDivision;
        for (int i = 0; i < subChunkDiv.x; i++)
            for (int j = 0; j < subChunkDiv.y; j++)
            {
                // string subChunkName = GetRandomSubChunkDataName(chunkPos, new(i, j));
                string subChunkName = GetZigzagSubChunkDataName(chunkPos, new(i, j));
                string subChunkDataString = ResourceManager.Instance.GetChunkData(subChunkName);
                JObject subChunkData = null;
                if (subChunkDataString != "")
                    subChunkData = EditFile.JsonToJObject(subChunkDataString);
                if (subChunkData != null)
                    subChunkDatas.Add(subChunkData);
                else
                    subChunkDatas.Add(null); // ここ変えてもいい
            }
    }

    string GetRandomSubChunkDataName(Vector2Int chunkPos, Vector2Int subChunkPos)
    {
        if (_randomSubChunkDataEntries == null || _randomSubChunkDataEntries.Count == 0)
        {
            Debug.LogWarning("_subChunkDataEntries is empty");
            return "";
        }

        int hash = (GameManager.Instance.Seed.ToString() + chunkPos.ToString() + subChunkPos.ToString()).GetHashCode();
        System.Random rng = new(hash);
        float value = (float)rng.NextDouble();

        float totalWeight = 0f;
        foreach (var entry in _randomSubChunkDataEntries)
            totalWeight += entry.weight;

        float scaledValue = value * totalWeight;

        float cumulative = 0f;
        foreach (var entry in _randomSubChunkDataEntries)
        {
            cumulative += entry.weight;
            if (scaledValue <= cumulative)
                return entry.subChunkDataName;
        }

        return _randomSubChunkDataEntries[^1].subChunkDataName;
    }

    [SerializeField] List<VerticalSubChunkDataEntry> _verticalSubChunkDataEntries = new();
    [SerializeField] List<HorizontalSubChunkDataEntry> _horizontalSubChunkDataEntries = new();

    string GetZigzagSubChunkDataName(Vector2Int chunkPos, Vector2Int subChunkPos)
    {
        if (_verticalSubChunkDataEntries == null || _verticalSubChunkDataEntries.Count == 0 || _horizontalSubChunkDataEntries == null || _horizontalSubChunkDataEntries.Count == 0)
        {
            Debug.LogWarning("_zigzagChunkDataEntries is empty");
            return "";
        }

        var subChunkDiv = TerrainManager.Instance.SubChunkDivision;
        var globalSubChunkPos = new Vector2Int(chunkPos.x * subChunkDiv.x + subChunkPos.x, chunkPos.y * subChunkDiv.y + subChunkPos.y);
        Vector2Int masterGlobalSubChunkPos;

        bool isMaster = false;
        bool isVertical = false;
        switch (((globalSubChunkPos.x - globalSubChunkPos.y) % 4 + 4) % 4)
        {
            case 0: isMaster = false; isVertical = true; masterGlobalSubChunkPos = new(globalSubChunkPos.x, globalSubChunkPos.y + 1); break;
            case 1: isMaster = true; isVertical = true; masterGlobalSubChunkPos = globalSubChunkPos; break;
            case 2: isMaster = false; isVertical = false; masterGlobalSubChunkPos = new(globalSubChunkPos.x - 1, globalSubChunkPos.y); break;
            case 3: isMaster = true; isVertical = false; masterGlobalSubChunkPos = globalSubChunkPos; break;
            default: masterGlobalSubChunkPos = new(); break;
        }
        int hash = (GameManager.Instance.Seed.ToString() + masterGlobalSubChunkPos.ToString()).GetHashCode();
        System.Random rng = new(hash);
        float value = (float)rng.NextDouble();

        float totalWeight = 0f;
        if (isVertical)
            foreach (var entry in _verticalSubChunkDataEntries)
                totalWeight += entry.weight;
        else
            foreach (var entry in _horizontalSubChunkDataEntries)
                totalWeight += entry.weight;

        float scaledValue = value * totalWeight;

        float cumulative = 0f;
        if (isVertical)
            foreach (var entry in _verticalSubChunkDataEntries)
            {
                cumulative += entry.weight;
                if (scaledValue <= cumulative)
                {
                    if (isMaster)
                        return entry.upperSubChunkDataName;
                    else
                        return entry.lowerSubChunkDataName;
                }
            }
        else
            foreach (var entry in _horizontalSubChunkDataEntries)
            {
                cumulative += entry.weight;
                if (scaledValue <= cumulative)
                {
                    if (isMaster)
                        return entry.leftSubChunkDataName;
                    else
                        return entry.rightSubChunkDataName;
                }
            }

        return _verticalSubChunkDataEntries[^1].upperSubChunkDataName;
    }

    /// <summary>
    /// チャンクが生成されたときに呼ばれます。
    /// </summary>
    public void OnChunkGenerated(IChunkManager chunkManager, Vector2Int chunkPos)
    {
        if (!_activeChunkManagersInArea.ContainsKey(chunkPos))
        {
            _activeChunkManagersInArea.Add(chunkPos, chunkManager);
        }
    }

    /// <summary>
    /// チャンクが非アクティブ化されたときに呼ばれます。
    /// </summary>
    public void OnChunkDeactivated(IChunkManager chunkManager, Vector2Int chunkPos)
    {
        _activeChunkManagersInArea.Remove(chunkPos);
    }

    /// <summary>
    /// エリア全体のデータ（チャンク以外の情報など）を作成します。
    /// </summary>
    public virtual void Save()
    {
        EditFile.CompressAndSaveJson(AreaDataPath, GetComponent<SerializeManager>().SaveState().ToString());
    }
}

[Serializable]
public class RandomSubChunkDataEntry
{
    public string subChunkDataName;
    [Range(0f, 1f)]
    public float weight = 1f;
}

[Serializable]
public class VerticalSubChunkDataEntry
{
    public string upperSubChunkDataName;
    public string lowerSubChunkDataName;
    [Range(0f, 1f)]
    public float weight = 1f;
}

[Serializable]
public class HorizontalSubChunkDataEntry
{
    public string leftSubChunkDataName;
    public string rightSubChunkDataName;
    [Range(0f, 1f)]
    public float weight = 1f;
}