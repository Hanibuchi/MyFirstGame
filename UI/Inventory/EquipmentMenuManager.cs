using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public class EquipmentMenuManager : UI
{
    /// <summary>
    /// EquipmentMenuが開いているか
    /// </summary>
    public bool IsOpen => gameObject.activeInHierarchy;
    [SerializeField] EquipmentMenuFrame EquipmentMenuFrame;

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.SetEquipmentMenuManager(this);
        Close();
    }

    public void InsertMember(NPCEquipmentMenu npcEquipmentMenu, int index)
    {
        EquipmentMenuFrame.InsertMember(npcEquipmentMenu, index);
    }
    public void InsertMember(NPCEquipmentMenu npcEquipmentMenu)
    {
        EquipmentMenuFrame.InsertMember(npcEquipmentMenu);
    }

    public void SwitchMembers(NPCEquipmentMenu npc1, NPCEquipmentMenu npc2)
    {
        EquipmentMenuFrame.SwitchMembers(npc1, npc2);
    }

    public void RemoveMember(NPCEquipmentMenu npcEquipmentMenu)
    {
        EquipmentMenuFrame.RemoveMember(npcEquipmentMenu);
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
}
