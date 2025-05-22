using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class SerializeManager : MonoBehaviour
{
    public string SaveState()
    {
        var components = GetComponents<ISerializableComponent>();

        var dict = new Dictionary<string, string>();

        foreach (var comp in components)
        {
            comp.OnBeforeSerializeData();

            var type = comp.GetType();
            var json = comp.Serialize();
            dict[type.FullName] = json;
        }

        return JsonConvert.SerializeObject(dict, Formatting.Indented);
    }

    public void LoadState(string json)
    {
        var components = GetComponents<ISerializableComponent>();
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        foreach (var comp in components)
        {
            var type = comp.GetType();
            if (dict.TryGetValue(type.FullName, out var compJson))
            {
                comp.Deserialize(compJson);

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
    public void OnBeforeSerializeData() { }
    string Serialize()
    {
        var setting = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.None,
        };
        return JsonConvert.SerializeObject(this, setting);
    }
    void Deserialize(string json)
    {
        JsonConvert.PopulateObject(json, this);
    }
    /// <summary>
    /// デシリアライズ後に実行される。適用に特殊な処理が必要ならこのときする。
    /// </summary>
    public void OnAfterDeserializeData() { }
}
