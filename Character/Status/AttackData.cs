using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackData
{
    public Damage baseDamage;
    public StatusCurveParameters damageModifierGrowthCurve;
    public LayerMask baseTargetLayer;
}
