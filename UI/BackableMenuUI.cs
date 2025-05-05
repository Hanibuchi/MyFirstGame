using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.Collections.LowLevel.Unsafe;

public class BackableMenuUI : BaseMenuUI
{
    public Button backButton;

    protected override void Awake()
    {
        base.Awake();
        backButton?.onClick.AddListener(Back);
    }
    protected override void OnCancelButtonPushed(InputAction.CallbackContext callback)
    {
        Back();
    }
    public virtual void Back()
    {
        Debug.Log($"{ApplicationManager.Instance.CurrentScene.ToString()}");
        Action callback = ApplicationManager.Instance.CurrentScene switch
        {
            ApplicationManager.SceneType.TitleScene => () => UIManager.Instance.Open(UIManager.UIType.TitleUI),
            ApplicationManager.SceneType.MainGameScene => () => UIManager.Instance.Open(UIManager.UIType.PauseUI),
            _ => null,
        };
        Close(callback);
    }

    protected virtual void OnDestroy()
    {
        backButton?.onClick.RemoveAllListeners();
    }
}
