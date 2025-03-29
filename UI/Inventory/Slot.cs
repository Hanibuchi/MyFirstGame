using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyGame
{
    public class Slot : UI
    {
        public void PointerEnter(PointerEventData eventData)
        {
            // Debug.Log("pointerEnter");
            GameObject draged = eventData.pointerDrag;
            // Debug.Log($"draged != null: {draged != null}, draged != gameObject: {draged != gameObject}, !IsAncestor(draged.transform){!IsAncestor(draged?.transform)}, ");
            if (draged != null && draged != gameObject && !IsAncestor(draged.transform) && draged.TryGetComponent<ItemSlot>(out var itemSlot))
            {
                // 入所希望のアイテムがItemに入れるか検査。OKだったらUIに反映。
                if (CanAddItem(itemSlot))
                {
                    // Debug.Log("can add item");
                    ProcessOnPointerEnter(itemSlot);
                }
                else
                    Debug.Log("The item cannot be added");
            }
            // eventData.Use();
        }

        /// <summary>
        /// OnPointerEnterの内部で行う処理
        /// </summary>
        /// <param name="itemSlot"></param>
        protected virtual void ProcessOnPointerEnter(ItemSlot itemSlot)
        {
            itemSlot.parentAfterDrag = transform;
            itemSlot.transform.SetParent(transform);
            itemSlot.isFixed = true;
        }

        public void PointerExit(PointerEventData eventData)
        {
            // Debug.Log("OnPOinterExit");
            GameObject draged = eventData.pointerDrag;

            if (draged != null && draged != gameObject && draged.TryGetComponent<ItemSlot>(out var itemSlot))
            {
                ProcessOnPointerExit(itemSlot);
            }
            // eventData.Use();
        }

        /// <summary>
        /// OnPointerExitの内部で行う処理
        /// </summary>
        /// <param name="itemSlot"></param>
        public virtual void ProcessOnPointerExit(ItemSlot itemSlot)
        {
            itemSlot.DragSetup();
        }

        public void Drop(PointerEventData eventData)
        {
            // アイテムを落としたら子に追加する
            GameObject dropped = eventData.pointerDrag;
            if (dropped != null && dropped.TryGetComponent(out ItemSlot itemSlot))
            {
                // Debug.Log("Dropped");
                ProcessOnDrop(itemSlot);
            }
            eventData.Use(); // これするとこれ以降別のオブジェクトでOnDropが呼び出されなくなる。
        }

        /// <summary>
        /// OnDropのかっこの中ですること。
        /// </summary>
        /// <param name="itemSlot"></param>
        public virtual void ProcessOnDrop(ItemSlot itemSlot)
        {
            itemSlot.parentAfterDrag = transform;
            itemSlot.Item.RemovePrevRelation();
        }

        // アイテムをこのスロットに追加できるか
        public virtual bool CanAddItem(ItemSlot item)
        {
            if (item != null)
                if (transform.childCount <= 0)
                    return true;
                else
                    return false;
            else return false;
        }

        /// <summary>
        /// 祖先かどうか。
        /// </summary>
        /// <param name="ancestor"></param>
        /// <returns></returns>
        public bool IsAncestor(Transform ancestor)
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                if (parent == ancestor)
                    return true;
                parent = parent.transform.parent;
            }
            return false;
        }
    }
}
