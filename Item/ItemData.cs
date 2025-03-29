using MyGame;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ItemData/Default", order = 0)]
public class ItemData : ScriptableObject
{
    public int ItemCapacity;
    public float ReloadTime;
    public LayerMask TargetLayer;
    public float MP;
    public Damage Damage;
    public float Diffusion;
    public float Speed;
    public float Duration;
    public float Recoil;
    public float AdditionalSize;
    public float AdditionalAmount;
    public ResourceManager.UIID SlotID = ResourceManager.UIID.DefaultSlot;
}