using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HealthData
{
    public StatusCurveParameters baseMaxHPGrowthCurve;
    public Damage baseDamageRate = Damage.DefaultDamageRate;
    public StatusCurveParameters damageRateModifierGrowthCurve;
}
