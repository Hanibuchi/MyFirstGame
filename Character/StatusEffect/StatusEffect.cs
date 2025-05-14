using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;
using Newtonsoft.Json;

[Serializable]
public class StatusEffect
{
    [SerializeReference] StatusEffectDurationStrategy m_statusEffectDurationStrategy;
    [SerializeField] StatusEffectDurationType m_statusEffectDurationType;
    [SerializeField] protected StatusEffectManager m_statusEffectManager;

    float m_wetLevel;
    float m_movementDryRate;
    float m_naturalDryRate;
    float m_duration;
    public StatusEffect()
    {
        m_statusEffectDurationType = StatusEffectDurationType.Default;
    }
    public void ToWetStatusEffect(float wetLevel, float movementDryRate, float naturalDryRate)
    {
        m_statusEffectDurationType = StatusEffectDurationType.Wet;
        m_wetLevel = wetLevel;
        m_movementDryRate = movementDryRate;
        m_naturalDryRate = naturalDryRate;
    }
    public void ToDurationStatusEffect(float duration)
    {
        m_statusEffectDurationType = StatusEffectDurationType.Duration;
        m_duration = duration;
    }

    public virtual bool CanAdd(StatusEffectManager statusEffectManager)
    {
        m_statusEffectManager = statusEffectManager;
        return true;
    }

    public void OnAdded(StatusEffectManager statusEffectManager)
    {
        m_statusEffectManager = statusEffectManager;
        m_statusEffectDurationStrategy = m_statusEffectDurationType switch
        {
            StatusEffectDurationType.Wet => new WetStatusEffectDurationStrategy(m_statusEffectManager, this, m_wetLevel, m_movementDryRate, m_naturalDryRate),
            StatusEffectDurationType.Duration => new DurationStatusEffectDurationStrategy(m_statusEffectManager, this, m_duration),
            _ => new StatusEffectDurationStrategy(m_statusEffectManager, this),
        };
        Excute();
    }

    public virtual void Excute() { }

    public void OnRemoved()
    {
        Expire();
        m_statusEffectDurationStrategy.OnExpire();
    }

    public virtual void Expire()
    {
        m_statusEffectManager.Recalculate(); // 例。もっと処理を軽くできるならする。
    }

    public void OnCannotAdd(StatusEffectManager statusEffectManager)
    {
        m_statusEffectManager = statusEffectManager;
        // 例: ステータス保有上限を超えていた場合，最も残り持続時間などが小さいものを追加しようしたものにする。
        // UpdateShortestRemaining();
    }

    protected void UpdateShortestRemaining()
    {
        switch (m_statusEffectDurationType)
        {
            case StatusEffectDurationType.Wet:
                m_statusEffectManager.UpdateShortestRemaining(m_statusEffectDurationType, this.GetType(), m_wetLevel);
                break;
            case StatusEffectDurationType.Duration:
                m_statusEffectManager.UpdateShortestRemaining(m_statusEffectDurationType, this.GetType(), m_duration);
                break;
            default:
                break;
        }
    }

    public void SetRemaining(float remaining)
    {
        if (m_statusEffectDurationStrategy is DurationStatusEffectDurationStrategy durationStatusEffectDurationStrategy)
        {
            durationStatusEffectDurationStrategy.SetDuration(remaining);
        }
        if (m_statusEffectDurationStrategy is WetStatusEffectDurationStrategy wetStatusEffectDurationStrategy)
        {
            wetStatusEffectDurationStrategy.SetWetLevel(remaining);
        }
    }
}
