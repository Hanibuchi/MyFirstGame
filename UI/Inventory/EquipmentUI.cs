using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public class EquipmentUI : UIPageBase, IEquipmentUI
{
    override public bool IsPermanent => true;
    /// <summary>
    /// EquipmentMenuが開いているか
    /// </summary>
    public bool IsOpen => gameObject.activeInHierarchy;
    [SerializeField] Transform m_memberEquipmentUIFrame;
    public void DetachMemberUIs()
    {
        m_memberEquipmentUIFrame.DetachChildren();
    }

    public void SetMemberUI(GameObject memberEquipmentUI)
    {
        memberEquipmentUI.transform.SetParent(m_memberEquipmentUIFrame);
    }
}
