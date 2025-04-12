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
            Destroy(gameObject); // 既にインスタンスが存在する場合、このオブジェクトを破棄
        }
    }

    enum DraggingObjectType
    {
        None,
        Item,
        ItemSlot,
    }
    DraggingObjectType CurrentDraggingObjectType;

    [SerializeField] Item draggingItem;
    /// <summary>
    /// 今ドラッグしているアイテム。
    /// </summary>
    public Item DraggingItem
    {
        get => draggingItem;
        private set => draggingItem = value;
    }

    [SerializeField] ItemSlot draggingItemSlot;
    public ItemSlot DraggingItemSlot
    {
        get => draggingItemSlot;
        private set => draggingItemSlot = value;
    }

    private void Update()
    {
        if (CurrentDraggingObjectType == DraggingObjectType.Item)
        {
            DraggingItem?.MoveItem();
        }
        else if (CurrentDraggingObjectType == DraggingObjectType.ItemSlot)
        {
            DraggingItemSlot?.SetAtMousePos();
        }

    }

    public void ItemBeginDrag(Item item)
    {
        CurrentDraggingObjectType = DraggingObjectType.Item;
        DraggingItem = item;
        item.BeginDrag();
    }

    public void ItemSlotBeginDrag(ItemSlot itemSlot, Item item)
    {
        CurrentDraggingObjectType = DraggingObjectType.ItemSlot;
        DraggingItemSlot = itemSlot;
        DraggingItem = item; // ItemSlotはなくてもいいがItemは常になければならない。
        itemSlot.BeginDrag();
    }

    public void OnOpenEquipmentMenu()
    {
        if (DraggingItem != null)
        {
            CurrentDraggingObjectType = DraggingObjectType.ItemSlot;
            DraggingItem.EndDrag();
            DraggingItemSlot = DraggingItem.HideItemAndShowUI();
            DraggingItemSlot.BeginDrag();
        }
    }
    public void OnCloseEquipmentMenu()
    {
        if (DraggingItemSlot != null)
        {
            CurrentDraggingObjectType = DraggingObjectType.Item;
            DraggingItemSlot = null;
            DraggingItem.ShowItemAndHideUI();
            DraggingItem.BeginDrag();
        }
    }
    public void EndDrag()
    {
        CurrentDraggingObjectType = DraggingObjectType.None;
        DraggingItem.EndDrag();
        DraggingItem = null;
        if (DraggingItemSlot != null)
        {
            DraggingItemSlot.EndDrag();
            DraggingItemSlot = null;
        }
    }
}
