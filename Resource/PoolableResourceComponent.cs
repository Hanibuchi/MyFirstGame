using System;
using Zenject;

public class PoolableResourceComponent : ResourceComponent, IPoolableResourceComponent
{
	public event Action ReleaseCallback;
	/// <summary>
	/// Releaseされたとき呼び出される。
	/// </summary>
	public void OnRelease()
	{
		ReleaseCallback?.Invoke();
	}

	public void Release()
	{
		switch (Type)
		{
			case ResourceType.Item: ResourceManager.Instance.ReleaseItem(this); break;
			case ResourceType.Projectile: ResourceManager.Instance.ReleaseProjectile(this); break;
			case ResourceType.Mob: ResourceManager.Instance.ReleaseMob(this); break;
			case ResourceType.Other: ResourceManager.Instance.ReleaseOther(this); break;
		}
	}
}//