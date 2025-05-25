using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIPageBase : UI, IUIPage
{
    public virtual bool IsPermanent => false;
    CanvasGroup canvasGroup;
    [SerializeField] float showDuration = 0.2f;
    public virtual void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public virtual void Show()
    {
        if (gameObject.activeSelf)
        {
            Debug.LogWarning("Show is called but gameObject is already active.");
            return;
        }
        gameObject.SetActive(true);
        StartCoroutine(FadeUI(0f, 1f, showDuration, OnOpenCompleted));
    }
    protected virtual void OnOpenCompleted()
    {
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void Hide()
    {
        if (gameObject.activeSelf == false)
        {
            Debug.LogWarning("Hide is called but gameObject is not active.");
            return;
        }
        canvasGroup.blocksRaycasts = false;
        StartCoroutine(FadeUI(1f, 0f, showDuration, OnCloseCompleted));
    }
    public virtual void HideImd()
    {
        OnCloseCompleted();
        canvasGroup.blocksRaycasts = false;
    }

    protected virtual void OnCloseCompleted()
    {
        gameObject.SetActive(false);
    }
    IEnumerator FadeUI(float startAlpha, float endAlpha, float time, Action callback)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / time);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        callback.Invoke();
    }

    public virtual void OnBack()
    {

    }

    [SerializeField] Button m_backButton;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Awaked");
        Debug.Log($"backButton: {m_backButton}, isNull: {m_backButton == null}");
        m_backButton?.onClick.AddListener(Back);
    }
    void Back()
    {
        Debug.Log("Back");
        UIManager.Instance.Back();
    }

    protected virtual void OnDestroy()
    {
        m_backButton?.onClick.RemoveAllListeners();
    }
}
