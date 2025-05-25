using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    string StatisticsPath => Path.Combine(GameManager.PlayerDataPath, "Statistics");
    [SerializeField] StatisticsData m_statisticsData = new();
    public void OnGameStart()
    {
        Instance = this;
        if (File.Exists(StatisticsPath))
        {
            Load();
        }
        else
            m_statisticsData = new();
    }
    public void Set(string name, object num)
    {
        m_statisticsData.statistics[name] = num;
    }
    // public void Set(string name, DateTime dateTime)
    // {
    //     statisticsData.dateTimeDict[name] = dateTime;
    // }
    public object Get(string name)
    {
        if (m_statisticsData.statistics.ContainsKey(name))
        {
            return m_statisticsData.statistics[name];
        }
        else
        {
            return null;
        }
    }
    public void Increase(string name, ulong num = (ulong)1)
    {
        var statistics = m_statisticsData.statistics;
        if (statistics.TryGetValue(name, out object value))
        {
            if (value is ulong ulongValue)
                statistics[name] = ulongValue + num;
        }
        else
        {
            m_statisticsData.statistics[name] = num;
        }
    }
    public void Save()
    {
        EditFile.SaveCompressedJson(StatisticsPath, m_statisticsData);
    }
    public void Load()
    {
        StatisticsData data = EditFile.LoadCompressedJson<StatisticsData>(StatisticsPath);
        if (data != null)
            m_statisticsData = data;
    }

    public enum StatType
    {
        GameStart,// DateTime

    }
}
