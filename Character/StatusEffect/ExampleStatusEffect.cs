using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleStatusEffect : StatusEffect
{
    Attack m_attack;
    public override void Excute()
    {
        if (m_attack == null)
        {
            m_attack = m_statusEffectManager.GetComponent<Attack>();
        }
        if (m_attack != null)
        {
            Damage additionalDamage = new()
            {
                physical = 5,
            };
            m_attack.AddDamage(additionalDamage);
        }
    }
}
