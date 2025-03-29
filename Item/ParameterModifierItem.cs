using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class ParameterModifierItem : Item, IParameterModifier
    {
        public void EditParameters(Shot shot)
        {
            if (!IsMPSufficient)
                return;

            // ここでshotのパラメータたちを編集。

            foreach (Item nextItem in NextItems)
            {
                if (nextItem is IParameterModifier parameterModifierItem)
                {
                    parameterModifierItem.EditParameters(shot);
                }
            }
        }
        /** 
        ParameterModifier（単にパラメータを編集するだけで，Fire直後に処理を終了するもの）を使用する。
        ↓
        発射したオブジェクトのリストをshot.Projectilesに代入する。
        ↓
        ProjectileModifier（放射物を後から編集し，Fire直後に処理を終了するもの）を使用する。（ProjectileModifierのFire内ではshot.NextItemsを編集してはいけない）。
        **/
        /**
        ・NextItemsにはParameterModifierしか入らないと仮定。面白みがないため，中に入れられそうなItemを考える。
        ・shot.NextItemsは編集してはいけない
        **/
        /// ParameterModifierItemを使用するメソッド。中にAttackItemが入っていた場合，shot.NextItemsに入れて一緒に出力する。
        public override void Fire(Shot shot)
        {
            // // ここでAdditionalな値を編集。
            // shot.AddAdditionalValues(CurrentDamage, CurrentDiffusion, CurrentSpeed, CurrentDuration, CurrentAdditionalSize, CurrentAdditionalAmount, CurrentRecoil, targetLayer);

            // foreach (Item nextItem in NextItems)
            // {
            //     if (nextItem is AttackItem)
            //     {
            //         shot.NextItems.Add(nextItem);
            //     }
            // }

            // // NextItemsのParameterModifierを実行
            // foreach (Item nextItem in NextItems)
            // {
            //     if (nextItem is ParameterModifierItem)
            //     {
            //         nextItem.Fire(shot); // shotのパラメータ（AdditionalDamage, AdditionalDiffusion, AdditionalSpeed, AdditionalDuration）が編集されるだけ
            //     }
            // }
        }
    }
}
