using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemParent
{
	Party OwnerParty { get; set; }
	IItemOwner Owner { get; set; }
	List<Item> Items { get; }

	void InitItems();
	/// <summary>
	/// アイテムを追加できるかどうか。同名のindexを指定する方は直下に追加できるかを確認するが，これは直下のBagに入れられるのなら入れる。
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	bool CanAddItem(Item item);
	/// <summary>
	/// アイテムを追加できるかどうか。直下のBagが空いていても，indexの示す場所にアイテムを入れられないならfalseを返す。
	/// </summary>
	/// <param name="index"></param>
	/// <param name="item"></param>
	/// <returns></returns>
	bool CanAddItem(int index, Item item);

	/// <summary>
	/// アイテムを追加する。直下のBagが空いていればそこにも追加する。
	/// </summary>
	/// <param name="item"></param>
	void AddItem(Item item);
	/// <summary>
	/// アイテムを追加する。指定されたindexの場所にアイテムを入れる。Bagは考慮しない
	/// </summary>
	/// <param name="index"></param>
	/// <param name="item"></param>
	void AddItem(int index, Item item);
	void RemoveItem(Item item);

	/// <summary>
	/// 直下のItemsのUIたちを更新する。
	/// </summary>
	void RefreshItemSlotUIs();
}
