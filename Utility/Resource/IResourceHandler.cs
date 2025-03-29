using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceHandler
{
    /// <summary>
    /// Getされたときに呼び出される。ここでこのResourceのIDを取得できる。
    /// </summary>
    /// <param name="id"></param>
    public void OnGet(int id) { }

    /// <summary>
    /// Releaseされたとき呼び出される。
    /// </summary>
    public void OnRelease() { }
}
