using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ChildItemHolder : IChildItemHolder, ITrackablePartyItem, ITrackableMemberItem
{
    public PartyItemTracker PartyItemTracker { get; private set; }
    public MemberItemTracker MemberItemTracker { get; private set; }

    public virtual bool IsBag { get; private set; }

    public int ItemCapacity { get; private set; } = 8;
    public virtual bool IsFixedSize { get; set; } = false;
    [SerializeReference] List<IChildItemHolder> _items = new();
    public List<IChildItemHolder> Items { get => _items; private set => _items = value; }

    [SerializeField] int m_attackItemCapacity = 4;
    [SerializeField] int m_parameterModifierItemCapacity = 4;
    [SerializeField] int m_projectileModifierItemCapacity = 4;

    public void ResetItems()
    {
        Items.Clear();
        if (IsFixedSize)
        {
            for (int i = 0; i < ItemCapacity; i++)
            {
                Items.Add(null);
            }
        }
        _childItemUIRefresher.RefreshUI();
    }
    public void SetPartyItemTracker(PartyItemTracker partyItemTracker)
    {
        PartyItemTracker = partyItemTracker;
    }
    public void SetMemberItemTracker(MemberItemTracker memberItemTracker)
    {
        MemberItemTracker = memberItemTracker;
    }
    public bool CanAddItem(IChildItemHolder childItemHolder)
    {
        if (IsFixedSize)
        {
            Debug.Log($"IsWithinItemLimits(childItemHolder) && Items.Any(x => x == null): {IsWithinItemLimits(childItemHolder)} && {Items.Any(x => x == null)}");
            if (IsWithinItemLimits(childItemHolder) && Items.Any(x => x == null))
                return true;
            else
            {
                foreach (var item in Items)
                {
                    if (item.IsBag)
                    {
                        Debug.Log($"bag.CanAddItem(item): {item.CanAddItem(childItemHolder)}");
                        if (item.CanAddItem(childItemHolder))
                            return true;
                    }
                }
            }
            return false;
        }
        else
        {
            if (CanAddItemAt(Items.Count, childItemHolder))
                return true;
            else
            {
                foreach (var item in Items)
                {
                    if (item.IsBag)
                    {
                        if (item.CanAddItem(childItemHolder))
                            return true;
                    }
                }
            }
            return false;
        }
    }
    public bool CanAddItemAt(int index, IChildItemHolder childItemHolder)
    {
        if (IsFixedSize)
        {
            return IsWithinItemLimits(childItemHolder) && Items.Any(x => x == null);
        }
        else
        {
            if (childItemHolder != null && index <= Items.Count)
                return IsWithinItemLimits(childItemHolder);
            else
                return false;
        }
    }
    bool IsWithinItemLimits(IChildItemHolder childItemHolder)
    {
        ItemType itemType = childItemHolder.GetItemType();
        if (Items.Count(a => a != null) >= ItemCapacity)
            return false;
        if ((itemType & ItemType.Attack) != 0 && Items.Count(a => a != null && (a.GetItemType() & ItemType.Attack) != 0) >= m_attackItemCapacity)
            return false;
        if ((itemType & ItemType.ParameterModifier) != 0 && Items.Count(a => a != null && (a.GetItemType() & ItemType.ParameterModifier) != 0) >= m_parameterModifierItemCapacity)
            return false;
        if ((itemType & ItemType.ProjectileModifier) != 0 && Items.Count(a => a != null && (a.GetItemType() & ItemType.ProjectileModifier) != 0) >= m_projectileModifierItemCapacity)
            return false;
        return true;
    }
    public void AddItem(IChildItemHolder childItemHolder)
    {
        if (IsFixedSize)
        {
            if (CanAddItem(childItemHolder))
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] == null && CanAddItemAt(i, childItemHolder))
                    {
                        Debug.Log($"item: {childItemHolder}");
                        AddItemAt(i, childItemHolder);
                        return;
                    }
                }
                foreach (var item in Items)
                {
                    if (item.IsBag && item.CanAddItem(childItemHolder))
                    {
                        item.AddItem(childItemHolder);
                        return;
                    }
                }
            }
        }
        else
        {
            if (CanAddItemAt(Items.Count, childItemHolder))
            {
                AddItemAt(Items.Count, childItemHolder);
                return;
            }
            else if (CanAddItem(childItemHolder))
            {
                foreach (var item in Items)
                {
                    if (item.IsBag && item.CanAddItem(childItemHolder))
                    {
                        item.AddItem(childItemHolder);
                        return;
                    }
                }
            }
        }
    }
    public void AddItemAt(int index, IChildItemHolder childItemHolder)
    {
        if (CanAddItemAt(index, childItemHolder))
        {
            childItemHolder.ClearPrevRelation();
            if (IsFixedSize)
            {
                if (Items[index] != null)
                {
                    int offset = 1, target;
                    while (true)
                    {
                        int left = Mathf.Clamp(index - offset, 0, ItemCapacity - 1);
                        if (Items[left] == null)
                        {
                            target = left;
                            break;
                        }
                        int right = Mathf.Clamp(index + offset, 0, ItemCapacity - 1);
                        if (Items[right] == null)
                        {
                            target = right;
                            break;
                        }
                        if (left == 0 && right == ItemCapacity - 1)
                        {
                            Debug.LogWarning("null does not exist");
                            return;
                        }
                        offset++;
                    }
                    if (target < index)
                    {
                        for (int i = target; i < index; i++)
                        {
                            Items[i] = Items[i + 1];
                        }
                    }
                    if (target > index)
                    {
                        for (int i = target; i > index; i--)
                        {
                            Items[i] = Items[i - 1];
                        }
                    }
                }
                Items[index] = childItemHolder;
            }
            else
                Items.Insert(index, childItemHolder);
            childItemHolder.OnAddedToItem(PartyItemTracker, _partyItemHolder, MemberItemTracker, _memberItemHolder, this);
            _childItemUIRefresher.RefreshUI();
        }
    }
    public void RemoveItem(IChildItemHolder childItemHolder)
    {
        if (Items.Contains(childItemHolder))
        {
            if (IsFixedSize)
            {
                int index = Items.IndexOf(childItemHolder);
                Items[index] = null;
            }
            else
            {
                Items.Remove(childItemHolder);
            }
            childItemHolder.OnRemovedFromItem();
            _childItemUIRefresher.RefreshUI();
        }
        else
            Debug.LogWarning("item is not in Items");
    }
    public void AddItemCapacity()
    {
        ItemCapacity++;
        if (IsFixedSize)
            Items.Add(null);
        _childItemUIRefresher.RefreshUI();
    }
    public void OnAddedToParty(PartyItemTracker partyItemTracker, PartyItemHolder partyItemHolder)
    {
        SetPartyItemTracker(partyItemTracker);
        PartyItemTracker.RegisterItem(this);
        _partyItemHolder = partyItemHolder;
        foreach (var item in Items)
        {
            item?.OnAddedToParty(partyItemTracker, partyItemHolder);
        }
    }
    public void OnRemovedFromParty()
    {
        PartyItemTracker.UnregisterItem(this);
        SetPartyItemTracker(null);
        _partyItemHolder = null;
        foreach (var item in Items)
        {
            item?.OnRemovedFromParty();
        }
    }
    public PartyItemHolder PartyItemHolder => _partyItemHolder;
    PartyItemHolder _partyItemHolder;
    public MemberItemHolder MemberItemHolder => _memberItemHolder;
    MemberItemHolder _memberItemHolder;
    public ChildItemHolder ParentItemHolder => _parentChildItemHolder;
    ChildItemHolder _parentChildItemHolder;
    public void OnAddedToMember(PartyItemTracker partyItemTracker, PartyItemHolder partyItemHolder, MemberItemTracker memberItemTracker, MemberItemHolder memberItemHolder)
    {
        SetPartyItemTracker(partyItemTracker);
        SetMemberItemTracker(memberItemTracker);
        PartyItemTracker?.RegisterItem(this);
        MemberItemTracker?.RegisterItem(this);
        _partyItemHolder = partyItemHolder;
        _memberItemHolder = memberItemHolder;
        foreach (var item in Items)
        {
            item?.OnAddedToMember(partyItemTracker, partyItemHolder, memberItemTracker, memberItemHolder);
        }
    }
    public void OnRemovedFromMember()
    {
        PartyItemTracker?.UnregisterItem(this);
        MemberItemTracker?.UnregisterItem(this);
        SetPartyItemTracker(null);
        SetMemberItemTracker(null);
        _partyItemHolder = null;
        _memberItemHolder = null;
        foreach (var item in Items)
        {
            item?.OnRemovedFromMember();
        }
    }
    public void OnAddedToItem(PartyItemTracker partyItemTracker, PartyItemHolder partyItemHolder, MemberItemTracker memberItemTracker, MemberItemHolder memberItemHolder, ChildItemHolder parentItemHolder)
    {
        SetPartyItemTracker(partyItemTracker);
        SetMemberItemTracker(memberItemTracker);
        PartyItemTracker?.RegisterItem(this);
        MemberItemTracker?.RegisterItem(this);
        _partyItemHolder = partyItemHolder;
        _memberItemHolder = memberItemHolder;
        _parentChildItemHolder = parentItemHolder;
        foreach (var item in Items)
        {
            item?.OnAddedToItem(partyItemTracker, _partyItemHolder, memberItemTracker, _memberItemHolder, this);
        }
    }
    public void OnRemovedFromItem()
    {
        PartyItemTracker?.UnregisterItem(this);
        MemberItemTracker?.UnregisterItem(this);
        SetPartyItemTracker(null);
        SetMemberItemTracker(null);
        _partyItemHolder = null;
        _memberItemHolder = null;
        _parentChildItemHolder = null;
        foreach (var item in Items)
        {
            item?.OnRemovedFromItem();
        }
    }

    public void SetItemTypeProvider(IItemTypeProvider itemTypeProvider)
    {
        _itemTypeProvider = itemTypeProvider;
    }
    IItemTypeProvider _itemTypeProvider;
    public ItemType GetItemType()
    {
        return _itemTypeProvider.GetItemType();
    }

    IPartyRegistrationHandler _partyRegistrationHandler;
    public void SetPartyRegistrationHandler(IPartyRegistrationHandler partyRegistrationHandler)
    {
        _partyRegistrationHandler = partyRegistrationHandler;
    }
    IMemberRegistrationHandler _memberRegistrationHandler;
    public void SetMemberRegistrationHandler(IMemberRegistrationHandler memberRegistrationHandler)
    {
        _memberRegistrationHandler = memberRegistrationHandler;
    }
    public void OnRegisterd(IPartyModifier partyModifier)
    {
        _partyRegistrationHandler.OnRegistered(partyModifier);
    }
    public void OnUnregistered(IPartyModifier partyModifier)
    {
        _partyRegistrationHandler.OnUnregistered(partyModifier);
    }
    public void OnRegisterd(IMemberModifier memberModifier)
    {
        _memberRegistrationHandler.OnRegistered(memberModifier);
    }
    public void OnUnregistered(IMemberModifier memberModifier)
    {
        _memberRegistrationHandler.OnUnregistered(memberModifier);
    }

    IItem _item;
    public void SetItem(IItem item)
    {
        _item = item;
    }
    public IItem GetItem()
    {
        if (_item != null)
            return _item;
        else
        {
            Debug.LogWarning("_item is null. use SetItem(IItem)");
            return null;
        }
    }

    /// <summary>
    /// ItemCapacityDataを受け取って値を代入するだけ。Itemsのリセットとかはしない。
    /// </summary>
    /// <param name="itemCapacityData"></param>
    public void SetItemCapacityData(ItemCapacityData itemCapacityData)
    {
        ItemCapacity = itemCapacityData.itemCapacity;
        m_attackItemCapacity = itemCapacityData.attackItemCapacity;
        m_parameterModifierItemCapacity = itemCapacityData.parameterModifierItemCapacity;
        m_projectileModifierItemCapacity = itemCapacityData.projectileModifierItemCapacity;
        IsBag = itemCapacityData.isBag;
        IsFixedSize = itemCapacityData.isFixedSize;
    }
    IChildItemUIRefresher _childItemUIRefresher;
    public void SetChildItemUIRefresher(IChildItemUIRefresher childItemUIRefresher)
    {
        _childItemUIRefresher = childItemUIRefresher;
    }

    public void ClearPrevRelation()
    {
        if (ParentItemHolder != null)
        {
            Debug.Log("ClearPrevRelation: ParentItemHolder.RemoveItem(this)");
            ParentItemHolder.RemoveItem(this);
        }
        else if (MemberItemHolder != null)
        {
            Debug.Log("ClearPrevRelation: MemberItemHolder.RemoveItem(this)");
            MemberItemHolder.RemoveItem(this);
        }
        else if (PartyItemHolder != null)
        {
            Debug.Log("ClearPrevRelation: PartyItemHolder.RemoveItem(this)");
            PartyItemHolder.RemoveItem(this);
        }
    }
}
