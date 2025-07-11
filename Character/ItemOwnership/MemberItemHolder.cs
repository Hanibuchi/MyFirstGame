using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MemberItemHolder : ITrackablePartyMember, IMemberItemHolder
{
    public PartyItemTracker PartyItemTracker { get; private set; }
    [SerializeField] MemberItemTracker _memberItemTracker;
    public MemberItemTracker MemberItemTracker => _memberItemTracker;
    public int ItemCapacity { get; private set; } = 4;
    public bool IsFixedSize => true;
    [SerializeReference] List<IChildItemHolder> _items = new();
    public List<IChildItemHolder> Items { get => _items; private set => _items = value; }
    public void SetItemCapacity(int itemCapacity)
    {
        ItemCapacity = itemCapacity;
    }

    public void ResetItems()
    {
        Items.Clear();
        for (int i = 0; i < ItemCapacity; i++)
        {
            Items.Add(null);
        }
        _memberItemUIRefresher.RefreshUI();
    }
    public void SetPartyItemTracker(PartyItemTracker partyItemTracker)
    {
        PartyItemTracker = partyItemTracker;
    }
    public void SetMemberItemTracker(MemberItemTracker memberItemTracker)
    {
        _memberItemTracker = memberItemTracker;
    }
    public bool CanAddItem(IChildItemHolder childItemHolder)
    {
        if (Items.Any(x => x == null))
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
    public bool CanAddItemAt(int index, IChildItemHolder childItemHolder)
    {
        return 0 <= index && index < Items.Count && Items.Any(x => x == null);
    }
    public void AddItem(IChildItemHolder childItemHolder)
    {
        Debug.Log($"CanAddItem: {CanAddItem(childItemHolder)}");
        if (CanAddItem(childItemHolder))
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null && CanAddItemAt(i, childItemHolder))
                {
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
    public void AddItemAt(int index, IChildItemHolder childItemHolder)
    {
        if (CanAddItemAt(index, childItemHolder))
        {
            childItemHolder.ClearPrevRelation();
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
            childItemHolder.OnAddedToMember(PartyItemTracker, _partyItemHolder, MemberItemTracker, this);
            _memberItemUIRefresher.RefreshUI();
        }
        else
            Debug.LogWarning("item cannot be added");
    }
    public void RemoveItem(IChildItemHolder childItemHolder)
    {
        if (Items.Contains(childItemHolder))
        {
            int index = Items.IndexOf(childItemHolder);
            Items[index] = null;
            childItemHolder.OnRemovedFromMember();
            _memberItemUIRefresher.RefreshUI();
        }
        else
            Debug.LogWarning("Item is not in this party.");
    }
    public void AddItemCapacity()
    {
        ItemCapacity++;
        Items.Add(null);
        _memberItemUIRefresher.RefreshUI();
    }
    [SerializeReference] PartyItemHolder _partyItemHolder;
    public PartyItemHolder PartyItemHolder => _partyItemHolder;
    public void OnAddedToParty(PartyItemTracker partyItemTracker, PartyItemHolder partyItemHolder)
    {
        SetPartyItemTracker(partyItemTracker);
        PartyItemTracker?.RegisterMember(this);
        _partyItemHolder = partyItemHolder;
        foreach (var item in Items)
        {
            item?.OnAddedToParty(partyItemTracker, partyItemHolder);
        }
    }
    public void OnRemovedFromParty()
    {
        PartyItemTracker?.UnregisterMember(this);
        SetPartyItemTracker(null);
        _partyItemHolder = null;
        foreach (var item in Items)
        {
            item?.OnRemovedFromParty();
        }
    }


    IPartyRegistrationHandler _partyRegistrationHandler;
    public void SetPartyRegistrationHandler(IPartyRegistrationHandler partyRegistrationHandler)
    {
        _partyRegistrationHandler = partyRegistrationHandler;
    }
    public void OnRegistered(IPartyModifier partyModifier)
    {
        _partyRegistrationHandler.OnRegistered(partyModifier);
    }
    public void OnUnregistered(IPartyModifier partyModifier)
    {
        _partyRegistrationHandler.OnUnregistered(partyModifier);
    }
    IMemberItemUIRefresher _memberItemUIRefresher;
    public void SetMemberItemUIRefresher(IMemberItemUIRefresher memberItemUIRefresher)
    {
        _memberItemUIRefresher = memberItemUIRefresher;
    }

    IItemUser _itemUser;
    public void SetItemUser(IItemUser itemUser)
    {
        _itemUser = itemUser;
    }
    public IItemUser GetItemUser()
    {
        return _itemUser;
    }
}
