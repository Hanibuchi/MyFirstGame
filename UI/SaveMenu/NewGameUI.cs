using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

public class NewGameUI : BackableMenuUI
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
    public void NewGameNormal()
    {
        NewGame(GameMode.Normal);
    }
    public void NewGameHard()
    {
        NewGame(GameMode.Hard);
    }
    public void NewGameImpossible()
    {
        NewGame(GameMode.Impossible);
    }
    public void NewGameStory()
    {
        NewGame(GameMode.Story);
    }
    void NewGame(GameMode gameMode)
    {
        Close(() =>
        {
            var initGameData = GetInitGameData(gameMode);
            Debug.Log($"NewGame!!! SaveSlotName:{initGameData.SaveSlotName}, GameMode:{initGameData.GameMode.ToString()}, Seed:{initGameData.Seed}");
            ApplicationManager.Instance.CreateNewWorld(GetInitGameData(gameMode));
        });
    }
    public override void Back()
    {
        Close(() =>
        {
            UIManager.Instance.Open(UIManager.UIType.SaveMenu);
        });
    }
    protected override void OnCloseCompleted()
    {
        ResourceManager.ReleaseOther(ResourceManager.UIID.NewGameUI.ToString(), gameObject);
    }
}
