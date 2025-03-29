using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Functions : MonoBehaviour
{
    /// <summary>
    /// シグモイド関数を返す
    /// </summary>
    /// <param name="x">入れる値</param>
    /// <param name="lowerBound">x->-∞で収束する値</param>
    /// <param name="upperBound">x->∞で収束する値</param>
    /// <returns></returns>
    public static float Sigmoid(float x, float lowerBound, float upperBound)
    {
        return lowerBound + (upperBound - lowerBound) / (1 + math.exp(-x));
    }
    /// <summary>
    /// シグモイド関数を返す。x->-∞で0, x->∞で1に収束。
    /// </summary>
    /// <param name="x">入れる値</param>
    /// <returns></returns>
    public static float Sigmoid(float x)
    {
        return Sigmoid(x, 0, 1);
    }

    /// <summary>
    /// 0未満でe^x，0以上でx+1を返す。0未満の収束が早すぎて面白くないときはHyperbolicLinearの方を使う
    /// </summary>
    /// <param name="x">入れる値</param>
    /// <returns></returns>
    public static float ExpLinear(float x)
    {
        if (x >= 0)
        {
            return x + 1;
        }
        else
        {
            return math.exp(x);
        }
    }

    /// <summary>
    /// 0未満で1/(1-x)，0以上でx+1を返す。
    /// </summary>
    /// <param name="x">入れる値</param>
    /// <returns></returns>
    public static float HyperbolicLinear(float x)
    {
        if (x >= 0)
        {
            return x + 1;
        }
        else
        {
            return 1 / (1 - x);
        }
    }

    public static string FormatTime(float seconds)
    {
        TimeSpan timeSpatn = TimeSpan.FromSeconds(seconds);
        return $"{timeSpatn.Hours}:{timeSpatn.Minutes:D2}";
    }

    static public int GenerateSeed()
    {
        return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}
