using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UIPageBase
{
    public override bool IsRoot => true;
    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button achievementButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button languageButton;

    protected override void Awake()
    {
        base.Awake();
        playButton.onClick.AddListener(Play);
        settingsButton.onClick.AddListener(Settings);
        achievementButton.onClick.AddListener(Achievements);
        quitButton.onClick.AddListener(Quit);
        languageButton.onClick.AddListener(Language);
    }
    private void Start()
    {
        // Close();
    }

    protected override void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        achievementButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        languageButton.onClick.RemoveAllListeners();
    }
    protected override void OnOpenCompleted()
    {
        base.OnOpenCompleted();
        isQuit = false;
    }

    void Play()
    {
        UIManager.Instance.Show(UIPageType.SaveMenuUI);
    }
    void Settings()
    {
        UIManager.Instance.Show(UIPageType.SettingsUI);
    }
    void Achievements()
    {
        UIManager.Instance.Show(UIPageType.AchievementsUI);
    }
    void Language()
    {
        UIManager.Instance.Show(UIPageType.LanguageUI);
    }
    void Quit()
    {
        isQuit = true;
        UIManager.Instance.Hide(UIPageType.InventoryUI);
        UIManager.Instance.Hide(UIPageType.PlayerStatusUI);
        if (UIManager.Instance.GetEquipmentUI().IsOpen)
            UIManager.Instance.Hide(UIPageType.EquipmentUI);
        UIManager.Instance.CloseAllStack();
    }
    bool isQuit;
    protected override void OnCloseCompleted()
    {
        base.OnCloseCompleted();
        if (isQuit)
            ApplicationManager.Instance.Quit();

    }
}
