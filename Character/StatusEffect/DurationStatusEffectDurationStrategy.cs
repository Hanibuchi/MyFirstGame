using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DurationStatusEffectDurationStrategy : StatusEffectDurationStrategy
{
    public float Duration
    {
        get => m_duration;
        private set
        {
            m_duration = value;
            if (m_duration <= 0)
                Expire();
        }
    }
    [SerializeField] float m_duration;
    public DurationStatusEffectDurationStrategy(StatusEffectManager statusEffectManager, StatusEffect statusEffect, float duration) : base(statusEffectManager, statusEffect)
    {
        SetDuration(duration);
        m_statusEffectManager.OnTimePassed += ChangeDuration;
    }
    public override void OnExpire()
    {
        base.OnExpire();
        m_statusEffectManager.OnTimePassed -= ChangeDuration;
    }
    public void SetDuration(float duration)
    {
        m_duration = duration;
    }
    void ChangeDuration(float deltaTime)
    {
        Duration -= deltaTime;
    }
}
