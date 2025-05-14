using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class WetStatusEffectDurationStrategy : StatusEffectDurationStrategy
{
    [SerializeField] float m_wetLevel;
    public float WetLevel
    {
        get => m_wetLevel;
        private set
        {
            m_wetLevel = Math.Clamp(value, 0, 1);
            // Debug.Log($"WetLevel: {m_wetLevel}");
            if (value <= 0)
                Expire();
        }
    }
    [SerializeField] float m_movementDryRate;
    [SerializeField] float m_naturalDryRate;
    public WetStatusEffectDurationStrategy(StatusEffectManager statusEffectManager, StatusEffect statusEffect, float wetLevel, float movementDryRate, float nutralDryRate) : base(statusEffectManager, statusEffect)
    {
        SetWetLevel(wetLevel);
        m_movementDryRate = movementDryRate;
        m_naturalDryRate = nutralDryRate;

        m_statusEffectManager.OnMove += ChangeWetLevelBySpeed;
        if (m_naturalDryRate > 0)
            m_statusEffectManager.OnTimePassed += ChangeWetLevelByNaturalDry;
    }
    public override void OnExpire()
    {
        base.OnExpire();
        m_statusEffectManager.OnMove -= ChangeWetLevelBySpeed;
        if (m_naturalDryRate > 0)
            m_statusEffectManager.OnTimePassed -= ChangeWetLevelByNaturalDry;
    }
    public void SetWetLevel(float wetLevel)
    {
        WetLevel = wetLevel;
    }
    /// <summary>
    /// 速さによってWetLevelを減らす。
    /// </summary>
    /// <param name="sqrtSpeedPerSecond"></param>
    void ChangeWetLevelBySpeed(float sqrtSpeedPerSecond)
    {
        WetLevel -= m_movementDryRate * sqrtSpeedPerSecond;
    }
    void ChangeWetLevelByNaturalDry(float deltaTime)
    {
        WetLevel -= m_naturalDryRate * deltaTime;
    }
}
