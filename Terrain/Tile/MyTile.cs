using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

[CreateAssetMenu]
public class MyTile : RuleTile, IResourceComponent, IDamageable
{
    public ResourceType Type { get; private set; }
    public string ID { get => name; }
    string TileObjID = ResourceManager.TileObjID.DefaultTileObj.ToString();

    public void OnGet(ResourceType type, string id)
    {
        Type = type;
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
            // GameObject tile = ResourceManager.Instance.GetOther(TileObjID);
            // if (tile != null && tile.TryGetComponent(out TileObjManager tm))
            //     tm.Init(position);
        }

        return base.StartUp(position, tilemap, obj);
    }
    [SerializeField] float _hp = 10f;
    [SerializeField] Damage _damageRate = Damage.DefaultDamageRate;
    bool _isDead = false;

    public void TakeDamage(Damage damage, Attack user, Vector2 direction)
    {
        Debug.Log($"damage: {damage} @mytile");
        Damage calculatedDamage = damage.CalculateDamageWithDamageRates(_damageRate);
        if (calculatedDamage.totalDamageRate * calculatedDamage.GetTotalDamage() >= _hp)
            _isDead = true;
    }

    public bool IsDead()
    {
        Debug.Log($"IsDead: {_isDead} @mytile");
        bool isDead = _isDead;
        _isDead = false;
        return isDead;
    }
    public int GetLayer()
    {
        return LayerMask.NameToLayer("Ground");
    }
}
