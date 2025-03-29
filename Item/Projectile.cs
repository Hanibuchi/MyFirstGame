using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour, IResourceHandler
{
    [SerializeField] ResourceManager.ProjectileID ID;
    [SerializeField] Shot Shot;
    [SerializeField] float lifeTime;

    /// <summary>
    /// 発射されたとき実行されるメソッド。
    /// </summary>
    /// <param name="shot"></param>
    /// <param name="pool"></param>
    public void Launch(Shot shot)
    {
        Shot = shot;
        lifeTime = Shot.Duration;
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
            if ((Shot.TargetLayer & (1 << damageableObj.gameObject.layer)) == 0)
            {
                Debug.Log("layer do not match");
                return;
            }

            Vector2 direction = damageableObj.transform.position - transform.position;
            objectManager.TakeDamage(Shot.Damage, Shot.User, direction);
            Destroyed();
        }
    }

    public void Destroyed()
    {
        ResourceManager.Release(ID, gameObject);
    }

    public void OnGet(int id)
    {
        ID = (ResourceManager.ProjectileID)id;
    }
}
