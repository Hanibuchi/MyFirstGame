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
        if (itemSlotObject != null && itemSlotObject.TryGetComponent(out ItemSlot itemSlot) && itemSlot.ItemParent != null)
        {
            Item item = (Item)itemSlot.ItemParent;
            MobManager owner;
            if (item.Owner is MobManager mobManager)
                owner = mobManager;
            else
                owner = GameManager.Instance.PlayerNPCManager;

            // 投げ飛ばす場合，ItemSlotがSetActive(false)されてOnEndDragが呼び出されないため，手動で呼び出す。
            DragSystem.Instance.EndDrag();

            Vector2 target = GameManager.Utility.GetMousePos();
            item.Parent.RemoveItem(item);
            owner.ThrowItem(item, target);
        }
    }
}