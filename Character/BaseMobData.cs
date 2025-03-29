using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMobStatus", menuName = "Status/MobStatus", order = 0)]
public class BaseMobData : BaseObjectData
{
    public LayerMask BasetTargetLayer;
    public float BaseMaxMP;
    public float BaseMPRegen;
    public ulong BaseLevel;
    public float BaseSpeed;
    public Damage BaseDamage;
}
