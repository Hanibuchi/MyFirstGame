using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public interface IAttackItem : IItem
{
    public bool CanAttack(Shot shot);
    public void Attack(Shot shot)
    {
        if (!CanAttack(shot))
            return;

        shot.EditParameters = SetBaseExtras; // 最初に固有のパラメータをセットする。
        shot.EditProjectiles += ApplySize;

        RegisterNextItemsToShot(shot);

        shot.generateProjectileMethods.Add(GenerateProjectile);

        shot.NextAttack = NextAttack;

        // 以下はオプション。このメソッドを編集しなくていいよう，新しいメソッドに切り出してもいい。
        // shot.OnDestroyed = NextAttack;
        // shot.OnHit = NextAttack;
        // shot.OnTimeout = NextAttack;

        // ここからshotの本格的な編集が始まる。
        shot.EditParameters?.Invoke(shot);

        ApplyAmount(shot, GenerateAndInitProjectile);

        shot.EditProjectiles?.Invoke(shot);

        // Recoil(反動)の実装
        ApplyRecoil(shot);
    }

    public void ApplySize(Shot shot);

    public GameObject GenerateProjectile(Shot shot);
    public void NextAttack(Shot shot);
    public void ApplyAmount(Shot shot, Action<Shot> generateAndInitProjectile);
    public void GenerateAndInitProjectile(Shot shot);


    public Shot CreateNextShot(Shot shot);
    public void ApplyRecoil(Shot shot);
}
