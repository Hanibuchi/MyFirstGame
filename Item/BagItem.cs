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
		// var list = new List<Item>(Items);
		// foreach (var item in list)
		// {
		// 	if (item != null)
		// 	{
		// 		RemoveItem(item);
		// 	}
		// }
		// Items.Clear();

		// for (int i = 0; i < m_itemCapacity; i++)
		// 	Items.Add(null);
	}
	// Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。

	// protected override int GetItemsCount()
	// {
	// 	// return Items.Count(a => a != null);
	// }

	// Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
	// Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
}