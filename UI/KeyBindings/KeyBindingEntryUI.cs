using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyBindingEntryUI : UI
{
    KeyBindingsUI BossKeyBindingsUI;
    [SerializeField] int MaxKeyNum;
    public MyInputSystem.ActionType ActionType { get; private set; }
    [SerializeField] List<KeyBindingKeyUI> KeyBindingKeyUIs = new();
    [SerializeField] List<string> KeyPaths;
    [SerializeField] Transform KeyBindingKeyUIFrameTf;
    [SerializeField] TMP_Text ActionNameFrame;
    [SerializeField] Button plusButton;
    KeyBindingKeyUI rebindingBind;
    public void Init(KeyBindingsUI keyBindingsUI, MyInputSystem.ActionType action, int maxKeyNum)
    {
        BossKeyBindingsUI = keyBindingsUI;
        ActionType = action;
        ActionNameFrame.text = action.ToString(); // 後々言語別のにできるようにする
        MaxKeyNum = maxKeyNum;
        plusButton.onClick.AddListener(Add);

        AddKeyBindingKeyUI();
    }

    public void StartRebind(int index)
    {
        rebindingBind = KeyBindingKeyUIs[index];
        BossKeyBindingsUI.StartRebind(ActionType, index);
    }

    public void EndRebind(string keyPath)
    {
        KeyPaths[rebindingBind.Index] = keyPath;
        rebindingBind.SetDisplayKeyName(keyPath);
        CleanUpOperation();
    }

    public void Cancel()
    {
        rebindingBind.SetDisplayKeyName(KeyPaths[rebindingBind.Index]);
        CleanUpOperation();
    }

    public void Delete()
    {
        KeyPaths.RemoveAt(rebindingBind.Index);
        KeyBindingKeyUIs.Remove(rebindingBind);
        rebindingBind.Delete();
        CleanUpOperation();
    }

    public void ResetKeyBind(string keyPath)
    {
        // var inputAction = InputSystem.GameInputs.FindAction(ActionType.ToString());
        // if (inputAction == null)
        // {
        //     Debug.LogWarning("inputAction is null");
        //     return;
        // }
        // string keyPath = string.Empty;
        // int index = rebindingBind.Index;
        // if (index < inputAction.bindings.Count)
        //     keyPath = inputAction.bindings[index].path;
        // KeyPaths[index] = keyPath;
        KeyPaths[rebindingBind.Index] = keyPath;
        rebindingBind.SetDisplayKeyName(keyPath);
        CleanUpOperation();
    }

    void CleanUpOperation()
    {
        rebindingBind = null;
    }

    public override void OnRelease()
    {
        base.OnRelease();
        plusButton.onClick.RemoveAllListeners();
    }

    public void Add()
    {
        if (KeyBindingKeyUIs.Count < MaxKeyNum)
        {
            AddKeyBindingKeyUI();
        }
    }

    InputAction GetInputAction()
    {
        var inputAction = MyInputSystem.GameInputs.FindAction(ActionType.ToString());
        if (inputAction == null)
        {
            Debug.LogWarning("cannot find this action");
        }
        return inputAction;
    }

    void AddKeyBindingKeyUI()
    {
        var tmp = ResourceManager.Instance.GetOther(ResourceManager.UIID.KeyBindingKeyUI.ToString());
        tmp.transform.SetParent(KeyBindingKeyUIFrameTf);
        if (tmp.TryGetComponent(out KeyBindingKeyUI keyBindingKeyUI))
        {
            var inputAction = GetInputAction();
            string keyPath;
            if (inputAction != null)
            {
                var bindings = inputAction.bindings;
                if (bindings.Count >= KeyBindingKeyUIs.Count + 1)
                    keyPath = bindings[KeyBindingKeyUIs.Count].effectivePath;
                else keyPath = string.Empty;
            }
            else
                keyPath = string.Empty;

            keyBindingKeyUI.Init(this, KeyBindingKeyUIs.Count, keyPath);
            KeyBindingKeyUIs.Add(keyBindingKeyUI);
            KeyPaths.Add(keyPath);
        }
        else
            Debug.LogWarning("keyBindingKeyUI is null");
    }
}
