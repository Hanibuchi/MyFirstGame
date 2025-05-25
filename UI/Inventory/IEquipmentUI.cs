using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipmentUI
{
    bool IsOpen { get; }
    void DetachMemberUIs();
    void SetMemberUI(GameObject memberEquipmentUI);
}
