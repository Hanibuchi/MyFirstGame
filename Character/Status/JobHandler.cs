using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class JobHandler : MonoBehaviour, ISerializeHandler
{
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

    public void Initialize(JobData jobData)
    {
        BaseJob = jobData.baseJob;
        ResetToBase();
    }
    public void ResetToBase()
    {
        Job = BaseJob;
    }
}

public enum JobType
{
    Fighter,

}
