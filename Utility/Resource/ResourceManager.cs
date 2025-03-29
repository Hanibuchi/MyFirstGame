using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MyGame;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    /// <summary>
    /// Resourceたちをロードできたかどうか。仕様変更の際だるいためあまり使いたくない。falseならGetはnullを返すためそれを使ってほしい。特殊な事情がある場合は使ってもいい。
    /// </summary>
    public static bool IsLoaded { get; private set; }

    /// <summary>
    /// いつでも使用するアセットなどをロードする。
    /// </summary>
    public async void OnAppStart(Action callback)
    {
        IsLoaded = false;
        // Debug.Log($"isLoaded: {IsLoaded}");
        await MakeObjectPoolItem();
        await MakeObjectPoolProjectile();
        await MakeObjectPoolMob();
        await MakeObjectPoolArea();
        await MakeObjectPoolChunk();
        await MakeObjectPoolTileObj();
        await MakeObjectPoolUI();
        await MakeObjectPoolDecorObject();
        await MakeObjectPoolTile();
        await MakeObjectPoolOther();
        IsLoaded = true;
        callback?.Invoke();
    }

    public enum ItemID
    {
        PurpleFurball,
        BubbleWand,
        MagnifyingGlass,
        Homing,

    }
    static readonly Dictionary<ItemID, GameObjectPool> Item = new();
    /// <summary>
    /// ItemSlotのオブジェクトプールを作る。ここに使うスロットのプールの生成処理を並べていく。
    /// </summary>
    async Task MakeObjectPoolItem()
    {
        await MakeObjectPool("Item", Item, ItemDelegate);
    }
    GameObjectPool ItemDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {
            _ => new(result, 5, 10),
        };
    }
    // async Task MakeObjectPoolItem()
    // {
    //     foreach (ItemID name in Enum.GetValues(typeof(ItemID)))
    //     {
    //         List<string> labels = new() { name.ToString(), "Item" };
    //         var handle = Addressables.LoadAssetsAsync<GameObject>(labels, null, Addressables.MergeMode.Intersection);
    //         await handle.Task;
    //         if (handle.Status == AsyncOperationStatus.Succeeded)
    //         {
    //             var result = handle.Result[0];
    //             Item[name] = name switch
    //             {
    //                 _ => new(result, 5, 10),
    //             };
    //         }
    //     }
    // }
    public static GameObject Get(ItemID itemID)
    {
        return Get(Item, itemID);
    }
    public static void Release(ItemID itemID, GameObject item)
    {
        Release(Item, itemID, item);
    }
    public static void Clear(ItemID itemID)
    {
        Clear(Item, itemID);
    }

    public enum ProjectileID
    {
        PurpleFurball,
        BubbleWand,
    }
    static readonly Dictionary<ProjectileID, GameObjectPool> Projectile = new();
    async Task MakeObjectPoolProjectile()
    {
        await MakeObjectPool("Projectile", Projectile, ProjectileDelegate);
    }
    GameObjectPool ProjectileDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {

            _ => new(result, 20, 100),
        };
    }
    public static GameObject Get(ProjectileID projectileID)
    {
        return Get(Projectile, projectileID);
    }
    public static void Release(ProjectileID projectileID, GameObject projectile)
    {
        Release(Projectile, projectileID, projectile);
    }
    public static void Clear(ProjectileID projectileID)
    {
        Clear(Projectile, projectileID);
    }

    public enum MobID
    {
        NPC,
        Enemy,
    }
    static readonly Dictionary<MobID, GameObjectPool> Mob = new();
    async Task MakeObjectPoolMob()
    {
        await MakeObjectPool("Mob", Mob, MobDelegate);
    }
    GameObjectPool MobDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {
            _ => new(result, 5, 100),
        };
    }
    public static GameObject Get(MobID mobID)
    {
        return Get(Mob, mobID);
    }
    public static void Release(MobID mobID, GameObject mob)
    {
        Release(Mob, mobID, mob);
    }
    public static void Clear(MobID mobID)
    {
        Clear(Mob, mobID);
    }

    public enum AreaID
    {
        DefaultArea,
    }
    static readonly Dictionary<AreaID, GameObjectPool> Area = new();
    async Task MakeObjectPoolArea()
    {
        await MakeObjectPool("Area", Area, AreaDelegate);
    }
    GameObjectPool AreaDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {
            _ => new(result, 5, 5),
        };
    }
    public static GameObject Get(AreaID areaID)
    {
        return Get(Area, areaID);
    }
    public static void Release(AreaID areaID, GameObject area)
    {
        Release(Area, areaID, area);
    }
    public static void Clear(AreaID areaID)
    {
        Clear(Area, areaID);
    }

    public enum ChunkID
    {
        DefaultChunk,
    }
    static readonly Dictionary<ChunkID, GameObjectPool> Chunk = new();
    async Task MakeObjectPoolChunk()
    {
        await MakeObjectPool("Chunk", Chunk, ChunkDelegate);
    }
    GameObjectPool ChunkDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {
            _ => new(result, 20, 100),
        };
    }
    public static GameObject Get(ChunkID chunkID)
    {
        return Get(Chunk, chunkID);
    }
    public static void Release(ChunkID chunkID, GameObject chunk)
    {
        Release(Chunk, chunkID, chunk);
    }
    public static void Clear(ChunkID chunkID)
    {
        Clear(Chunk, chunkID);
    }


    public enum TileObjID
    {
        DefaultTile,
    }
    static readonly Dictionary<TileObjID, GameObjectPool> TileObj = new();
    async Task MakeObjectPoolTileObj()
    {
        await MakeObjectPool("TileObj", TileObj, TileObjDelegate);
    }
    GameObjectPool TileObjDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {
            _ => new(result, 81, 729),
        };
    }
    public static GameObject Get(TileObjID tileObjID)
    {
        return Get(TileObj, tileObjID);
    }
    public static void Release(TileObjID tileObjID, GameObject tileObj)
    {
        Release(TileObj, tileObjID, tileObj);
    }
    public static void Clear(TileObjID tileObjID)
    {
        Clear(TileObj, tileObjID);
    }


    public enum UIID
    {
        EquipmentMenuCanvas,
        NPCEquipmentMenu,
        DefaultSlot,
        SlotSpacing,
        InventoryCanvas,
        PlayerStatusUI,
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
    static readonly Dictionary<UIID, GameObjectPool> UI = new();
    async Task MakeObjectPoolUI()
    {
        await MakeObjectPool("UI", UI, UIDelegate);
    }
    GameObjectPool UIDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {
            UIID.DefaultSlot => new(result, 100, 200),
            UIID.SlotSpacing => new(result, 100, 200),
            UIID.NPCEquipmentMenu => new GameObjectPool(result, 10, 10),
            _ => new GameObjectPool(result, 10, 10),
        };
    }
    public static GameObject Get(UIID uiID)
    {
        return Get(UI, uiID);
    }
    public static void Release(UIID uiID, GameObject ui)
    {
        Release(UI, uiID, ui);
    }
    public static void Clear(UIID uiID)
    {
        Clear(UI, uiID);
    }


    public enum DecorObjectID
    {

    }
    /// <summary>
    /// 背景などの飾りを管理
    /// </summary>
    static readonly Dictionary<DecorObjectID, GameObjectPool> DecorObject = new();
    async Task MakeObjectPoolDecorObject()
    {
        await MakeObjectPool("DecorObject", DecorObject, DecorObjectDelegate);
    }
    GameObjectPool DecorObjectDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {
            _ => new(result, 5, 10),
        };
    }
    public static GameObject Get(DecorObjectID decorObjectID)
    {
        return Get(DecorObject, decorObjectID);
    }
    public static void Release(DecorObjectID decorObjectID, GameObject decorObject)
    {
        Release(DecorObject, decorObjectID, decorObject);
    }
    public static void Clear(DecorObjectID decorObjectID)
    {
        Clear(DecorObject, decorObjectID);
    }



    public enum OtherID
    {
        TerrainGrid,
    }
    static readonly Dictionary<OtherID, GameObjectPool> Other = new();
    async Task MakeObjectPoolOther()
    {
        await MakeObjectPool("Other", Other, OtherDelegate);
    }
    GameObjectPool OtherDelegate<TID>(GameObject result, TID name) where TID : Enum
    {
        return name switch
        {
            _ => new GameObjectPool(result, 1, 1),
        };
    }
    public static GameObject Get(OtherID otherID)
    {
        return Get(Other, otherID);
    }
    public static void Release(OtherID otherID, GameObject other)
    {
        Release(Other, otherID, other);
    }
    public static void Clear(OtherID otherID)
    {
        Clear(Other, otherID);
    }


    async Task MakeObjectPool<TID>(string idString, Dictionary<TID, GameObjectPool> dictionary, MakeObjectPoolDelegate<TID> makeObjectPoolDelegate) where TID : Enum
    {
        foreach (TID name in Enum.GetValues(typeof(TID)))
        {
            List<string> labels = new() { name.ToString(), idString };
            var handle = Addressables.LoadAssetsAsync<GameObject>(labels, null, Addressables.MergeMode.Intersection);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var result = handle.Result[0];
                dictionary[name] = makeObjectPoolDelegate(result, name);
            }
        }
    }
    delegate GameObjectPool MakeObjectPoolDelegate<TID>(GameObject result, TID name) where TID : Enum;
    /// <summary>
    /// 汎用的な定義。まだロードされていなければnullを返す
    /// </summary>
    /// <typeparam name="TID"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    static GameObject Get<TID>(Dictionary<TID, GameObjectPool> dictionary, TID id) where TID : struct, Enum
    {
        if (!IsLoaded)
        {
            // Debug.LogWarning("isLoaded is false");
            return null;
        }
        if (dictionary.ContainsKey(id) && dictionary[id] != null)
        {
            var item = dictionary[id].Get();
            var handlers = item.GetComponents<IResourceHandler>();
            foreach (var handler in handlers)
                handler?.OnGet(Convert.ToInt32(id));
            return item;
        }
        Debug.LogWarning("this key is not contained");
        return null;
    }
    static void Release<TID>(Dictionary<TID, GameObjectPool> dictionary, TID id, GameObject item) where TID : struct, Enum
    {
        if (dictionary.ContainsKey(id))
        {
            if (item.TryGetComponent(out IResourceHandler handler))
            {
                handler.OnRelease();
            }
            dictionary[id].Release(item);
        }
        else
            Debug.Log("this id is not contained");
    }
    static void Clear<TID>(Dictionary<TID, GameObjectPool> dictionary, TID id) where TID : struct, Enum
    {
        if (dictionary.ContainsKey(id))
            dictionary[id].Clear();
        else
            Debug.Log("this id is not contained");
    }


    public enum TileID
    {
        None,
        DefaultTile,
    }
    static readonly Dictionary<TileID, BaseTile> Tile = new();
    async Task MakeObjectPoolTile()
    {
        // Debug.Log("loaded other");
        foreach (TileID name in Enum.GetValues(typeof(TileID)))
        {
            if (name == TileID.None)
                continue;

            List<string> labels = new() { name.ToString(), "Tile" };
            var handle = Addressables.LoadAssetsAsync<BaseTile>(labels, null, Addressables.MergeMode.Intersection);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var result = handle.Result[0];
                Tile[name] = result;
                result.OnGet((int)name);
            }
            else
                Debug.LogWarning("load failed");
        }
    }
    public static BaseTile GetTile(TileID tileID)
    {
        if (Tile.ContainsKey(tileID))
        {
            return Tile[tileID];
        }
        // Debug.LogWarning("this key is not contained");
        return null;
    }

    /// <summary>
    /// アセットをロードしたら辞書Assetsに登録するメソッド。
    /// </summary>
    /// <param name="handle"></param>
    // private void OnAssetLoaded(GameObject asset)
    // {
    //     // アセットを辞書に登録する
    //     if (asset != null)
    //     {
    //         // アセットの名前をキーにして辞書に登録（必要に応じて変更）
    //         Assets[asset.name] = asset;
    //         // Debug.Log($"Loaded asset: {asset.name}");
    //     }
    // }

    // 辞書からアセットを取得するメソッド
    // public GameObject GetAsset(string assetName)
    // {
    //     if (Assets.TryGetValue(assetName, out var asset))
    //     {
    //         return asset;
    //     }

    //     Debug.LogWarning($"Asset {assetName} not found in dictionary.");
    //     return null;
    // }
}
