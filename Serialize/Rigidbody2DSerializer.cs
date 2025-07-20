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
    public void OnBeforeSerializeData()
    {
        rb = GetComponent<Rigidbody2D>();
        m_velocity = rb.velocity;
    }

    public void OnAfterDeserializeData()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = m_velocity;
    }
}
