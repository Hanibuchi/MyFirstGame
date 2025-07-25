using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Health : MonoBehaviour, IDamageable, ISerializableComponent
{
    [SerializeField] HealthData m_healthData;
    [JsonProperty][SerializeField] float m_baseMaxHP;
    public float BaseMaxHP
    {
        get => m_baseMaxHP;
        protected set { if (m_baseMaxHP != value) { m_baseMaxHP = math.max(value, 0); OnBaseMaxHPChanged?.Invoke(m_baseMaxHP); } }
    }
    public event Action<float> OnBaseMaxHPChanged;

    [JsonProperty][SerializeField] float m_maxHP;
    public float MaxHP
    {
        get => m_maxHP;
        protected set { if (m_maxHP != value) { m_maxHP = math.max(value, 0); OnMaxHPChanged?.Invoke(m_maxHP); } }
    }
    public event Action<float> OnMaxHPChanged;

    [JsonProperty][SerializeField] float m_hp;
    public float HP
    {
        get => m_hp;
        protected set { if (m_hp != value) { m_hp = math.max(value, 0); OnHPChanged?.Invoke(m_hp); } }
    }
    public event Action<float> OnHPChanged;

    [JsonProperty][SerializeField] Damage m_baseDamageRate;
    public Damage BaseDamageRate
    {
        get => m_baseDamageRate;
        protected set { m_baseDamageRate = value; OnBaseDamageRateChanged?.Invoke(m_baseDamageRate); }
    }
    public event Action<Damage> OnBaseDamageRateChanged;

    [JsonProperty][SerializeField] Damage m_damageRate;
    public Damage DamageRate
    {
        get => m_damageRate;
        protected set { m_damageRate = value; OnDamageRateChanged?.Invoke(m_damageRate); }
    }
    public event Action<Damage> OnDamageRateChanged;

    [JsonProperty][SerializeField] float m_damageRateModifier;
    public float DamageRateModifier
    {
        get => m_damageRateModifier;
        protected set { m_damageRateModifier = value; OnDamageRateModifierChanged?.Invoke(m_damageRateModifier); }
    }
    public event Action<float> OnDamageRateModifierChanged;


    // DeathHandler作ったらコメントアウト外す。他にもある
    DeathHandler m_deathHandler;
    KnockbackHandler m_knockbackHandler;
    LevelHandler m_levelHandler;

    /// <summary>
    /// healthDataの値をセットする。
    /// </summary>
    /// <param name="healthData"></param>
    public void Initialize(HealthData healthData)
    {
        m_healthData = healthData;
        m_deathHandler = GetComponent<DeathHandler>();
        m_knockbackHandler = GetComponent<KnockbackHandler>();
        if (TryGetComponent(out m_levelHandler))
        {
            m_levelHandler.OnLevelChanged += OnLevelChanged;
        }
    }
    /// <summary>
    /// HPとDamageRateをBaseの値にする。
    /// </summary>
    public void ResetToBase()
    {
        MaxHP = BaseMaxHP;
        DamageRate = BaseDamageRate;
    }
    /// <summary>
    /// HPをMaxHPの値にする。
    /// </summary>
    public void RestoreHP()
    {
        HP = MaxHP;
    }


    /// <summary>
    /// ダメージ処理のトリガーはHealth側で行う。こうすることで子オブジェクトにProjectileがぶつかった時もこのCollisionEnter2Dが呼び出されてくれる。
    /// </summary>
    protected void OnCollisionEnter2D(Collision2D other)
    {
        DetectCollision(other.gameObject);

        DetectItemCollision(other.gameObject);
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

    /// <summary>
    /// アイテムとの衝突
    /// </summary>
    /// <param name="other"></param>
    protected void DetectItemCollision(GameObject other)
    {
        if (other.TryGetComponent(out Item item))
        {
            item.MobHit(this);
        }
    }


    public virtual void TakeDamage(Damage damage, Attack user, Vector2 direction)
    {
        if (m_deathHandler != null)
        {
            if (m_deathHandler.IsDead)
                return;

            m_deathHandler.SetLastDamageTaker(user);
        }


        Damage calculatedDamage = damage.CalculateDamageWithDamageRates(DamageRate);
        float HP5per = HP * 0.05f;
        if (calculatedDamage.ice > HP5per)
        {
            // 凍り付く処理
        }
        if (calculatedDamage.fire > HP5per)
        {
            //燃える処理
        }
        if (calculatedDamage.water > HP5per)
        {
            // 水による処理
        }
        if (calculatedDamage.electric > HP5per)
        {
            // 電撃による処理
        }
        if (calculatedDamage.wind > HP5per)
        {
            // 風による処理
        }
        if (calculatedDamage.ice > HP5per)
        {
            // 氷による処理
        }
        if (calculatedDamage.poison > HP5per)
        {
            // 毒による処理
        }
        if (calculatedDamage.physical > HP5per)
        {
            // 物理ダメージによる処理
        }
        if (calculatedDamage.caterpillar > HP5per)
        {
            // 毛虫による処理
        }
        if (calculatedDamage.heal > HP5per)
        {
            // 回復に関する処理
        }

        ChangeHP(-calculatedDamage.totalDamageRate * calculatedDamage.GetTotalDamage());

        m_knockbackHandler?.Knockback(calculatedDamage.knockback, direction);

        if (Random.Randoms[RandomName.InstantDeath.ToString()].Value() < calculatedDamage.instantDeathRate)
        {
            m_deathHandler?.Die();
        }
    }
    public void ChangeHP(float additionalHP)
    {
        HP = math.clamp(HP + additionalHP, 0, MaxHP);
        if (HP <= 0)
        {
            m_deathHandler?.Die();
        }
    }


    public void OnLevelChanged(ulong level)
    {
        BaseMaxHP = m_healthData.baseMaxHPGrowthCurve.Function(level);
        BaseDamageRate = m_healthData.baseDamageRate.Multiple(m_healthData.damageRateModifierGrowthCurve.Function(level));
    }

    public int GetLayer()
    {
        return gameObject.layer;
    }
}
