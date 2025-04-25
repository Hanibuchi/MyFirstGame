using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MyGame;
using Unity.VisualScripting;

public class Slot : UI, IDropHandler
{
    [SerializeField] protected Transform m_itemSlotFrame;
    public int m_id;
    IItemParentUI m_itemParentUI;

    public void SetID(int id)
    {
        m_id = id;
    }

    public void SetItemParentUI(IItemParentUI itemParentUI)
    {
        m_itemParentUI = itemParentUI;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null && dropped.TryGetComponent(out ItemSlot itemSlot))
        {
            m_itemParentUI?.AddItem(m_id, (Item)itemSlot.ItemParent);
        }
        eventData.Use();
    }

    /// <summary>
    /// 子のitemSlotたちとの親子関係をなくす。
    /// </summary>
    public void DetachChildrenUI()
    {
        m_itemSlotFrame.DetachChildren();
    }
    public virtual void SetItemSlot(ItemSlot itemSlot)
    {
        if (itemSlot == null)
        {
            Debug.LogWarning("itemSlot is null");
            return;
        }
        itemSlot.transform.SetParent(m_itemSlotFrame);
    }


    public override void OnRelease()
    {
        base.OnRelease();
        foreach (Transform child in m_itemSlotFrame)
        {
            if (child.TryGetComponent(out IPoolable poolable))
            {
                poolable.Release();
            }
        }
    }













    // public void PointerEnter(PointerEventData eventData)
    // {
    //     // Debug.Log("pointerEnter");
    //     GameObject draged = eventData.pointerDrag;
    //     // Debug.Log($"draged != null: {draged != null}, draged != gameObject: {draged != gameObject}, !IsAncestor(draged.transform){!IsAncestor(draged?.transform)}, ");
    //     if (draged != null && draged != gameObject && !IsAncestor(draged.transform) && draged.TryGetComponent<ItemSlot>(out var itemSlot))
    //     {
    //         // 入所希望のアイテムがItemに入れるか検査。OKだったらUIに反映。
    //         if (CanAddItem(itemSlot))
    //         {
    //             // Debug.Log("can add item");
    //             ProcessOnPointerEnter(itemSlot);
    //         }
    //         else
    //             Debug.Log("The item cannot be added");
    //     }
    //     // eventData.Use();
    // }

    // /// <summary>
    // /// OnPointerEnterの内部で行う処理
    // /// </summary>
    // /// <param name="itemSlot"></param>
    // protected virtual void ProcessOnPointerEnter(ItemSlot itemSlot)
    // {
    //     itemSlot.parentAfterDrag = transform;
    //     itemSlot.transform.SetParent(transform);
    //     itemSlot.isFixed = true;
    // }

    // public void PointerExit(PointerEventData eventData)
    // {
    //     // Debug.Log("OnPOinterExit");
    //     GameObject draged = eventData.pointerDrag;

    //     if (draged != null && draged != gameObject && draged.TryGetComponent<ItemSlot>(out var itemSlot))
    //     {
    //         ProcessOnPointerExit(itemSlot);
    //     }
    //     // eventData.Use();
    // }

    // /// <summary>
    // /// OnPointerExitの内部で行う処理
    // /// </summary>
    // /// <param name="itemSlot"></param>
    // public virtual void ProcessOnPointerExit(ItemSlot itemSlot)
    // {
    //     itemSlot.DragSetup();
    // }

    // public void Drop(PointerEventData eventData)
    // {
    //     // アイテムを落としたら子に追加する
    //     GameObject dropped = eventData.pointerDrag;
    //     if (dropped != null && dropped.TryGetComponent(out ItemSlot itemSlot))
    //     {
    //         // Debug.Log("Dropped");
    //         ProcessOnDrop(itemSlot);
    //     }
    //     eventData.Use(); // これするとこれ以降別のオブジェクトでOnDropが呼び出されなくなる。
    // }

    // /// <summary>
    // /// OnDropのかっこの中ですること。
    // /// </summary>
    // /// <param name="itemSlot"></param>
    // public virtual void ProcessOnDrop(ItemSlot itemSlot)
    // {
    //     itemSlot.parentAfterDrag = transform;
    //     itemSlot.Item.RemovePrevRelation();
    // }

    // // アイテムをこのスロットに追加できるか
    // public virtual bool CanAddItem(ItemSlot item)
    // {
    //     if (item != null)
    //         if (transform.childCount <= 0)
    //             return true;
    //         else
    //             return false;
    //     else return false;
    // }

    // /// <summary>
    // /// 祖先かどうか。
    // /// </summary>
    // /// <param name="ancestor"></param>
    // /// <returns></returns>
    // public bool IsAncestor(Transform ancestor)
    // {
    //     Transform parent = transform.parent;
    //     while (parent != null)
    //     {
    //         if (parent == ancestor)
    //             return true;
    //         parent = parent.transform.parent;
    //     }
    //     return false;
    // }

}