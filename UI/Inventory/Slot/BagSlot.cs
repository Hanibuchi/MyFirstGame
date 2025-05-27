using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagSlot : ItemSlot
{
    public override void InitSlots(int slotCount)
    {
        base.InitSlots(slotCount);
        for (int i = 0; i < slotCount; i++)
        {
            GenAndSetSlot(i);
        }
    }

    void GenAndSetSlot(int index)
    {
        string invSlotName;
        if (ItemParent is BagItem bag)
            invSlotName = bag.InvSlotID;
        else
            invSlotName = ResourceManager.ItemSlotID.InventorySlot.ToString();

        var slot = m_resourceManager.GetOther(invSlotName).GetComponent<InventorySlot>();
        slot.SetID(index);
        slot.SetItemParentUI(this);
        slot.transform.SetParent(m_itemSlotFrame);
    }

    public override void DetachChildrenUI()
    {
        foreach (Transform slotTrs in m_itemSlotFrame)
        {
            if (slotTrs.TryGetComponent(out InventorySlot slot))
            {
                slot.DetachChildrenUI();
            }
        }
    }

    public override void SetItemSlot(ItemSlot itemSlot, int index)
    {
        var slot = m_itemSlotFrame.GetChild(index).GetComponent<InventorySlot>();
        if (itemSlot != null)
        {
            slot.SetItemSlot(itemSlot);
        }
        else
        {
            Debug.Log("itemSlot is null");
            slot.DetachChildrenUI();
        }
    }

    protected override void SetRayCastTarget(bool isActive)
    {
        base.SetRayCastTarget(isActive);
        foreach (Transform slotTrs in m_itemSlotFrame)
        {
            if (slotTrs.TryGetComponent(out Image image))
                image.raycastTarget = isActive;
        }
    }
}
