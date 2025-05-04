using MyGame;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBagData", menuName = "ItemData/Bag", order = 0)]
public class BagData : ItemData
{
    public ResourceManager.ItemSlotID invSlotID = ResourceManager.ItemSlotID.InventorySlot;
}