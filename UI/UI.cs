using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class UI : MonoBehaviour, IPoolable
{
    public string ID { get; private set; }

    public void OnGet(string id)
    {
        ID = id;
    }

    public virtual void OnRelease() { }

    public void Release()
    {
        ResourceManager.ReleaseOther(ID, gameObject);
    }
}
