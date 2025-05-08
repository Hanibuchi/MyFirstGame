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
    private void Awake()
    {
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
        
        Initialize();
    }

    public void Initialize()
    {
        m_health?.Initialize(m_initialStatusData.healthData);
        m_mana?.Initialize(m_initialStatusData.manaData);
        m_attack?.Initialize(m_initialStatusData.attackData);
        m_levelHandler?.Initialize(m_initialStatusData.levelData);
        m_speedHandler?.Initialize(m_initialStatusData.speedData);
        m_jobHandler?.Initialize(m_initialStatusData.jobData);
    }
}
