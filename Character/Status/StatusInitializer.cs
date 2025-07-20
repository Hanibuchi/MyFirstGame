using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusInitializer : MonoBehaviour
{
    [SerializeField] InitialStatusData m_initialStatusData;

    PoolableResourceComponent m_poolableResourceComponent;
    Health m_health;
    Mana m_mana;
    Attack m_attack;
    LevelHandler m_levelHandler;
    SpeedHandler m_speedHandler;
    JobHandler m_jobHandler;
    DeathHandler m_deathHandler;
    DropHandler m_dropHandler;
    ItemUser m_itemUser;
    private void Awake()
    {
        if (!isInitialized)
            Initialize();
    }

    [SerializeField] bool isInitialized = false;
    public void Initialize()
    {
        if (isInitialized)
        {
            Debug.LogWarning("already initialized");
            return;
        }
        if (m_initialStatusData == null)
        {
            Debug.LogWarning("initialStatudData is null!");
            return;
        }

        if (TryGetComponent(out m_poolableResourceComponent))
        {
            m_poolableResourceComponent.ReleaseCallback += OnRelease;
        }

        m_health = GetComponent<Health>();
        m_mana = GetComponent<Mana>();
        m_attack = GetComponent<Attack>();
        m_levelHandler = GetComponent<LevelHandler>();
        m_speedHandler = GetComponent<SpeedHandler>();
        m_jobHandler = GetComponent<JobHandler>();
        m_deathHandler = GetComponent<DeathHandler>();
        m_dropHandler = GetComponent<DropHandler>();
        m_itemUser = GetComponent<ItemUser>();


        m_health?.Initialize(m_initialStatusData.healthData);
        m_mana?.Initialize(m_initialStatusData.manaData);
        m_attack?.Initialize(m_initialStatusData.attackData);
        m_levelHandler?.Initialize(m_initialStatusData.levelData);
        m_speedHandler?.Initialize(m_initialStatusData.speedData);
        m_jobHandler?.Initialize(m_initialStatusData.jobData);
        m_dropHandler?.Initialize(m_initialStatusData.deathData);
        m_itemUser?.Initialize(m_initialStatusData.itemUserData);

        if (m_deathHandler != null)
        {
            m_deathHandler.OnDead += OnDead;
        }

        m_levelHandler?.SetBaseLevel(1);
        ResetToBase();
        Restore();

        isInitialized = true;
    }
    void OnRelease()
    {
        SetEnabled(true);
    }
    void OnDead()
    {
        SetEnabled(false);
    }

    public void ResetToBase()
    {
        m_health?.ResetToBase();
        m_mana?.ResetToBase();
        m_attack?.ResetToBase();
        m_levelHandler?.ResetToBase();
        m_speedHandler?.ResetToBase();
        m_jobHandler?.ResetToBase();
    }

    public void Restore()
    {
        m_health?.RestoreHP();
        m_mana?.RestoreMP();
    }

    public void SetEnabled(bool enabled)
    {
        if (m_health != null)
            m_health.enabled = enabled;
        if (m_mana != null)
            m_mana.enabled = enabled;
        if (m_attack != null)
            m_attack.enabled = enabled;
        if (m_levelHandler != null)
            m_levelHandler.enabled = enabled;
        if (m_speedHandler != null)
            m_speedHandler.enabled = enabled;
        if (m_jobHandler != null)
            m_jobHandler.enabled = enabled;
        if (m_deathHandler != null)
            m_deathHandler.enabled = enabled;
        if (m_dropHandler != null)
            m_dropHandler.enabled = enabled;
        if (m_itemUser != null)
            m_itemUser.enabled = enabled;
    }
}
