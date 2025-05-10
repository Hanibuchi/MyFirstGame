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
    public float criticalRate;
    public float instantDeathRate;
    public float totalDamageRate;
    public float knockback;
    public float destruction;

    public float fire;
    public float water;
    public float electric;
    public float wind;
    public float ice;
    public float poison;
    public float physical;
    public float caterpillar;
    public float heal;

    /// <summary>
    /// 単純な足し算。値型であるため非破壊的。
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public Damage Add(Damage damage)
    {
        criticalRate += damage.criticalRate;
        instantDeathRate += damage.instantDeathRate;
        totalDamageRate += damage.totalDamageRate;
        knockback += damage.knockback;
        destruction += damage.destruction;

        fire += damage.fire;
        ice += damage.ice;
        physical += damage.physical;
        electric += damage.electric;
        caterpillar += damage.caterpillar;
        poison += damage.poison;
        water += damage.water;
        wind += damage.wind;
        heal += damage.heal;
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
            criticalRate = criticalRate + DamageRate.criticalRate,
            instantDeathRate = instantDeathRate + DamageRate.instantDeathRate,
            totalDamageRate = this.totalDamageRate + DamageRate.totalDamageRate,

            destruction = destruction + DamageRate.destruction, // これだけ特別に引き算

            knockback = knockback * DamageRate.knockback,

            fire = fire * DamageRate.fire,
            ice = ice * DamageRate.ice,
            physical = physical * DamageRate.physical,
            electric = electric * DamageRate.electric,
            caterpillar = caterpillar * DamageRate.caterpillar,
            poison = poison * DamageRate.poison,
            water = water * DamageRate.water,
            wind = wind * DamageRate.wind,
            heal = heal * DamageRate.heal,
        };

        return result;
    }

    /// <summary>
    /// すべての属性ダメージにスカラをかける。要素が0から1までのDamageに対して，0から1まででレベルに対して各要素が単調減少となる要素のDamageを計算するのに使う。あんまりよく考えてない。本格的にゲームバランスを考えるときに考え直す。
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns></returns>
    public Damage Multiple(float modifier)
    {
        Damage result = new()
        {
            knockback = knockback * modifier,
            fire = fire * modifier,
            ice = ice * modifier,
            physical = physical * modifier,
            electric = electric * modifier,
            caterpillar = caterpillar * modifier,
            poison = poison * modifier,
            water = water * modifier,
            heal = heal * modifier,
        };
        return result;
    }

    /// <summary>
    /// 属性ダメージの合計を返す
    /// </summary>
    /// <returns></returns>
    public readonly float GetTotalDamage()
    {
        float totalDamage = 0;
        totalDamage += destruction;
        totalDamage += fire;
        totalDamage += ice;
        totalDamage += physical;
        totalDamage += electric;
        totalDamage += caterpillar;
        totalDamage += poison;
        totalDamage += water;
        totalDamage += wind;
        totalDamage += electric;
        totalDamage -= heal;
        return totalDamage;
    }

    public static Damage DefaultDamageRate => new()
    {
        criticalRate = 0,
        instantDeathRate = 0,
        totalDamageRate = 1,
        destruction = 0,

        knockback = 1,
        fire = 1,
        water = 1,
        electric = 1,
        wind = 1,
        ice = 1,
        poison = 1,
        physical = 1,
        caterpillar = 1,
        heal = 1,
    };

    public static Damage Zero => new()
    {
        criticalRate = 0,
        instantDeathRate = 0,
        totalDamageRate = 0,
        knockback = 0,
        destruction = 0,

        fire = 0,
        water = 0,
        electric = 0,
        wind = 0,
        ice = 0,
        poison = 0,
        physical = 0,
        caterpillar = 0,
        heal = 0,
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
