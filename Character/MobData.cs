using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;


/// <summary>
/// 再読み込みの際に使用される，mobの状態を保存するためのクラス
/// </summary>
[Serializable]
public class MobData : ObjectData
{
    public string MobID;

    public LayerMask BaseTargetLayer;
    public LayerMask CurrentTargetLayer;
    public float BaseMaxMP;
    public float CurrentMaxMP;
    public float CurrentMP;
    public float BaseMPRegen;
    public float CurrentMPRegen;
    public ulong BaseLevel;
    public ulong CurrentLevel;
    public float BaseSpeed;
    public float CurrentSpeed;
    public Damage BaseDamage;
    public Damage CurrentDamage;

    public int ItemCapacity;
    public List<ObjectData> Items = new();

    public float Experience;
    public float ExperienceToNextLevel;
}
