using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool
{
	public GameObject Prefab;
	ObjectPool<GameObject> m_Pool;
	ObjectPool<GameObject> Pool
	{
		get
		{
			return m_Pool;
		}
	}

	public GameObjectPool(GameObject prefab, int defaultCapacity, int maxSize, bool collectionChecks = false)
	{
		Prefab = prefab;
		m_Pool ??= new ObjectPool<GameObject>(CreatePool, ActionOnGet, ActionOnRelease, ActionOnDestroy, collectionChecks, defaultCapacity, maxSize);
	}

	GameObject CreatePool()
	{
		GameObject item = Object.Instantiate(Prefab);
		return item;
	}

	void ActionOnGet(GameObject item)
	{
		item?.SetActive(true);
	}

	void ActionOnRelease(GameObject item)
	{
		item.SetActive(false);
	}

	void ActionOnDestroy(GameObject item)
	{
		Object.Destroy(item);
	}

	public GameObject Get()
	{
		return Pool.Get();
	}

	public void Release(GameObject prefab)
	{
		Pool.Release(prefab);
	}

	public void Clear()
	{
		Pool.Clear();
	}
}