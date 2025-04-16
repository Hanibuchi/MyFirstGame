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
    public float speed = 20;
    public float duration = 3;
    public float recoil;
    public float size = 1;
    public float amount;
    public ResourceManager.ItemSlotID slotID = ResourceManager.ItemSlotID.DefaultSlot;
}