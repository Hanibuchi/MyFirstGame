using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrackablePartyItem
{
    void OnRegisterd(IPartyModifier partyModifier);
    void OnUnregistered(IPartyModifier partyModifier);
}
