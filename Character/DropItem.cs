using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ドロップのアイテムと確率を表す。
/// </summary>
[Serializable]
public class DropItem
{
    public DropItem(float dropRate, string itemName)
    {
        DropRate = dropRate;
        ItemName = itemName;
    }
    public float DropRate;
    public string ItemName;
}
