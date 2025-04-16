using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

interface IParameterModifierItem : IItem
{
    bool CanEditParam(Shot shot);
    void EditParameters(Shot shot)
    {
        if (!CanEditParam(shot))
            return;

        Shot newShot = new();
        newShot.CopyCore(shot);
        newShot.EditParameters = EditParametersCore;
        RegisterNextItemsToShot(newShot);

        newShot.EditParameters?.Invoke(shot);
    }
    void EditParametersCore(Shot shot);
}
