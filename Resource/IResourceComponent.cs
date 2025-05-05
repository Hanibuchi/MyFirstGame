using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceComponent
{
    ResourceType Type { get; }
    string ID { get; }
    /// <summary>
    /// Getされたときに呼び出される。IPoolableはここで必ずIDにidを代入する。そうしないとRelease(IPoolable)が使えない
    /// </summary>
    /// <param name="id"></param>
    void OnGet(ResourceType type, string id);
}

public interface IPoolableResourceComponent : IResourceComponent
{
    /// <summary>
    /// Releaseされたとき呼び出される。
    /// </summary>
    void OnRelease() { }

    void Release(); // どのメソッドでリリースするかわからないためこのメソッド使う。
}
