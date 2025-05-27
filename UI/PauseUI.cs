using System;
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
    void Continu()
    {
        m_closeCallback = GameManager.Instance.ResumeGame;
        UIManager.Instance.Back();
    }
    void Achievements()
    {
        UIManager.Instance.Show(UIPageType.AchievementsUI);
    }
    void Settings()
    {
        UIManager.Instance.Show(UIPageType.SettingsUI);
    }
    void Statistics()
    {
        UIManager.Instance.Show(UIPageType.StatisticsUI);
    }
    void ReturnToTitle()
    {
        m_closeCallback = ApplicationManager.Instance.ReturnToTitle;
        UIManager.Instance.Back();
    }
    event Action m_closeCallback;
    protected override void OnCloseCompleted()
    {
        base.OnCloseCompleted();
        m_closeCallback?.Invoke();
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
