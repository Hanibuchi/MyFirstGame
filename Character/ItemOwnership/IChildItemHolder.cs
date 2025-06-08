using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChildItemHolder : IItemHolder, IItemTypeProvider
{
    void OnAddedToParty(PartyItemTracker partyItemTracker);
    void OnRemovedFromParty();
    void OnAddedToMember(PartyItemTracker partyItemTracker, MemberItemTracker memberItemTracker);
    void OnRemovedFromMember();
    void OnAddedToItem(PartyItemTracker partyItemTracker, MemberItemTracker memberItemTracker);
    void OnRemovedFromItem();
}
