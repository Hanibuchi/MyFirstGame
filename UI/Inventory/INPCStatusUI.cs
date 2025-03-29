using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

interface INPCStatusUI
{
    public void RegisterStatus(NPCManager npcManager);
    public void UnregisterStatus();
}
