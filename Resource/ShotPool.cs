using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.Pool;

public class ShotPool : MyObjectPool<Shot>
{
    public ShotPool(int defaultCapacity, int maxSize, bool collectionChecks = false) : base(defaultCapacity, maxSize, collectionChecks)
    {
        pool ??= new ObjectPool<Shot>(CreatePool, ActionOnGet, ActionOnRelease, ActionOnDestroy, collectionChecks, defaultCapacity, maxSize);
    }

    protected override void ActionOnGet(Shot item)
    {
    }

    protected override void ActionOnRelease(Shot item)
    {
    }

    protected override void ActionOnDestroy(Shot item)
    {
    }
}
