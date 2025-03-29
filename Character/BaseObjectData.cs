using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectStatus", menuName = "Status/ObjectStatus", order = 0)]
public class BaseObjectData : ScriptableObject
{
    public float BaseMaxHP;
    public Damage BaseDamageRate = Damage.DefaultDamageRate;
}
