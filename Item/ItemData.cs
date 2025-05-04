using MyGame;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ItemData/Default", order = 0)]
public class ItemData : ScriptableObject
{
    public int itemCapacity = 10;
    public float reloadTime = 0.5f;
    public LayerMask targetLayer;
    public float mp = 10;
    public Damage damage;
    public float diffusion;
    public float speed = 20;
    public float duration = 3;
    public float recoil = 0.5f;
    public float size = 1;
    public float amount;
    public ResourceManager.ItemSlotID slotID = ResourceManager.ItemSlotID.DefaultSlot;



    public int m_itemCapacity = 10;
    public int m_attackItemCapacity = 2;
    public int m_parameterModifierItemCapacity = 10;
    public int m_projectileModifierItemCapacity = 3;
}