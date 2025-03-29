using System;
using UnityEngine;

public class StatisticsUI : BackableMenuUI
{
    [SerializeField] Transform statEntryFrame;
    public override void Open()
    {
        base.Open();
        foreach (StatisticsManager.StatType stat in Enum.GetValues(typeof(StatisticsManager.StatType)))
        {
            object value = StatisticsManager.Instance.Get(stat.ToString());
            var entryUI = ResourceManager.Get(ResourceManager.UIID.StatisticsEntryUI).GetComponent<StatisticsEntryUI>();
            entryUI.Set(stat.ToString(), value.ToString());
            entryUI.transform.SetParent(statEntryFrame);
        }
    }
    protected override void OnCloseCompleted()
    {
        base.OnCloseCompleted();
        foreach (Transform child in statEntryFrame)
        {
            ResourceManager.Release(ResourceManager.UIID.StatisticsEntryUI, child.gameObject);
        }
    }
}
