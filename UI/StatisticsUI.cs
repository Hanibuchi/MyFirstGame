using System;
using UnityEngine;

public class StatisticsUI : UIPageBase
{
    [SerializeField] Transform statEntryFrame;
    public override void Show()
    {
        base.Show();
        foreach (StatisticsManager.StatType stat in Enum.GetValues(typeof(StatisticsManager.StatType)))
        {
            object value = StatisticsManager.Instance.Get(stat.ToString());
            var entryUI = ResourceManager.GetOther(ResourceManager.UIID.StatisticsEntryUI.ToString()).GetComponent<StatisticsEntryUI>();
            entryUI.Set(stat.ToString(), value.ToString());
            entryUI.transform.SetParent(statEntryFrame);
        }
    }
    protected override void OnCloseCompleted()
    {
        base.OnCloseCompleted();
        foreach (Transform child in statEntryFrame)
        {
            ResourceManager.ReleaseOther(ResourceManager.UIID.StatisticsEntryUI.ToString(), child.gameObject);
        }
    }
}
