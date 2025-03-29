using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BaseMenuUI : UI
{
    CanvasGroup canvasGroup;
    public float duration = 1f;
    public virtual void Open()
    {
        gameObject.SetActive(true);
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        // Debug.Log("open");
        StartCoroutine(FadeUI(0f, 1f, duration, OnOpenCompleted));
        MyInputSystem.GameInputs.UI.Cancel.canceled += OnCancelButtonPushed;
    }
    protected virtual void OnOpenCompleted()
    {
    }
    protected virtual void OnCancelButtonPushed(InputAction.CallbackContext callback)
    {
    }
    public virtual void Close()
    {
        Close(null);
    }
    public virtual void Close(Action callback)
    {
        canvasGroup.blocksRaycasts = false;
        StartCoroutine(FadeUI(1f, 0f, duration, () =>
        {
            OnCloseCompleted(); // NewGameUIでリリースしてから新しいシーンへ行きたいためこの順番にする。
            callback?.Invoke();
        }));
        MyInputSystem.GameInputs.UI.Cancel.canceled -= OnCancelButtonPushed;
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
}
