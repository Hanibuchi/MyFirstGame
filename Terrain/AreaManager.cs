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

    /// <summary>
    /// エリアに属するチャンクデータを取得します。ファイルからの読み込みを試みます。
    /// </summary>
    public virtual JObject GetChunkData(Vector2Int chunkPos)
    {
        string chunkDataPath = ChunkManager.GetChunkFilePath(chunkPos);
        var chunkData = EditFile.ReadAndDecompressJson(chunkDataPath);

        if (chunkData == "")
        {
            chunkData = ResourceManager.Instance.GetChunkData(ResourceManager.ChunkID.DefaultChunk.ToString());
        }
        Debug.Log($"chunkData: {chunkData} in AreaManager");

        return JObject.Parse(chunkData);
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
