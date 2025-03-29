using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[Serializable]
public struct Damage
{
    public float CriticalRate;
    public float InstantDeathRate;
    public float TotalDamageRate;
    public float Knockback;
    public float Destruction;

    public float Fire;
    public float Water;
    public float Electric;
    public float Wind;
    public float Ice;
    public float Poison;
    public float Physical;
    public float Caterpillar;
    public float Heal;

    // public float CriticalRate
    // {
    //     get => criticalRate;
    //     set => criticalRate = Mathf.Max(0, value);
    // }

    // public float InstantDeathRate
    // {
    //     get => instantDeathRate;
    //     set => instantDeathRate = Mathf.Max(0, value);
    // }

    // public float AdditionalDamageRate
    // {
    //     get => additionalDamageRate;
    //     set => additionalDamageRate = Mathf.Max(0, value);
    // }

    // /// <summary>
    // /// ノックバック。撃たれた方が受ける力
    // /// </summary>
    // public float Knockback
    // {
    //     get => knockback;
    //     set => knockback = Mathf.Max(0, value);
    // }

    // public float Destruction
    // {
    //     get => destruction;
    //     set => destruction = Mathf.Max(0, value);
    // }

    // public float Fire
    // {
    //     get => fire;
    //     set => fire = Mathf.Max(0, value);
    // }

    // public float Water
    // {
    //     get => water;
    //     set => water = Mathf.Max(0, value);
    // }

    // public float Electric
    // {
    //     get => electric;
    //     set => electric = Mathf.Max(0, value);
    // }

    // public float Wind
    // {
    //     get => wind;
    //     set => wind = Mathf.Max(0, value);
    // }

    // public float Ice
    // {
    //     get => ice;
    //     set => ice = Mathf.Max(0, value);
    // }

    // public float Poison
    // {
    //     get => poison;
    //     set => poison = Mathf.Max(0, value);
    // }

    // public float Physical
    // {
    //     get => physical;
    //     set => physical = Mathf.Max(0, value);
    // }

    // public float Caterpillar
    // {
    //     get => caterpillar;
    //     set => caterpillar = Mathf.Max(0, value);
    // }

    // public float Heal
    // {
    //     get => heal;
    //     set => heal = Mathf.Max(0, value);
    // }

    /// <summary>
    /// 単純な足し算。値型であるため非破壊的。
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public Damage Add(Damage damage)
    {
        CriticalRate += damage.CriticalRate;
        InstantDeathRate += damage.InstantDeathRate;
        TotalDamageRate += damage.TotalDamageRate;
        Knockback += damage.Knockback;
        Destruction += damage.Destruction;

        Fire += damage.Fire;
        Ice += damage.Ice;
        Physical += damage.Physical;
        Electric += damage.Electric;
        Caterpillar += damage.Caterpillar;
        Poison += damage.Poison;
        Water += damage.Water;
        Wind += damage.Wind;
        Heal += damage.Heal;
        return this;
    }

    /// <summary>
    /// 耐性の計算をする。確率系は引き算（負の数は0），属性ダメージ系は(1-防御率)を掛ける。
    /// </summary>
    /// <param name="DamageRate"></param>
    /// <returns></returns>
    public Damage CalculateDamageWithDamageRates(Damage DamageRate)
    {
        Damage result = new()
        {
            CriticalRate = CriticalRate + DamageRate.CriticalRate,
            InstantDeathRate = InstantDeathRate + DamageRate.InstantDeathRate,
            TotalDamageRate = this.TotalDamageRate + DamageRate.TotalDamageRate,

            Destruction = Destruction + DamageRate.Destruction, // これだけ特別に引き算

            Knockback = Knockback * DamageRate.Knockback,

            Fire = Fire * DamageRate.Fire,
            Ice = Ice * DamageRate.Ice,
            Physical = Physical * DamageRate.Physical,
            Electric = Electric * DamageRate.Electric,
            Caterpillar = Caterpillar * DamageRate.Caterpillar,
            Poison = Poison * DamageRate.Poison,
            Water = Water * DamageRate.Water,
            Wind = Wind * DamageRate.Wind,
            Heal = Heal * DamageRate.Heal,
        };

        return result;
    }

    /// <summary>
    /// 属性ダメージの合計を返す
    /// </summary>
    /// <returns></returns>
    public float GetTotalDamage()
    {
        float totalDamage = 0;
        totalDamage += Destruction;
        totalDamage += Fire;
        totalDamage += Ice;
        totalDamage += Physical;
        totalDamage += Electric;
        totalDamage += Caterpillar;
        totalDamage += Poison;
        totalDamage += Water;
        totalDamage += Wind;
        totalDamage += Electric;
        totalDamage -= Heal;
        return totalDamage;
    }

    public static Damage DefaultDamageRate => new()
    {
        CriticalRate = 0,
        InstantDeathRate = 0,
        TotalDamageRate = 1,
        Knockback = 1,
        Destruction = 0,

        Fire = 1,
        Water = 1,
        Electric = 1,
        Wind = 1,
        Ice = 1,
        Poison = 1,
        Physical = 1,
        Caterpillar = 1,
        Heal = 1,
    };

    public override string ToString()
    {
        Type type = this.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        string result = "";
        foreach (var field in fields)
        {
            object value = field.GetValue(this);
            result += $"{field.Name}: {value}\n";
        }
        return result;
    }
}
