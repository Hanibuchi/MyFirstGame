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
	public bool IsPlayer => Party != null && Party.IsPlayerParty && IsLeader;

	public bool IsLeader;
	public Party Party;
	[SerializeField] Jobs job;
	public Jobs Job
	{
		get => job;
		private set
		{
			if (value != job)
			{
				job = value;
				OnJobChanged?.Invoke(job);
			}
		}
	}
	public event Action<Jobs> OnJobChanged;
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


	[SerializeField] NPCEquipmentMenu equipmentMenu;
	public NPCEquipmentMenu EquipmentMenu
	{
		get => equipmentMenu;
		private set => equipmentMenu = value;
	}

	string causeOfdeath;

	protected override void ResetToGeneratedStatus()
	{
		if (Data is BaseNPCData npcData)
		{
			Job = npcData.Job;
		}
		base.ResetToGeneratedStatus();
	}

	public void OnJoinParty(Party party, int index)
	{
		Party = party;
		transform.SetParent(party.transform);

		if (party.IsPlayerParty)
		{
			GenerateNPCEquipmentMenu(index);
		}
	}

	public void OnBecomeLeader()
	{
		IsLeader = true;
		// Debug.Log($"{IsPlayer}");
		if (Party.IsPlayerParty)
		{
			if (GetComponent<PlayerController>() == null)
			{
				gameObject.AddComponent<PlayerController>();
			}
			UIManager.Instance.PlayerStatusUI.RegisterStatus(this);
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
		Party = null;
		transform.SetParent(null);
	}

	/// <summary>
	/// NPCEquipmentMenu(UI)を作成。PartyMemberを追加するとき呼び出す。追加するのがリーダーの場合はメインの画面にステータスが出るようにする。
	/// </summary>
	/// <param name="index">NPCEquipmentMenuを追加する位置</param>
	/// <returns></returns>
	public void GenerateNPCEquipmentMenu(int index)
	{
		if (EquipmentMenu != null)
		{
			return;
			// ResourceManager.Release(ResourceManager.UIIDs.NPCEquipmentMenu, EquipmentMenu.gameObject);
			// EquipmentMenu = null;
		}

		GameObject equipmentMenuInstance = ResourceManager.Get(ResourceManager.UIID.NPCEquipmentMenu);
		EquipmentMenu = equipmentMenuInstance.GetComponent<NPCEquipmentMenu>();
		if (index == -1)
			UIManager.Instance.EquipmentMenuManager.InsertMember(EquipmentMenu);
		else
			UIManager.Instance.EquipmentMenuManager.InsertMember(EquipmentMenu, index);

		// NPCの画像を取得
		NPCImage = GetImage();
		// Debug.Log($"NPCImage: {NPCImage}");
		EquipmentMenu.RegisterStatus(this);
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

	/// <summary>
	/// Colliderと重なったアイテムを手持ちに追加。拾うのは一つだけ。
	/// </summary>
	public override void PickupItem()
	{
		List<Collider2D> colliders = DetectNearbyColliders(new ContactFilter2D().NoFilter());

		// 検出されたコライダーの中から，Itemコンポネントを持つものを一つ見つけて手持ちに追加する
		Item itemComponent;
		foreach (Collider2D collider in colliders)
		{
			// Itemクラスの子クラスを持つコンポーネントがあるか確認
			if (collider.TryGetComponent(out itemComponent))
			{
				if (IsPlayer && itemComponent.CanBePickedUp()) // プレイヤーの場合はインベントリへ。
				{
					itemComponent.AddToInventory();
				}
				else
				{
					AddItemToEmptySlot(itemComponent);
				}
				return;
			}
		}
	}

	protected override void ResetToBase()
	{
		base.ResetToBase();
		GetComponent<Rigidbody2D>().freezeRotation = true;
		transform.rotation = Quaternion.identity;
	}

	public override void Die()
	{
		base.Die();
		GetComponent<Rigidbody2D>().freezeRotation = false;
		if (Party != null)
		{
			if (IsLeader)
				Party.GameOver(causeOfdeath);
			else
				Party.RemoveMember(this);
		}
	}
	public void Surrender()
	{
		gameObject.layer = LayerMask.NameToLayer(GameManager.Layer.Ally.ToString());
		Party = null;
	}

	/// <summary>
	/// 雇うのにかかる費用。新たに忠誠度というパラメータを増やし，PlayerPartyに長くいるほど増えるように，この値に追加する。
	/// </summary>
	/// <returns></returns>
	public float GetHiringCost()
	{
		return BaseLevel * 10 + 100;
	}

	// EquipmentMenu(UI)を削除。PartyMemberを削除するとき呼び出す。
	public void DestroyEquipmentMenu()
	{
		if (EquipmentMenu == null)
			return;

		ResourceManager.Release(ResourceManager.UIID.NPCEquipmentMenu, EquipmentMenu.gameObject);
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
				Party.AddMember(opponentNPCManager);
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
		Release();
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
			npcRestoreData.Job = Job;
		}
		return npcData;
	}

	public void ApplyNPCData(NPCData npcData)
	{
		ApplyMobData(npcData);
		Job = npcData.Job;
	}

	public void OnRespawn()
	{
		ResetToBase();
	}

	public static NPCManager SpawnNPC(NPCData npc)
	{
		var npcObj = ResourceManager.Get(npc.MobID);
		if (npcObj == null) { Debug.LogWarning("npc is null"); return null; }
		if (npcObj.TryGetComponent(out NPCManager npcManager))
		{
			npcManager.ApplyNPCData(npc);
		}
		return npcManager;
	}
}


public enum Jobs
{
	Fighter,

}