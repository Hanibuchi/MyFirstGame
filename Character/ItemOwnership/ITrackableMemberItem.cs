using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrackableMemberItem
{
    void OnRegisterd(IMemberModifier memberModifier);
    void OnUnregistered(IMemberModifier memberModifier);
}
