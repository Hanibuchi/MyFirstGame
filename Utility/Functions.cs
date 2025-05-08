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

    /// <summary>
    /// 1次直線。nが値，value1が傾き，value2が定数項。
    /// </summary>
    /// <param name="n"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static float Linear(ulong n, float value1, float value2)
    {
        return value1 * n + value2;
    }

    /// <summary>
    /// 2次曲線。nが値，value1が2次係数，value2が1次係数，value3が定数項。
    /// </summary>
    /// <param name="n"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    /// <returns></returns>
    public static float Quadratic(ulong n, float value1, float value2, float value3)
    {
        return value1 * n * n + value2 * n + value3;
    }

    /// <summary>
    /// 底が2の指数関数。value1は指数係数，value2は初期値。底が2だと計算が速いらしい(GPT曰く)。
    /// </summary>
    /// <param name="n"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static float Exponential2(ulong n, float value1, float value2)
    {
        return value2 * Mathf.Pow(2, value1 * n);
    }

    /// <summary>
    /// 底が2のロジスティック曲線。value1は指数係数，value2は収束値，valu3は初期値。
    /// </summary>
    /// <param name="n"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    /// <returns></returns>
    public static float Logistic2(ulong n, float value1, float value2, float value3)
    {
        return value2 / (1 + Mathf.Pow(2, -value1 * n) * (value2 / value3 - 1));
    }

    /// <summary>
    /// 底が2のガンペルツ曲線。value1は指数係数，value2は収束値，value3は初期値。
    /// </summary>
    /// <param name="n"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    /// <returns></returns>
    public static float Gompertz2(ulong n, float value1, float value2, float value3)
    {
        return value2 * Mathf.Pow(value2 / value3, -Mathf.Pow(2, value1 * n));
    }
}
