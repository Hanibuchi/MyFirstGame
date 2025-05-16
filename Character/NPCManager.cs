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


	[SerializeField] NPCEquipmentMenu m_equipmentMenu;
	public NPCEquipmentMenu EquipmentMenu
	{
		get
		{
			if (m_equipmentMenu == null)
			{
				m_equipmentMenu = ResourceManager.GetOther(ResourceManager.UIID.NPCEquipmentMenu.ToString()).GetComponent<NPCEquipmentMenu>();
				m_equipmentMenu.SetItemParent(this);
				m_equipmentMenu.RegisterStatus(gameObject);
			}
			return m_equipmentMenu;
		}
		private set => m_equipmentMenu = value;
	}

	string causeOfdeath;

	protected override void ResetToGeneratedStatus()
	{
		base.ResetToGeneratedStatus();
	}

	public void OnJoinParty(Party party, int index)
	{
		OwnerParty = party;
		transform.SetParent(party.transform);

		if (party.IsPlayerParty)
		{
			MoveEquipmentMenuUI(index);
		}
	}

	public void OnBecomeLeader()
	{
		IsLeader = true;
		// Debug.Log($"{IsPlayer}");
		if (OwnerParty.IsPlayerParty)
		{
			if (GetComponent<PlayerController>() == null)
			{
				gameObject.AddComponent<PlayerController>();
			}
			UIManager.Instance.PlayerStatusUI.RegisterStatus(gameObject);
			GameManager.Instance.SetPlayerNPCManager(this);
		}
	}

	public void OnResignLeader()
	{
		IsLeader = false;
		if (TryGetComponent(out PlayerController playerController))
			Destroy(playerController);
	}

	public void OnLeaveParty()
	{
		OwnerParty = null;
		transform.SetParent(null);
	}

	/// <summary>
	/// NPCEquipmentMenuを移動させる。
	/// </summary>
	/// <param name="index">NPCEquipmentMenuを追加する位置</param>
	/// <returns></returns>
	public void MoveEquipmentMenuUI(int index)
	{
		if (index == -1)
			UIManager.Instance.EquipmentMenuManager.InsertMember(EquipmentMenu);
		else
			UIManager.Instance.EquipmentMenuManager.InsertMember(EquipmentMenu, index);

		// NPCの画像を取得
		// NPCImage = GetImage();
		// Debug.Log($"NPCImage: {NPCImage}");
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









	public override void RefreshItemSlotUIs()
	{
		Debug.Log("RefreshItemSlotUIs");
		EquipmentMenu.DetachChildrenUI();

		for (int i = 0; i < Items.Count; i++)
		{
			ItemSlot itemSlot = null;
			if (Items[i] != null)
			{
				Debug.Log($"Item: {Items[i]}");
				itemSlot = Items[i].GetItemSlotUI();
				if (itemSlot == null)
				{
					Items[i].RefreshItemSlotUIs();
					itemSlot = Items[i].GetItemSlotUI();
				}
			}
			EquipmentMenu.SetItemSlot(itemSlot, i);
		}
	}
	public override void AddItemSlot()
	{
		base.AddItemSlot();
		EquipmentMenu.AddSlot();
	}









	protected override void ResetToBase()
	{
		base.ResetToBase();
		GetComponent<Rigidbody2D>().freezeRotation = true;
		transform.rotation = Quaternion.identity;
	}

	public  void Die()
	{
		GetComponent<Rigidbody2D>().freezeRotation = false;
		if (OwnerParty != null)
		{
			if (IsLeader)
				OwnerParty.GameOver(causeOfdeath);
			else
				OwnerParty.RemoveMember(this);
		}
	}
	public void Surrender()
	{
		gameObject.layer = LayerMask.NameToLayer(GameManager.Layer.Ally.ToString());
		OwnerParty = null;
	}

	LevelHandler levelHandler;

	/// <summary>
	/// 雇うのにかかる費用。新たに忠誠度というパラメータを増やし，PlayerPartyに長くいるほど増えるように，この値に追加する。
	/// </summary>
	/// <returns></returns>
	public float GetHiringCost()
	{
		return levelHandler.BaseLevel * 10 + 100;
	}

	// EquipmentMenu(UI)を削除。PartyMemberを削除するとき呼び出す。
	public void DestroyEquipmentMenu()
	{
		if (EquipmentMenu == null)
			return;

		ResourceManager.ReleaseOther(ResourceManager.UIID.NPCEquipmentMenu.ToString(), EquipmentMenu.gameObject);
		EquipmentMenu = null;
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
				OwnerParty.AddMember(opponentNPCManager);
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

	public void OnRespawn()
	{
		ResetToBase();
	}

	public static NPCManager SpawnNPC(NPCData npc)
	{
		var npcObj = ResourceManager.GetMob(npc.MobID);
		if (npcObj == null) { Debug.LogWarning("npc is null"); return null; }
		if (npcObj.TryGetComponent(out NPCManager npcManager))
		{
			npcManager.ApplyNPCData(npc);
		}
		return npcManager;
	}
}
