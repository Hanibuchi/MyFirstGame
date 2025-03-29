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
// using System.Numerics;

namespace MyGame
{

    public class TerrainManager : MonoBehaviour
    {
        public string TerrainDataDirectoryPath => Path.Combine(GameManager.SaveSlotDirectoryPath, "TerrainData");
        public static TerrainManager Instance;
        /// <summary>
        /// TerrainのGrid
        /// </summary>
        public Grid Grid;
        /// <summary>
        /// Terrainが使うTilemap
        /// </summary>
        public Tilemap TerrainTilemap;
        enum AreaIDs
        {
            DefaultArea,
        }
        readonly Dictionary<AreaIDs, AreaManager> Areas = new();

        enum ChunkState
        {
            None,
            Active,
            Inactive,
        }
        /// <summary>
        /// チャンクの状態を記録
        /// </summary>
        readonly Dictionary<Vector2Int, ChunkState> ChunkStates = new();

        /// <summary>
        /// チャンク1つあたりの縦横のタイル数
        /// </summary>
        public Vector3Int ChunkSize = new(128, 128, 1);
        public Vector2 CellSize => Grid.cellSize;

        Vector2? PlayerPos => PlayerManager != null ? PlayerManager.transform.position : null;

        NPCManager PlayerManager => GameManager.Instance?.PlayerNPCManager;

        public Vector2Int? PlayerChunkPos => WorldPosToChunkPos((Vector2)PlayerPos);

        // [SerializeField] Vector2Int test_chunkPos;
        // public void Test()
        // {
        //     Debug.Log($"ChunkStates[{test_chunkPos.x}, {test_chunkPos.y}] = {ChunkStates[test_chunkPos]}");
        // }

        /// <summary>
        /// チャンクを生成する範囲。Playerからの距離(u)であらわされる
        /// </summary>
        public Vector2 ChunkGenerateRange = new(64f, 64f);

        /// <summary>
        /// 準備が整ったかどうか。Updateのエラーがうるさいからこうした。
        /// </summary>
        bool isReady;

        public void NewGame()
        {
            Init();
        }

        public void LoadWorld(MyTerrainData terrainData)
        {
            Init();
            // ここでterrainDataから生成済みの地形を生成する。
        }

        void Init()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            var TerrainObj = ResourceManager.Get(ResourceManager.OtherID.TerrainGrid);
            Grid = TerrainObj.GetComponent<Grid>();
            TerrainTilemap = TerrainObj.GetComponentInChildren<Tilemap>();

            Areas[AreaIDs.DefaultArea] = ResourceManager.Get(ResourceManager.AreaID.DefaultArea).GetComponent<AreaManager>();
            foreach (var keyValue in Areas)
            {
                keyValue.Value.Init(this);
            }
            isReady = true;
        }

        public void Save()
        {
            foreach (var keyValue in Areas)
            {
                keyValue.Value.Save();
            }
            // ここでTerrainDataを保存。（あれば（全体に関係するデータはPlayerData, エリアごとのデータはAreaDataに保存されるため必要ない気がする。））
        }

        private void Update()
        {
            // プレイヤーの近くにあるチャンクを生成する
            if (isReady)
                UpdateChunksAroundPlayer(PlayerPos);
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
            // Debug.Log($"Chunk at {chunkPos} was Prepared");
            switch (ChunkStates.GetValueOrDefault(chunkPos, ChunkState.None))
            {
                case ChunkState.None:
                    // Debug.Log("chunk is none");
                    StartCoroutine(Generate(chunkPos));
                    break;

                case ChunkState.Active:
                    // Debug.Log("chunk is active");
                    break;

                case ChunkState.Inactive:
                    // Debug.Log("chunk is inactive");
                    StartCoroutine(Activate(chunkPos));
                    break;
            }
        }

        public void UnloadChunk(Vector2Int chunkPos)
        {
            // Debug.Log($"Chunk at {chunkPos} was unloaded");
            switch (ChunkStates.GetValueOrDefault(chunkPos, ChunkState.None))
            {
                case ChunkState.None:
                    // Debug.Log("chunk is none");
                    break;

                case ChunkState.Active:
                    // Debug.Log("chunk is active");
                    StartCoroutine(Deactivate(chunkPos));
                    break;

                case ChunkState.Inactive:
                    // Debug.Log("chunk is inactive");
                    break;
            }
        }

        private IEnumerator Generate(Vector2Int chunkPos)
        {
            AreaManager area = GetArea(chunkPos);
            if (area == null)
                yield break;

            bool result = area.Generate(chunkPos);

            while (!result)
            {
                yield return null;
                result = area.Generate(chunkPos);
            }
            ChunkStates[chunkPos] = ChunkState.Active;
        }

        /// <summary>
        /// chunkPosのチャンクを非アクティブにする。UpdateChunksAroundPlayerの実装の関係から1度しか実行することができず失敗すればそのままになってしまう。そのため，成功するまで何度も繰り返すようにした。
        /// </summary>
        /// <param name="chunkPos"></param>
        /// <returns></returns>
        private IEnumerator Deactivate(Vector2Int chunkPos)
        {
            AreaManager area = GetArea(chunkPos);
            if (area == null)
                yield break;

            bool result = area.Deactivate(chunkPos);

            while (!result)
            {
                yield return null;
                result = area.Deactivate(chunkPos);
            }
            ChunkStates[chunkPos] = ChunkState.Inactive;
        }

        private IEnumerator Activate(Vector2Int chunkPos)
        {
            AreaManager area = GetArea(chunkPos);
            if (area == null)
                yield break;

            bool result = area.Activate(chunkPos);

            while (!result)
            {
                yield return null;
                result = area.Activate(chunkPos);
            }
            ChunkStates[chunkPos] = ChunkState.Active;
        }

        private AreaManager GetArea(Vector2Int chunkPos)
        {
            return Areas.GetValueOrDefault(AreaIDs.DefaultArea, null);
        }

        public void Reset()
        {
            ChunkStates.Clear();
            foreach (var keyValue in Areas)
            {
                keyValue.Value.Reset();
            }
        }

        /// <summary>
        /// プレイヤーのワールド座標からチャンク座標を取得する
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public Vector2Int? WorldPosToChunkPos(Vector2 worldPos)
        {
            if (worldPos == null)
                return null;

            // Debug.Log($"{MethodBase.GetCurrentMethod().Name}, playerPosition: {playerPosition}");
            int chunkX = Mathf.FloorToInt(worldPos.x / (ChunkSize.x * CellSize.x));
            int chunkY = Mathf.FloorToInt(worldPos.y / (ChunkSize.y * CellSize.y));
            return new Vector2Int(chunkX, chunkY);
        }

        public MyTerrainData MakeTerrainData()
        {
            return new MyTerrainData();
        }
    }
}