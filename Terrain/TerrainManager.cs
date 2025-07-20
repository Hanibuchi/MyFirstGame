using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Reflection;
using MyGame;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using System.IO;
using Newtonsoft.Json.Linq;

public class TerrainManager : MonoBehaviour
{
    public string TerrainDataDirectoryPath => Path.Combine(GameManager.SaveSlotDirectoryPath, "TerrainData");
    public static TerrainManager Instance;
    /// <summary>
    /// TerrainのGrid
    /// </summary>
    [SerializeField] Grid _grid;
    [SerializeField] Tilemap _tileMap;
    /// <summary>
    /// Terrainが使うTilemap
    /// </summary>
    public Tilemap TerrainTilemap { get => _tileMap; }

    /// <summary>
    /// チャンク1つあたりの縦横のタイル数
    /// </summary>
    public Vector3Int ChunkSize { get; private set; } = new(128, 128, 1);
    public Vector2 CellSize => _grid.cellSize;
    public Vector2Int SubChunkDivision { get; private set; } = new(4, 4);


    Vector2? GetPlayerPos()
    {
        var player = GameManager.Instance?.Player;
        return player != null ? player.transform.position : null;
    }

    public (int, int) GetSubChunkSize()
    {
        int sizex, sizey, tmp;
        (tmp, sizex) = Functions.GetKthDivision(ChunkSize.x, SubChunkDivision.x, 0);
        (tmp, sizey) = Functions.GetKthDivision(ChunkSize.y, SubChunkDivision.y, 0);

        return (sizex, sizey);
    }

    /// <summary>
    /// チャンクを生成する範囲。Playerからの距離(u)であらわされる
    /// </summary>
    public Vector2 ChunkGenerateRange = new(64f, 64f);

    public TileBase[] GetTiles(Vector2Int chunkPos)
    {
        return TerrainTilemap?.GetTilesBlock(GetBoundsInt(chunkPos));
    }

    public TileBase[] GetTiles(Vector2Int chunkPos, Vector2Int subChunkPos)
    {
        return TerrainTilemap?.GetTilesBlock(GetBoundsInt(chunkPos, subChunkPos));
    }

    /// <summary>
    /// ChunkPosに対応するBoundsIntを返す。
    /// </summary>
    /// <returns></returns>
    public BoundsInt GetBoundsInt(Vector2Int chunkPos)
    {
        Vector3Int pos = new((int)(ChunkSize.x * (chunkPos.x - 0.5f)), (int)(ChunkSize.y * (chunkPos.y - 0.5f)), 0);
        return new(pos, ChunkSize);
    }

    public BoundsInt GetBoundsInt(Vector2Int chunkPos, Vector2Int subChunkPos)
    {
        int xOffset, yOffset, xSize, ySize;
        (xOffset, xSize) = Functions.GetKthDivision(ChunkSize.x, SubChunkDivision.x, subChunkPos.x);
        (yOffset, ySize) = Functions.GetKthDivision(ChunkSize.y, SubChunkDivision.y, subChunkPos.y);
        Vector3Int pos = new((int)(ChunkSize.x * (chunkPos.x - 0.5f)) + xOffset, (int)(ChunkSize.y * (chunkPos.y - 0.5f)) + yOffset, 0);
        Vector3Int subChunkSize = new(xSize, ySize, 1);
        return new(pos, subChunkSize);
    }

    /// <summary>
    /// 準備が整ったかどうか。Updateのエラーがうるさいからこうした。
    /// </summary>
    bool isReady;

    public void OnGameStart()
    {
        if (File.Exists(TerrainDataDirectoryPath))
        {
            TerrainData terrainData = EditFile.LoadCompressedJsonAsObject<TerrainData>(TerrainDataDirectoryPath);
        }

        Init();
    }

    void Init()
    {
        InitAsSingleton();
        _missedItemManager = GetComponent<IMissedItemManager>();

        foreach (var areaManager in _areaManagers)
        {
            if (!_areaNameToAreaManagerMap.ContainsKey(areaManager.AreaName))
            {
                _areaNameToAreaManagerMap.Add(areaManager.AreaName, areaManager);
            }
        }
        isReady = true;
    }
    void InitAsSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        // プレイヤーの近くにあるチャンクを生成する
        if (isReady)
            UpdateChunksAroundPlayer(GetPlayerPos());
    }

    readonly HashSet<Vector2Int> previousActiveChunks = new();
    readonly HashSet<Vector2Int> currentActiveChunks = new();
    readonly HashSet<Vector2Int> chunksToActivate = new();
    readonly HashSet<Vector2Int> chunksToDeactivate = new();

    // / <summary>
    // / プレイヤーの近くのチャンクを生成・削除。生成の中心を簡単に変えられるよう，引数として定義
    // / </summary>
    // / <param name="playerChunkPos"></param>
    void UpdateChunksAroundPlayer(Vector2? nullablePlayerPos)
    {
        Vector2 playerPos;
        if (nullablePlayerPos == null)
            return;
        else
            playerPos = (Vector2)nullablePlayerPos;

        currentActiveChunks.Clear();

        Vector2 realChunkSize = new(ChunkSize.x * CellSize.x, ChunkSize.y * CellSize.y);

        Vector2 playerChunkPosFlt = new(playerPos.x / realChunkSize.x, playerPos.y / realChunkSize.y);
        Vector2 chunkPosRange = new(ChunkGenerateRange.x / realChunkSize.x, ChunkGenerateRange.y / realChunkSize.y);

        for (int x = Mathf.CeilToInt(playerChunkPosFlt.x - chunkPosRange.x); x <= Mathf.FloorToInt(playerChunkPosFlt.x + chunkPosRange.x); x++)
        {
            for (int y = Mathf.CeilToInt(playerChunkPosFlt.y - chunkPosRange.y); y <= Mathf.FloorToInt(playerChunkPosFlt.y + chunkPosRange.y); y++)
            {
                currentActiveChunks.Add(new(x, y));
            }
        }
        chunksToActivate.Clear();
        chunksToActivate.UnionWith(currentActiveChunks);
        chunksToActivate.ExceptWith(previousActiveChunks);
        foreach (var chunk in chunksToActivate)
            PrepareChunk(chunk);

        chunksToDeactivate.Clear();
        chunksToDeactivate.UnionWith(previousActiveChunks);
        chunksToDeactivate.ExceptWith(currentActiveChunks);
        foreach (var chunk in chunksToDeactivate)
        {
            UnloadChunk(chunk);
        }

        previousActiveChunks.Clear();
        previousActiveChunks.UnionWith(currentActiveChunks);
    }

    public void PrepareChunk(Vector2Int chunkPos)
    {
        Generate(chunkPos);
    }

    public void UnloadChunk(Vector2Int chunkPos)
    {
        Deactivate(chunkPos);
    }

    public Vector2Int? WorldPosToChunkPos(Vector2 worldPos)
    {
        if (worldPos == null)
            return null;

        // Debug.Log($"{MethodBase.GetCurrentMethod().Name}, playerPosition: {playerPosition}");
        int chunkX = Mathf.FloorToInt(worldPos.x / (ChunkSize.x * CellSize.x));
        int chunkY = Mathf.FloorToInt(worldPos.y / (ChunkSize.y * CellSize.y));
        return new Vector2Int(chunkX, chunkY);
    }









    [SerializeField] List<AreaManager> _areaManagers;

    // 内部データ
    readonly Dictionary<Vector2Int, JObject> _chunkDatas = new();
    Dictionary<Vector2Int, IChunkManager> _activeChunkManagers = new Dictionary<Vector2Int, IChunkManager>();

    private Dictionary<string, IAreaManager> _areaNameToAreaManagerMap = new Dictionary<string, IAreaManager>();
    private Dictionary<Vector2Int, string> _chunkToAreaMap = new Dictionary<Vector2Int, string>();
    private Dictionary<string, List<Vector2Int>> _areaToChunksMap = new Dictionary<string, List<Vector2Int>>();

    // 依存関係
    IMissedItemManager _missedItemManager; // 後でDIで注入

    /// <summary>
    /// エリアマネージャーを登録します。
    /// </summary>
    public void RegisterAreaManager(string areaName, IAreaManager areaManager)
    {
        if (!_areaNameToAreaManagerMap.ContainsKey(areaName))
        {
            _areaNameToAreaManagerMap.Add(areaName, areaManager);
            areaManager.Init(this); // AreaManagerにTerrainManagerを渡す
        }
        else
        {
            Debug.LogWarning($"エリア名 '{areaName}' はすでに登録されています。");
        }
    }

    /// <summary>
    /// 指定された位置にチャンクを生成またはアクティブ化します。
    /// </summary>
    public void Generate(Vector2Int chunkPos)
    {
        Debug.Log($"generated: {chunkPos}");
        if (_activeChunkManagers.ContainsKey(chunkPos))
        {
            Debug.LogWarning("this chunk is already active: " + chunkPos);
            // 既にアクティブなチャンクはスキップ
            return;
        }

        IChunkManager chunkManager = ResourceManager.Instance.GetOther(ResourceManager.OtherID.ChunkManager.ToString()).GetComponent<IChunkManager>();

        JObject chunkData = null;
        IAreaManager areaManager = GetArea(chunkPos); // チャンクが属するエリアを取得

        if (_chunkDatas.ContainsKey(chunkPos))
        {
            chunkData = _chunkDatas[chunkPos];
            Debug.Log($"get from memory, chunkData: {chunkData}");
        }
        else if (areaManager != null)
        {
            chunkData = areaManager.GetChunkData(chunkPos);
            Debug.Log($"get from areaManager, chunkData: {chunkData}");
        }
        else
        {
            Debug.LogWarning("area manager is null");
            return;
        }

        if (chunkData == null)
        {
            Debug.LogWarning("chunkData is invalid");
            return;
        }

        chunkManager.OnGenerated(areaManager, chunkPos);
        Debug.Log($"chunkData: {chunkData}");
        chunkManager.ApplyChunkData(chunkData);

        _activeChunkManagers.Add(chunkPos, chunkManager);

        areaManager?.OnChunkGenerated(chunkManager, chunkPos);
    }

    /// <summary>
    /// 指定された位置のチャンクを非アクティブ化します。
    /// </summary>
    public void Deactivate(Vector2Int chunkPos)
    {
        Debug.Log($"deactivate: {chunkPos}");
        if (!_activeChunkManagers.TryGetValue(chunkPos, out IChunkManager chunkManager))
        {
            return;
        }

        _activeChunkManagers.Remove(chunkPos);
        IAreaManager areaManager = GetArea(chunkPos);

        areaManager?.OnChunkDeactivated(chunkManager, chunkPos);

        var deactivatedChunkData = chunkManager.OnDeactivated();

        if (_missedItemManager == null)
            Debug.LogWarning(_missedItemManager is null);
        deactivatedChunkData = _missedItemManager?.CollectMissedItems(deactivatedChunkData);
        _chunkDatas[chunkPos] = deactivatedChunkData; // チャンクデータをメモリに保存

        if (chunkManager is MonoBehaviour monoBehaviour)
        {
            monoBehaviour.GetComponent<PoolableResourceComponent>()?.Release();
        }
    }

    /// <summary>
    /// 指定されたチャンク位置が属するエリアマネージャーを取得します。
    /// TerrainManagerがエリア間の衝突を仲裁します。
    /// </summary>
    public IAreaManager GetArea(Vector2Int chunkPos)
    {
        var areaName = GetAreaName(chunkPos);
        if (_areaNameToAreaManagerMap.TryGetValue(areaName, out IAreaManager areaManager))
        {
            return areaManager;
        }
        return null;
    }

    /// <summary>
    /// 指定されたチャンク位置が属するエリア名を取得します。
    /// </summary>
    private string GetAreaName(Vector2Int chunkPos)
    {
        if (_chunkToAreaMap.TryGetValue(chunkPos, out string areaName))
        {
            return areaName;
        }
        // デフォルトのエリア名を返すか、エラー処理を行う
        return "DefaultArea";
    }

    /// <summary>
    /// 指定されたエリア名のチャンクデータをすべて削除します。
    /// </summary>
    public void ClearChunkDatas(string areaName)
    {
        if (_areaToChunksMap.TryGetValue(areaName, out List<Vector2Int> chunkPosList))
        {
            foreach (Vector2Int chunkPos in chunkPosList)
            {
                // アクティブなチャンクであれば非アクティブ化
                // if (_activeChunkManagers.ContainsKey(chunkPos))
                // {
                //     Deactivate(chunkPos);
                // }
                // メモリ上のチャンクデータを削除
                _chunkDatas.Remove(chunkPos);
            }
        }
        else
        {
            Debug.LogWarning($"エリア '{areaName}' は見つかりませんでした。");
        }
    }

    /// <summary>
    /// チャンクとエリアの関連付けを登録します。
    /// </summary>
    private void RegisterChunkToArea(string areaName, Vector2Int chunkPos)
    {
        if (!_areaToChunksMap.ContainsKey(areaName))
        {
            _areaToChunksMap.Add(areaName, new List<Vector2Int>());
        }
        if (!_areaToChunksMap[areaName].Contains(chunkPos))
        {
            _areaToChunksMap[areaName].Add(chunkPos);
            _chunkToAreaMap[chunkPos] = areaName;
        }
    }

    /// <summary>
    /// チャンクとエリアの関連付けを解除します。
    /// </summary>
    private void RemoveChunkFromArea(string areaName, Vector2Int chunkPos)
    {
        if (_areaToChunksMap.TryGetValue(areaName, out List<Vector2Int> chunkPosList))
        {
            chunkPosList.Remove(chunkPos);
        }
        _chunkToAreaMap.Remove(chunkPos);
    }

    /// <summary>
    /// TerrainManagerが保持するすべてのチャンクデータとエリアデータをファイルに保存します。
    /// </summary>
    public void Save()
    {
        // エリアデータの保存
        foreach (var entry in _areaNameToAreaManagerMap)
        {
            string areaName = entry.Key;
            IAreaManager areaManager = entry.Value; areaManager.Save();
            Debug.Log($"'{areaName}' was saved");
        }

        foreach (var entry in _activeChunkManagers)
        {
            Vector2Int chunkPos = entry.Key;
            _chunkDatas[chunkPos] = entry.Value.MakeChunkData(); // メモリ上のデータを更新

            // すべてのチャンクデータを保存
            foreach (var data in _chunkDatas)
            {
                var chunkData = data.Value;
                EditFile.CompressAndSaveJson(ChunkManager.GetChunkFilePath(data.Key), chunkData.ToString());
                Debug.Log($"チャンク {data.Key} のデータを保存しました。");
            }
            Debug.Log("すべてのマップデータを保存しました。");
        }
    }

    /// <summary>
    /// シード値と地形をリセットします。
    /// 地上に帰った場合などに呼ばれます。
    /// </summary>
    public void ResetSeedAndTerrain()
    {
        _chunkDatas.Clear();
        // アクティブなチャンクをすべて非アクティブ化
        List<Vector2Int> activeChunkPositions = new List<Vector2Int>(_activeChunkManagers.Keys);
        foreach (Vector2Int chunkPos in activeChunkPositions)
        {
            Deactivate(chunkPos);
        }
    }





    [SerializeField] Vector2Int _targetChunkPos;
    [SerializeField] Vector2Int _targetSubChunkPos;

    [SerializeField] ChunkManager _chunkManagerForChunkDataGeneration;
    [SerializeField] string chunkAssetsDirectoryPath = "Assets/ChunkAssets";
    [SerializeField] string chunkAssetName = "ChunkAssetName.json";
    /// <summary>
    /// マップ作製用メソッド。子オブジェクトをChunkDataに記録しjsonファイルを作る。
    /// </summary>
    public void CreateChunkAsset()
    {
        if (_chunkManagerForChunkDataGeneration != null)
        {
            InitAsSingleton();
            _chunkManagerForChunkDataGeneration.SetChunkPos(_targetChunkPos);
            _chunkManagerForChunkDataGeneration.OnBeforeCreateChunkAsset();
            string path = Path.Combine(chunkAssetsDirectoryPath, chunkAssetName);
            EditFile.SaveJson(path, _chunkManagerForChunkDataGeneration.MakeChunkData().ToString());
        }
        else
            Debug.LogWarning("_chunkManagerForChunkDataGeneration is null");
    }//
    [SerializeField] SubChunkGenerator _subChunkHandlerForChunkDataGeneration;
    [SerializeField] string subChunkAssetsDirectoryPath = "Assets/SubChunkAssets";
    [SerializeField] string subChunkAssetName = "SubChunkAssetName.json";
    /// <summary>
    /// マップ作製用メソッド。子オブジェクトをChunkDataに記録しjsonファイルを作る。
    /// </summary>
    public void CreateSubChunkAsset()
    {
        if (_subChunkHandlerForChunkDataGeneration != null)
        {
            InitAsSingleton();
            _subChunkHandlerForChunkDataGeneration.SetChunkPos(_targetChunkPos);
            _subChunkHandlerForChunkDataGeneration.SetSubChunkPos(_targetSubChunkPos);
            _subChunkHandlerForChunkDataGeneration.OnBeforeCreateChunkAsset();
            string path = Path.Combine(subChunkAssetsDirectoryPath, subChunkAssetName);
            EditFile.SaveJson(path, _subChunkHandlerForChunkDataGeneration.MakeChunkData().ToString());
        }
        else
            Debug.LogWarning("_subChunkManagerForChunkDataGeneration is null");
    }

    [SerializeField] TextAsset _chunkDataForChunkGeneration;
    [SerializeField] ChunkManager _chunkManagerForChunkGeneration;
    public void GenerateChunk()
    {
        if (_chunkDataForChunkGeneration != null && _chunkManagerForChunkGeneration != null)
        {
            InitAsSingleton();
            _chunkManagerForChunkGeneration.SetChunkPos(_targetChunkPos);
            _chunkManagerForChunkGeneration.ApplyChunkData(EditFile.JsonToJObject(_chunkDataForChunkGeneration.text));
        }
        else
            Debug.LogWarning("_chunkDataForChunkGeneration or _chunkManagerForChunkGeneration is null");
    }

    [SerializeField] TextAsset _subChunkDataForSubChunkGeneration;
    [SerializeField] SubChunkGenerator _subChunkHandlerForSubChunkGeneration;
    public void GenerateSubChunk()
    {
        if (_subChunkHandlerForSubChunkGeneration != null && _subChunkDataForSubChunkGeneration != null)
        {
            InitAsSingleton();
            _subChunkHandlerForSubChunkGeneration.SetChunkPos(_targetChunkPos);
            _subChunkHandlerForSubChunkGeneration.SetSubChunkPos(_targetSubChunkPos);
            _subChunkHandlerForSubChunkGeneration.ApplySubChunkData(EditFile.JsonToJObject(_subChunkDataForSubChunkGeneration.text));
        }
        else
            Debug.LogWarning("_subChunkDataForSubChunkGeneration or _subChunkHandlerForSubChunkGeneration is null");
    }
}
