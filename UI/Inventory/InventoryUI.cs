using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : UIPageBase, IItemParentUI, IPartyInventoryUI
{
	public override bool IsPermanent => true;
	public Party Party { get; private set; }
	[SerializeField] Transform m_itemSlotFrame;

	public void SetParty(Party party)
	{
		Party = party;
	}

	public void AddItem(int index, Item item)
	{
		if (Party.ItemHolder.CanAddItemAt(index, item.ItemHolder))
			Party.ItemHolder.AddItemAt(index, item.ItemHolder);
		else
		{
			Debug.Log("Cannot add item to this slot.");
			// ここでアイテムが入れられなかった時の処理。
			item?.OnAddItemFailed();
		}
	}
	InventorySlot GenAndSetSlot(int index)
	{
		var slot = ResourceManager.Instance.GetOther(ResourceManager.ItemSlotID.InventorySlot.ToString()).GetComponent<InventorySlot>();
		slot.SetID(index);
		slot.SetItemParentUI(this);
		slot.transform.SetParent(m_itemSlotFrame);
		return slot;
	}

	public void UpdateInventoryUI(List<IChildItemHolder> itemHolders)
	{
		foreach (Transform slotTrs in m_itemSlotFrame)
		{
			if (slotTrs.TryGetComponent(out InventorySlot slot))
			{
				slot.DetachChildrenUI();
			}
			if (slotTrs.TryGetComponent(out PoolableResourceComponent poolable))
			{
				poolable.Release();
			}
		}

		for (int i = 0; i < itemHolders.Count; i++)
		{
			InventorySlot slot = GenAndSetSlot(i);

			ItemSlot itemSlot;
			var itemHolder = itemHolders[i];
			if (itemHolder != null)
			{
				var item = itemHolder.GetItem();
				itemSlot = item.GetItemSlotUI();
				if (itemSlot == null)
				{
					item.RefreshUI();
					itemSlot = item.GetItemSlotUI();
				}
				slot.SetItemSlot(itemSlot);
			}
		}
	}
}
