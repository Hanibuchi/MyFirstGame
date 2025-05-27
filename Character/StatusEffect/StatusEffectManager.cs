using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    StatusInitializer m_statusInitializer;
    [JsonProperty][SerializeReference] List<StatusEffect> m_statusEffectList = new();
    public List<StatusEffect> StatusEffectList => m_statusEffectList;
    public void AddStatusEffect(StatusEffect statusEffect)
    {
        if (statusEffect.CanAdd(this))
        {
            StatusEffectList.Add(statusEffect);
            statusEffect.OnAdded(this);
        }
        else
        {
            statusEffect.OnCannotAdd(this);
        }
    }
    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        StatusEffectList.Remove(statusEffect);
        statusEffect.OnRemoved();
    }
    public void UpdateShortestRemaining(StatusEffectDurationType statusEffectDurationType, Type statusEffectType, float remaining) { }
    public void Recalculate()
    {
        m_statusInitializer?.ResetToBase();
        foreach (var statusEffect in m_statusEffectList)
        {
            statusEffect.Excute();
        }
    }










    public event Action<float> OnTimePassed;
    public event Action<float> OnMove;
    Rigidbody2D m_rb;
    Vector3 m_prevPos;
    // Start is called before the first frame update

    private void Awake()
    {
        if (TryGetComponent(out PoolableResourceComponent component))
        {
            component.ReleaseCallback += OnRelease;
        }
        m_rb = GetComponent<Rigidbody2D>();
        m_statusInitializer = GetComponent<StatusInitializer>();
    }

    protected virtual void OnRelease()
    {
        StatusEffectList.Clear();
        Recalculate();
    }
    private void Update()
    {
        OnTimePassed?.Invoke(Time.deltaTime);

        if (transform.position != m_prevPos && m_rb != null)
        {
            OnMove?.Invoke(Mathf.Sqrt(m_rb.velocity.magnitude) * Time.deltaTime);
            m_prevPos = transform.position;
        }
    }
}
