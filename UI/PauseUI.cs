using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseUI : BaseMenuUI
{
    [SerializeField] Button continu;
    [SerializeField] Button achievements;
    [SerializeField] Button settings;
    [SerializeField] Button statistics;
    [SerializeField] Button returnToTitle;
    protected void Awake()
    {
        continu.onClick.AddListener(Continu);
        achievements.onClick.AddListener(Achievements);
        settings.onClick.AddListener(Settings);
        statistics.onClick.AddListener(Statistics);
        returnToTitle.onClick.AddListener(ReturnToTitle);
    }
    public override void Open()
    {
        base.Open();
    }
    protected override void OnOpenCompleted()
    {
        base.OnOpenCompleted();
        GameManager.Instance.PauseGame();
    }
    protected override void OnCancelButtonPushed(InputAction.CallbackContext callback)
    {
        Debug.Log($"GameManager.Instance.GameState: {GameManager.Instance.GameState.ToString()}");
        if (GameManager.Instance.GameState == GameManager.GameStateType.Paused)
            Continu();
    }
    public override void Close()
    {
        base.Close();
        GameManager.Instance.ResumeGame();
    }
    public void Continu()
    {
        Close();
    }
    public void Achievements()
    {
        base.Close();
        UIManager.Instance.Open(UIManager.UIType.AchievementsUI);
    }
    public void Settings()
    {
        base.Close();
        UIManager.Instance.Open(UIManager.UIType.SettingsUI);
    }
    public void Statistics()
    {
        base.Close();
        UIManager.Instance.Open(UIManager.UIType.StatisticsUI);
    }
    public void ReturnToTitle()
    {
        ApplicationManager.Instance.ReturnToTitle();
    }
    protected void OnDestroy()
    {
        continu.onClick.RemoveAllListeners();
        achievements.onClick.RemoveAllListeners();
        settings.onClick.RemoveAllListeners();
        statistics.onClick.RemoveAllListeners();
        returnToTitle.onClick.RemoveAllListeners();
    }
}
