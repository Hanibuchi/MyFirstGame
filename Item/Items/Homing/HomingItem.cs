using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingItem : ProjectileModifierItem
{
    public override void EditProjectilesCore(Shot shot)
    {
        // shot.Projectilesを編集。
        foreach (GameObject projectile in shot.projectiles)
        {
            Modif_Homing homing = projectile.AddComponent<Modif_Homing>();
            homing.Init(shot);
            Destroy(homing, shot.duration); // 一定時間後削除される。Projectileは再利用されるため，コンポネントは消さないといけない。
        }
        Debug.Log($"HomingItem, EditProjectilesCore, shot: {shot}, projectile: {shot.projectiles}");
    }
}
