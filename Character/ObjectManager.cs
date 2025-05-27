using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class ObjectManager : MonoBehaviour, IChunkHandler//, IStatusAffectable
{

    [SerializeField] ChunkManager bossChunkManager;
    public ChunkManager BossChunkManager { get => bossChunkManager; set => bossChunkManager = value; }


    [Inject] protected IResourceManager m_resourceManager;


    /// <summary>
    /// ステータスを初期値にする
    /// </summary>
    protected virtual void ResetToGeneratedStatus()
    {
    }



    public void OnChunkGenerate()
    {
    }

    public virtual void OnChunkDeactivate()
    {
        // Release();
    }

    public virtual void OnChunkActivate()
    {
    }

    public void OnChunkReset()
    {
        // Release();
    }

    public virtual ObjectData MakeObjectData()
    {
        return FillObjectData(new ObjectData());
    }

    /// <summary>
    /// MakeRestoreDataの補助メソッド。冗長性をなくすために作られた。
    /// </summary>
    /// <param name="objectData"></param>
    /// <returns></returns>
    protected virtual ObjectData FillObjectData(ObjectData objectData)
    {
        // objectData.StatusDataList.Clear();
        // foreach (var status in StatusList)
        // {
        //     objectData.StatusDataList.Add(status.MakeData());
        // }

        // BossChunkManagerを親にした時のローカル座標等を計算
        objectData.LocalPos = BossChunkManager.transform.InverseTransformPoint(transform.position);
        objectData.LocalRotate = Quaternion.Inverse(BossChunkManager.transform.rotation) * transform.rotation;
        Vector3 parentScale = BossChunkManager.transform.lossyScale;
        Vector3 targetScale = transform.lossyScale;
        Vector3 localScale = new(
            targetScale.x / parentScale.x,
            targetScale.y / parentScale.y,
            targetScale.z / parentScale.z
        );
        objectData.LocalScale = localScale;
        return objectData;
    }
    public void ApplyObjectData(ObjectData objectData)
    {
        // foreach (var data in objectData.StatusDataList)
        // {
        //     // ((IStatusAffectable)this).AddStatus(data);
        // }

        transform.SetLocalPositionAndRotation(objectData.LocalPos, objectData.LocalRotate);
        transform.localScale = objectData.LocalScale;
    }
    // public static void SpawnItem(ObjectData item)
    // {
    //     var itemObj = m_resourceManager.GetItem(item.ItemID);
    //     if (itemObj == null) { Debug.LogWarning("item is null"); return; }
    //     if (itemObj.TryGetComponent(out ObjectManager objectManager))
    //     {
    //         objectManager.ApplyObjectData(item);
    //     }
    // }

    // public virtual void OnStatusPowerBoost(StatusEffectDurationStrategy status)
    // {

    // }
}