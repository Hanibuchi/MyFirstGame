using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

public class NewGameUI : UIPageBase, INewGameUI
{
    string saveSlotName;
    [SerializeField] Button Normal;
    [SerializeField] Button Hard;
    [SerializeField] Button Impossible;
    [SerializeField] Button Story;
    protected override void Awake()
    {
        base.Awake();
        Normal.onClick.AddListener(NewGameNormal);
        Hard.onClick.AddListener(NewGameHard);
        Impossible.onClick.AddListener(NewGameImpossible);
        Story.onClick.AddListener(NewGameStory);
    }
    public void SetSaveSlotName(string saveSlotName)
    {
        this.saveSlotName = saveSlotName;
    }
    InitGameData GetInitGameData(GameMode gameMode)
    {
        return new()
        {
            SaveSlotName = saveSlotName,
            GameMode = gameMode,
            Seed = Functions.GenerateSeed(),
        };
    }
    protected override void OnOpenCompleted()
    {
        base.OnOpenCompleted();
        m_gameMode = GameMode.None;
    }
    void NewGameNormal()
    {
        m_gameMode = GameMode.Normal;
        NewGame();
    }
    void NewGameHard()
    {
        m_gameMode = GameMode.Hard;
        NewGame();
    }
    void NewGameImpossible()
    {
        m_gameMode = GameMode.Impossible;
        NewGame();
    }
    void NewGameStory()
    {
        m_gameMode = GameMode.Story;
        NewGame();
    }
    GameMode m_gameMode;
    void NewGame()
    {
        UIManager.Instance.CloseAll();
    }
    protected override void OnCloseCompleted()
    {
        base.OnCloseCompleted();
        if (m_gameMode == GameMode.None)
        {
            return;
        }
        var initGameData = GetInitGameData(m_gameMode);
        Debug.Log($"NewGame!!! SaveSlotName:{initGameData.SaveSlotName}, GameMode:{initGameData.GameMode.ToString()}, Seed:{initGameData.Seed}");
        ApplicationManager.Instance.CreateNewWorld(GetInitGameData(m_gameMode));
    }
}
