using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCStatus", menuName = "Status/NPCStatus", order = 0)]
public class BaseNPCData : BaseMobData
{
    public JobType Job;
}