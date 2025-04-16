using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Modif_Homing : MonoBehaviour
{
    public Shot Shot;
    Transform target;
    Rigidbody2D rb;
    LayerMask targetLayer;
    float addDrag;

    // shotがHomingに渡されるのはAwakeの処理が終わってからのため，Awakeでshotを参照することはできない。shotを参照したいときはStartを使う。
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Shot shot)
    {
        Shot = shot;
        if (Shot != null)
        {
            targetLayer = Shot.targetLayer;
            if (rb != null)
            {
                addDrag = Functions.HyperbolicLinear(-Shot.recoil) * 5/*減衰運動の動きを決める定数。2未満だと不足減衰，2だと臨界減衰，2より大きいと過減衰のはずが全然そんなことない*/ * math.sqrt(Shot.speed / 10 / rb.mass)/*=b/m<2(k/m)^0.5。*/;
                rb.drag += addDrag;
            }
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, Shot.size, targetLayer);

            // 取得したオブジェクトの中で、指定したタグを持つものを処理
            foreach (Collider2D hitCollider in hitColliders)
            {
                target = hitCollider.transform;
                break;
            }
        }
        else
        {
            if (rb != null)
            {
                // m d^2x/dt^2 + b dx/dt + k x = 0 とすると不足減衰の条件は b^2 < 4km → b<2(km)^0.5
                Vector2 hormingForce = (Vector2)(target.position - transform.position) * Shot.speed / 10/*=k*/;

                // ターゲットに向かって前進する
                rb.AddForce(hormingForce);
            }
        }
    }
    void OnDestroy()
    {
        // dragをもとに戻す
        rb.drag -= addDrag;
    }
}
