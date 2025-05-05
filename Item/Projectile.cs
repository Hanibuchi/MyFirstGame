using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

[RequireComponent(typeof(PoolableResourceComponent))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] Shot shot;
    [SerializeField] float lifeTime;

    /// <summary>
    /// 発射されたとき実行されるメソッド。
    /// </summary>
    /// <param name="shot"></param>
    public void Launch(Shot shot)
    {
        this.shot = shot;
        lifeTime = this.shot.duration;
    }
    PoolableResourceComponent m_poolableResourceComponent;

    private void Awake()
    {
        if (!TryGetComponent(out m_poolableResourceComponent))
            Debug.LogWarning("m_poolableResourceComponent is null");
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroyed();
        }
    }

    /// <summary>
    /// ダメージ処理以外の処理を行う。ダメージ処理はMob側で行う。
    /// </summary>
    /// <param name="collider"></param>
    public void Hit(IDamageable objectManager)
    {
        if (objectManager is MonoBehaviour damageableObj)
        {
            if ((shot.targetLayer & (1 << damageableObj.gameObject.layer)) == 0)
            {
                Debug.Log("layer do not match");
                return;
            }

            Vector2 direction = damageableObj.transform.position - transform.position;
            objectManager.TakeDamage(shot.damage.CalculateDamageWithDamageRates(shot.userDamageRate), shot.user, direction);
            Destroyed();
            shot.referenceObject = gameObject;
            shot.NextAttack?.Invoke(shot);
        }
    }

    public void Destroyed()
    {
        m_poolableResourceComponent.Release();
    }
}
