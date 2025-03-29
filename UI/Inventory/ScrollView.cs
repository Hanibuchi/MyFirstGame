using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyGame
{
    public class ScrollView : MonoBehaviour, IDropHandler
    {
        // アイテムをイベントリから投げ飛ばす。
        public void OnDrop(PointerEventData eventData)
        {

            GameObject itemSlotObject = eventData.pointerDrag;
            if (itemSlotObject != null && itemSlotObject.TryGetComponent(out ItemSlot itemSlot) && itemSlot.Item != null)
            {
                Item item = itemSlot.Item;
                MobManager owner = item.Owner;
                if (owner == null)
                    owner = GameManager.Instance.PlayerNPCManager;

                if (owner != null)
                {
                    // 投げ飛ばす場合，ItemSlotがSetActive(false)されてOnEndDragが呼び出されないため，手動で呼び出す。
                   DragSystem.Instance.EndDrag();

                    Vector2 target = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
                    owner.ThrowItem(item, target);

                    // // itemSlotがSetActive(false)されてOnEndDragが呼び出されないため手動で呼び出す。
                    // if (GameManager.Instance.CurrentlyDraggedItem != null)
                    // {
                    //     if (GameManager.Instance.CurrentlyDraggedItem.GetComponent<Item>())
                    //         item.OnEndDrag(new PointerEventData(EventSystem.current));
                    //     else
                    //         itemSlot.OnEndDrag(new PointerEventData(EventSystem.current));
                    // }
                }
                else
                    Debug.LogWarning("Player Manager is null");
            }

            // if (true) // ここでEquipmentMenuを開いたまま発射できるか設定できるかも
            // eventData.Use();
        }
    }
}