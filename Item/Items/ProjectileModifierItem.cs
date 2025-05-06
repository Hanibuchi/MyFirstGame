using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using JetBrains.Annotations;

public class ProjectileModifierItem : Item, IProjectileModifierItem
{
    // shotがHomingに渡されるのは，Awakeの処理が終わってからのため，Awakeでshotを参照することはできない。shotを参照したいときはStartを使う。

    public bool CanEditProj(Shot shot)
    {
        return IsMPSufficient;
    }


    public virtual void EditProjectilesCore(Shot shot)
    {
        Debug.Log($"ProjectileModifierItem, EditProjectilesCore, shot: {shot}, projectile: {shot.projectiles}");
    }

    public override void Fire(Shot shot)
    {
        Parent.RemoveItem(this);
        shot.mobMan?.ThrowItem(this, shot.target);
    }
}
