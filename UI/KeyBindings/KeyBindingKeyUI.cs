using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyBindingKeyUI : UI
{
    public KeyBindingEntryUI BossKeyBindingEntryUI { get; private set; }
    public int Index { get; private set; }
    [SerializeField] Button button;
    [SerializeField] TMP_Text tmp_Text;
    [SerializeField] bool IsInitialized = false;
    public void Init(KeyBindingEntryUI keyBindingEntry, int index, string key)
    {
        if (!IsInitialized)
        {
            // tmp_Text = GetComponentInChildren<TMP_Text>();
            BossKeyBindingEntryUI = keyBindingEntry;
            SetIndex(index);
            SetDisplayKeyName(key);
            // button = GetComponent<Button>();
            button.onClick.AddListener(StartRebind);

            IsInitialized = true;
        }
        else
            Debug.LogWarning("already initialized");
    }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public void StartRebind()
    {
        BossKeyBindingEntryUI.StartRebind(Index);
        SetDisplayKeyName("?");
    }

    public void SetDisplayKeyName(string keyPath)
    {
        if (keyPath == string.Empty)
        {
            tmp_Text.text = keyPath;
        }
        else
        {
            InputControl control = InputSystem.FindControl(keyPath);
            string key = control?.displayName ?? keyPath;
            tmp_Text.text = key;
        }
    }

    public void Delete()
    {
        GetComponent<PoolableResourceComponent>().Release();
    }

    public override void OnRelease()
    {
        base.OnRelease();
        if (IsInitialized)
        {
            // tmp_Text = null;
            button.onClick.RemoveListener(StartRebind);
            // button = null;
            IsInitialized = false;
        }
    }
}
