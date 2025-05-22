using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public class EquipmentUI : UI, IEquipmentUI
{
    /// <summary>
    /// EquipmentMenuが開いているか
    /// </summary>
    public bool IsOpen => gameObject.activeInHierarchy;
    [SerializeField] Transform m_memberEquipmentUIFrame;

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.SetEquipmentMenuManager(this);
        Close();
    }

    /// <summary>
    /// EquipmentMenuを閉じたり開いたりする。
    /// </summary>
    /// <returns>開いたらtrue，閉じたらfalse</returns>
    public bool ToggleEquipmentMenu()
    {
        if (IsOpen)
            Close();
        else
            Open();
        return IsOpen;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void DetachMemberUIs()
    {
        m_memberEquipmentUIFrame.DetachChildren();
    }

    public void SetMemberUI(GameObject memberEquipmentUI)
    {
        memberEquipmentUI.transform.SetParent(m_memberEquipmentUIFrame);
    }
}
