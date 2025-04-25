using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemImageFrame : MonoBehaviour, IDropHandler//, IPointerEnterHandler
{
	[SerializeField] ItemSlot m_itemSlot;

	public void OnDrop(PointerEventData eventData)
	{
		m_itemSlot.OnDrop(eventData);
	}

	// これら2つはItemSlotの方に担当してもらう。
	// public void OnPointerExit(PointerEventData eventData)
	// {
	//     ItemSlot.PointerExit(eventData);
	// }

	// // OnPointerEnterで既にCanAddItemで入れられるか判定しているため，ここでは判定しない。 
	// public void OnDrop(PointerEventData eventData)
	// {
	//     ItemSlot.Drop(eventData);
	// }
}

