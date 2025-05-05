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
public class TileObjManager : MonoBehaviour, IDamageable, IChunkHandler
{
    [SerializeField] ChunkManager bossChunkManager;
    /// <summary>
    /// このオブジェクトが所属するチャンク
    /// </summary>
    public ChunkManager BossChunkManager { get => bossChunkManager; set => bossChunkManager = value; }

    public Vector3Int Position { get; private set; } // tilemap上の位置を保持する変数

    protected MobManager lastDamageTaker;

    [SerializeField] TileObjData Data;

    // 基本ステータス
    [SerializeField] float maxHP;
    public float MaxHP
    {
        get => maxHP;
        protected set
        {
            if (maxHP != value)
            {
                maxHP = Math.Max(value, 0);
            }
        }
    }

    [SerializeField] float currentHP;
    public float CurrentHP
    {
        get => currentHP;
        protected set
        {
            if (currentHP != value)
            {
                currentHP = Math.Max(value, 0);
            }
        }
    }

    [SerializeField] Damage damageRate; // 防御率
    public Damage DamageRate
    {
        get => damageRate;
        set
        {
            damageRate = value;
        }
    }

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

        if (Data == null)
        {
            Debug.LogWarning("Data is null!!!");
            return;
        }
        MaxHP = Data.MaxHP;
        DamageRate = Data.DamageRate;
        CurrentHP = MaxHP;
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
    protected void OnCollisionEnter2D(Collision2D other)
    {
        DetectCollision(other.gameObject);
    }
    protected void OnCollisionStay2D(Collision2D other)
    {
        DetectCollision(other.gameObject);
    }

    /// <summary>
    /// Projectileとの衝突
    /// </summary>
    /// <param name="other"></param>
    protected virtual void DetectCollision(GameObject other)
    {
        if (other.TryGetComponent(out Projectile projectile))
        {
            projectile.Hit(this);
        }
    }

    public virtual void TakeDamage(Damage damage, MobManager user, Vector2 direction)
    {
        if (DamageRate.destruction > damage.destruction)
        {
            return;
        }

        lastDamageTaker = user;
        ((IDamageable)this).TakeDamageSub(damage, direction);
    }

    /// <summary>
    /// HPを増やすメソッド。このようにメソッドで編集すると，後でアニメーションなどをつけやすくなる
    /// </summary>
    /// <param name="additionalHP"></param>
    public void IncreaseCurrentHP(float additionalHP)
    {
        CurrentHP = math.clamp(CurrentHP + additionalHP, 0, MaxHP);
        if (additionalHP > 0)
        {
            // HPが増えたときの演出。
        }
        else if (additionalHP < 0)
        {
            // HPが減った時の演出。
        }
        else
        {
            // 何も起きなかった時の演出。
        }
        if (CurrentHP <= 0)
            Die();
    }
    public void ApplyKnockback(float knockback, Vector2 direction)
    {
    }

    // 倒されたときに相手に与える経験値を計算する
    private float CalculateExperience()
    {
        return MaxHP;
    }

    public virtual void Die()
    {
        lastDamageTaker?.AddExperience(CalculateExperience());

        BossChunkManager?.DeleteTile(this);
    }
}
