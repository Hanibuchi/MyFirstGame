using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PartyItemTracker
{
    IPartyModifier _partyModifier;
    public void SetPartyModifier(IPartyModifier partyModifier)
    {
        _partyModifier = partyModifier;
    }
    public void RegisterItem(ITrackablePartyItem trackablePartyItem)
    {
        trackablePartyItem.OnRegisterd(_partyModifier);
    }
    public void UnregisterItem(ITrackablePartyItem trackablePartyItem)
    {
        trackablePartyItem.OnUnregistered(_partyModifier);
    }
    public void RegisterMember(ITrackablePartyMember trackablePartyMember)
    {
        trackablePartyMember.OnRegistered(_partyModifier);
    }
    public void UnregisterMember(ITrackablePartyMember trackablePartyMember)
    {
        trackablePartyMember.OnUnregistered(_partyModifier);
    }
}
