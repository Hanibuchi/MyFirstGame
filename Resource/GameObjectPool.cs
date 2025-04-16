using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool : MyObjectPool<GameObject>
{
	protected GameObject obj;
	public GameObjectPool(GameObject obj, int defaultCapacity, int maxSize, bool collectionChecks = false) : base(defaultCapacity, maxSize, collectionChecks)
	{
		this.obj = obj;
		pool ??= new ObjectPool<GameObject>(CreatePool, ActionOnGet, ActionOnRelease, ActionOnDestroy, collectionChecks, defaultCapacity, maxSize);
	}

	protected override GameObject CreatePool()
	{
		GameObject item = Object.Instantiate(obj);
		return item;
	}

	protected override void ActionOnGet(GameObject item)
	{
		item.SetActive(true);
	}

	protected override void ActionOnRelease(GameObject item)
	{
		item.SetActive(false);
	}

	protected override void ActionOnDestroy(GameObject item)
	{
		Object.Destroy(item);
	}
}