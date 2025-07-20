using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(Damage damage, Attack user, Vector2 direction);
    int GetLayer();
}
