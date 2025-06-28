using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChildItemHolder : IItemHolder, IItemTypeProvider, IItemProvider
{
    PartyItemHolder PartyItemHolder { get; }
    MemberItemHolder MemberItemHolder { get; }
    ChildItemHolder ParentItemHolder { get; }
    bool IsBag { get; }
    void OnAddedToParty(PartyItemTracker partyItemTracker, PartyItemHolder partyItemHolder);
    void OnRemovedFromParty();
    void OnAddedToMember(PartyItemTracker partyItemTracker, PartyItemHolder partyItemHolder, MemberItemTracker memberItemTracker, MemberItemHolder memberItemHolder);
    void OnRemovedFromMember();
    void OnAddedToItem(PartyItemTracker partyItemTracker, PartyItemHolder partyItemHolder, MemberItemTracker memberItemTracker, MemberItemHolder memberItemHolder, ChildItemHolder parentItemHolder);
    void OnRemovedFromItem();
    void SetItemTypeProvider(IItemTypeProvider itemTypeProvider);
    void SetItem(IItem item);
    void SetItemCapacityData(ItemCapacityData itemCapacityData);
    void SetPartyRegistrationHandler(IPartyRegistrationHandler partyRegistrationHandler);
    void SetMemberRegistrationHandler(IMemberRegistrationHandler memberRegistrationHandler);
    void SetChildItemUIRefresher(IChildItemUIRefresher childItemUIRefresher);
    void ClearPrevRelation();
}
