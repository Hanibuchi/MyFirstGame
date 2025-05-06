using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.Mathematics;
using UnityEngine;

public class WetStatus : Status
{
    public override StatusType Type => StatusType.Wet;
    [SerializeField] float m_wetLevel;
    public float WetLevel
    {
        get => m_wetLevel;
        set
        {
            if (value != m_wetLevel && !isExpired)
            {
                m_wetLevel = Math.Clamp(value, 0, 1);
                // Debug.Log($"WetLevel: {m_wetLevel}");
                if (value <= 0)
                    Expire();
            }
        }
    }
    /// <summary>
    /// 動きによるWetLevelの減少を計算する際の係数。蒸発量は風速の平方根に比例するらしいが，その比例定数。
    /// </summary>
    public float MovementDryRate { get; private set; }
    /// <summary>
    /// 自然乾燥によるWetLevelの減少を計算する際の係数。
    /// </summary>
    public float NaturalDryRate { get; private set; }
    public IStatusAffectable StatusAffectable;
    Action<float> WetLevelChangeAction;

    /// <summary>
    /// 使っちゃダメ
    /// </summary>
    /// <param name="id"></param>
    /// <param name="statusAffectable"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override sealed bool Add(StatusID id, IStatusAffectable statusAffectable)
    {
        throw new InvalidOperationException("Use other overload!");
    }


    /// <summary>
    /// 追加時に1度だけ実行する。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="wetLevelChangeAction"></param>
    /// <param name="statusList"></param>
    /// <returns></returns>
    public virtual bool Add(StatusID id, float wetLevel, IStatusAffectable statusAffectable)
    {
        if (!base.Add(id, statusAffectable))
            return false;

        WetLevelChangeAction += ChangeWetLevelBySpeed;
        StatusAffectable = statusAffectable;
        StatusAffectable.SetMoveAction(WetLevelChangeAction);

        MovementDryRate = id switch // ここでStatusごとの動きによる乾きやすさをかえる。マジックナンバーなのはそのうち改善したいかも
        {
            _ => 0.05f,

        };
        SetWetLevel(wetLevel);
        ApplicationManager.Instance.StartCoroutine(StartTimedDry());
        return true;
    }

    public void SetWetLevel(float wetLevel)
    {
        WetLevel = wetLevel;
    }

    bool isStartTimedDry = false;
    /// <summary>
    /// 時間で自然乾燥するようにする。一度しか実行してはいけない。
    /// </summary>
    /// <returns></returns>
    IEnumerator StartTimedDry()
    {
        if (isStartTimedDry)
            yield break;
        isStartTimedDry = true;

        NaturalDryRate = ID switch // ここで自然乾燥のしやすさ変える
        {
            _ => 0.01f,
        };
        while (WetLevel > 0)
        {
            ChangeWetLevel(-Time.deltaTime * NaturalDryRate);
            yield return null;
        }
    }

    /// <summary>
    /// 速さによってWetLevelを減らす。
    /// </summary>
    /// <param name="speed">速さ[m/s]</param>
    public void ChangeWetLevelBySpeed(float speed)
    {
        ChangeWetLevel(-MovementDryRate * math.sqrt(speed) * Time.deltaTime);
    }

    public void ChangeWetLevel(float dWetLevel)
    {
        WetLevel += dWetLevel;
    }

    bool isExpired = false;
    public override void Expire()
    {
        base.Expire();
        WetLevelChangeAction = StatusAffectable.MoveAction;
        WetLevelChangeAction -= ChangeWetLevelBySpeed;
        StatusAffectable.SetMoveAction(WetLevelChangeAction);
        isExpired = true;
        WetLevel = 0;
    }

    protected override StatusData FillData(StatusData data)
    {
        base.FillData(data);
        data.Remaining = WetLevel;
        return data;
    }

    public override Status ApplyData(StatusData data)
    {
        base.ApplyData(data);
        WetLevel = data.Remaining;
        return this;
    }
}
