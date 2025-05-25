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
    [SerializeField] GameObject m_player;
    public GameObject Player => m_player;

    [SerializeField] private StatisticsManager statisticsManager;
    public StatisticsManager StatisticsManager => statisticsManager;

    [SerializeField] private EventFlagManager eventFlagManager;
    public EventFlagManager EventFlagManager => eventFlagManager;

    [SerializeField] PartyManager m_partyManager;
    public PartyManager PartyManager => m_partyManager;


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
        foreach (RandomName rand in Enum.GetValues(typeof(RandomName)))
            Random.Randoms[rand.ToString()] = new MyRandom(state);
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
        PartyManager.OnGameStart();
        TerrainManager.OnGameStart();
    }

    public void RespawnPlayer()
    {
        Player.transform.position = respawnPoint;
        PlayerNPCManager.OnRespawn();
        UIManager.Instance.Show(UIPageType.PlayerStatusUI);
        UIManager.Instance.Show(UIPageType.InventoryUI);
    }

    public void GameOver(string causeOfDeath, AreaManager areaManager, Vector2Int chunkPos, Vector3 pos, float hiringCost, List<HiredMemberData> traitorDatas)
    {
        UIManager.Instance.CloseAll();

        ResourceManager.GetOther(ResourceManager.UIID.GameOverUI.ToString()).GetComponent<GameOverUI>().Open(causeOfDeath, areaManager, chunkPos, pos, hiringCost, traitorDatas);
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
        PartyManager.Save();
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
        /// マウスのグローバル座標を取得する。CameraがPerspectiveのため特殊な方法が必要になる。UI以外で使う。
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetMousePos()
        {
            Vector3 mousePos = Pointer.current.position.ReadValue();
            if (Instance.Player != null)
                mousePos.z = Instance.Player.transform.position.z - Camera.main.transform.position.z;
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

    public void SetPlayer(GameObject player)
    {
        m_player = player;
    }
}
/// <summary>
/// Random.Stateを管理するための辞書に入れる文字列を管理するための列挙型。ここに追加していく。
/// </summary>
public enum RandomName
{
    InstantDeath,
    Diffusion,
    DropItem,
    HireAmount,
}
