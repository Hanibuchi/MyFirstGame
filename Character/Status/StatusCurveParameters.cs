using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusCurveParameters
{
    public FunctionType functionType;
    // Lv.1のとき必要となる経験値。
    public float baseValue;
    public float value1;
    public float value2;
    public float Function(ulong level)
    {
        float result = 0;
        switch (functionType)
        {
            case FunctionType.Linear: result = Functions.Linear(level - 1, value1, baseValue); break;
            case FunctionType.Quadratic: result = Functions.Quadratic(level - 1, value1, value2, baseValue); break;
            case FunctionType.Exponential: result = Functions.Exponential2(level - 1, value1, baseValue); break;
            case FunctionType.Logistic: result = Functions.Logistic2(level - 1, value1, value2, baseValue); break;
            case FunctionType.Gompertz: result = Functions.Gompertz2(level - 1, value1, value2, baseValue); break;
        }
        return result;
    }
}

public enum FunctionType
{
    Linear,
    Quadratic,
    Exponential,
    Logistic,
    Gompertz,
}