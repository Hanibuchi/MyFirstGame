using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHealth : Health
{
    public override void TakeDamage(Damage damage, Attack user, Vector2 direction)
    {
        if (DamageRate.destruction > damage.destruction)
        {
            return;
        }

        base.TakeDamage(damage, user, direction);
    }
}
