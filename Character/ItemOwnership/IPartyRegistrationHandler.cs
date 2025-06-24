using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPartyRegistrationHandler
{
    void OnRegistered(IPartyModifier partyModifier);
    void OnUnregistered(IPartyModifier partyModifier);
}
