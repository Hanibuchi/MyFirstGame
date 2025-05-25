using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyBindingsController
{
    void SetKeybindingsUI(IKeyBindingsUI keyBindingsUI);
}

public delegate void RebindUIDelegate(MyInputSystem.ActionType actionName, int index);