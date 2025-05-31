using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

[CreateAssetMenu]
public class BaseTile : RuleTile, IResourceComponent
{
    public ResourceType Type { get; private set; }
    public string ID { get; private set; }
    string TileObjID = ResourceManager.TileObjID.DefaultTileObj.ToString();

    public void OnGet(ResourceType type, string id)
    {
        Type = type;
        ID = id;
    }

    /// <summary>
    /// 登録されているGameObject(GroundManager)生成時に自分のpositionを記憶させ，壊れるときにそのpositionのtileを置き換える。
    /// </summary>
    /// <param name="position"></param>
    /// <param name="tilemap"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject obj)
    {
        // Debug.Log($"Application.isPlaying: {Application.isPlaying}, GetPool() != null{GetPool()}, StartUP was called");
        if (Application.isPlaying)
        {
            GameObject tile = ResourceManager.Instance.GetOther(TileObjID);
            if (tile != null && tile.TryGetComponent(out TileObjManager GM))
                GM.Init(position);
        }

        return base.StartUp(position, tilemap, obj);
    }
}
