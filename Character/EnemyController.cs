using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;
    Rigidbody2D rb;

    [SerializeField] private Vector2 horizontalSpeed;
    [SerializeField] private Vector2 verticalSpeed;
    float sinPi8;
    Vector2 nextPos;

    // Start is called before the first frame update
    void Start()
    {
        agent = transform.GetComponentInChildren<NavMeshAgent>();
        rb = transform.GetComponent<Rigidbody2D>();
        sinPi8 = math.sin(math.PI / 8);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    /// <summary>
    /// 動かすメソッド。単位円を8等分した点の中で一番近いベクトルを採用。
    /// </summary>
    public void Move()
    {
        // Debug.Log($"agent.path: {agent.path.corners}");
        // for (int i = 0; i < agent.path.corners.Length; i++)
        // {
        //     Debug.Log($"agent.path.corners[{i}]: {agent.path.corners[i]}");
        // }
        if (agent.path.corners.Length >= 2)
        {
            nextPos = transform.position;
            Vector3 moveDirection = (agent.path.corners[1] - transform.position).normalized;
            // Debug.Log($"moveDirection: {moveDirection}");

            if (math.abs(moveDirection.x) > sinPi8)
                // MoveHorizontally(moveDirection.x);
                nextPos += math.sign(moveDirection.x) * Time.deltaTime * horizontalSpeed;
            if (math.abs(moveDirection.y) > sinPi8)
                // MoveVertically(moveDirection.y);
                nextPos += math.sign(moveDirection.y) * Time.deltaTime * verticalSpeed;

            // Debug.Log($"move delta pos: {nextPos - (Vector2)transform.position}");

            rb.MovePosition(nextPos);
        }
    }

    /// <summary>
    /// 横方向に移動。終端速度が狙った値になるよう調整されている。rb.dragは(0,1]でないといけない。
    /// </summary>
    /// <param name="direction">正が右，負が左，0は移動しない。絶対値は関係ない</param>
    public void MoveHorizontally(float direction)
    {
        rb.AddForce(math.sign(direction) * rb.drag * rb.mass * horizontalSpeed / (1f - rb.drag * Time.fixedDeltaTime));
    }

    /// <summary>
    /// 縦方向に移動。
    /// </summary>
    /// <param name="direction">正が上，負が下，0は移動しない。絶対値は関係ない</param>
    public void MoveVertically(float direction)
    {
        rb.AddForce(math.sign(direction) * rb.drag * rb.mass * verticalSpeed / (1f - rb.drag * Time.fixedDeltaTime));
    }
}
