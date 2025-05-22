using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class TransformSerializer : MonoBehaviour, ISerializableComponent
{
    [JsonProperty] Vector3 m_position;
    [JsonProperty] Quaternion m_rotation;
    [JsonProperty] Vector3 m_scale;
    public void OnBeforeSerializeData()
    {
        m_position = transform.localPosition;
        m_rotation = transform.localRotation;
        m_scale = transform.localScale;
    }

    public void OnAfterDeserializeData()
    {
        transform.SetLocalPositionAndRotation(m_position, m_rotation);
        transform.localScale = m_scale;
    }
}
