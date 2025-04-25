using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IItemOwner : IItemParent
{
	int ItemCapacity { get; set; }
	void RegisterItem(Item item);
	void UnregisterItem(Item item);
	/// <summary>
	/// Itemsをリセットし，ItemCapacity個のnullをつめる。
	/// </summary>
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

		for (int i = 0; i < ItemCapacity; i++)
			Items.Add(null);
	}
	/// <summary>
	/// Itemsの枠を増やす。ResetItemsとここ以外ではItems.Addは使用しない。
	/// </summary>
	void AddItemSlot()
	{
		ItemCapacity++;
		Items.Add(null);
	}
}
