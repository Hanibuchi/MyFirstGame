using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HiredMemberData
{
    public string memberData;
    public AreaManager hiredArea;
    public Vector2Int hiredChunkPos;
    public float hiringCost;
}
