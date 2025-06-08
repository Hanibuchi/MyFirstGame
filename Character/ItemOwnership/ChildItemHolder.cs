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

    public int ItemCapacity { get; private set; } = 8;
    public virtual bool IsFixedSize { get; set; } = false;
    public List<IChildItemHolder> Items { get; private set; } = new();

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
                    if (item is IBagChildItemHolder bag)
                    {
                        Debug.Log($"bag.CanAddItem(item): {bag.CanAddItem(childItemHolder)}");
                        if (bag.CanAddItem(childItemHolder))
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
                    if (item is IBagChildItemHolder bag)
                    {
                        if (bag.CanAddItem(childItemHolder))
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
        if ((itemType & ItemType.Attack) != 0 && Items.Count(a => (a.GetItemType() & ItemType.Attack) != 0) >= m_attackItemCapacity)
            return false;
        if ((itemType & ItemType.ParameterModifier) != 0 && Items.Count(a => (a.GetItemType() & ItemType.ParameterModifier) != 0) >= m_parameterModifierItemCapacity)
            return false;
        if ((itemType & ItemType.ProjectileModifier) != 0 && Items.Count(a => (a.GetItemType() & ItemType.ProjectileModifier) != 0) >= m_projectileModifierItemCapacity)
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
                    if (item is IBagChildItemHolder bag && bag.CanAddItem(childItemHolder))
                    {
                        bag.AddItem(childItemHolder);
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
                    if (item is IBagChildItemHolder bag && bag.CanAddItem(childItemHolder))
                    {
                        bag.AddItem(childItemHolder);
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
            childItemHolder.OnAddedToItem(PartyItemTracker, MemberItemTracker);
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
        }
        else
            Debug.LogWarning("item is not in Items");
    }
    public void AddItemCapacity()
    {
        ItemCapacity++;
        if (IsFixedSize)
            Items.Add(null);
    }
    public void OnAddedToParty(PartyItemTracker partyItemTracker)
    {
        SetPartyItemTracker(partyItemTracker);
        PartyItemTracker.RegisterItem(this);
        foreach (var item in Items)
        {
            item?.OnAddedToParty(partyItemTracker);
        }
    }
    public void OnRemovedFromParty()
    {
        PartyItemTracker.UnregisterItem(this);
        SetPartyItemTracker(null);
        foreach (var item in Items)
        {
            item?.OnRemovedFromParty();
        }
    }
    public void OnAddedToMember(PartyItemTracker partyItemTracker, MemberItemTracker memberItemTracker)
    {
        SetPartyItemTracker(partyItemTracker);
        SetMemberItemTracker(memberItemTracker);
        PartyItemTracker?.RegisterItem(this);
        MemberItemTracker?.RegisterItem(this);
        foreach (var item in Items)
        {
            item?.OnAddedToMember(partyItemTracker, memberItemTracker);
        }
    }
    public void OnRemovedFromMember()
    {
        PartyItemTracker?.UnregisterItem(this);
        MemberItemTracker?.UnregisterItem(this);
        SetPartyItemTracker(null);
        SetMemberItemTracker(null);
        foreach (var item in Items)
        {
            item?.OnRemovedFromMember();
        }
    }
    public void OnAddedToItem(PartyItemTracker partyItemTracker, MemberItemTracker memberItemTracker)
    {
        SetPartyItemTracker(partyItemTracker);
        SetMemberItemTracker(memberItemTracker);
        PartyItemTracker.RegisterItem(this);
        MemberItemTracker.RegisterItem(this);
        foreach (var item in Items)
        {
            item?.OnAddedToItem(partyItemTracker, memberItemTracker);
        }
    }
    public void OnRemovedFromItem()
    {
        PartyItemTracker.UnregisterItem(this);
        MemberItemTracker.UnregisterItem(this);
        SetPartyItemTracker(null);
        SetMemberItemTracker(null);
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

    public void OnRegisterd(IMemberModifier memberModifier)
    {
        // ここにパーティやメンバーに登録されたときする処理を記述する。
    }
    public void OnUnregistered(IMemberModifier memberModifier)
    {

    }
    public void OnRegisterd(IPartyModifier partyModifier) { }
    public void OnUnregistered(IPartyModifier partyModifier) { }
}
