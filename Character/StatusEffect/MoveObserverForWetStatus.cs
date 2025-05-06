using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveObserverForWetStatus : MonoBehaviour
{
    IStatusAffectable StatusAffectable;
    Rigidbody2D Rb;
    Vector3 PrevPos;
    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent(out StatusAffectable))
            Debug.LogWarning("IStatusAffectable is null");
        TryGetComponent(out Rb);
    }

    private void Update()
    {
        if (transform.position != PrevPos)
        {
            // Debug.Log("Invoked");
            StatusAffectable.MoveAction?.Invoke(Rb.velocity.magnitude);
            PrevPos = transform.position;
        }
    }
}
