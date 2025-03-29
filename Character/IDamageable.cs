using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public interface IDamageable
{
    public float CurrentHP { get; }
    public Damage DamageRate { get; }
    public void TakeDamage(Damage damage, MobManager user, Vector2 direction);
    public virtual void TakeDamage(Damage damage, Vector2 direction)
    {
        TakeDamage(damage, null, direction);
    }
    public void TakeDamageSub(Damage damage, Vector2 direction)
    {
        Damage calculatedDamage = damage.CalculateDamageWithDamageRates(DamageRate);
        // Debug.Log($"damage: {calculatedDamage}");
        float HP5per = CurrentHP * 0.05f;
        if (calculatedDamage.Ice > HP5per)
        {
            // 凍り付く処理
        }
        if (calculatedDamage.Fire > HP5per)
        {
            //燃える処理
        }
        if (calculatedDamage.Water > HP5per)
        {
            // 水による処理
        }
        if (calculatedDamage.Electric > HP5per)
        {
            // 電撃による処理
        }
        if (calculatedDamage.Wind > HP5per)
        {
            // 風による処理
        }
        if (calculatedDamage.Ice > HP5per)
        {
            // 氷による処理
        }
        if (calculatedDamage.Poison > HP5per)
        {
            // 毒による処理
        }
        if (calculatedDamage.Physical > HP5per)
        {
            // 物理ダメージによる処理
        }
        if (calculatedDamage.Caterpillar > HP5per)
        {
            // 毛虫による処理
        }
        if (calculatedDamage.Heal > HP5per)
        {
            // 回復に関する処理
        }

        IncreaseCurrentHP(-calculatedDamage.TotalDamageRate * calculatedDamage.GetTotalDamage());
        ApplyKnockback(calculatedDamage.Knockback, direction);
        if (GameManager.Randoms[GameManager.RandomNames.InstantDeath].Value() < calculatedDamage.InstantDeathRate)
        {
            Die();
        }
    }
    public void IncreaseCurrentHP(float additionalHP);
    public void ApplyKnockback(float knockback, Vector2 direction);
    public void Die();
}
