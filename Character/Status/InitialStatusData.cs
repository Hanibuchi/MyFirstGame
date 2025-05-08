using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusData", menuName = "StatusData", order = 0)]
public class InitialStatusData : ScriptableObject
{
    public HealthData healthData;
    public ManaData manaData;
    public AttackData attackData;
    public LevelData levelData;
    public SpeedData speedData;
    public JobData jobData;
}
