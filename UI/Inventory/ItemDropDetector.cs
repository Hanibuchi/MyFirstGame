using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using MyGame;

public class ItemDropDetector : MonoBehaviour, IDropHandler
{
    // アイテムをイベントリから投げ飛ばす。
    public void OnDrop(PointerEventData eventData)
    {
        GameObject itemSlotObject = eventData.pointerDrag;
        if (itemSlotObject != null && itemSlotObject.TryGetComponent(out ItemSlot itemSlot) && itemSlot.Item != null)
        {
            Item item = (Item)itemSlot.Item;
            IItemUser itemUser = null;
            if (item.ItemHolder.MemberItemHolder != null && item.ItemHolder.MemberItemHolder.GetItemUser() != null)
                itemUser = item.ItemHolder.MemberItemHolder.GetItemUser();
            if (itemUser == null)
                itemUser = GameManager.Instance.Player.GetComponent<IItemUser>();

            // 投げ飛ばす場合，ItemSlotがSetActive(false)されてOnEndDragが呼び出されないため，手動で呼び出す。
            DragSystem.Instance.EndDrag();

            Vector2 target = GameManager.Utility.GetMousePos();
            item.Drop();
            if (itemUser is MonoBehaviour mono)
            {
                Debug.Log($"Throwing item: {item.name} by {mono.gameObject.name} at target: {target}");
                if (mono.TryGetComponent<Attack>(out Attack attack))
                {
                    attack.ThrowItem(item, target);
                }
                else
                {
                    Debug.LogWarning($"{mono.gameObject.name} does not have Attack component.");
                }
            }
        }
    }
}