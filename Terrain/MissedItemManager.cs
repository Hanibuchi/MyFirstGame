using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MissedItemManager : MonoBehaviour, IMissedItemManager
{
    [SerializeField] List<JObject> _missedItems = new();
    public JObject CollectMissedItems(JObject chunkData)
    {
        JArray objects = (JArray)chunkData["ChunkManager"]?["objects"];
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            var obj = objects[i];

            var type = (ResourceType)(int)obj["Item1"];
            string id = (string)obj["Item2"];

            if (type == ResourceType.Item)
            {
                if ( id == "PurpleFurball")
                {
                    _missedItems.Add((JObject)obj);
                    objects.RemoveAt(i);
                }
            }
        }
        Debug.Log($"missedItem: {_missedItems.Count}");
        return chunkData;
    }
}
