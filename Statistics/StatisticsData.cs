using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class StatisticsData
{
    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
    public Dictionary<string, object> statistics = new();
}
