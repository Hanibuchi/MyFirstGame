using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyInputSystem : MonoBehaviour
{
    public static MyInputSystem Instance { get; private set; }
    public static GameInputs GameInputs { get; private set; }
    public string CancelKey => GameInputs.FindAction("RebindCancel").bindings[0].effectivePath;
    public string DeleteKey => GameInputs.FindAction("RebindDelete").bindings[0].effectivePath;
    public string ResetKey => GameInputs.FindAction("RebindReset").bindings[0].effectivePath;
    public enum ActionType
    {
        Left,
        Right,
        Jump,
        Down,
        Interact,
        Fire,
        Throw,
        Kick,
        Sleep,
        EquipmentMenu,
        Map,
        Pause,

        SelectItemUp,
        SelectItemDown,
        Item1,
        Item2,
        Item3,
        Item4,
        Item5,
        Item6,
        Item7,
        Item8,

        RebindCancel,
        RebindDelete,
        RebindReset,
    }
    public enum StateType
    {
        None,
        Rebinding,
    }
    public StateType State { get; private set; }

    public delegate void RebindDelegate(InputAction inputAction, int index);
    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void OnAppStart()
    {
        Instance = this;
        GameInputs = new GameInputs();
        GameInputs.Enable();
    }

    /// <summary>
    /// ゲーム開始時に呼び出される。
    /// </summary>
    public void OnGameStart()
    {
        RegisterCallbacksOnGameStart(true);
    }

    int ccgs = 0;
    void RegisterCallbacksOnGameStart(bool doRegist)
    {
        if (doRegist)
        {
            if (ccgs >= 1)
                return;
            ccgs++;
            GameInputs.UI.Pause.canceled += OnPauseButtonPushed;
            GameInputs.UI.EquipmentMenu.performed += OnEquipmentMenuButtonPushed;
            Debug.Log("register callback");
        }
        else
        {
            ccgs = Math.Max(0, ccgs - 1);
            GameInputs.UI.EquipmentMenu.performed -= OnEquipmentMenuButtonPushed;
            GameInputs.UI.Pause.canceled -= OnPauseButtonPushed;
        }
    }

    void OnPauseButtonPushed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameState == GameManager.GameStateType.Playing)
        {
            Debug.Log("aaa");
            UIManager.Instance.Open(UIManager.UIType.PauseUI);
        }
    }

    void OnEquipmentMenuButtonPushed(InputAction.CallbackContext context)
    {
        UIManager.Instance.EquipmentUI.ToggleEquipmentMenu();

        if (UIManager.Instance.EquipmentUI.IsOpen) // 開けるとき
        {
            PlayerController.OnEquipmentMenuOpenEventHandler?.Invoke();

            DragSystem.Instance.OnOpenEquipmentMenu();
        }
        else // 閉じるとき
        {
            DragSystem.Instance.OnCloseEquipmentMenu();
        }
    }

    /// <summary>
    /// ゲーム実行時のキー入力の登録を解除する。
    /// </summary>
    public void OnReturnToTitle()
    {
        RegisterCallbacksOnGameStart(false);
    }

    RebindDelegate endRebindCallback;
    RebindDelegate cancelCallback;
    RebindDelegate deleteCallback;
    RebindDelegate resetCallback;
    public void StartRebind(InputAction inputAction, int index, RebindDelegate endRebindCallback, RebindDelegate cancelCallback, RebindDelegate deleteCallback, RebindDelegate resetCallback)
    {
        rebindingOperation?.Cancel();
        inputAction.Disable();
        State = StateType.Rebinding;
        this.endRebindCallback = endRebindCallback;
        this.cancelCallback = cancelCallback;
        this.deleteCallback = deleteCallback;
        this.resetCallback = resetCallback;

        rebindingOperation = inputAction
        .PerformInteractiveRebinding(index)
        .WithCancelingThrough(CancelKey)
        .OnComplete(oper =>
        {
            string newKey = GetKeyName(oper.selectedControl.path);
            string deleteKey = GetKeyName(DeleteKey);
            string resetKey = GetKeyName(ResetKey);
            if (newKey == deleteKey)
            {
                Debug.Log($"delete: new:{newKey}, DeleteKey: {deleteKey}");
                Delete(inputAction, index);
            }
            else if (newKey == resetKey)
            {
                Debug.Log($"Reset: new: {newKey}, DeleteKey: {resetKey}");
                ResetKeyBind(inputAction, index);
            }
            else
                EndRebind(inputAction, index);

            OnFinished();
        })
        .OnCancel(_ =>
        {
            Cancel(inputAction, index);
            OnFinished();
        })
        .Start();

        void OnFinished()
        {
            CleanUpOperation();
            inputAction.Enable();
            State = StateType.None;
        }
    }
    string GetKeyName(string path)
    {
        return path[(path.LastIndexOf('/') + 1)..];
    }

    void CleanUpOperation()
    {
        rebindingOperation?.Dispose();
        rebindingOperation = null;
        endRebindCallback = null;
        cancelCallback = null;
        deleteCallback = null;
        resetCallback = null;
    }

    void EndRebind(InputAction inputAction, int index)
    {
        endRebindCallback.Invoke(inputAction, index);
    }

    public void Cancel(InputAction inputAction, int index)
    {
        cancelCallback.Invoke(inputAction, index);
    }

    public void Delete(InputAction inputAction, int index)
    {
        int max = inputAction.bindings.Count - 1;
        if (index <= -1 || index > max)
        {
            Debug.LogWarning("index is out of range");
            return;
        }
        for (int i = index; i < max; i++)
        {
            inputAction.ApplyBindingOverride(i, inputAction.bindings[i + 1].effectivePath);
        }
        inputAction.ApplyBindingOverride(max, string.Empty);

        deleteCallback.Invoke(inputAction, index);
    }

    public void ResetKeyBind(InputAction inputAction, int index)
    {
        inputAction.RemoveBindingOverride(index);

        resetCallback.Invoke(inputAction, index);
    }
}
