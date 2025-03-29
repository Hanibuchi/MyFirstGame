using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyGame;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static string SaveSlotName = "SaveSlot_";
    public static string SaveSlotDirectoryPath => Path.Combine(ApplicationManager.SaveDirectoryPath, SaveSlotName);
    public static string PlayerDataPath => GetPlayerDataPath(SaveSlotDirectoryPath);
    public static string GetPlayerDataPath(string saveSlotDirectoryPath)
    {
        return Path.Combine(saveSlotDirectoryPath, "PlayerData");
    }
    public static GameManager Instance { get; private set; }
    public int Seed { get; private set; }
    public GameMode GameMode { get; private set; }
    [SerializeField] private TerrainManager terrainManager;
    public TerrainManager TerrainManager => terrainManager;

    [SerializeField] private NPCManager playerNPCManager;
    public NPCManager PlayerNPCManager => playerNPCManager;

    [SerializeField] private StatisticsManager statisticsManager;
    public StatisticsManager StatisticsManager => statisticsManager;

    [SerializeField] private EventFlagManager eventFlagManager;
    public EventFlagManager EventFlagManager => eventFlagManager;

    [SerializeField] private PlayerParty playerParty;
    public PlayerParty PlayerParty => playerParty;

    [SerializeField] private Inventory inventory;
    public Inventory Inventory => inventory;
    /// <summary>
    /// ドラッグ中のItemSlotの親
    /// </summary>
    public GameObject DragContainer;

    Vector3 respawnPoint;

    public enum GameStateType
    {
        Playing,
        Paused,
    }

    public GameStateType GameState { get; private set; }

    void InitRandoms()
    {
        UnityEngine.Random.InitState(Seed);

        UnityEngine.Random.State state = UnityEngine.Random.state;
        foreach (RandomNames rand in Enum.GetValues(typeof(RandomNames)))
            Randoms[rand] = new MyRandom(state);
    }

    /// <summary>
    /// Random.Stateを管理するための辞書に入れる文字列を管理するための列挙型。ここに追加していく。
    /// </summary>
    public enum RandomNames
    {
        InstantDeath,
        Diffusion,
        DropItem,
        HireAmount,
    }

    /// <summary>
    /// カテゴリーごとにRandom.Stateを保持するためのインスタンス。これを使えば，同じシード，同じカテゴリー，同じ順番なら出るものは同じにできる。
    /// </summary>
    public static readonly Dictionary<RandomNames, MyRandom> Randoms = new(); // Stateを保持するための辞書

    /// <summary>
    /// カテゴリーごとにRandom.Stateを保持するためのクラス。これを使えば，同じシード，同じカテゴリー，同じ順番なら出るものは同じにできる。
    /// </summary>
    public class MyRandom
    {
        private UnityEngine.Random.State state;
        public MyRandom(UnityEngine.Random.State initState)
        {
            state = initState;
        }
        /// <summary>
        /// [0,1]のランダムな値を返す。
        /// </summary>
        /// <returns></returns>
        public float Value()
        {
            UnityEngine.Random.state = state;
            float value = UnityEngine.Random.value;
            state = UnityEngine.Random.state;
            return value;
        }
        /// <summary>
        /// 正規分布の乱数を返す。ボックス＝ミュラー変換を用いた。
        /// </summary>
        /// <param name="sd">標準偏差</param>
        /// <returns></returns>
        public float NormalDistribution()
        {
            UnityEngine.Random.state = state;
            float u1 = 1.0f - UnityEngine.Random.value;
            float u2 = 1.0f - UnityEngine.Random.value;

            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

            // 平均値meanと標準偏差standardDeviationに合わせてスケーリング
            state = UnityEngine.Random.state;
            return randStdNormal;
        }
    }
    private void Awake()
    {
        ApplicationManager.Instance.SetGameManager(this);
    }

    /// 下の2つのメソッド，それぞれの機能が既存ファイルがあるかを自分で判断するようにしたい。

    /// <summary>
    /// 新規作成時実行される。
    /// </summary>
    /// <param name="initGameData"></param>
    public void CreateNewWorld(InitGameData initGameData)
    {
        Init();
        StatisticsManager.Instance.Set(StatisticsManager.StatType.GameStart.ToString(), DateTime.Now);
        SaveSlotName = initGameData.SaveSlotName;
        Seed = initGameData.Seed;
        GameMode = initGameData.GameMode;

        TerrainManager.NewGame();
        PlayerParty.NewGame();

        ApplicationManager.Instance.OnGameInitializationComplete();
        Debug.Log($"SaveSlotDirectoryPath: {SaveSlotDirectoryPath}");
    }
    /// <summary>
    /// 作成済みのワールドを生成するとき使われる。
    /// </summary>
    /// <param name="saveSlotName"></param>
    public void LoadWorld(string saveSlotName)
    {
        Init();
        SaveSlotName = saveSlotName;
        // ここでDirectoryPathからJsonファイルを取り出していろいろDataクラス取り出す。
        // SeedやGameModeも取り出して代入。

        TerrainManager.LoadWorld(new());
        PlayerParty.LoadWorld(new());
        ApplicationManager.Instance.OnGameInitializationComplete();
    }

    void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンが変更されてもオブジェクトが破棄されないようにする
        }
        else
        {
            Destroy(gameObject); // 既にインスタンスが存在する場合、このオブジェクトを破棄
        }
        InitRandoms();
        UIManager.Instance.OnGameStart();
        MyInputSystem.Instance.OnGameStart();
        AchievementsManager.Instance.OnGameStart();
        statisticsManager.OnGameStart();
    }

    public void RespawnPlayer()
    {
        PlayerNPCManager.transform.position = respawnPoint;
        PlayerNPCManager.OnRespawn();
        UIManager.Instance.Open(UIManager.UIType.PlayerStatusUI);
        UIManager.Instance.Open(UIManager.UIType.InventoryUI);
    }

    public void GameOver(string causeOfDeath, AreaManager areaManager, Vector2Int chunkPos, Vector3 pos, float hiringCost, List<HiredMemberData> traitorDatas)
    {
        UIManager.Instance.Close(UIManager.UIType.PlayerStatusUI);
        UIManager.Instance.Close(UIManager.UIType.InventoryUI);

        ResourceManager.Get(ResourceManager.UIID.GameOverUI).GetComponent<GameOverUI>().Open(causeOfDeath, areaManager, chunkPos, pos, hiringCost, traitorDatas);
    }

    public void OnReturnToTitle()
    {
        Save();
        AchievementsManager.Instance.OnReturnToTitle(); // Save()の後で実行。Achievementをリセットするため先にセーブする。Saveを非同期で行うとちゃんとセーブできない可能性がある。
        MyInputSystem.Instance.OnReturnToTitle();
    }

    public void Save()
    {
        SavePlayerData();
        TerrainManager.Save();
    }

    public void SavePlayerData()
    {
        AchievementsManager.Instance.Save();
        StatisticsManager.Save();
        EventFlagManager.Save();
        PlayerParty.Save();
        Inventory.Save();
    }

    public void PauseGame()
    {
        GameState = GameStateType.Paused;
        // ここで時間を止める。BGMはそのまま。
    }

    public void ResumeGame()
    {
        GameState = GameStateType.Playing;
        // 時間を進める。
    }

    public class Utility
    {
        /// <summary>
        /// マウスの位置を取得する。CameraがPerspectiveのため特殊な方法が必要になる。UI以外で使う。
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetMousePos()
        {
            Vector3 mousePos = Pointer.current.position.ReadValue();
            if (Instance.PlayerNPCManager != null)
                mousePos.z = Instance.PlayerNPCManager.transform.position.z - Camera.main.transform.position.z;
            else
                mousePos.z = -Camera.main.transform.position.z;

            return Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    public enum Layer
    {
        Projectile,
        Ally,
        Enemy,
        Neutral,
        Item,
        Chunk,
        Ground,
        GroundHitDetection,
    }

    public void SetPlayerNPCManager(NPCManager nPCManager)
    {
        playerNPCManager = nPCManager;
    }
}
