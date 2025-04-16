using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

interface IProjectileModifierItem : IItem
{
    bool CanEditProj(Shot shot);

    /// <summary>
    /// shotの中のprojectileを編集する。何か機能を追加したい際はその機能を実行するメソッドを新たに定義し，このメソッドの途中で実行する。このメソッドは極力オーバライドしない。
    /// </summary>
    /// <param name="shot"></param>
    void EditProjectiles(Shot shot)
    {
        if (!CanEditProj(shot))
            return;

        // このアイテムについてる修飾アイテムの効果がもとのshotに適用されないよう，Coreだけをコピーした新しいshotを作成する。
        Shot newShot = new();
        newShot.CopyCore(shot);
        newShot.projectiles = shot.projectiles; // projectilesを編集するためのメソッドであるためこれだけは引き継ぐ。

        newShot.EditParameters = SetBaseExtras;
        newShot.EditProjectiles = EditProjectilesCore;

        RegisterNextItemsToShot(newShot);

        newShot.EditParameters?.Invoke(newShot);

        newShot.EditProjectiles?.Invoke(newShot);
    }

    /// <summary>
    /// 放射物を編集する。EditProjectileは事務的な処理も含むが，これは純粋に放射物を編集するのみ。
    /// </summary>
    /// <param name="shot"></param>
    void EditProjectilesCore(Shot shot);
}
