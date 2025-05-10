using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Attack : MonoBehaviour, ISerializeHandler
{
    [SerializeField] AttackData m_attackData;
    [JsonProperty][SerializeField] Damage m_baseDamage;
    public Damage BaseDamage
    {
        get => m_baseDamage;
        protected set { m_baseDamage = value; OnBaseDamageChanged?.Invoke(m_baseDamage); }
    }
    public event Action<Damage> OnBaseDamageChanged;

    [JsonProperty][SerializeField] Damage m_damage;
    public Damage Damage
    {
        get => m_damage;
        protected set { m_damage = value; OnDamageChanged?.Invoke(m_damage); }
    }
    public event Action<Damage> OnDamageChanged;

    [JsonProperty][SerializeField] float m_damageModifier;
    public float DamageModifier
    {
        get => m_damageModifier;
        protected set { m_damageModifier = value; OnDamageModifierChanged?.Invoke(m_damageModifier); }
    }
    public event Action<float> OnDamageModifierChanged;

    [JsonProperty][SerializeField] LayerMask m_baseTargetLayer;
    public LayerMask BaseTargetLayer
    {
        get => m_baseTargetLayer;
        protected set { m_baseTargetLayer = value; OnBaseTargetLayerChanged?.Invoke(m_baseTargetLayer); }
    }
    public event Action<LayerMask> OnBaseTargetLayerChanged;

    [JsonProperty][SerializeField] LayerMask m_targetLayer;
    public LayerMask TargetLayer
    {
        get => m_targetLayer;
        protected set { m_targetLayer = value; OnTargetLayerChanged?.Invoke(m_targetLayer); }
    }
    public event Action<LayerMask> OnTargetLayerChanged;

    LevelHandler m_levelHandler;

    public void Initialize(AttackData attackData)
    {
        m_attackData = attackData;
        if (TryGetComponent(out m_levelHandler))
        {
            m_levelHandler.OnLevelChanged += OnLevelChanged;
        }
    }
    public void ResetToBase()
    {
        Damage = BaseDamage;
        TargetLayer = BaseTargetLayer;
    }
    /// <summary>
    /// Damageを追加する。負の値を入れると引くことも出来る。
    /// </summary>
    /// <param name="damage"></param>
    public void AddDamage(Damage damage)
    {
        Damage.Add(damage);
    }

    public void OnLevelChanged(ulong level)
    {
        BaseDamage = m_attackData.baseDamage;
        DamageModifier = m_attackData.damageModifierGrowthCurve.Function(level);
        BaseTargetLayer = m_attackData.baseTargetLayer;
    }
}
