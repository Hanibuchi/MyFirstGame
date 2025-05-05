using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

[RequireComponent(typeof(PoolableResourceComponent))]
public class UI : MonoBehaviour
{
    protected PoolableResourceComponent m_poolableResourceComponent;

    protected virtual void Awake()
    {
        if (!TryGetComponent(out m_poolableResourceComponent))
            Debug.LogWarning("m_poolableResourceComponent is null");
        m_poolableResourceComponent.ReleaseCallback += OnRelease;
    }

    public virtual void OnRelease() { }
}
