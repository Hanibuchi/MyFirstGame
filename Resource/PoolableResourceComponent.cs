using System;

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
			case ResourceType.Item: ResourceManager.ReleaseItem(this); break;
			case ResourceType.Projectile: ResourceManager.ReleaseProjectile(this); break;
			case ResourceType.Mob: ResourceManager.ReleaseMob(this); break;
			case ResourceType.Other: ResourceManager.ReleaseOther(this); break;
		}
	}
}//