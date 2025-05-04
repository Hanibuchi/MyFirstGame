// using System.Collections;
// using System.Collections.Generic;
// using MyGame;
// using UnityEngine;
// using UnityEngine.EventSystems;

// public class SlotSpacing : UI, IPointerEnterHandler, IDropHandler
// {
//     public ItemSlot OwnerItemSlot; // このスロットを持っているアイテム
//     public Transform PartnerItemSlot; // 左側にいるパートナーのアイテムスロット。

//     private void Start()
//     {
//         OwnerItemSlot = transform.parent.GetComponent<ItemSlot>();
//     }

//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         // Debug.Log($"OnPointerEnter: {gameObject.name}");
//         GameObject draged = eventData.pointerDrag;
//         if (draged != null && draged.transform != PartnerItemSlot && !OwnerItemSlot.IsAncestor(draged.transform) && draged.TryGetComponent<ItemSlot>(out var itemSlot))
//         {
//             // 入所希望のアイテムがItemに入れるか検査。OKだったらUIに反映。
//             if (OwnerItemSlot.CanAddItem(itemSlot))
//             {
//                 OwnerItemSlot.SetItemSlot(itemSlot, transform.GetSiblingIndex() + 1);
//                 itemSlot.isFixed = true;
//                 // Destroy(gameObject);
//             }
//             else
//                 Debug.LogWarning("The item cannot be added");
//         }
//     }

//     public void OnPointerExit(PointerEventData eventData)
//     {
//         Debug.Log("OnPOinterExit");
//         GameObject draged = eventData.pointerDrag;

//         if (draged != null && draged != OwnerItemSlot.gameObject && draged.TryGetComponent<ItemSlot>(out var itemSlot))
//             itemSlot.DragSetup();

//         // eventData.Use();
//     }

//     public void OnDrop(PointerEventData eventData)
//     {
//         // アイテムを落としたら子に追加する
//         GameObject dropped = eventData.pointerDrag;
//         if (dropped != null && dropped.TryGetComponent(out ItemSlot itemSlot))
//             OwnerItemSlot.ProcessOnDrop(itemSlot);

//         eventData.Use(); // これするとこれ以降別のオブジェクトでOnDropが呼び出されなくなる。
//     }
// }
