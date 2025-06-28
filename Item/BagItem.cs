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
}