using MyGame;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ItemData/Default", order = 0)]
public class ItemData : ScriptableObject
{
    public int itemCapacity;
    public float reloadTime;
    public LayerMask targetLayer;
    public float mp;
    public Damage damage;
    public float diffusion;
    public float speed;
    public float duration;
    public float recoil;
    public float additionalSize;
    public float additionalAmount;
    public ResourceManager.ItemSlotID slotID = ResourceManager.ItemSlotID.DefaultSlot;
}