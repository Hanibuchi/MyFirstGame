using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMemberItemHolder : IItemHolder, IItemUserProvider
{
    PartyItemHolder PartyItemHolder { get; }
    void OnAddedToParty(PartyItemTracker partyItemTracker, PartyItemHolder partyItemHolder);
    void OnRemovedFromParty();
    void SetPartyRegistrationHandler(IPartyRegistrationHandler partyRegistrationHandler);
    void SetItemUser(IItemUser itemUser);
    void SetMemberItemUIRefresher(IMemberItemUIRefresher memberItemUIRefresher);
}
