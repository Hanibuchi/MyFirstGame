using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SerializeManager : MonoBehaviour
{
    public JObject SaveState()
    {
        var components = GetComponents<ISerializableComponent>();

        var dict = new Dictionary<string, JObject>();

        foreach (var comp in components)
        {
            comp.OnBeforeSerializeData();

            var type = comp.GetType();
            var jsonObj = comp.ToJObject();

            dict[type.FullName] = jsonObj;
        }

        return JObject.FromObject(dict);
    }

    public void LoadState(JObject json)
    {
        if (json == null)
        {
            Debug.LogWarning("json is invalid");
            return;
        }
        var components = GetComponents<ISerializableComponent>();
        var dict = json;

        foreach (var comp in components)
        {
            var type = comp.GetType();
            if (dict.TryGetValue(type.FullName, out var compJson))
            {
                comp.FromJObject((JObject)compJson);
                comp.OnAfterDeserializeData();
            }
        }
    }
}
interface ISerializableComponent
{
    /// <summary>
    /// シリアライズ前に実行される。特殊な処理が必要なときはこのときする。
    /// </summary>
    void OnBeforeSerializeData() { }
    JObject ToJObject()
    {
        var setting = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.None,
        };
        return JObject.FromObject(this, JsonSerializer.Create(setting));
    }
    void FromJObject(JObject jobject)
    {
        JsonConvert.PopulateObject(jobject.ToString(), this);
    }
    /// <summary>
    /// デシリアライズ後に実行される。適用に特殊な処理が必要ならこのときする。
    /// </summary>
    void OnAfterDeserializeData() { }
}
