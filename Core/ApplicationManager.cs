using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class ApplicationManager : MonoBehaviour
{
    const string GAME_NAME = "GameName";
    public static string MainDirectoryPath => Path.Combine(Application.persistentDataPath, GAME_NAME);
    public static string ConfigDirectoryPath => Path.Combine(MainDirectoryPath, "Config");
    public static string SaveDirectoryPath => Path.Combine(MainDirectoryPath, "Saves");

    /// <summary>
    /// シングルトンインスタンス。インスペクタから設定したい変数にはこれを使う。注：直接GameManagerからアクセスするのではなく，.Instanceも書かないといけない。このクラスのインスタンスを一つのゲームと考えると，ゲーム内で使用する変数は.Instanceと書くのが面倒でもstaticを使わないのがベターらしい。関数とかlayer名とかはゲーム間で変更がないものはstatic使ってもいいかも。
    /// </summary>
    public static ApplicationManager Instance { get; private set; }

    GameManager gameManager;

    [SerializeField] ResourceManager resourceManager;
    [SerializeField] UIManager uIManager;
    [SerializeField] AchievementsManager achievementsManager;
    [SerializeField] MyInputSystem inputSystem;
    [SerializeField] SettingsManager settingsManager;
    bool isNewGame;
    InitGameData initWorldData;
    string saveSlotName;
    /// <summary>
    /// ゲーム開始しているかどうか。CurrentSceneができたためそれほど使わない。
    /// </summary>
    public bool IsGameRunning { get; private set; }

    /// <summary>
    /// Sceneの名前を管理するenum。Sceneと同じ名前でなければならない。
    /// </summary>
    public enum SceneType
    {
        TitleScene,
        MainGameScene,
    }
    public SceneType CurrentScene { get; private set; }

    float startTime;
    [SerializeField] float minWaitTime = 5;

    // /// <summary>
    // /// Layer名を管理するためのクラス
    // /// </summary>
    // public static class Layers
    // {
    //     public const string Enemy = "Enemy";
    //     public const string Ally = "Ally";
    //     public const string Projectile = "Projectile";
    // }

    // public BaseTile test_baseTile;
    // public void Test()
    // {
    // }

    private void Awake()
    {
        OnAppStart();
    }


    void OnAppStart()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        startTime = Time.time;

        resourceManager.OnAppStart(OnResourceReady);
    }

    public void OnResourceReady()
    {
        achievementsManager.OnAppStart();
        inputSystem.OnAppStart();
        uIManager.OnAppStart();
        settingsManager.OnAppStart();

        StartCoroutine(WaitMinTime(startTime, () =>
        {
            UIManager.Instance.Show(UIPageType.TitleUI);
        }));
    }

    IEnumerator WaitMinTime(float startTime, Action callback)
    {
        float elapsedTime = Time.time - startTime;
        // Debug.Log($"elapsedTime: {elapsedTime}");
        // Debug.Log($"minWaitTime - elapsedTime: {minWaitTime - elapsedTime}");
        if (elapsedTime < minWaitTime)
        {
            yield return new WaitForSeconds(minWaitTime - elapsedTime);
        }
        // Debug.Log("elapsedTime is minWaitTime");
        callback?.Invoke();
    }

    /// <summary>
    /// ゲームを新規作成。
    /// </summary>
    /// <param name="initWorldData"></param>
    public void CreateNewWorld(InitGameData initWorldData)
    {
        isNewGame = true;
        this.initWorldData = initWorldData;
        CurrentScene = SceneType.MainGameScene;
        SceneManager.LoadScene(SceneType.MainGameScene.ToString());
    }
    public void LoadWorld(string saveSlotName)
    {
        isNewGame = false;
        this.saveSlotName = saveSlotName;
        CurrentScene = SceneType.MainGameScene;
        SceneManager.LoadScene(SceneType.MainGameScene.ToString());
    }
    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
        if (isNewGame)
            this.gameManager.CreateNewWorld(initWorldData);
        else
            this.gameManager.LoadWorld(saveSlotName);
    }

    /// <summary>
    /// ゲームの初期化が完了すれば呼ばれるメソッド。ロード中の画面をゲーム画面に替える。
    /// </summary>
    public void OnGameInitializationComplete()
    {
        IsGameRunning = true;
    }

    public void ReturnToTitle()
    {
        IsGameRunning = false;
        gameManager.OnReturnToTitle();
        uIManager.OnReturnToTitle();
        CurrentScene = SceneType.TitleScene;
        SceneManager.LoadScene(SceneType.TitleScene.ToString());
    }


    public void Quit()
    {
        if (CurrentScene == SceneType.MainGameScene)
        {
            gameManager.Save();
        }
        Save();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 何かアプリ全体でセーブすることがあれば。
    /// </summary>
    void Save()
    {

    }

    public void DeleteSaveSlot(string saveSlotName)
    {
        string filePath = Path.Combine(SaveDirectoryPath, saveSlotName);
        Debug.Log("filePath: " + filePath + ", isExist: " + Directory.Exists(filePath));
        if (Directory.Exists(filePath))
        {
            try
            {
                Directory.Delete(filePath, true);
            }
            catch (IOException ex)
            {
                Debug.LogWarning($"IOException: {ex.Message}");
            }
            string message = saveSlotName + "was deleted";
            Debug.Log(message);
            var messageUI = ResourceManager.GetOther(ResourceManager.UIID.MessageUI.ToString()).GetComponent<MessageUI>();
            messageUI.Open(message, () =>
    UIManager.Instance.Show(UIPageType.SaveMenuUI));
        }
    }
}


// PPU(pixel per unit)の設定
// public class CustomTextureImporter : AssetPostprocessor
// {
//     private void OnPreprocessTexture()
//     {
//         TextureImporter importer = assetImporter as TextureImporter;

//         if (importer != null)
//         {
//             // デフォルトのPixels Per Unitの値を設定します
//             importer.spritePixelsPerUnit = 288; // ここに希望の値を設定してください
//         }
//     }
// }

