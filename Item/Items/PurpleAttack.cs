using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    // 具体的な攻撃クラスの例
    public class PurpleAttack : TestAttackItem
    {
        public override GameObject GenerateProjectile(Shot shot)
        {
            Debug.Log($"AttackItem, GenerateProjectile, shot: {shot}");

            // 参照オブジェクトの位置と回転を取得
            Transform referenceTransform = shot.referenceObject.transform;
            float aimingErrorAngle = GameManager.Randoms[GameManager.RandomNames.Diffusion].NormalDistribution() * shot.diffusion;

            GameObject nextProjectileObj = ResourceManager.GetProjectile(projectileID);
            nextProjectileObj.transform.SetPositionAndRotation(referenceTransform.position, referenceTransform.rotation * Quaternion.Euler(0, 0, aimingErrorAngle));
            // Debug.Log("projectile thrown");
            if (nextProjectileObj.TryGetComponent(out Rigidbody2D rb))
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(nextProjectileObj.transform.up * shot.speed, ForceMode2D.Impulse);
            }// 生成したオブジェクトを加速。

            Projectile projectile = nextProjectileObj.GetComponent<Projectile>();

            projectile.Launch(shot);

            return projectile.gameObject;
        }
    }
}