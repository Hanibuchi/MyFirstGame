using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BagItem : Item
{
	public string InvSlotID { get; private set; }

	protected override void Init()
	{
		base.Init();
		if (m_itemData is BagData bagData)
		{
			InvSlotID = bagData.invSlotID.ToString();
		}
	}
	void ResetItems()
	{
		var list = new List<Item>(Items);
		foreach (var item in list)
		{
			if (item != null)
			{
				RemoveItem(item);
			}
		}
		Items.Clear();

		for (int i = 0; i < m_itemCapacity; i++)
			Items.Add(null);
	}
	public override void InitItems()
	{
		ResetItems();
	}
	// Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
	public override bool CanAddItem(Item item)
	{
		Debug.Log($"item != null: {item != null}");
		Debug.Log($"IsWithinItemLimits(item): {IsWithinItemLimits(item)}");
		Debug.Log($"");
		if (IsWithinItemLimits(item) && Items.Count(x => x == null) >= 1)
			return true;
		else
		{
			foreach (var nextItem in Items)
			{
				if (nextItem is BagItem bag)
				{
					if (bag.CanAddItem(item))
						return true;
				}
			}
		}
		return false;
	}
	// Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
	public override bool CanAddItem(int index, Item item)
	{
		Debug.Log($"0 <= index: {0 <= index}");
		Debug.Log($"index < Items.Count: {index < Items.Count}");
		Debug.Log($"item != null: {item != null}");
		Debug.Log($"IsWithinItemLimits(item): {IsWithinItemLimits(item)}");
		Debug.Log($"Items[index] == null: {Items[index] == null}");
		Debug.Log($"");
		if (0 <= index && index < Items.Count && item != null && IsWithinItemLimits(item) && Items[index] == null)
		{
			return true;
		}
		else
			return false;
	}

	protected override int GetItemsCount()
	{
		return Items.Count(a => a != null);
	}

	// Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
	public override void AddItem(Item item)
	{
		Debug.Log("BagItem.AddItem");
		if (CanAddItem(item))
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i] == null && CanAddItem(i, item))
				{
					Debug.Log($"item: {item}");
					AddItem(i, item);
					return;
				}
			}
			foreach (var nextItem in Items)
			{
				if (nextItem is BagItem bag && bag.CanAddItem(item))
				{
					bag.AddItem(item);
					return;
				}
			}
		}
	}
	// Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
	public override void AddItem(int index, Item item)
	{
		Debug.Log($"item: {item}");
		if (CanAddItem(index, item))
		{
			item.RemovePrevRelation();
			Items[index] = item;
			item.OnAdded(this);
		}
		else
			Debug.LogWarning("item cannot be added");
	}
	public override void RemoveItem(Item item)
	{
		if (Items.Contains(item))
		{
			int index = Items.IndexOf(item);
			Items[index] = null;
			item.OnRemoved();
		}
		else
			Debug.LogWarning("item is not in Items");
	}
}