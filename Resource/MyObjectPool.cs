using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MyObjectPool<T> where T : class, new()
{
    protected ObjectPool<T> pool;

    public MyObjectPool(int defaultCapacity, int maxSize, bool collectionChecks = false)
    {
        pool ??= new ObjectPool<T>(CreatePool, ActionOnGet, ActionOnRelease, ActionOnDestroy, collectionChecks, defaultCapacity, maxSize);
    }

    protected virtual T CreatePool()
    {
        T obj = new();
        return obj;
    }

    protected virtual void ActionOnGet(T obj)
    {
    }

    protected virtual void ActionOnRelease(T obj)
    {
    }

    protected virtual void ActionOnDestroy(T obj)
    {
    }

    public T Get()
    {
        return pool.Get();
    }

    public void Release(T obj)
    {
        pool.Release(obj);
    }

    public void Clear()
    {
        pool.Clear();
    }

}
