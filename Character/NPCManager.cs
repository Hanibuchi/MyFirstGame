using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Linq;
using MyGame;

// MobManagerとの違いはPartyを持つかどうか。
public class NPCManager : MobManager
{
	public bool IsLeader;
	[SerializeField] Sprite npcImage;
	public Sprite NPCImage
	{
		get => npcImage;
		protected set
		{
			npcImage = value;
			OnNPCImageChanged?.Invoke(npcImage);
		}
	}
	public event Action<Sprite> OnNPCImageChanged;


	protected override void ResetToGeneratedStatus()
	{
		base.ResetToGeneratedStatus();
	}

	Sprite GetImage()
	{
		var npcSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if (npcSpriteRenderer != null)
		{
			return npcSpriteRenderer.sprite; // npcImageが取得できた場合の処理
		}
		else
		{
			Debug.LogWarning("SpriteRenderer component not found on the child object.");
			return null;
		}
	}









	public override void RefreshUI()
	{
		Debug.Log("RefreshItemSlotUIs");
		// EquipmentMenuUI.DetachChildrenUI();

		// for (int i = 0; i < Items.Count; i++)
		// {
		// 	ItemSlot itemSlot = null;
		// 	if (Items[i] != null)
		// 	{
		// 		Debug.Log($"Item: {Items[i]}");
		// 		itemSlot = Items[i].GetItemSlotUI();
		// 		if (itemSlot == null)
		// 		{
		// 			Items[i].RefreshUI();
		// 			itemSlot = Items[i].GetItemSlotUI();
		// 		}
		// 	}
		// 	EquipmentMenuUI.SetItemSlot(itemSlot, i);
		// }
	}









	/// <summary>
	/// 雇うのにかかる費用。新たに忠誠度というパラメータを増やし，PlayerPartyに長くいるほど増えるように，この値に追加する。
	/// </summary>
	/// <returns></returns>
	public float GetHiringCost()
	{
		return 0;
	}

	/// <summary>
	/// NPCと会話する
	/// </summary>
	public void InteractNPC()
	{
		List<Collider2D> colliders = DetectNearbyColliders(new ContactFilter2D().NoFilter());

		// 検出されたコライダーの中から，NPCManagerコンポネントを持つものを一つ見つけて手持ちに追加する。そのうち会話するだけにとどめる。
		foreach (Collider2D collider in colliders)
		{
			// NPCManagerコンポネントがあるか確認
			if (collider.TryGetComponent(out NPCManager opponentNPCManager))
			{
				// ここを変更
				// OwnerParty.AddMember(opponentNPCManager);
				return;
			}
		}
	}

	public override void OnChunkDeactivate()
	{
		Deactivate();
	}

	void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public override void OnChunkActivate()
	{
		Activate();
	}

	void Activate()
	{
		gameObject.SetActive(true);
	}

	public NPCData MakeNPCDataAndRelease()
	{
		NPCData npcData = MakeNPCData();
		// Release();
		return npcData;
	}

	public NPCData MakeNPCData()
	{
		return FillNPCData(new NPCData());
	}

	protected NPCData FillNPCData(NPCData npcData)
	{
		FillMobData(npcData);
		if (npcData is NPCData npcRestoreData)
		{
		}
		return npcData;
	}

	public void ApplyNPCData(NPCData npcData)
	{
		ApplyMobData(npcData);
	}


	// public static NPCManager SpawnNPC(NPCData npc)
	// {
	// 	var npcObj = m_resourceManager.GetMob(npc.MobID);
	// 	if (npcObj == null) { Debug.LogWarning("npc is null"); return null; }
	// 	if (npcObj.TryGetComponent(out NPCManager npcManager))
	// 	{
	// 		npcManager.ApplyNPCData(npc);
	// 	}
	// 	return npcManager;
	// }
}
