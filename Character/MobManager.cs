using System;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using Unity.Mathematics;
using UnityEngine;

public class MobManager : ObjectManager, IItemOwner
{

    [SerializeField] int selectedSlotNumber;
    /// <summary>
    /// 選択されているスロットの番号。この番号のアイテムが実行される。
    /// </summary>
    public int SelectedSlotNumber
    {
        get => selectedSlotNumber;
        private set => selectedSlotNumber = (value + ItemCapacity) % ItemCapacity;
    }

    /// <summary>
    /// アイテムを持つ位置を調整
    /// </summary>
    protected Vector2 HandOffset;
    /// <summary>
    /// ドロップと確率
    /// </summary>
    [SerializeField] private List<DropItem> dropItems;


    protected override void ResetToGeneratedStatus()
    {// ResetToBaseを後で実行するためにこれは上の処理の後に実行する。

        ((IItemOwner)this).ResetItems();
    }

    public void SetSelectedSlotNumber(int num)
    {
        SelectedSlotNumber = num;
    }
    public void AddSelectedSlotNumber(int num)
    {
        SetSelectedSlotNumber(SelectedSlotNumber + num);
    }

    Attack m_attack;

    /// <summary>
    /// 攻撃する。itemNumberで何番目のアイテムを使用するか選ぶ。shotのNextProjectiles2はそのアイテム自身を設定する。ItemModifierを持って攻撃する場合を考えて何とかしないといけない
    /// </summary>
    /// <param name="target">目標の*グローバル座標*</param>
    /// <returns>次に攻撃できるまでの時間(s)を返す</returns>
    public void Fire(Vector2 target)
    {
        // if (IsDead)
        //     return;

        if (target == null)
            Debug.LogWarning("target is null");

        if (Items[SelectedSlotNumber] != null)
        {
            SetItemPosition(Items[SelectedSlotNumber].gameObject, target - (Vector2)transform.position);

            Shot shot = new();
            shot.SetCore(this, Items[SelectedSlotNumber].gameObject, target, m_attack.TargetLayer, m_attack.Damage);
            // shot.SetExtra(CurrentTargetLayer, Damage.Zero, 0, 0, 0, 0, 0, 0);

            Items[SelectedSlotNumber].FirstFire(shot);
        }
        else
        {
            // ここに選択されているアイテムがnullのとき行う処理を書く。例えば素手で殴るなど。Mobごとのデフォルトの攻撃手段を記述。
            DefaultFire(target);
        }
    }


    // Mobごとのデフォルトの攻撃。例えば素手で殴るなど
    protected void DefaultFire(Vector2 target)
    {

    }

    public void ApplyRecoil(Shot shot)
    {
        Vector2 direction = shot.target - (Vector2)transform.position;
        // Debug.Log($"direction: {direction} = shot.Target: {shot.Target} - user.transform.position: {transform.position}");
        Vector3 recoilForce = (-1) * shot.recoil * direction.normalized;
        GetComponent<Rigidbody2D>().AddForce(recoilForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Colliderと重なったアイテムを手持ちに追加。拾うのは一つだけ。
    /// </summary>
    public void PickupItem()
    {
        List<Collider2D> colliders = DetectNearbyColliders(new ContactFilter2D().NoFilter());

        // 検出されたコライダーの中から，Itemコンポネントを持つものを一つ見つけて手持ちに追加する
        foreach (Collider2D collider in colliders)
        {
            // Itemクラスの子クラスを持つコンポーネントがあるか確認
            if (collider.TryGetComponent(out Item item))
            {
                if (SubPickupItem(item))
                    return;
            }
        }
    }

    /// <summary>
    /// アイテムを拾うメソッドPickupItemのサブメソッド。追加できたらtrueを返す。
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual bool SubPickupItem(Item item)
    {
        if (OwnerParty != null && OwnerParty.CanAddItem(item))
        {
            OwnerParty.AddItem(item);
            return true;
        }
        else
        {
            if (CanAddItem(item))
            {
                AddItem(item);
                return true;
            }
            else return false;
        }
    }

    /// <summary>
    /// 子オブジェクト達と重なっているコライダーをすべて検出する。
    /// </summary>
    /// <returns></returns>
    protected List<Collider2D> DetectNearbyColliders(ContactFilter2D filter2D)
    {
        // コライダーと重なるコライダーを格納するためのリストを準備
        List<Collider2D> colliders = new();

        if (TryGetComponent(out Collider2D mobCollider))
            mobCollider.OverlapCollider(filter2D, colliders);

        return colliders;
    }









    // ここからアイテム管理関連の改良版

    private void Awake()
    {
        Owner = this;
        InitItems();
    }
    [SerializeField] Party m_party;
    public Party OwnerParty
    {
        get { return m_party; }
        set
        {
            if (value != m_party)
            {
                m_party = value;

                foreach (Item item in Items)
                    if (item != null)
                        item.OwnerParty = value;
            }
        }
    }
    public IItemOwner Owner { get; set; }
    [SerializeField] List<Item> m_items = new();
    /// <summary>
    /// 装備アイテム。Listを使っているが，基本的にAddなどでサイズを変更してはならない。
    /// </summary>
    public List<Item> Items { get => m_items; private set => m_items = value; }
    /// <summary>
    /// 装備できるアイテムの最大値
    /// </summary>
    public int ItemCapacity { get; set; } = 4;
    public void InitItems()
    {
        ((IItemOwner)this).ResetItems();
    }
    public bool CanAddItem(Item item)
    {
        if (Items.Count(x => x == null) >= 1)
            return true;
        else
        {
            foreach (var nextItem in Items)
            {
                if (nextItem is BagItem bag)
                {
                    if (bag.CanAddItem(item))
                        return true;
                }
            }
        }
        return false;
    }
    public bool CanAddItem(int index, Item item)
    {
        if (0 <= index && index < Items.Count && item != null && Items[index] == null)
            return true;
        else
            return false;
    }
    public void AddItem(Item item)
    {
        if (CanAddItem(item))
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null && CanAddItem(i, item))
                {
                    AddItem(i, item);
                    return;
                }
            }
            foreach (var nextItem in Items)
            {
                if (nextItem is BagItem bag && bag.CanAddItem(item))
                {
                    bag.AddItem(item);
                    return;
                }
            }
        }
    }
    public void AddItem(int index, Item item)
    {
        if (CanAddItem(index, item))
        {
            item.RemovePrevRelation();
            Items[index] = item;
            item.OnAdded(this);
        }
        else
            Debug.LogWarning("item cannot be added");
    }
    public void RemoveItem(Item item)
    {
        if (Items.Contains(item))
        {
            int index = Items.IndexOf(item);
            Items[index] = null;
            item.OnRemoved();
        }
        else
            Debug.LogWarning("Item is not in Items.");
    }
    public virtual void RefreshItemSlotUIs()
    {

    }

    public void RegisterItem(Item item)
    {
        Debug.Log("Mob, RegisterItem was called");
    }
    public void UnregisterItem(Item item)
    {
        Debug.Log("Mob, UnregisterItem was called");
    }
    public virtual void AddItemSlot()
    {
        ItemCapacity++;
        Items.Add(null);
    }
    // ここまでアイテム管理関連の改良版







    /// <summary>
    /// アイテムを投げる。アイテムは所有している前提（注意）。アイテムは所有していなくてもいい。targetはグローバル座標。
    /// </summary>
    public void ThrowItem(Vector2 target)
    {
        RemoveItem(Items[SelectedSlotNumber]);
        ThrowItem(Items[SelectedSlotNumber], target);
    }
    /// <summary>
    /// アイテムを投げる。アイテムはアイテム化してる前提。アイテムは所有していなくてもいい。
    /// </summary>
    public virtual void ThrowItem(Item item)
    {
        var target = (Vector2)transform.position + Vector2.up;
        ThrowItem(item, target);
    }
    /// <summary>
    /// アイテムを投げる。アイテムはアイテム化してる前提。アイテムは所有していなくてもいい。targetはグローバル座標。
    /// </summary>
    /// <param name="item"></param>
    /// <param name="target"></param>
    public virtual void ThrowItem(Item item, Vector2 target)
    {
        if (item == null)
            Debug.LogWarning("item is null");
        if (item.Owner != null)
        {
            Debug.LogWarning("item owner is not null");
            item.Parent.RemoveItem(item);
        }

        Shot shot = new()
        {
            projectiles = new List<GameObject>() { item.gameObject },
            targetLayer = m_attack.TargetLayer,
            target = target,
            speed = (target - (Vector2)transform.position).magnitude
        };
        // Debug.Log($"Projectiles: {shot.Projectiles}");
        // Debug.Log($"TargetLayer: {shot.TargetLayer}");
        // Debug.Log($"Target: {shot.Target}");
        // Debug.Log($"Speed: {shot.Speed}");
        // Debug.Log($"shot:{shot} ");
        SetItemPosition(item.gameObject, target - (Vector2)transform.position);
        item.ThrowItem(shot);
    }

    /// <summary>
    /// アイテムの発射位置を決めるメソッド。itemは持つアイテム，targetは狙う点のローカル座標
    /// </summary>
    /// <param name="item"></param>
    /// <param name="target"></param>
    protected void SetItemPosition(GameObject item, Vector2 target)
    {
        Vector2 gripPosition = new(transform.lossyScale.x * HandOffset.x, transform.lossyScale.y * HandOffset.y);
        Vector2 direction = target - gripPosition;
        float angle = Mathf.Atan2(direction.y, direction.x);
        item.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg - 90));

        Vector2 wandPivotOffset = Vector2.zero;
        if (transform.GetChild(0) != null && transform.GetChild(0).TryGetComponent(out SpriteRenderer sprite))
        {
            Vector2 spriteSize = sprite.bounds.size;
            wandPivotOffset = new Vector2(spriteSize.y / 2f * Mathf.Cos(angle), spriteSize.y / 2f * Mathf.Sin(angle));
        }
        item.transform.position = (Vector2)gameObject.transform.position + gripPosition + wandPivotOffset;
    }


    public MobData MakeMobData()
    {
        return FillMobData(new MobData());
    }

    protected MobData FillMobData(MobData mobData)
    {
        base.FillObjectData(mobData);

        foreach (var item in Items)
        {
            if (item == null)
                mobData.Items.Add(null);
            else
            {
                if (item.TryGetComponent(out ObjectManager objectManager))
                    mobData.Items.Add(objectManager.MakeObjectData());
                else
                    mobData.Items.Add(null);
            }
        }
        return mobData;
    }

    public void ApplyMobData(MobData mobData)
    {
        ApplyObjectData(mobData);

        // Itemを装備させる
        ItemCapacity = mobData.ItemCapacity;
        ((IItemOwner)this).ResetItems();
        for (int i = 0; i < mobData.ItemCapacity; i++)
        {
            var itemData = mobData.Items[i];
            var itemObj = ResourceManager.Instance.GetItem(itemData.ItemID);
            if (itemObj != null && itemObj.TryGetComponent(out ObjectManager itemMng))
            {
                itemMng.ApplyObjectData(itemData);
                AddItem(i, itemMng.GetComponent<Item>());
            }
        }

    }
    // public static void SpawnMob(MobData mob)
    // {
    //     var mobObj = ResourceManager.GetMob(mob.MobID);
    //     if (mobObj == null) { Debug.LogWarning("mob is null"); return; }
    //     if (mobObj.TryGetComponent(out MobManager mobManager))
    //     {
    //         mobManager.ApplyMobData(mob);
    //     }
    // }
}

