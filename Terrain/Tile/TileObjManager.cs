using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.Mathematics;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.Tilemaps;

// このオブジェクトをDestroyするとRuleTileはそのまま残る。DestroyしてからRuleTile.SetTile(pos,null)するとエラー出る。RuleTileを消したいときは必ずこのオブジェクトより先に消さないといけない。
[RequireComponent(typeof(PoolableResourceComponent))]
public class TileObjManager : MonoBehaviour, IChunkHandler
{
    [SerializeField] ChunkManager bossChunkManager;
    /// <summary>
    /// このオブジェクトが所属するチャンク
    /// </summary>
    public ChunkManager BossChunkManager { get => bossChunkManager; set => bossChunkManager = value; }

    public Vector3Int Position { get; private set; } // tilemap上の位置を保持する変数

    protected MobManager lastDamageTaker;

    PoolableResourceComponent m_poolableResourceComponent;

    private void Awake()
    {
        if (!TryGetComponent(out m_poolableResourceComponent))
            Debug.LogWarning("m_poolableResourceComponent is null");
    }

    public void Init(Vector3Int pos)
    {
        Position = pos;
        transform.position = TerrainManager.Instance.TerrainTilemap.GetCellCenterWorld(pos);
    }

    public void OnChunkGenerate() { }

    public void OnChunkDeactivate()
    {
        // Debug.Log("GM wa Deactivated");
        BossChunkManager?.DeleteTile(this);
    }

    public void OnChunkActivate() { }
    public void OnChunkReset() { }

    /// <summary>
    /// ダメージ処理のトリガーはMob側で行う。こうすることで子オブジェクトにProjectileがぶつかった時もこのCollisionEnter2Dが呼び出されてくれる。
    /// </summary>
    /// <param name="collider"></param>
    // protected void OnCollisionEnter2D(Collision2D other)
    // {
    //     DetectCollision(other.gameObject);
    // }
    // protected void OnCollisionStay2D(Collision2D other)
    // {
    //     DetectCollision(other.gameObject);
    // }

    // /// <summary>
    // /// Projectileとの衝突
    // /// </summary>
    // /// <param name="other"></param>
    // protected virtual void DetectCollision(GameObject other)
    // {
    //     if (other.TryGetComponent(out Projectile projectile))
    //     {
    //         projectile.Hit(this);
    //     }
    // }


    // 倒されたときに相手に与える経験値を計算する
    private float CalculateExperience()
    {
        return 0;
    }

    public virtual void Die()
    {
        if (lastDamageTaker != null && lastDamageTaker.TryGetComponent(out LevelHandler levelHandelr)) levelHandelr.AddExperience(CalculateExperience());

        BossChunkManager?.DeleteTile(this);
    }
}
