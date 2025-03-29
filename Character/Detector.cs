using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace MyGame
{
    [RequireComponent(typeof(NPCManager))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Detector : MonoBehaviour
    {
        public CircleCollider2D circleCollider2D;
        public LayerMask ObstacleLayer; // 障害物のレイヤー
        public float DetectionRadius;

        public void InitDetector()
        {
        }

        // public void SetCollider(float radius)
        // {
        //     if (circleCollider2D != null)
        //     {
        //         circleCollider2D.radius = radius;
        //     }
        // }

        // 一番近い敵を検知 // 範囲内のすべてのコライダーを検知するため指数オーダーになってよくない。いったん作ってみてやばかったら改良する。
        public GameObject DetectEnemy(float detectionRadius)
        {
            // Debug.Log($"DetectEnemy used");
            // 検知範囲内の全てのコライダーを取得
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
            GameObject nearestDetectedEnemy = null;
            float nearestDistance = detectionRadius;

            foreach (Collider2D hitCollider in hitColliders)
            {
                // Debug.Log($"hitCollider used");
                // "Enemy"タグが付いたオブジェクトのみを対象とする
                if (hitCollider.CompareTag("Enemy"))
                {
                    // Debug.Log($"detected Enemy");
                    // プレイヤーから敵への方向を計算
                    Vector2 directionToEnemy = hitCollider.transform.position - transform.position;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToEnemy, detectionRadius, ObstacleLayer);

                    // 障害物に当たらなければ敵をリストに追加
                    if (hit.collider == null && directionToEnemy.magnitude < nearestDistance)
                    {
                        // Debug.Log($"this didnt hit obstacle");
                        nearestDetectedEnemy = hitCollider.gameObject;
                        nearestDistance = directionToEnemy.magnitude;
                        Debug.DrawLine(transform.position, hitCollider.transform.position, Color.green); // デバッグ用に線を描く
                    }
                }
            }

            DetectionRadius = detectionRadius;

            return nearestDetectedEnemy;
        }

        // 検知範囲を視覚的に表示（デバッグ用）
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, DetectionRadius);
        }
    }
}