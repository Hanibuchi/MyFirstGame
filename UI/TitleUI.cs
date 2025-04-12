using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : BaseMenuUI
{
    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button achievementButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button languageButton;

    private void Awake()
    {
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

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        achievementButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        languageButton.onClick.RemoveAllListeners();
    }

    void Play()
    {
        Close(() => UIManager.Instance.Open(UIManager.UIType.SaveMenu));
    }
    void Settings()
    {
        Close(() => UIManager.Instance.Open(UIManager.UIType.SettingsUI));
    }
    void Achievements()
    {
        Close(() => UIManager.Instance.Open(UIManager.UIType.AchievementsUI));
    }
    void Quit()
    {
        Close(() => ApplicationManager.Instance.Quit());
    }
    void Language()
    {
        Close();
        // UIManager.Instance.Open(UIManager.UIType.LanguageUI);
    }
}
