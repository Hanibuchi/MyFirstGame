using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public LevelHandler.GrowthType growthType;
    // Lv.1のとき必要となる経験値。
    public float baseValue;
    public float value1;
    public float value2;
}
