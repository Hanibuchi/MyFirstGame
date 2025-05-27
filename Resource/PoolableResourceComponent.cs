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

    [Inject] IResourceManager m_resourceManager;
	public void Release()
	{
		switch (Type)
		{
			case ResourceType.Item: m_resourceManager.ReleaseItem(this); break;
			case ResourceType.Projectile: m_resourceManager.ReleaseProjectile(this); break;
			case ResourceType.Mob: m_resourceManager.ReleaseMob(this); break;
			case ResourceType.Other: m_resourceManager.ReleaseOther(this); break;
		}
	}
}//