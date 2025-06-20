using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : UIPageBase, IItemParentUI
{
	public override bool IsPermanent => true;
	public IItemParent ItemParent { get; private set; }
	[SerializeField] Transform m_itemSlotFrame;

	public void SetItemParent(IItemParent itemParent)
	{
		ItemParent = itemParent;
	}

	/// <summary>
	/// InventorySlotをItemCapacityの数だけ生成する。
	/// </summary>
	/// <param name="slotCount"></param>
	public void InitSlots(int slotCount)
	{
		for (int i = 0; i < slotCount; i++)
		{
			GenAndSetSlot(i);
		}
	}

	public void DetachChildrenUI()
	{
		foreach (Transform slotTrs in m_itemSlotFrame)
		{
			if (slotTrs.TryGetComponent(out InventorySlot slot))
			{
				slot.DetachChildrenUI();
			}
		}
	}

	/// <summary>
	/// 子供のUIをセットする。nullのときはそのUIをなくす。
	/// </summary>
	/// <param name="itemSlot"></param>
	public void SetItemSlot(ItemSlot itemSlot, int index)
	{
		var slot = m_itemSlotFrame.GetChild(index).GetComponent<InventorySlot>();
		if (itemSlot != null)
		{
			slot.SetItemSlot(itemSlot);
		}
		else
		{
			Debug.Log("itemSlot is null");
			slot.DetachChildrenUI();
		}
	}


	public void AddItem(int index, Item item)
	{
		if (ItemParent.CanAddItem(index, item))
			ItemParent.AddItem(index, item);
		else
		{
			Debug.Log("Cannot add item to this slot.");
			// ここでアイテムが入れられなかった時の処理。
			item?.OnAddItemFailed();
		}
	}

	/// <summary>
	/// InventorySlotを追加する。
	/// </summary>
	public void AddSlot()
	{
		GenAndSetSlot(m_itemSlotFrame.childCount);
	}
	void GenAndSetSlot(int index)
	{
		var slot = ResourceManager.Instance.GetOther(ResourceManager.ItemSlotID.InventorySlot.ToString()).GetComponent<InventorySlot>();
		slot.SetID(index);
		slot.SetItemParentUI(this);
		slot.transform.SetParent(m_itemSlotFrame);
	}
}
