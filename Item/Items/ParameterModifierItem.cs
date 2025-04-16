using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public class ParameterModifierItem : Item, IParameterModifierItem
{

    public bool CanEditParam(Shot shot)
    {
        return IsMPSufficient;
    }

    /// <summary>
    /// shotのパラメータを編集する。EditParametersは事務的な処理も含むが，これは純粋にパラメータを編集するのみ。
    /// </summary>
    /// <param name="shot"></param>
    public virtual void EditParametersCore(Shot shot)
    {
        Debug.Log($"ParameterModifierItem, EditParametersCore, shot: {shot}");
    }

    public override void Fire(Shot shot)
    {
        shot.user?.ThrowItem(this, shot.target);
    }
}
