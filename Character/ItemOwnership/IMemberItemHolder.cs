using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMemberItemHolder : IItemHolder
{
    void OnAddedToParty(PartyItemTracker partyItemTracker);
    void OnRemovedFromParty();
}
