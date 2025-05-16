using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusInitializer : MonoBehaviour
{
    [SerializeField] InitialStatusData m_initialStatusData;

    Health m_health;
    Mana m_mana;
    Attack m_attack;
    LevelHandler m_levelHandler;
    SpeedHandler m_speedHandler;
    JobHandler m_jobHandler;
    DeathHandler m_deathHandler;
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

        m_health = GetComponent<Health>();
        m_mana = GetComponent<Mana>();
        m_attack = GetComponent<Attack>();
        m_levelHandler = GetComponent<LevelHandler>();
        m_speedHandler = GetComponent<SpeedHandler>();
        m_jobHandler = GetComponent<JobHandler>();
        m_deathHandler = GetComponent<DeathHandler>();

        m_health?.Initialize(m_initialStatusData.healthData);
        m_mana?.Initialize(m_initialStatusData.manaData);
        m_attack?.Initialize(m_initialStatusData.attackData);
        m_levelHandler?.Initialize(m_initialStatusData.levelData);
        m_speedHandler?.Initialize(m_initialStatusData.speedData);
        m_jobHandler?.Initialize(m_initialStatusData.jobData);
        m_deathHandler?.Initialize(m_initialStatusData.deathData);

        m_levelHandler?.SetBaseLevel(1);
        ResetToBase();
        Restore();

        isInitialized = true;
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
}
