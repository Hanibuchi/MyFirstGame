using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackHandler : MonoBehaviour
{
    Rigidbody2D m_rigidbody2D;
    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public void Knockback(float knockback, Vector2 direction)
    {
        m_rigidbody2D.AddForce(knockback * direction.normalized, ForceMode2D.Impulse);
    }
}
