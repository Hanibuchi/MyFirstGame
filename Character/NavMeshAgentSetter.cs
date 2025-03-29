using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavMeshAgentSetter : MonoBehaviour
{
    [SerializeField] private Transform target;
    NavMeshAgent navMeshAgent;
    /// <summary>
    /// NavMeshAgentの位置をリセットする間隔（tick)
    /// </summary>
    [SerializeField] int posCheckInterval;
    int posCheckTick = 30;
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out navMeshAgent) == false)
            Debug.LogWarning("NavMeshAgent is not attacked");
    }

    void Update()
    {
        ResetNavMeshPos();
    }

    /// <summary>
    /// NavMeshAgentの位置をリセットする。
    /// </summary>
    void ResetNavMeshPos()
    {
        if (posCheckTick >= posCheckInterval && target != null)
        {
            navMeshAgent.destination = target.position;
            transform.localPosition = Vector3.zero;
            // navMeshAgent.nextPosition = transform.position;
            posCheckTick = 0;
        }
        posCheckTick++;
    }

    /// <summary>
    /// ターゲットをセットする。
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
