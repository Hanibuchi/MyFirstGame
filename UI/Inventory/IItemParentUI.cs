using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemParentUI
{
    IItemParent ItemParent { get; }
    void SetItemParent(IItemParent itemParent);
    void InitSlots(int slotCount);

    void DetachChildrenUI();
    void SetItemSlot(ItemSlot itemSlot, int index);
    void AddItem(int index, Item item);
}
