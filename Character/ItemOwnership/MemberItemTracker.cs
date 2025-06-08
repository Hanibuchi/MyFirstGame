using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MemberItemTracker
{
    IMemberModifier _memberModifier;
    public void SetMemberModifier(IMemberModifier memberModifier)
    {
        _memberModifier = memberModifier;
    }
    public void RegisterItem(ITrackableMemberItem trackableMemberItem)
    {
        trackableMemberItem.OnRegisterd(_memberModifier);
    }
    public void UnregisterItem(ITrackableMemberItem trackableMemberItem)
    {
        trackableMemberItem.OnUnregistered(_memberModifier);
    }
}
