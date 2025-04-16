using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleProjectile : Projectile
{
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.gravityScale = -.1f;
        }
    }
}
