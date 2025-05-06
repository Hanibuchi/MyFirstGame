using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Mana : MonoBehaviour, ISerializeHandler
{
    [JsonProperty][SerializeField] private float m_baseMaxMP;
    public float BaseMaxMP
    {
        get => m_baseMaxMP;
        protected set { if (m_baseMaxMP != value) { m_baseMaxMP = math.max(value, 0); OnBaseMaxMPChanged?.Invoke(m_baseMaxMP); } }
    }
    public event Action<float> OnBaseMaxMPChanged;

    [JsonProperty][SerializeField] private float m_maxMP;
    public float MaxMP
    {
        get => m_maxMP;
        protected set { if (m_maxMP != value) { m_maxMP = math.max(value, 0); OnMaxMPChanged?.Invoke(m_maxMP); } }
    }
    public event Action<float> OnMaxMPChanged;

    [JsonProperty][SerializeField] private float m_mp;
    public float MP
    {
        get => m_mp;
        protected set { if (m_mp != value) { m_mp = math.max(value, 0); OnMPChanged?.Invoke(m_mp); } }
    }
    public event Action<float> OnMPChanged;

    [JsonProperty][SerializeField] private float m_baseMPRegen;
    public float BaseMPRegen
    {
        get => m_baseMPRegen;
        protected set { if (m_baseMPRegen != value) { m_baseMPRegen = value; OnBaseMPRegenChanged?.Invoke(m_baseMPRegen); } }
    }
    public event Action<float> OnBaseMPRegenChanged;

    [JsonProperty][SerializeField] private float m_mpRegen;
    public float MPRegen
    {
        get => m_mpRegen;
        protected set { if (m_mpRegen != value) { m_mpRegen = value; OnMPRegenChanged?.Invoke(m_mpRegen); } }
    }
    public event Action<float> OnMPRegenChanged;

    public void Initialize(ManaData manaData)
    {
        BaseMaxMP = manaData.baseMaxMP;
        BaseMPRegen = manaData.baseMPRegen;
        ResetToBase();
        RestoreMP();
    }
    public void ResetToBase()
    {
        MaxMP = BaseMaxMP;
        MPRegen = BaseMPRegen;
    }
    public void RestoreMP()
    {
        MP = MaxMP;
    }
    private void Update()
    {
        if (MP < MaxMP)
        {
            ChangeMP(MPRegen * Time.deltaTime);
        }
    }

    public void ChangeMP(float additionalMP)
    {
        MP = math.clamp(MP + additionalMP, 0, MaxMP);
    }
}
