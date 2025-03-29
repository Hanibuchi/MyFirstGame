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

namespace MyGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ObjectManager : MonoBehaviour, IDamageable, IChunkHandler, IResourceHandler, IStatusAffectable
    {
        public BaseObjectData Data;
        /// <summary>
        /// このオブジェクトのPoolの識別子。PrefabManagerで設定される。←生成時に設定されるようにする可能性←やっぱりProjectileManagerで設定。
        /// </summary>
        public int ID { get; private set; }
        [SerializeField] bool isDead;
        public bool IsDead { get => isDead; private set => isDead = value; } // 死んだかどうか。

        // 基本ステータス
        [SerializeField] private float baseMaxHP;
        public float BaseMaxHP
        {
            get => baseMaxHP;
            protected set
            {
                if (baseMaxHP != value)
                {
                    baseMaxHP = math.max(value, 0);
                    OnBaseMaxHPChanged?.Invoke(baseMaxHP);
                }
            }
        }
        public event Action<float> OnBaseMaxHPChanged;

        [SerializeField] private float currentMaxHP;
        public float CurrentMaxHP
        {
            get => currentMaxHP;
            protected set
            {
                if (currentMaxHP != value)
                {
                    currentMaxHP = math.max(value, 0);
                    OnCurrentMaxHPChanged?.Invoke(currentMaxHP);
                }
            }
        }
        public event Action<float> OnCurrentMaxHPChanged;

        [SerializeField] private float currentHP;
        public float CurrentHP
        {
            get => currentHP;
            protected set
            {
                if (currentHP != value)
                {
                    currentHP = math.max(value, 0);
                    OnCurrentHPChanged?.Invoke(currentHP);
                }
            }
        }
        public event Action<float> OnCurrentHPChanged;

        [SerializeField] private Damage baseDamageRate; // 防御率
        public Damage BaseDamageRate
        {
            get => baseDamageRate;
            protected set
            {
                baseDamageRate = value;
                OnBaseDamageRateChanged?.Invoke(baseDamageRate);
            }
        }
        public event Action<Damage> OnBaseDamageRateChanged;

        [SerializeField] private Damage damageRate; // ダメージ率。大きいほどより多くのダメージを受ける
        public Damage DamageRate
        {
            get => damageRate;
            protected set
            {
                damageRate = value;
                OnDamageRateChanged?.Invoke(damageRate);
            }
        }
        public event Action<Damage> OnDamageRateChanged;

        [SerializeField] ChunkManager bossChunkManager;
        public ChunkManager BossChunkManager { get => bossChunkManager; set => bossChunkManager = value; }

        [SerializeField] List<Status> statusList = new();
        public List<Status> StatusList { get => statusList; }
        public Action<float> MoveAction { get; set; }


        /// <summary>
        /// IDをセット。ResourceManagerで生成時に実行される。
        /// </summary>
        /// <param name="id"></param>
        public virtual void OnGet(int id)
        {
            ResetToGeneratedStatus();
            ID = id;
        }

        /// <summary>
        /// ステータスを初期値にする
        /// </summary>
        protected virtual void ResetToGeneratedStatus()
        {
            if (Data == null)
            {
                Debug.LogWarning("Data is null!!!");
                return;
            }
            BaseMaxHP = Data.BaseMaxHP;
            BaseDamageRate = Data.BaseDamageRate;
            ResetToBase();
        }

        /// <summary>
        /// ステータスをBaseの値にする
        /// </summary>
        protected virtual void ResetToBase()
        {
            IsDead = false;
            CurrentMaxHP = BaseMaxHP;
            CurrentHP = BaseMaxHP;
            DamageRate = BaseDamageRate;
        }

        protected MobManager LastDamageTaker;


        /// <summary>
        /// ダメージ処理のトリガーはMob側で行う。こうすることで子オブジェクトにProjectileがぶつかった時もこのCollisionEnter2Dが呼び出されてくれる。
        /// </summary>
        /// <param name="collider"></param>
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
        /// アイテムとの衝突を検知。
        /// </summary>
        /// <param name="other"></param>
        protected void DetectItemCollision(GameObject other)
        {
            if (other.TryGetComponent(out Item item))
            {
                item.MobHit(this);
            }
        }

        public virtual void TakeDamage(Damage damage, MobManager user, Vector2 direction)
        {
            if (IsDead)
                return;

            LastDamageTaker = user;
            ((IDamageable)this).TakeDamageSub(damage, direction);
        }

        /// <summary>
        /// HPを増やすメソッド。このようにメソッドで編集すると，後でアニメーションなどをつけやすくなる
        /// </summary>
        /// <param name="additionalHP"></param>
        public void IncreaseCurrentHP(float additionalHP)
        {
            CurrentHP = math.clamp(CurrentHP + additionalHP, 0, CurrentMaxHP);
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
            if (TryGetComponent(out Rigidbody2D rb))
            {
                if (direction != null)
                    rb.AddForce(knockback * direction.normalized, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// CurrentHPを設定する
        /// </summary>
        /// <param name="hp"></param>
        public void SetCurrentHP(float hp)
        {
            CurrentHP = hp;
        }

        // 倒されたときに相手に与える経験値を計算する
        private float CalculateExperience()
        {
            return BaseMaxHP;
        }

        public virtual void Die()
        {
            if (IsDead)
                return;

            IsDead = true;

            if (LastDamageTaker != null && LastDamageTaker != this)
                LastDamageTaker.AddExperience(CalculateExperience());
            // Release();
        }

        protected virtual void Release()
        {
            ResourceManager.Release((ResourceManager.ItemID)ID, gameObject);
            List<Status> statuses = new(StatusList);
            foreach (var status in statuses)
            {
                status.Expire();
            }
        }

        public void OnChunkGenerate()
        {
        }

        public virtual void OnChunkDeactivate()
        {
            Release();
        }

        public virtual void OnChunkActivate()
        {
        }

        public void OnChunkReset()
        {
            Release();
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
            objectData.ItemID = (ResourceManager.ItemID)ID;
            objectData.BaseMaxHP = BaseMaxHP;
            objectData.CurrentMaxHP = CurrentMaxHP;
            objectData.CurrentHP = CurrentHP;
            objectData.BaseDamageRate = BaseDamageRate;
            objectData.CurrentDamageRate = DamageRate;
            objectData.StatusDataList.Clear();
            foreach (var status in StatusList)
            {
                objectData.StatusDataList.Add(status.MakeData());
            }

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
            BaseMaxHP = objectData.BaseMaxHP;
            CurrentMaxHP = objectData.CurrentMaxHP;
            CurrentHP = objectData.CurrentHP;
            BaseDamageRate = objectData.BaseDamageRate;
            DamageRate = objectData.CurrentDamageRate;
            foreach (var data in objectData.StatusDataList)
            {
                ((IStatusAffectable)this).AddStatus(data);
            }

            transform.SetLocalPositionAndRotation(objectData.LocalPos, objectData.LocalRotate);
            transform.localScale = objectData.LocalScale;
        }
        public static void SpawnItem(ObjectData item)
        {
            var itemObj = ResourceManager.Get(item.ItemID);
            if (itemObj == null) { Debug.LogWarning("item is null"); return; }
            if (itemObj.TryGetComponent(out ObjectManager objectManager))
            {
                objectManager.ApplyObjectData(item);
            }
        }

        public virtual void OnStatusPowerBoost(Status status)
        {

        }
    }


}
