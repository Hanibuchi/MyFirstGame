using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class TransformSerializer : MonoBehaviour, ISerializableComponent
{
    [JsonProperty] MyVector3 m_position;
    [JsonProperty] Quaternion m_rotation;
    [JsonProperty] Vector3 m_scale;
    public void OnBeforeSerializeData()
    {
        m_position = new MyVector3(transform.localPosition);
        m_rotation = transform.localRotation;
        m_scale = transform.localScale;
    }

    public void OnAfterDeserializeData()
    {
        transform.SetLocalPositionAndRotation(m_position.ToVector3(), m_rotation);
        transform.localScale = m_scale;
    }
}
[Serializable]
public class MyVector3
{
    public float x;
    public float y;
    public float z;
    public MyVector3(Vector3 vector3)
    {
        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
