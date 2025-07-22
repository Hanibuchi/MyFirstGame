using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour, IInitializableResourceManager, IResourceManager
{
    // 以下のIDは直接には使用されず，.ToString()で文字列に変換されて使用される．
    public enum ItemID
    {
        PurpleFurball,
        BubbleWand,
        MagnifyingGlass,
        Homing,

    }
    public enum ProjectileID
    {
        PurpleFurball_Projectile,
        BubbleWand_Projectile,
    }
    public enum MobID
    {
        NPC,
        Enemy,
    }
    // 以下はすべてOtherに分類される
    public enum AreaID
    {
        DefaultArea,
    }
    public enum TileObjID
    {
        DefaultTileObj,
    }
    public enum ItemSlotID
    {
        DefaultSlot,
        InventorySlot,
    }
    /// <summary>
    /// UIのリソースの名前を管理するためのenum。UIPageはUIPageTypeで管理されてる。
    /// </summary>
    public enum UIID
    {
        EquipmentUI,
        MemberEquipmentUI,
        SlotSpacing,
        KeyBindingsUI,
        KeyBindingKeyUI,
        KeyBindingEntryUI,
        SaveSlotUI,
        MessageUI,
        GameOverUI,
        GameOverInfoUI,
        AchievementEntryUI,
        StatisticsEntryUI,
    }
    public enum DecorObjectID
    {

    }
    public enum OtherID
    {
        TerrainGrid,
        Party,
        ChunkManager,
        Light,
    }

    public enum ChunkID
    {
        DefaultChunk,
    }

    public enum TileID
    {
        None,
        DefaultTile,
    }
    readonly Dictionary<string, GameObjectPool> itemPools = new();
    readonly Dictionary<string, GameObjectPool> projectilePools = new();
    readonly Dictionary<string, GameObjectPool> mobPools = new();
    readonly Dictionary<string, GameObjectPool> otherPools = new();

    readonly Dictionary<string, string> chunkDatas = new();
    readonly Dictionary<string, MyTile> baseTiles = new();
    readonly ShotPool shotPool = new(30, 50);

    public static IResourceManager Instance { get; private set; }
    /// <summary>
    /// いつでも使用するアセットなどをロードする。
    /// </summary>
    public async void OnAppStart(Action callback)
    {
        Instance = this;
        await LoadAsset("InitLoad");

        callback?.Invoke();
    }

    /// <summary>
    /// アセットごとにオブジェクトプールの設定を替えるためのデリゲート。
    /// </summary>
    /// <param name="result"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    delegate GameObjectPool MakeObjectPoolDelegate(GameObject result, string id);
    GameObjectPool ItemDelegate(GameObject result, string id)
    {
        return id switch
        {
            _ => new(result, 5, 10),
        };
    }
    GameObjectPool ProjectileDelegate(GameObject result, string id)
    {
        return id switch
        {
            _ => new(result, 20, 100),
        };
    }
    GameObjectPool MobDelegate(GameObject result, string id)
    {
        return id switch
        {
            _ => new(result, 5, 100),
        };
    }
    GameObjectPool OtherDelegate(GameObject result, string id)
    {
        return id switch
        {
            _ => new(result, 5, 100),
        };
    }

    /// <summary>
    /// アセットをロードする。アセットはItem, Projectile, Mob, Otherのどれかのラベルを持っていないといけない。特にItem, Projectile, MobはそれぞれのIDを持っている必要がある。
    /// </summary>
    /// <param name="groupName">ロードするアセットがもつ共通のラベル</param>
    async Task LoadAsset(string groupName)
    {
        foreach (ResourceType kind in Enum.GetValues(typeof(ResourceType)))
        {
            switch (kind)
            {
                case ResourceType.Item:
                    await LoadAssetsToObjectPool(groupName, kind);
                    break;
                case ResourceType.Projectile:
                    await LoadAssetsToObjectPool(groupName, kind);
                    break;
                case ResourceType.Mob:
                    await LoadAssetsToObjectPool(groupName, kind);
                    break;
                case ResourceType.Other:
                    await LoadAssetsToObjectPool(groupName, kind);
                    break;
                case ResourceType.ChunkData:
                    await LoadChunkData(groupName);
                    break;
                case ResourceType.Tile:
                    await LoadTile(groupName);
                    break;
            }
        }
    }

    async Task LoadAssetsToObjectPool(string groupName, ResourceType kind)
    {
        Dictionary<string, GameObjectPool> dict;
        MakeObjectPoolDelegate poolDelegate;
        switch (kind)
        {
            case ResourceType.Item:
                dict = itemPools;
                poolDelegate = ItemDelegate;
                break;
            case ResourceType.Projectile:
                dict = projectilePools;
                poolDelegate = ProjectileDelegate;
                break;
            case ResourceType.Mob:
                dict = mobPools;
                poolDelegate = MobDelegate;
                break;
            default:
                dict = otherPools;
                poolDelegate = OtherDelegate;
                break;
        }

        List<string> labels = new() { groupName, kind.ToString() };
        var locationHandle = Addressables.LoadResourceLocationsAsync(labels, Addressables.MergeMode.Intersection);
        await locationHandle.Task;

        if (locationHandle.Status != AsyncOperationStatus.Succeeded || locationHandle.Result.Count == 0)
        {
            Debug.LogWarning($"No assets found for labels: {groupName}, {kind}");
            return;
        }

        var loadTasks = new List<Task>();
        foreach (var location in locationHandle.Result)
        {
            var loadHandle = Addressables.LoadAssetAsync<GameObject>(location);
            loadHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var asset = handle.Result;
                    Debug.Log($"name: {asset.name}");
                    dict[asset.name] = poolDelegate(asset, asset.name);
                }
                else
                {
                    Debug.LogWarning($"Failed to load asset: {location.PrimaryKey}");
                }
            };
            loadTasks.Add(loadHandle.Task);
        }
        await Task.WhenAll(loadTasks);
    }

    async Task LoadChunkData(string groupName)
    {
        List<string> labels = new() { groupName, ResourceType.ChunkData.ToString() };

        // JSONファイルを読み込む前提で TextAsset をターゲット型に
        var locationHandle = Addressables.LoadResourceLocationsAsync(labels, Addressables.MergeMode.Intersection, typeof(TextAsset));
        await locationHandle.Task;

        if (locationHandle.Status != AsyncOperationStatus.Succeeded || locationHandle.Result.Count == 0)
        {
            Debug.LogWarning($"No JSON files found for labels: {String.Join(", ", labels)}");
            return;
        }

        var loadTasks = new List<Task>();
        foreach (var location in locationHandle.Result)
        {
            var loadHandle = Addressables.LoadAssetAsync<TextAsset>(location);

            loadHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var textAsset = handle.Result;
                    chunkDatas[textAsset.name] = textAsset.text;
                    Debug.Log($"chunkname: {textAsset.name}");
                }
                else
                {
                    Debug.LogWarning($"Failed to load asset: {location}");
                }
            };

            loadTasks.Add(loadHandle.Task);
        }

        await Task.WhenAll(loadTasks);
    }

    async Task LoadTile(string groupName)
    {
        List<string> labels = new() { groupName, ResourceType.Tile.ToString() };
        var locationHandle = Addressables.LoadResourceLocationsAsync(labels, Addressables.MergeMode.Intersection, typeof(MyTile));
        await locationHandle.Task;

        if (locationHandle.Status != AsyncOperationStatus.Succeeded || locationHandle.Result.Count == 0)
        {
            Debug.LogWarning($"No assets found for labels: {String.Join(", ", labels)}");
            return;
        }

        var loadTasks = new List<Task>();
        foreach (var location in locationHandle.Result)
        {
            var loadHandle = Addressables.LoadAssetAsync<MyTile>(location);
            loadHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var asset = handle.Result;
                    baseTiles[asset.name] = asset;
                }
                else
                {
                    Debug.LogWarning($"Failed to load asset: {location}");
                }
            };
            loadTasks.Add(loadHandle.Task);
        }
        await Task.WhenAll(loadTasks);
    }


    public GameObject GetItem(string id)
    {
        return Get(ResourceType.Item, id);
    }
    public void ReleaseItem(IPoolableResourceComponent poolable)
    {
        if (poolable is MonoBehaviour mono)
            Release(itemPools, poolable.ID, mono.gameObject);
        else
            Debug.LogWarning("IPoolable must extend MonoBehaviour");
    }
    public void ClearItemPool(string id)
    {
        Clear(itemPools, id);
    }


    public GameObject GetProjectile(string id)
    {
        return Get(ResourceType.Projectile, id);
    }
    public void ReleaseProjectile(IPoolableResourceComponent poolable)
    {
        if (poolable is MonoBehaviour mono)
            Release(projectilePools, poolable.ID, mono.gameObject);
        else
            Debug.LogWarning("IPoolable must extend MonoBehaviour");
    }
    public void ClearProjectilePool(string id)
    {
        Clear(projectilePools, id);
    }

    public GameObject GetMob(string id)
    {
        return Get(ResourceType.Mob, id);
    }
    public void ReleaseMob(IPoolableResourceComponent poolable)
    {
        if (poolable is MonoBehaviour mono)
            Release(mobPools, poolable.ID, mono.gameObject);
        else
            Debug.LogWarning("IPoolable must extend MonoBehaviour");
    }
    public void ClearMobPool(string id)
    {
        Clear(mobPools, id);
    }

    public GameObject GetOther(string id)
    {
        return Get(ResourceType.Other, id);
    }
    public void ReleaseOther(string id, GameObject obj)
    {
        Release(otherPools, id, obj);
    }
    public void ReleaseOther(IPoolableResourceComponent poolable)
    {
        if (poolable is MonoBehaviour mono)
            Release(otherPools, poolable.ID, mono.gameObject);
        else
            Debug.LogWarning("IPoolable must extend MonoBehaviour");
    }
    public void ClearOtherPool(string id)
    {
        Clear(otherPools, id);
    }

    public string GetChunkData(string id)
    {
        if (!chunkDatas.ContainsKey(id))
        {
            Debug.LogWarning($"this key({id}) is not contained");
            return "";
        }
        var chunkData = chunkDatas[id];
        return chunkData;
    }

    public MyTile GetTile(string id)
    {
        if (id == null)
        {
            Debug.LogWarning("id can not be null");
            return null;
        }
        if (!baseTiles.ContainsKey(id))
        {
            Debug.LogWarning($"this key({id}) is not contained");
            return null;
        }
        var baseTile = baseTiles[id];
        if (baseTile is IPoolableResourceComponent handler)
        {
            handler?.OnGet(ResourceType.Tile, id);
        }
        return baseTile;
    }

    public GameObject Get(ResourceType type, string id)
    {
        switch (type)
        {
            case ResourceType.Item:
                return GetFromObjectPool(type, itemPools, id);
            case ResourceType.Projectile:
                return GetFromObjectPool(type, projectilePools, id);
            case ResourceType.Mob:
                return GetFromObjectPool(type, mobPools, id);
            case ResourceType.Other:
                return GetFromObjectPool(type, otherPools, id);
            default:
                Debug.LogWarning($"Resource type {type} is not supported.");
                return null;
        }
    }

    GameObject GetFromObjectPool(ResourceType type, Dictionary<string, GameObjectPool> dictionary, string id)
    {
        if (!dictionary.ContainsKey(id))
        {
            Debug.LogWarning($"this key({id}) is not contained");
            return null;
        }
        var gameObj = dictionary[id].Get();
        var handlers = gameObj.GetComponents<IPoolableResourceComponent>();
        foreach (var handler in handlers)
        {
            handler?.OnGet(type, id);
        }
        return gameObj;
    }
    void Release(Dictionary<string, GameObjectPool> dictionary, string id, GameObject item)
    {
        if (!dictionary.ContainsKey(id))
        {
            Debug.Log("this id is not contained");
            return;
        }
        if (item.TryGetComponent(out IPoolableResourceComponent handler))
        {
            handler.OnRelease();
        }
        dictionary[id].Release(item);
    }
    void Clear(Dictionary<string, GameObjectPool> dictionary, string id)
    {
        if (!dictionary.ContainsKey(id))
        {
            Debug.Log("this id is not contained");
            return;
        }
        dictionary[id].Clear();
    }

    public Shot GetShot()
    {
        return shotPool.Get();
    }
    public void ReleaseShot(Shot shot)
    {
        shotPool.Release(shot);
    }
}
public enum ResourceType
{
    Item,
    Projectile,
    Mob,
    Other,
    ChunkData,
    Tile,
}