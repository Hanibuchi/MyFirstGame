using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyingGlass : ParameterModifierItem
{
    public override void EditParametersCore(Shot shot)
    {
        shot.diffusion += Diffusion;
        Debug.Log($"MagnifyingGlass, EditParametersCore, shot: {shot}");
    }
}
