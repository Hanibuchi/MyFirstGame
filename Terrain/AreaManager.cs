using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading;
using MyGame;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class AreaManager : MonoBehaviour, IResourceHandler
{
    public string AreaDirectoryPath => Path.Combine(bossTerrainManager.TerrainDataDirectoryPath, ID.ToString());
    string AreaDataPath => Path.Combine(AreaDirectoryPath, ID.ToString());
    public ResourceManager.AreaID ID { get; private set; }
    [SerializeField] TerrainManager bossTerrainManager;

    public TerrainManager BossTerrainManager { get => bossTerrainManager; private set => bossTerrainManager = value; }

    public bool IsVisited { get; private set; }

    /// <summary>
    /// チャンク位置からChunkManagerを返す辞書
    /// </summary>
    readonly Dictionary<Vector2Int, ChunkManager> ChunkManagers = new();

    readonly Dictionary<Vector2Int, ChunkData> ChunkDatas = new();

    float HiringCostMean;
    float HiringCostStdDev;

    public void Init(TerrainManager terrainManager)
    {
        BossTerrainManager = terrainManager;
    }

    public void Save()
    {
        // ここでAreaDataを保存。

        foreach (var keyValue in ChunkManagers)
        {
            ChunkDatas[keyValue.Key] = keyValue.Value.MakeChunkData();
        }
        foreach (var keyValue in ChunkDatas)
        {
            ApplicationManager.SaveCompressedJson(ChunkManager.GetChunkDataPath(AreaDirectoryPath, keyValue.Key), keyValue.Value);
        }
    }

    public float GetHireAmount()
    {
        return GameManager.Randoms[GameManager.RandomNames.HireAmount].NormalDistribution() * HiringCostStdDev + HiringCostMean;
    }

    public bool Generate(Vector2Int pos)
    {
        ChunkManager cm = GetChunk(pos);
        if (cm == null)
            return false;

        ChunkManagers[pos] = cm;
        return cm.Generate(pos);
    }

    public bool Deactivate(Vector2Int pos)
    {
        ChunkManager cm = ChunkManagers.GetValueOrDefault(pos, null);
        if (cm == null)
            return false;

        ChunkDatas[pos] = cm.Deactivate();
        ChunkManagers[pos] = null;

        return true;
    }

    public bool Activate(Vector2Int pos)
    {
        // Debug.Log("area was activated");
        ChunkManager cm = GetChunk(pos);
        if (cm == null)
            return false;

        ChunkManagers[pos] = cm;

        return cm.Activate(pos, ChunkDatas[pos]);
    }

    /// <summary>
    /// チャンク位置に対応するChunkManagerを生成して返す。生成した後initするのを忘れそうなためわざわざメソッドにした。取得できなかったらnullを返す。
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    ChunkManager GetChunk(Vector2Int pos)
    {
        ChunkManager chunkManager = ResourceManager.Get(GetChunkID(pos)).GetComponent<ChunkManager>();

        chunkManager?.Init(this);
        return chunkManager;
    }

    /// <summary>
    /// posに対応するチャンクのIDを返す。ここで生成するチャンクを決める。
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    protected ResourceManager.ChunkID GetChunkID(Vector2Int pos)
    {
        return ResourceManager.ChunkID.DefaultChunk;
    }

    /// <summary>
    /// リセットする。地上へ帰ったときに呼び出される。
    /// </summary>
    public void Reset()
    {
        foreach (var kv in ChunkManagers)
        {
            kv.Value.Reset();
            ResourceManager.Release(GetChunkID(kv.Key), kv.Value.gameObject);
        }
        ChunkManagers.Clear();

        ChunkDatas.Clear();
    }

    public void OnGet(int id)
    {
        ID = (ResourceManager.AreaID)id;
    }
}
