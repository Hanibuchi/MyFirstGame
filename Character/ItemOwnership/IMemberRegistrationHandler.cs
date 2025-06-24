using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMemberRegistrationHandler
{
    void OnRegistered(IMemberModifier partyModifier);
    void OnUnregistered(IMemberModifier partyModifier);
}
