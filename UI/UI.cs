using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;
using Zenject;

[RequireComponent(typeof(PoolableResourceComponent))]
public class UI : MonoBehaviour
{
    [Inject] protected IResourceManager m_resourceManager;
    protected PoolableResourceComponent m_poolableResourceComponent;

    protected virtual void Awake()
    {
        if (!TryGetComponent(out m_poolableResourceComponent))
            Debug.LogWarning("m_poolableResourceComponent is null");
        m_poolableResourceComponent.ReleaseCallback += OnRelease;
    }

    public virtual void OnRelease() { }
}
