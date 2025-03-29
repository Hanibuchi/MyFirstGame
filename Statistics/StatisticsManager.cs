using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public static StatisticsManager Instance { get; private set; }
    string StatisticsPath => Path.Combine(GameManager.PlayerDataPath, "Statistics");
    [SerializeField] StatisticsData statisticsData = new();
    public void OnGameStart()
    {
        Instance = this;
        Load();
    }
    public void Set(string name, object num)
    {
        statisticsData.statistics[name] = num;
    }
    // public void Set(string name, DateTime dateTime)
    // {
    //     statisticsData.dateTimeDict[name] = dateTime;
    // }
    public object Get(string name)
    {
        if (statisticsData.statistics.ContainsKey(name))
        {
            return statisticsData.statistics[name];
        }
        else
        {
            return null;
        }
    }
    public void Increase(string name, ulong num = (ulong)1)
    {
        var statistics = statisticsData.statistics;
        if (statistics.TryGetValue(name, out object value))
        {
            if (value is ulong ulongValue)
                statistics[name] = ulongValue + num;
        }
        else
        {
            statisticsData.statistics[name] = num;
        }
    }
    public void Save()
    {
        ApplicationManager.SaveCompressedJson(StatisticsPath, statisticsData);
    }
    public void Load()
    {
        StatisticsData data = ApplicationManager.LoadCompressedJson<StatisticsData>(StatisticsPath);
        if (data == null)
        {
            return;
        }
        statisticsData = data;
    }

    public enum StatType
    {
        GameStart,// DateTime

    }
}
