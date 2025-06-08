using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PartyItemHolder : IItemHolder
{
    [SerializeReference] List<IMemberItemHolder> _members = new();
    [SerializeField] PartyItemTracker _partyItemTracker;
    public PartyItemTracker PartyItemTracker => _partyItemTracker;
    public MemberItemTracker MemberItemTracker => null;
    public int ItemCapacity { get; private set; } = 1;
    public bool IsFixedSize => true;
    public List<IChildItemHolder> Items { get; private set; } = new();


    public void ResetItems()
    {
        Items.Clear();
        for (int i = 0; i < ItemCapacity; i++)
        {
            Items.Add(null);
        }
    }
    public void SetPartyItemTracker(PartyItemTracker partyItemTracker)
    {
        _partyItemTracker = partyItemTracker;
    }
    public void SetMemberItemTracker(MemberItemTracker memberItemTracker)
    {
        Debug.LogWarning("you should not set memberItemTracker in party");
    }
    public bool CanAddItem(IChildItemHolder childItemHolder)
    {
        if (Items.Any(x => x == null))
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
                if (item is IBagChildItemHolder bag && bag.CanAddItem(childItemHolder))
                {
                    bag.AddItem(childItemHolder);
                    return;
                }
            }
        }
    }
    public void AddItemAt(int index, IChildItemHolder childItemHolder)
    {
        if (CanAddItemAt(index, childItemHolder))
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
            childItemHolder.OnAddedToParty(PartyItemTracker);
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
            childItemHolder.OnRemovedFromParty();
        }
        else
            Debug.LogWarning("Item is not in this party.");
    }
    public void AddItemCapacity()
    {
        ItemCapacity++;
        Items.Add(null);
    }

    public void AddMember(IMemberItemHolder memberItemHolder)
    {
        if (memberItemHolder != null)
        {
            _members.Add(memberItemHolder);
            memberItemHolder.OnAddedToParty(PartyItemTracker);
        }
    }
    public void RemoveMember(IMemberItemHolder memberItemHolder)
    {
        if (_members.Contains(memberItemHolder))
        {
            _members.Remove(memberItemHolder);
            memberItemHolder.OnRemovedFromParty();
        }
    }
    public void ResetMembers()
    {
        foreach (var member in _members)
        {
            member.OnRemovedFromParty();
        }
        _members.Clear();
    }
}
