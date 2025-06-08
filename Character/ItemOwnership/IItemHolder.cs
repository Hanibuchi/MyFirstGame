using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemHolder
{
    PartyItemTracker PartyItemTracker { get; }
    MemberItemTracker MemberItemTracker { get; }

    int ItemCapacity { get; }
    bool IsFixedSize { get; }
    List<IChildItemHolder> Items { get; }

    void ResetItems();
    void SetPartyItemTracker(PartyItemTracker partyItemTracker);
    void SetMemberItemTracker(MemberItemTracker memberItemTracker);
    bool CanAddItem(IChildItemHolder childItemHolder);
    bool CanAddItemAt(int index, IChildItemHolder childItemHolder);
    void AddItem(IChildItemHolder childItemHolder);
    void AddItemAt(int index, IChildItemHolder childItemHolder);
    void RemoveItem(IChildItemHolder childItemHolder);
    void AddItemCapacity();
}
