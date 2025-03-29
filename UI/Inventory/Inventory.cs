using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	string InventoryPath => Path.Combine(GameManager.PlayerDataPath, "Inventory");
	public void Save()
	{

	}
	[SerializeField] Transform SlotsFrameTransform;

	private void Awake()
	{
		UIManager.Instance.SetInventory(this);
	}
	/// <summary>
	/// 空いてるスロットのtransformを返す。
	/// </summary>
	/// <returns></returns>
	public Transform GetEmptySlotTransform()
	{
		int childCount = SlotsFrameTransform.childCount;
		Transform emptyInventorySlotTransform;
		for (int i = 0; i < childCount; i++)
		{
			emptyInventorySlotTransform = SlotsFrameTransform.GetChild(i);
			if (emptyInventorySlotTransform.childCount == 0)
			{
				return emptyInventorySlotTransform;
			}
		}
		return null;
	}

	public void Open()
	{
		gameObject.SetActive(true);
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}
}
