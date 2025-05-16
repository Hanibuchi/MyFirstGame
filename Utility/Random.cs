using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random : MonoBehaviour
{
    /// <summary>
    /// カテゴリーごとにRandom.Stateを保持するためのインスタンス。これを使えば，同じシード，同じカテゴリー，同じ順番なら出るものは同じにできる。
    /// </summary>
    public static readonly Dictionary<string, MyRandom> Randoms = new(); // Stateを保持するための辞書
}

/// <summary>
/// カテゴリーごとにRandom.Stateを保持するためのクラス。これを使えば，同じシード，同じカテゴリー，同じ順番なら出るものは同じにできる。
/// </summary>
public class MyRandom
{
    private UnityEngine.Random.State state;
    public MyRandom(UnityEngine.Random.State initState)
    {
        state = initState;
    }
    /// <summary>
    /// [0,1]のランダムな値を返す。
    /// </summary>
    /// <returns></returns>
    public float Value()
    {
        UnityEngine.Random.state = state;
        float value = UnityEngine.Random.value;
        state = UnityEngine.Random.state;
        return value;
    }
    /// <summary>
    /// 正規分布の乱数を返す。
    /// </summary>
    /// <returns></returns>
    public float NormalDistribution()
    {
        UnityEngine.Random.state = state;
        float u1 = 1.0f - UnityEngine.Random.value;
        float u2 = 1.0f - UnityEngine.Random.value;

        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        // 平均値meanと標準偏差standardDeviationに合わせてスケーリング
        state = UnityEngine.Random.state;
        return randStdNormal;
    }
}
