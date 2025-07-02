using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEditor.iOS;
using UnityEditor;
using Zenject;
using UnityEditor.SearchService;
using Newtonsoft.Json.Linq;

[RequireComponent(typeof(SerializeManager))]
[RequireComponent(typeof(PoolableResourceComponent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[JsonObject(MemberSerialization.OptIn)]
public class ChunkManager : MonoBehaviour, IChunkManager, ISerializableComponent
{
    [SerializeField] AreaManager _areaManager;
    public AreaManager AreaManager { get => _areaManager; private set => _areaManager = value; }
    [SerializeField] List<ChunkHandler> Handlers = new();


    Vector3Int ChunkSize => TerrainManager.Instance.ChunkSize;

    PoolableResourceComponent m_poolableResourceComponent;

    private void Awake()
    {
        if (!TryGetComponent(out m_poolableResourceComponent))
            Debug.LogWarning("m_poolableResourceComponent is null");
        m_poolableResourceComponent.ReleaseCallback += OnRelease;
    }

    public static string GetChunkDataPath(string areaDirectoryPath, Vector2Int chunkPos)
    {
        return Path.Combine(areaDirectoryPath, $"Chunk_{chunkPos.x}x{chunkPos.y}");
    }


    /// <summary>
    /// ChunkPosをセットし，対応する場所へこのオブジェクトを移動させる。
    /// </summary>
    /// <param name="chunkPos"></param>
    void SetChunkPos()
    {
        transform.position = new(ChunkSize.x * ChunkPos.x, ChunkSize.y * ChunkPos.y, 0);
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
    /// <param name="tm"></param>
    public void DeleteTile(TileObjManager tm)
    {
        tm.GetComponent<PoolableResourceComponent>().Release();
        TerrainManager.Instance.TerrainTilemap.SetTile(tm.Position, null);
        Handlers.Remove(tm.GetComponent<ChunkHandler>());
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
    /// このチャンクマネジャに触れたIChunkHandlerを持つコンポネントを勝手に登録させる。
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("trigger entered");
        if (other.gameObject.TryGetComponent(out ChunkHandler handler))
        {
            if (handler.ChunkManager != this)
            {
                handler.ChunkManager?.Unregister(handler);
                Register(handler);
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out ChunkHandler handler))
        {
            Unregister(handler);
        }
        // Debug.Log("trigger exit");
    }
    void Register(ChunkHandler handler)
    {
        // Debug.Log("Registered");
        Handlers.Add(handler);
        handler.OnRegistered(this);
    }
    void Unregister(ChunkHandler handler)
    {
        // Debug.Log("Unregistered");
        if (Handlers.Contains(handler))
        {
            handler.OnUnregistered();
            Handlers.Remove(handler);
        }
    }

    [SerializeField] string chunkAssetsDirectoryPath = "Assets/ChunkAssets";
    [SerializeField] string chunkAssetName = "ChunkAssetName.asset";
    /// <summary>
    /// マップ作製用メソッド。子オブジェクトをChunkDataに記録しjsonファイルを作る。
    /// </summary>
    public void CreateChunkAsset()
    {
        var handlers = GetComponentsInChildren<ChunkHandler>();
        Handlers.AddRange(handlers);
        string path = Path.Combine(chunkAssetsDirectoryPath, chunkAssetName);

        EditFile.SaveJson(path, MakeChunkData().ToString());

        // ScriptableObjectとして保存してた時の処理
        // var asset = ScriptableObject.CreateInstance<ChunkAsset>();
        // asset.chunkData = MakeChunkData();

        // AssetDatabase.CreateAsset(asset, path);

        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();

        // EditorUtility.FocusProjectWindow();
        // Selection.activeObject = asset;
    }

    public void OnRelease()
    {
        Handlers.Clear();
    }

















    [SerializeField] Vector2Int _chunkPos;
    public Vector2Int ChunkPos { get => _chunkPos; private set => _chunkPos = value; }

    /// <summary>
    /// チャンクが生成されたときに呼ばれます。
    /// </summary>
    public void OnGenerated(IAreaManager areaManager, Vector2Int chunkPos)
    {
        _areaManager = (AreaManager)areaManager;
        this.ChunkPos = chunkPos;
        this.gameObject.name = $"Chunk_{chunkPos.x}_{chunkPos.y}";
        SetChunkPos();
    }

    /// <summary>
    /// チャンクが非アクティブ化されたときに呼ばれます。
    /// </summary>
    public JObject OnDeactivated()
    {
        // 現在のチャンクの状態からChunkDataを作成して返す
        var currentChunkData = MakeChunkData();
        // チャンク内のGameObjectなどを非アクティブ化、またはプールに戻す
        ClearChunkContents();
        return currentChunkData;
    }

    // ここからデータ保存用。名前勝手に変えるとJsonふぁいるを直接編集してるところが使えなくなるからやめる。
    [JsonProperty] string[] tileIDs;
    [JsonProperty] List<(ResourceType type, string id, JObject objData)> objects = new();
    /// <summary>
    /// 現在のチャンクの状態からChunkDataを作成します。
    /// </summary>
    public JObject MakeChunkData()
    {
        return GetComponent<SerializeManager>().SaveState();
    }

    public void OnBeforeSerializeData()
    {
        var tiles = TerrainManager.Instance.GetTiles(ChunkPos);
        tileIDs = new string[tiles.Length];
        Debug.Log($"tiles.Length: {tiles.Length}");
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            if (tile is BaseTile baseTile)
            {
                tileIDs[i] = baseTile.ID;
            }
        }

        objects.Clear();
        foreach (var handler in Handlers)
        {
            if (handler != null && handler.TryGetComponent(out SerializeManager serializable) && handler.TryGetComponent(out PoolableResourceComponent poolable))
            {
                if (!handler.TryGetComponent(out TileObjManager tileObjManager))
                {
                    objects.Add((poolable.Type, poolable.ID, serializable.SaveState()));
                }
            }
        }
    }

    /// <summary>
    /// ChunkDataからチャンクを生成
    /// </summary>
    /// <param name="chunkData"></param>
    public void ApplyChunkData(JObject chunkData)
    {
        GetComponent<SerializeManager>().LoadState(chunkData);
    }

    public void OnAfterDeserializeData()
    {
        BaseTile[] tiles = new BaseTile[tileIDs.Length];
        for (int i = 0; i < tileIDs.Length; i++)
        {
            var tileID = tileIDs[i];
            if (tileID == null)
                tiles[i] = null;
            else
                tiles[i] = ResourceManager.Instance.GetTile(tileID);
        }
        TerrainManager.Instance.TerrainTilemap.SetTilesBlock(GetBoundsInt(), tiles);

        foreach (var obj in objects)
        {
            var spawndObj = ResourceManager.Instance.Get(obj.type, obj.id);
            if (spawndObj != null)
            {
                spawndObj.transform.SetParent(transform);
                spawndObj.GetComponent<SerializeManager>().LoadState(obj.objData);
            }
            else
            {
                Debug.LogWarning($"Handler with ID {obj.id} not found.");
            }
        }
    }

    /// <summary>
    /// チャンク内のコンテンツ（タイルやオブジェクト）をクリアします。
    /// </summary>
    private void ClearChunkContents()
    {
        foreach (var handler in Handlers.ToList())
        {
            if (handler == null)
                continue;
            if (handler.TryGetComponent(out TileObjManager tileObjManager))
            {
                DeleteTile(tileObjManager);
            }
            else
            {
                handler.GetComponent<PoolableResourceComponent>().Release();
            }
        }
    }


    /// <summary>
    /// チャンクデータのファイルパスを取得します。
    /// </summary>
    public static string GetChunkFilePath(Vector2Int chunkPos)
    {
        return Path.Combine(TerrainManager.Instance.TerrainDataDirectoryPath, "Chunk", $"chunk_{chunkPos.x}_{chunkPos.y}.json");
    }
}

