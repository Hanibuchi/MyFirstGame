using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemCapacityData
{
    public int itemCapacity = 10;
    public int attackItemCapacity = 2;
    public int parameterModifierItemCapacity = 10;
    public int projectileModifierItemCapacity = 3;
    public bool isBag;
    public bool isFixedSize;
}
