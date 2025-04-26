using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyGame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    static readonly Dictionary<string, GameObjectPool> itemPools = new();
    static readonly Dictionary<string, GameObjectPool> projectilePools = new();
    static readonly Dictionary<string, GameObjectPool> mobPools = new();
    static readonly Dictionary<string, GameObjectPool> otherPools = new();

    static readonly Dictionary<string, ChunkData> chunkDatas = new();
    static readonly Dictionary<string, BaseTile> baseTiles = new();
    static readonly ShotPool shotPool = new(30, 50);
    public enum AssetType
    {
        Item,
        Projectile,
        Mob,
        Other,
        ChunkData,
        Tile,
    }

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
        EquipmentSlot
    }
    public enum UIID
    {
        EquipmentMenuCanvas,
        NPCEquipmentMenu,
        SlotSpacing,
        InventoryCanvas,
        PlayerStatusUI,
        KeyBindingsUI,
        KeyBindingKeyUI,
        KeyBindingEntryUI,
        SaveSlotUI,
        NewGameUI,
        SaveMenuUI,
        DeleteCautionUI,
        MessageUI,
        GameOverUI,
        GameOverInfoUI,
        TitleUI,
        PauseUI,
        AchievementEntryUI,
        AchievementsUI,
        SettingsUI,
        StatisticsUI,
        StatisticsEntryUI,
    }
    public enum DecorObjectID
    {

    }
    public enum OtherID
    {
        TerrainGrid,
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

    /// <summary>
    /// いつでも使用するアセットなどをロードする。
    /// </summary>
    public async void OnAppStart(Action callback)
    {
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
        foreach (AssetType kind in Enum.GetValues(typeof(AssetType)))
        {
            switch (kind)
            {
                case AssetType.Item:
                    await LoadAssetsToObjectPool(groupName, kind);
                    break;
                case AssetType.Projectile:
                    await LoadAssetsToObjectPool(groupName, kind);
                    break;
                case AssetType.Mob:
                    await LoadAssetsToObjectPool(groupName, kind);
                    break;
                case AssetType.Other:
                    await LoadAssetsToObjectPool(groupName, kind);
                    break;
                case AssetType.ChunkData:
                    await LoadChunkData(groupName);
                    break;
                case AssetType.Tile:
                    await LoadTile(groupName);
                    break;
            }
        }
    }

    async Task LoadAssetsToObjectPool(string groupName, AssetType kind)
    {
        Dictionary<string, GameObjectPool> dict;
        MakeObjectPoolDelegate poolDelegate;
        switch (kind)
        {
            case AssetType.Item:
                dict = itemPools;
                poolDelegate = ItemDelegate;
                break;
            case AssetType.Projectile:
                dict = projectilePools;
                poolDelegate = ProjectileDelegate;
                break;
            case AssetType.Mob:
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
        List<string> labels = new() { groupName, AssetType.ChunkData.ToString() };
        var locationHandle = Addressables.LoadResourceLocationsAsync(labels, Addressables.MergeMode.Intersection, typeof(ChunkAsset));
        await locationHandle.Task;

        if (locationHandle.Status != AsyncOperationStatus.Succeeded || locationHandle.Result.Count == 0)
        {
            Debug.LogWarning($"No assets found for labels: {String.Join(", ", labels)}");
            return;
        }

        var loadTasks = new List<Task>();
        foreach (var location in locationHandle.Result)
        {
            var loadHandle = Addressables.LoadAssetAsync<ChunkAsset>(location);
            loadHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var asset = handle.Result;
                    chunkDatas[asset.name] = asset.chunkData;
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
        List<string> labels = new() { groupName, AssetType.Tile.ToString() };
        var locationHandle = Addressables.LoadResourceLocationsAsync(labels, Addressables.MergeMode.Intersection, typeof(BaseTile));
        await locationHandle.Task;

        if (locationHandle.Status != AsyncOperationStatus.Succeeded || locationHandle.Result.Count == 0)
        {
            Debug.LogWarning($"No assets found for labels: {String.Join(", ", labels)}");
            return;
        }

        var loadTasks = new List<Task>();
        foreach (var location in locationHandle.Result)
        {
            var loadHandle = Addressables.LoadAssetAsync<BaseTile>(location);
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


    public static GameObject GetItem(string id)
    {
        return GetFromObjectPool(itemPools, id);
    }
    public static void ReleaseItem(IPoolable poolable)
    {
        if (poolable is MonoBehaviour mono)
            Release(itemPools, poolable.ID, mono.gameObject);
        else
            Debug.LogWarning("IPoolable must extend MonoBehaviour");
    }
    public static void ClearItemPool(string id)
    {
        Clear(itemPools, id);
    }


    public static GameObject GetProjectile(string id)
    {
        return GetFromObjectPool(projectilePools, id);
    }
    public static void ReleaseProjectile(IPoolable poolable)
    {
        if (poolable is MonoBehaviour mono)
            Release(projectilePools, poolable.ID, mono.gameObject);
        else
            Debug.LogWarning("IPoolable must extend MonoBehaviour");
    }
    public static void ClearProjectilePool(string id)
    {
        Clear(projectilePools, id);
    }

    public static GameObject GetMob(string id)
    {
        return GetFromObjectPool(mobPools, id);
    }
    public static void ReleaseMob(IPoolable poolable)
    {
        if (poolable is MonoBehaviour mono)
            Release(mobPools, poolable.ID, mono.gameObject);
        else
            Debug.LogWarning("IPoolable must extend MonoBehaviour");
    }
    public static void ClearMobPool(string id)
    {
        Clear(mobPools, id);
    }

    public static GameObject GetOther(string id)
    {
        return GetFromObjectPool(otherPools, id);
    }
    public static void ReleaseOther(string id, GameObject obj)
    {
        Release(otherPools, id, obj);
    }
    public static void ReleaseOther(IPoolable poolable)
    {
        if (poolable is MonoBehaviour mono)
            Release(otherPools, poolable.ID, mono.gameObject);
        else
            Debug.LogWarning("IPoolable must extend MonoBehaviour");
    }
    public static void ClearOtherPool(string id)
    {
        Clear(otherPools, id);
    }

    public static ChunkData GetChunkData(string id)
    {
        if (!chunkDatas.ContainsKey(id) || chunkDatas[id] == null)
        {
            Debug.LogWarning($"this key({id}) is not contained");
            return null;
        }
        var chunkData = chunkDatas[id];
        if (chunkData is IPoolable handler)
        {
            handler?.OnGet(id);
        }
        return chunkData;
    }

    public static BaseTile GetTile(string id)
    {
        if (!baseTiles.ContainsKey(id) || baseTiles[id] == null)
        {
            Debug.LogWarning($"this key({id}) is not contained");
            return null;
        }
        var baseTile = baseTiles[id];
        if (baseTile is IPoolable handler)
        {
            handler?.OnGet(id);
        }
        return baseTile;
    }

    static GameObject GetFromObjectPool(Dictionary<string, GameObjectPool> dictionary, string id)
    {
        if (!dictionary.ContainsKey(id) || dictionary[id] == null)
        {
            Debug.LogWarning($"this key({id}) is not contained");
            return null;
        }
        var gameObj = dictionary[id].Get();
        var handlers = gameObj.GetComponents<IPoolable>();
        foreach (var handler in handlers)
        {
            handler?.OnGet(id);
        }
        return gameObj;
    }
    static void Release(Dictionary<string, GameObjectPool> dictionary, string id, GameObject item)
    {
        if (!dictionary.ContainsKey(id) || dictionary[id] == null)
        {
            Debug.Log("this id is not contained");
            return;
        }
        if (item.TryGetComponent(out IPoolable handler))
        {
            handler.OnRelease();
        }
        dictionary[id].Release(item);
    }
    static void Clear(Dictionary<string, GameObjectPool> dictionary, string id)
    {
        if (!dictionary.ContainsKey(id) || dictionary[id] == null)
        {
            Debug.Log("this id is not contained");
            return;
        }
        dictionary[id].Clear();
    }

    public static Shot GetShot()
    {
        return shotPool.Get();
    }
    public static void ReleaseShot(Shot shot)
    {
        shotPool.Release(shot);
    }
}
