using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IMissedItemManager
{
    JObject CollectMissedItems(JObject chunkData);
}
