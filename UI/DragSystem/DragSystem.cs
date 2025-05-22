using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DragSystem : MonoBehaviour
{
    static public DragSystem Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsDragging => m_currentDragObjectType != DragObjectType.None;

    enum DragObjectType
    {
        None,
        Item,
        ItemSlot,
    }
    DragObjectType m_currentDragObjectType;

    /// <summary>
    /// 今ドラッグしているアイテム。
    /// </summary>
    [SerializeField] Item m_draggingItem;

    [SerializeField] ItemSlot m_draggingItemSlot;

    private void Update()
    {
        if (m_currentDragObjectType == DragObjectType.Item)
        {
            m_draggingItem?.MoveItem();
        }
        else if (m_currentDragObjectType == DragObjectType.ItemSlot)
        {
            m_draggingItemSlot?.SetAtMousePos();
        }

    }

    public void ItemBeginDrag(Item item)
    {
        m_currentDragObjectType = DragObjectType.Item;
        m_draggingItem = item;
        item.BeginDrag();
    }

    public void ItemSlotBeginDrag(ItemSlot itemSlot, Item item)
    {
        m_currentDragObjectType = DragObjectType.ItemSlot;
        m_draggingItemSlot = itemSlot;
        Debug.Log($"item: {item}");
        m_draggingItem = item; // ItemSlotはなくてもいいがItemは常になければならない。
        itemSlot.BeginDrag();

        if (!UIManager.Instance.EquipmentUI.IsOpen)
        {
            OnCloseEquipmentMenu();
        }
    }

    public void OnOpenEquipmentMenu()
    {
        if (!IsDragging)
        {
            Debug.Log("not dragging");
            return;
        }

        if (m_draggingItem != null)
        {
            m_draggingItem.EndDrag();
            m_draggingItem.EnableComponentsOnCollected(false);

            if (m_draggingItem.GetItemSlotUI() == null)
                m_draggingItem.RefreshItemSlotUIs();
            m_draggingItemSlot = m_draggingItem.GetItemSlotUI();

            m_draggingItemSlot.BeginDrag();
            m_draggingItemSlot = m_draggingItem.GetItemSlotUI();
            m_currentDragObjectType = DragObjectType.ItemSlot;
        }
    }
    public void OnCloseEquipmentMenu()
    {
        if (!IsDragging)
        {
            Debug.Log("not dragging");
            return;
        }

        if (m_draggingItemSlot != null)
        {
            m_draggingItem.Parent?.RemoveItem(m_draggingItem);
            // ドラッグし始めたオブジェクトがItemSlotの場合，OnEndDragを呼び出すためにはItemSlotがアクティブ状態でないといけない。親オブジェクトのEquipmentMenuManagerはここでは必ず非アクティブであるため，親をnullにしてからアクティブ状態にする。
            m_draggingItemSlot.transform.SetParent(null);
            m_draggingItemSlot.gameObject.SetActive(true);

            m_draggingItemSlot.EndDrag();
            m_draggingItemSlot = null;

            m_draggingItem.SetAtMousePos(); // マウスの位置に出現させる。こうしないと遠い場所に出現させてしまう可能性がある。

            m_draggingItem.BeginDrag();
            m_draggingItem.EnableComponentsOnCollected(true);
            m_currentDragObjectType = DragObjectType.Item;
        }
    }
    public void EndDrag()
    {
        // Debug.Log($"m_currentDragObjectType: {m_currentDragObjectType.ToString()}");
        if (m_currentDragObjectType == DragObjectType.None)
        {
            Debug.Log("m_currentDragObjectType is already DragObjectType.None");
            return;
        }

        m_currentDragObjectType = DragObjectType.None;
        Debug.Log($"m_draggingItem: {m_draggingItem}");
        m_draggingItem.EndDrag();
        m_draggingItem = null;
        if (m_draggingItemSlot != null)
        {
            m_draggingItemSlot.EndDrag();
            m_draggingItemSlot = null;
        }
    }
}
