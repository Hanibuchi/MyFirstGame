using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;


[CreateAssetMenu(fileName = "NewGroundData", menuName = "Status/GroundData", order = 0)]
public class TileObjData : ScriptableObject
{
    public float MaxHP;
    public Damage DamageRate;
}