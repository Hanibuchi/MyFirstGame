using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsEntryUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_Text value;
    public void Set(string label, string value)
    {
        this.label.text = label;
        this.value.text = value;
    }
}
