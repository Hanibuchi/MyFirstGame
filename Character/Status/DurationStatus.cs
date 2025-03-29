using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DurationStatus : Status
{
    public override StatusType Type => StatusType.Duration;
    float m_duration;
    public float Duration
    {
        get => m_duration;
        private set
        {
            if (m_duration != value && !isExpired)
            {
                m_duration = value;
                if (m_duration <= 0)
                    Expire();
            }
        }
    }

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

    bool isAdded = false;
    /// <summary>
    /// 追加時に1度だけ実行する。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator Add(StatusID id, float duration, IStatusAffectable statusAffectable)
    {
        if (isAdded || !base.Add(id, statusAffectable))
            yield break;
        isAdded = true;

        SetDuration(duration);
        while (Duration > 0)
        {
            AddDuration(-Time.deltaTime);
            yield return null;
        }
    }

    public void SetDuration(float duration)
    {
        Duration = duration;
    }

    public void AddDuration(float duration)
    {
        Duration += duration;
    }

    bool isExpired = false;
    public override void Expire()
    {
        base.Expire();
        isExpired = true;
        Duration = 0;
    }

    protected override StatusData FillData(StatusData data)
    {
        base.FillData(data);
        data.Remaining = Duration;
        return data;
    }

    public override Status ApplyData(StatusData data)
    {
        base.ApplyData(data);
        Duration = data.Remaining;
        return this;
    }
}
