using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrackablePartyMember
{
    void OnRegistered(IPartyModifier partyModifier);
    void OnUnregistered(IPartyModifier partyModifier);
}
