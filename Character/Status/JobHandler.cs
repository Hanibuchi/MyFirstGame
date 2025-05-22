using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class JobHandler : MonoBehaviour, ISerializableComponent
{
    [SerializeField] JobData m_jobData;
    [JsonProperty][SerializeField] JobType m_baseJob;
    public JobType BaseJob
    {
        get => m_baseJob;
        private set { m_baseJob = value; OnBaseJobChanged?.Invoke(m_baseJob); }
    }
    public event Action<JobType> OnBaseJobChanged;

    [JsonProperty][SerializeField] JobType m_job;
    public JobType Job
    {
        get => m_job;
        private set { m_job = value; OnJobChanged?.Invoke(m_job); }
    }
    public event Action<JobType> OnJobChanged;

    LevelHandler m_levelHandler;

    public void Initialize(JobData jobData)
    {
        if (TryGetComponent(out m_levelHandler))
        {
            m_levelHandler.OnLevelChanged += OnLevelChanged;
        }
        m_jobData = jobData;
        BaseJob = jobData.baseJob;
    }
    public void ResetToBase()
    {
        Job = BaseJob;
    }

    public void OnLevelChanged(ulong level)
    {

    }
}

public enum JobType
{
    Fighter,

}
