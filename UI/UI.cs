using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class UI : MonoBehaviour, IPoolable
{
    public string ID { get; private set; }

    /// <summary>
    /// Getされたとき呼び出される。必ずbase.OnGetを呼ぶこと。
    /// </summary>
    /// <param name="id"></param>
    public virtual void OnGet(string id)
    {
        ID = id;
    }

    public virtual void OnRelease() { }

    public void Release()
    {
        ResourceManager.ReleaseOther(ID, gameObject);
    }
}
