using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
[RequireComponent(typeof(Rigidbody2D))]
public class Rigidbody2DSerializer : MonoBehaviour, ISerializableComponent
{
    Rigidbody2D rb;
    [JsonProperty] Vector3 m_velocity;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void OnBeforeSerializeData()
    {
        m_velocity = rb.velocity;
    }

    public void OnAfterDeserializeData()
    {
        rb.velocity = m_velocity;
    }
}
