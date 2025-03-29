using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyGame
{
    public class ItemImageFrame : MonoBehaviour, IPointerEnterHandler
    {
        private void Start()
        {
            if (transform.parent.TryGetComponent(out ItemSlot) == false)
                Debug.LogWarning("Item Slot Prefab is wrong");
        }
        public ItemSlot ItemSlot;

        public void OnPointerEnter(PointerEventData eventData)
        {
            ItemSlot.PointerEnter(eventData);
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
}
