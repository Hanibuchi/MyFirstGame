using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerStatusUI
{
    void RegisterStatus(GameObject player);
    void UnregisterStatus();
}
