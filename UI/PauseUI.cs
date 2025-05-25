using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseUI : UIPageBase
{
    [SerializeField] Button continu;
    [SerializeField] Button achievements;
    [SerializeField] Button settings;
    [SerializeField] Button statistics;
    [SerializeField] Button returnToTitle;
    protected override void Awake()
    {
        base.Awake();
        continu.onClick.AddListener(Continu);
        achievements.onClick.AddListener(Achievements);
        settings.onClick.AddListener(Settings);
        statistics.onClick.AddListener(Statistics);
        returnToTitle.onClick.AddListener(ReturnToTitle);
    }
    public override void Show()
    {
        base.Show();
    }
    protected override void OnOpenCompleted()
    {
        base.OnOpenCompleted();
        GameManager.Instance.PauseGame();
    }
    public override void OnBack()
    {
        base.OnBack();
        Debug.Log($"GameManager.Instance.GameState: {GameManager.Instance.GameState.ToString()}");
        if (GameManager.Instance.GameState == GameManager.GameStateType.Paused)
            Continu();
    }
    public override void Hide()
    {
        base.Hide();
        GameManager.Instance.ResumeGame();
    }
    public void Continu()
    {
        UIManager.Instance.Back();
    }
    public void Achievements()
    {
        UIManager.Instance.Show(UIPageType.AchievementsUI);
    }
    public void Settings()
    {
        UIManager.Instance.Show(UIPageType.SettingsUI);
    }
    public void Statistics()
    {
        UIManager.Instance.Show(UIPageType.StatisticsUI);
    }
    public void ReturnToTitle()
    {
        ApplicationManager.Instance.ReturnToTitle();
    }
    protected override void OnDestroy()
    {
        continu.onClick.RemoveAllListeners();
        achievements.onClick.RemoveAllListeners();
        settings.onClick.RemoveAllListeners();
        statistics.onClick.RemoveAllListeners();
        returnToTitle.onClick.RemoveAllListeners();
    }
}
