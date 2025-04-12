using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceHandler
{
    /// <summary>
    /// Getされたときに呼び出される。IPoolableはここで必ずIDにidを代入する。そうしないとRelease(IPoolable)が使えない
    /// </summary>
    /// <param name="id"></param>
    public void OnGet(string id);
}

public interface IPoolable : IResourceHandler
{
    public string ID { get; }
    /// <summary>
    /// Releaseされたとき呼び出される。
    /// </summary>
    public void OnRelease() { }

    public void Release(); // どのメソッドでリリースするかわからないためこのメソッド使う。
}
