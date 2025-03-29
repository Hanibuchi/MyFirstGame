using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class UI : MonoBehaviour, IResourceHandler
{
    public ResourceManager.UIID ID { get; private set; }

    public void OnGet(int id)
    {
        ID = (ResourceManager.UIID)id;
    }

    public virtual void OnRelease() { }

    public void Release()
    {
        ResourceManager.Release(ID, gameObject);
    }
}
