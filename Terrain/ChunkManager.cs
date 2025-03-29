using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEditor.iOS;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ChunkManager : MonoBehaviour, IResourceHandler
{
    string ChunkDataPath => GetChunkDataPath(bossAreaManager.AreaDirectoryPath, ChunkPos);
    public ResourceManager.ChunkID ID { get; private set; }
    [SerializeField] AreaManager bossAreaManager;
    public AreaManager BossAreaManager { get => bossAreaManager; private set => bossAreaManager = value; }

    [SerializeField] Vector2Int chunkPos; // マップ作成時にも使うためInspectorで編集できるようにする
    /// <summary>
    /// このまねじゃのチャンク位置
    /// </summary>
    public Vector2Int ChunkPos { get => chunkPos; private set => chunkPos = value; }

    /// <summary>
    /// このまねじゃの部下みたいなものたち
    /// </summary>
    [SerializeField] List<IChunkHandler> Handlers = new();


    /// ここら辺全部消す。
    public static readonly string WorldDataDirectoryPath = "WorldData/";
    public static string InitialChunkDataDirectoryPath => $"{WorldDataDirectoryPath}InitialChunks/";
    /// <summary>
    /// このチャンクが生成しうるChunkDataの種類
    /// </summary>
    protected List<string> InitialChunkDataPaths => new()
    {
        $"{InitialChunkDataDirectoryPath}defaultChunk.json",
    };
    /// ここまで

    Vector3Int ChunkSize => BossAreaManager.BossTerrainManager.ChunkSize;

    public void Init(AreaManager areaManager)
    {
        BossAreaManager = areaManager;
    }

    public static string GetChunkDataPath(string areaDirectoryPath, Vector2Int chunkPos)
    {
        return Path.Combine(areaDirectoryPath, $"Chunk_{chunkPos.x}x{chunkPos.y}");
    }

    /// <summary>
    /// チャンクを生成。
    /// </summary>
    /// <param name="chunkPos"></param>
    /// <returns>チャンクの生成に成功したかどうか。</returns>
    public bool Generate(Vector2Int chunkPos)
    {
        // Debug.Log("ChunkManager.Generate was called");
        SetChunkPos(chunkPos);
        ChunkData chunkData;
        // Debug.Log($"Chunk at {ChunkPos} was Activated");
        if (File.Exists(ChunkDataPath))
        {
            chunkData = ApplicationManager.LoadCompressedJson<ChunkData>(ChunkDataPath);
            // Debug.Log("chunk was generated");
        }
        else
            chunkData = GetInitChunkData();
        ApplyChunkData(chunkData);

        Handlers.ForEach(a => a.OnChunkGenerate());
        return true;
    }

    /// <summary>
    /// このchunkで最初に生成するチャンクを返す。そのうちResourceManagerにChunk取得機能を実装する予定。そのとき改良する。
    /// </summary>
    /// <returns></returns>
    protected ChunkData GetInitChunkData()
    {
        string initialChunkPath = InitialChunkDataPaths[Math.Abs(HashCode.Combine(GameManager.Instance.Seed, ChunkPos)) % InitialChunkDataPaths.Count];

        if (File.Exists(initialChunkPath))
            return ConvertFromJson<ChunkData>(File.ReadAllText(initialChunkPath));
        else return null;
    }

    public ChunkData Deactivate()
    {
        ResourceManager.Release(ID, gameObject);

        new List<IChunkHandler>(Handlers).ForEach(a => a.OnChunkDeactivate());

        return MakeChunkData();
    }

    /// <summary>
    /// チャンクデータをもとにチャンクを作成
    /// </summary>
    /// <param name="chunkPos"></param>
    /// <returns></returns>
    public bool Activate(Vector2Int chunkPos, ChunkData chunkData)
    {
        SetChunkPos(chunkPos);

        ApplyChunkData(chunkData);

        Handlers.ForEach(a => a.OnChunkActivate());
        return true;
    }

    /// <summary>
    /// ChunkPosをセットし，対応する場所へこのオブジェクトを移動させる。
    /// </summary>
    /// <param name="chunkPos"></param>
    void SetChunkPos(Vector2Int chunkPos)
    {
        ChunkPos = chunkPos;
        transform.position = new(ChunkSize.x * ChunkPos.x, ChunkSize.y * ChunkPos.y, 0);
    }

    /// <summary>
    /// AreaManagerがリセットされたときに呼び出される
    /// </summary>
    public void Reset()
    {
        Handlers.ForEach(a => a.OnChunkReset());
        Handlers.Clear();
    }

    /// <summary>
    /// tileをセットするときは必ずしもこれを使わないといけないわけではない。ただし，baseTileを継承したtileしか設置しちゃダメ。
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="baseTile"></param>
    public void SetTile(Vector3Int pos, BaseTile baseTile)
    {
        TerrainManager.Instance.TerrainTilemap.SetTile(pos, baseTile);
    }

    /// <summary>
    /// tileを消すときは必ずこれを使用しなければならない
    /// </summary>
    /// <param name="gm"></param>
    public void DeleteTile(TileObjManager gm)
    {
        ResourceManager.Release(gm.ID, gm.gameObject);
        TerrainManager.Instance.TerrainTilemap.SetTile(gm.Position, null);
        Handlers.Remove(gm);
    }

    ///////////////// ここから削除予定
    /// <summary>
    /// Jsonへの変換をまとめるためのメソッド
    /// </summary>
    /// <returns></returns>
    string ConvertToJson(ChunkData chunkData)
    {
        return JsonUtility.ToJson(chunkData);
    }

    T ConvertFromJson<T>(string content)
    {
        return JsonUtility.FromJson<T>(content);
    }
    ////////////////// ここまで

    /// <summary>
    /// チャンクデータを作成。
    /// </summary>
    /// <returns></returns>
    public ChunkData MakeChunkData()
    {
        Vector3Int pos = new((int)(ChunkSize.x * (ChunkPos.x - 0.5f)), (int)(ChunkSize.y * (ChunkPos.y - 0.5f)), 0);

        var tiles = TerrainManager.Instance.TerrainTilemap?.GetTilesBlock(new(pos, ChunkSize));
        ResourceManager.TileID[] tileIDs = new ResourceManager.TileID[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            if (tile is BaseTile baseTile)
            {
                tileIDs[i] = baseTile.ID;
            }
        }

        ChunkData chunkData = new();

        foreach (var handler in Handlers)
        {
            if (handler is NPCManager npc)
            {
                if (npc.Party != null)
                {
                    if (npc.IsLeader)
                        chunkData.Partys.Add(npc.Party.MakePartyData());
                }
                else
                {
                    chunkData.NPCs.Add(npc.MakeNPCData());
                }
            }
            else if (handler is MobManager mob)
            {
                chunkData.Mobs.Add(mob.MakeMobData());
            }
            else if (handler is ObjectManager item)
            {
                chunkData.Items.Add(item.MakeObjectData());
            }
        }
        chunkData.TileIDs = tileIDs;
        return chunkData;
    }

    /// <summary>
    /// ChunkPosに対応するBoundsIntを返す。
    /// </summary>
    /// <returns></returns>
    public BoundsInt GetBoundsInt()
    {
        Vector3Int pos = new((int)(ChunkSize.x * (ChunkPos.x - 0.5f)), (int)(ChunkSize.y * (ChunkPos.y - 0.5f)), 0);
        return new(pos, ChunkSize);
    }

    /// <summary>
    /// ChunkDataからチャンクを生成
    /// </summary>
    /// <param name="chunkData"></param>
    public void ApplyChunkData(ChunkData chunkData)
    {
        if (chunkData == null)
        {
            Debug.LogWarning("ChunkData is null");
            return;
        }
        // Debug.Log("ChunkData was generated");
        var tileIDs = chunkData.TileIDs;
        BaseTile[] tiles = new BaseTile[tileIDs.Length];
        for (int i = 0; i < tileIDs.Length; i++)
        {
            var tileID = tileIDs[i];
            if (tileID != ResourceManager.TileID.None)
            {
                tiles[i] = ResourceManager.GetTile(tileID);
            }
        }
        TerrainManager.Instance.TerrainTilemap.SetTilesBlock(GetBoundsInt(), tiles);

        foreach (var party in chunkData.Partys)
        {
            Party.SpawnParty(party);
        }
        foreach (var npc in chunkData.NPCs)
        {
            NPCManager.SpawnNPC(npc);
        }
        foreach (var mob in chunkData.Mobs)
        {
            MobManager.SpawnMob(mob);
        }
        foreach (var item in chunkData.Items)
        {
            ObjectManager.SpawnItem(item);
        }
    }

    /// <summary>
    /// すべてのチャンクデータを削除
    /// </summary>
    // public static void ClearAllChunkData()
    // {
    //     string[] jsonFiles = Directory.GetFiles(SavedChunkDataDirectoryPath, "*.json");

    //     // すべてのJSONファイルを削除
    //     foreach (string file in jsonFiles)
    //     {
    //         try
    //         {
    //             File.Delete(file); // ファイルを削除
    //             Debug.Log($"Deleted: {file}");
    //         }
    //         catch (IOException ex)
    //         {
    //             Debug.LogError($"Error deleting file {file}: {ex.Message}");
    //         }
    //     }
    // }

    /// <summary>
    /// このチャンクマネジャに触れたIChunkHandlerを持つコンポネントを勝手に登録させる。
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IChunkHandler handler))
        {
            handler.BossChunkManager?.Unregister(handler);
            Register(handler);
        }
    }
    void Register(IChunkHandler handler)
    {
        handler.BossChunkManager = this;
        Handlers.Add(handler);
        // if (handler is MonoBehaviour mono)
        //     mono.transform.SetParent(transform);
    }
    void Unregister(IChunkHandler handler)
    {
        handler.BossChunkManager = null;
        Handlers.Remove(handler);
    }

    [SerializeField] string createChunkDataDirectoryPath;
    [SerializeField] string chunkDataName;
    /// <summary>
    /// マップ作製用メソッド。子オブジェクトをChunkDataに記録しjsonファイルを作る。
    /// </summary>
    public void CreateChunkDataAsset()
    {
        Handlers = GetComponentsInChildren<IChunkHandler>().ToList();
        string path = Path.Combine(createChunkDataDirectoryPath, chunkDataName);
        ApplicationManager.SaveCompressedJson(path, MakeChunkData());
    }

    public void OnGet(int id)
    {
        ID = (ResourceManager.ChunkID)id;
    }
    public void OnRelease()
    {
        Handlers.Clear();
    }
}

