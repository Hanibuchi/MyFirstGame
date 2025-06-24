using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemUser : MonoBehaviour, IItemUser, IMemberItemUIRefresher, IPartyRegistrationHandler
{
    [SerializeField] MemberItemHolder _itemHolder;
    public MemberItemHolder ItemHolder => _itemHolder;

    [SerializeField] MemberItemTracker _itemTracker;
    public MemberItemTracker ItemTracker => _itemTracker;


    public void Initialize(ItemUserData data)
    {
        _itemHolder.SetItemUser(this);
        _itemHolder.SetMemberItemTracker(_itemTracker);
        _itemHolder.SetMemberItemUIRefresher(this);
        _itemHolder.SetPartyRegistrationHandler(this);
        ItemHolder.SetItemCapacity(data.itemCapacity);
        ItemHolder.ResetItems();
    }

    /// <summary>
    /// Colliderと重なったアイテムを手持ちに追加。拾うのは一つだけ。
    /// </summary>
    public void PickupItem()
    {
        List<Collider2D> colliders = new();

        if (TryGetComponent(out Collider2D mobCollider))
            mobCollider.OverlapCollider(new ContactFilter2D().NoFilter(), colliders);

        // 検出されたコライダーの中から，Itemコンポネントを持つものを一つ見つけて手持ちに追加する
        foreach (Collider2D collider in colliders)
        {
            // Itemクラスの子クラスを持つコンポーネントがあるか確認
            if (collider.TryGetComponent(out Item item))
            {
                if (SubPickupItem(item))
                    return;
            }
        }
    }

    /// <summary>
    /// アイテムを拾うメソッドPickupItemのサブメソッド。追加できたらtrueを返す。
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual bool SubPickupItem(Item item)
    {
        if (ItemHolder.PartyItemHolder != null && ItemHolder.PartyItemHolder.CanAddItem(item.ItemHolder))
        {
            ItemHolder.PartyItemHolder.AddItem(item.ItemHolder);
            item.EnableComponentsOnCollected(false);
            return true;
        }
        else if (ItemHolder.CanAddItem(item.ItemHolder))
        {
            ItemHolder.AddItem(item.ItemHolder);
            item.EnableComponentsOnCollected(false);
            return true;
        }
        else return false;
    }


    [SerializeField] int selectedSlotNumber;
    /// <summary>
    /// 選択されているスロットの番号。この番号のアイテムが実行される。
    /// </summary>
    public int SelectedSlotNumber
    {
        get => selectedSlotNumber;
        set => selectedSlotNumber = (value + ItemHolder.ItemCapacity) % ItemHolder.ItemCapacity;
    }



    [SerializeField] IMemberEquipmentUI m_equipmentMenu;
    public IMemberEquipmentUI EquipmentMenuUI
    {
        get
        {
            if (m_equipmentMenu == null)
            {
                m_equipmentMenu = GetComponent<PartyMember>()?.GetMemberUI().GetComponent<IMemberEquipmentUI>();
                if (m_equipmentMenu == null)
                    return null;
                m_equipmentMenu.SetItemUser(this);
                m_equipmentMenu.RegisterStatus(gameObject);
                if (m_equipmentMenu is MonoBehaviour mono)
                    UIManager.Instance.GetEquipmentUI().SetMemberUI(mono.gameObject);
            }
            return m_equipmentMenu;
        }
        private set => m_equipmentMenu = value;
    }
    public void RefreshUI()
    {
        Debug.Log("RefreshItemSlotUIs");
        EquipmentMenuUI.UpdateEquipmentUI(ItemHolder.Items);
    }

    public void OnRegistered(IPartyModifier partyModifier)
    {
        Debug.Log("ItemUser OnRegistered called.");
    }

    public void OnUnregistered(IPartyModifier partyModifier)
    {
        Debug.Log("ItemUser OnUnregistered called.");
        // throw new System.NotImplementedException();
    }
}
