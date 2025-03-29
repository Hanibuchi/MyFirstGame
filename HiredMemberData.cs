using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HiredMemberData
{
    public NPCData NPCData;
    public AreaManager HiredArea;
    public Vector2Int HiredChunkPos;
    public float HiringCost;
}
