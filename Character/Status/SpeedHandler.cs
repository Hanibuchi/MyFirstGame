using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class SpeedHandler : MonoBehaviour, ISerializableComponent
{
    [SerializeField] SpeedData m_speedData;
    [JsonProperty][SerializeField] float m_baseSpeed;
    public float BaseSpeed
    {
        get => m_baseSpeed;
        protected set { m_baseSpeed = value; OnBaseSpeedChanged?.Invoke(m_baseSpeed); }
    }
    public event Action<float> OnBaseSpeedChanged;

    [JsonProperty][SerializeField] float m_speed;
    public float Speed
    {
        get => m_speed;
        protected set { m_speed = value; OnSpeedChanged?.Invoke(m_speed); }
    }
    public event Action<float> OnSpeedChanged;

    LevelHandler m_levelHandler;

    public void Initialize(SpeedData speedData)
    {
        m_speedData = speedData;
        if (TryGetComponent(out m_levelHandler))
        {
            m_levelHandler.OnLevelChanged += OnLevelChanged;
        }
    }

    public void ResetToBase()
    {
        Speed = BaseSpeed;
    }

    public void OnLevelChanged(ulong level)
    {
        BaseSpeed = m_speedData.baseSpeedGrowthCurve.Function(level);
    }
}
