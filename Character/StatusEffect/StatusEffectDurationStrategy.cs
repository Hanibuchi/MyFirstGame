using System;
using UnityEngine;

[Serializable]
public class StatusEffectDurationStrategy
{
    protected StatusEffectManager m_statusEffectManager;
    [NonSerialized] protected StatusEffect m_statusEffect;

    public StatusEffectDurationStrategy(StatusEffectManager statusEffectManager, StatusEffect statusEffect)
    {
        m_statusEffectManager = statusEffectManager;
        m_statusEffect = statusEffect;
    }
    protected void Expire()
    {
        m_statusEffectManager.RemoveStatusEffect(m_statusEffect);
    }
    public virtual void OnExpire()
    {

    }
}

public enum StatusEffectDurationType
{
    Default,
    Wet,
    Duration,
}
