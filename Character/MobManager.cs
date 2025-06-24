using System;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using Unity.Mathematics;
using UnityEngine;

public class MobManager : ObjectManager
{



    protected override void ResetToGeneratedStatus()
    {// ResetToBaseを後で実行するためにこれは上の処理の後に実行する。

    }




    public void ApplyRecoil(Shot shot)
    {
        Vector2 direction = shot.target - (Vector2)transform.position;
        // Debug.Log($"direction: {direction} = shot.Target: {shot.Target} - user.transform.position: {transform.position}");
        Vector3 recoilForce = (-1) * shot.recoil * direction.normalized;
        GetComponent<Rigidbody2D>().AddForce(recoilForce, ForceMode2D.Impulse);
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
    }
    public virtual void RefreshUI()
    {

    }
    // ここまでアイテム管理関連の改良版









    public MobData MakeMobData()
    {
        return FillMobData(new MobData());
    }

    protected MobData FillMobData(MobData mobData)
    {
        base.FillObjectData(mobData);

        // foreach (var item in Items)
        // {
        //     if (item == null)
        //         mobData.Items.Add(null);
        //     else
        //     {
        //         if (item.TryGetComponent(out ObjectManager objectManager))
        //             mobData.Items.Add(objectManager.MakeObjectData());
        //         else
        //             mobData.Items.Add(null);
        //     }
        // }
        return mobData;
    }

    public void ApplyMobData(MobData mobData)
    {
        ApplyObjectData(mobData);

        // Itemを装備させる
        for (int i = 0; i < mobData.ItemCapacity; i++)
        {
            // var itemData = mobData.Items[i];
            // var itemObj = ResourceManager.Instance.GetItem(itemData.ItemID);
            // if (itemObj != null && itemObj.TryGetComponent(out ObjectManager itemMng))
            // {
            //     itemMng.ApplyObjectData(itemData);
            //     AddItem(i, itemMng.GetComponent<Item>());
            // }
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

