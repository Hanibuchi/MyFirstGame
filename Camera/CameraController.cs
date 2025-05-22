using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem; // InputSystemの名前空間

namespace MyGame
{
    public class CameraController : MonoBehaviour
    {
        public float LimitCameraRadius;
        public float deadZoneRadius; // playerから半径deadZoneRadius以内ではカメラは動かないようにする。
        public float Speed;
        public Camera mainCamera;// メインカメラ
        [SerializeField] Transform player;  // プレイヤーのTransform
        public Vector3 offset;    // カメラとプレイヤーのオフセット
        Vector3 deltaPos; // playerからmouseまでのベクトル
        Rigidbody2D rb;

        void LateUpdate()
        {
            if (player == null && GameManager.Instance.Player != null)
                player = GameManager.Instance.Player.transform;

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0;

            if (player != null)
                if ((player.position - mouseWorldPos).magnitude > deadZoneRadius)
                {
                    deltaPos = mouseWorldPos - player.position;
                    deltaPos.z = 0;
                    if (deltaPos.magnitude >= LimitCameraRadius)
                        deltaPos = deltaPos.normalized * LimitCameraRadius;

                    MoveCamera(player.position + deltaPos + offset);
                }
                else
                    MoveCamera(player.position);
        }

        // 目標に向かってゆっくり動く
        public void MoveCamera(Vector3 target)
        {
            if (rb != null || mainCamera.TryGetComponent(out rb))
                rb.AddForce(((target - transform.position) * 2 - (Vector3)rb.velocity) * Speed);
        }
    }
}
