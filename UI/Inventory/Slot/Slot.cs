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

    public virtual void SetItemSlot(ItemSlot itemSlot)
    {
        if (itemSlot == null)
        {
            Debug.LogWarning("itemSlot is null");
            return;
        }
        itemSlot.transform.SetParent(m_itemSlotFrame);
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null)
        {
            Debug.Log("dropped != null");
            Item item = null;
            if (dropped.TryGetComponent(out ItemSlot itemSlot))
                item = (Item)itemSlot.Item;
            if (item == null)
                item = dropped.GetComponent<Item>();

            m_itemParentUI.AddItem(m_id, item);
            if (m_itemParentUI is MonoBehaviour monoBehaviour)
                Debug.Log($"m_itemParentUI: {monoBehaviour.gameObject}");
        }
        eventData.Use();
    }

    /// <summary>
    /// 子のitemSlotたちとの親子関係をなくす。
    /// </summary>
    public virtual void DetachChildrenUI()
    {
        m_itemSlotFrame.DetachChildren(); // ここIItemParentUIをリセットしなくていいのか疑問。
    }


    public override void OnRelease()
    {
        base.OnRelease();
        foreach (Transform child in m_itemSlotFrame)
        {
            if (child.TryGetComponent(out PoolableResourceComponent poolable))
            {
                poolable.Release();
            }
        }
    }
}