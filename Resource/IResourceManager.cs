using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceManager
{
	GameObject Get(ResourceType type, string id);
	GameObject GetItem(string id);
	void ReleaseItem(IPoolableResourceComponent poolable);
	void ClearItemPool(string id);
	GameObject GetProjectile(string id);
	void ReleaseProjectile(IPoolableResourceComponent poolable);
	void ClearProjectilePool(string id);
	GameObject GetMob(string id);
	void ReleaseMob(IPoolableResourceComponent poolable);
	void ClearMobPool(string id);
	GameObject GetOther(string id);
	void ReleaseOther(string id, GameObject obj);
	void ReleaseOther(IPoolableResourceComponent poolable);
	void ClearOtherPool(string id);
	string GetChunkData(string id);
	BaseTile GetTile(string id);
}
