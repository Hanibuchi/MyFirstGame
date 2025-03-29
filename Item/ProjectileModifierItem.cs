using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using JetBrains.Annotations;

public class ProjectileModifierItem : Item, IProjectileModifier
{
    // shotがHomingに渡されるのは，Awakeの処理が終わってからのため，Awakeでshotを参照することはできない。shotを参照したいときはStartを使う。

    /** ProjectileModifierItemを使用するメソッド。
    ParameterModifierを実行
    ↓
    shot.Projectilesを編集する
    ↓
    ProjectileModifier（放射物を後から編集し，Fire直後に処理を終了するもの）を使用する。（ProjectileModifierのFire内ではshot.NextItemsを編集してはいけない）。
    **/
    /**
    ・子のParameterModifierはこのProjectileModifier自体を修飾する。
    ・AttackItemを入れられるものと入れられないものの２種類がある。
    **/
    public void EditProjectile(Shot _shot)
    {
        if (!IsMPSufficient)
            return;

        Shot shot = new(_shot);
        // Additionalな要素をセット
        shot.SetAdditionalValues(Damage.Add(shot.User != null ? shot.User.CurrentDamage : new Damage()), Diffusion, Speed, Duration, AdditionalSize, AdditionalAmount, Recoil, TargetLayer | shot.TargetLayer);

        ModifyParams(shot);

        EditProjectileCore(shot);

        ExeNextProjModifier(shot); // ここで次のを実行するのではなく，例えばこのアイテムが放射物の周りに別の放射物を回すものなら，回ってる放射物に対してさらに次のを適応させるようなことがしたい。
    }

    /// <summary>
    /// 放射物を編集する。EditProjectileは事務的な処理も含むが，これは純粋に放射物を編集するのみ。
    /// </summary>
    /// <param name="shot"></param>
    void EditProjectileCore(Shot shot)
    {// shot.Projectilesを編集。
        foreach (GameObject projectile in shot.Projectiles)
        {
            Modif_Homing homing = projectile.AddComponent<Modif_Homing>();
            homing.Init(new(shot));
            Destroy(homing, shot.Duration); // 一定時間後削除される。Projectileは再利用されるため，コンポネントは消さないといけない。
        }
    }

    /// <summary>
    /// 次のProjectileModifierを実行する。
    /// </summary>
    /// <param name="shot"></param>
    void ExeNextProjModifier(Shot shot)
    {
        // ProjectileModifierを実行
        foreach (Item nextItem in NextItems)
        {
            if (nextItem is ProjectileModifierItem projectileModifierItem)
            {
                projectileModifierItem.EditProjectile(shot); // shot.Projectilesが編集される
            }
        }
    }
}
