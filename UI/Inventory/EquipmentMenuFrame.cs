using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public class EquipmentMenuFrame : UI
{
	/// <summary>
	/// NPCEquipmentMenuをindex番目に挿入する。
	/// </summary>
	/// <param name="index"></param>
	/// <param name="npcEquipmentMenu"></param>
	public void InsertMember(NPCEquipmentMenu npcEquipmentMenu, int index)
	{
		if (npcEquipmentMenu == null || index < 0)
		{
			Debug.LogWarning("Argument is not valid");
			return;
		}

		if (!npcEquipmentMenu.transform.IsChildOf(transform))
		{
			AddMember(npcEquipmentMenu);
		}

		npcEquipmentMenu.transform.SetSiblingIndex(index);
	}
	public void InsertMember(NPCEquipmentMenu npcEquipmentMenu)
	{
		InsertMember(npcEquipmentMenu, transform.childCount);
	}

	/// <summary>
	/// 2つのNPCEquipmentMenuの場所を交換する。
	/// </summary>
	/// <param name="npc1"></param>
	/// <param name="npc2"></param>
	public void SwitchMembers(NPCEquipmentMenu npc1, NPCEquipmentMenu npc2)
	{
		if (npc1 == null || npc2 == null)
		{
			Debug.LogWarning("args are not valid");
		}

		int firstIndex = npc1.transform.GetSiblingIndex();
		int secondIndex = npc2.transform.GetSiblingIndex();

		npc1.transform.SetSiblingIndex(secondIndex);
		npc2.transform.SetSiblingIndex(firstIndex);
	}

	void AddMember(NPCEquipmentMenu npcEquipmentMenu)
	{
		if (npcEquipmentMenu == null)
		{
			Debug.LogWarning("Argument is not valid");
			return;
		}
		npcEquipmentMenu.transform.SetParent(transform);
	}

	public void RemoveMember(NPCEquipmentMenu npcEquipmentMenu)
	{
		if (npcEquipmentMenu == null)
		{
			Debug.LogWarning("Argument is not valid");
			return;
		}
		npcEquipmentMenu.transform.SetParent(null);
	}
}
