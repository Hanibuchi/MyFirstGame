using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class MobManager : ObjectManager
    {
        [SerializeField] LayerMask baseTargetLayer;
        public LayerMask BaseTargetLayer
        {
            get => baseTargetLayer;
            protected set
            {
                if (baseTargetLayer != value)
                {
                    baseTargetLayer = value;
                    OnBaseTargetLayerChanged?.Invoke(baseTargetLayer);
                }
            }
        }
        public event Action<LayerMask> OnBaseTargetLayerChanged;

        [SerializeField] LayerMask currentTargetLayer;
        public LayerMask CurrentTargetLayer
        {
            get => currentTargetLayer;
            protected set
            {
                if (currentTargetLayer != value)
                {
                    currentTargetLayer = value;
                    OnCurrentTargetLayerChanged?.Invoke(currentTargetLayer);
                }
            }
        }
        public event Action<LayerMask> OnCurrentTargetLayerChanged;

        [SerializeField] private float baseMaxMP;
        public float BaseMaxMP
        {
            get => baseMaxMP;
            protected set
            {
                if (baseMaxMP != value)
                {
                    baseMaxMP = math.max(value, 0);
                    OnBaseMaxMPChanged?.Invoke(baseMaxMP);
                }
            }
        }
        public event Action<float> OnBaseMaxMPChanged;

        [SerializeField] private float currentMaxMP;
        public float CurrentMaxMP
        {
            get => currentMaxMP;
            protected set
            {
                if (currentMaxMP != value)
                {
                    currentMaxMP = math.max(value, 0);
                    OnCurrentMaxMPChanged?.Invoke(currentMaxMP);
                }
            }
        }
        public event Action<float> OnCurrentMaxMPChanged;

        [SerializeField] private float currentMP;
        public float CurrentMP
        {
            get => currentMP;
            protected set
            {
                if (currentMP != value)
                {
                    currentMP = math.max(value, 0);
                    OnCurrentMPChanged?.Invoke(currentMP);
                }
            }
        }
        public event Action<float> OnCurrentMPChanged;

        [SerializeField] private float baseMPRegen;
        public float BaseMPRegen
        {
            get => baseMPRegen;
            protected set
            {
                if (baseMPRegen != value)
                {
                    baseMPRegen = value;
                    OnBaseMPRegenChanged?.Invoke(baseMPRegen);
                }
            }
        }
        public event Action<float> OnBaseMPRegenChanged;

        [SerializeField] private float currentMPRegen;
        public float CurrentMPRegen
        {
            get => currentMPRegen;
            protected set
            {
                if (currentMPRegen != value)
                {
                    currentMPRegen = value;
                    OnCurrentMPRegenChanged?.Invoke(currentMPRegen);
                }
            }
        }
        public event Action<float> OnCurrentMPRegenChanged;

        [SerializeField] private ulong baseLevel;
        public ulong BaseLevel
        {
            get => baseLevel;
            protected set
            {
                if (baseLevel != value)
                {
                    baseLevel = value;
                    OnBaseLevelChanged?.Invoke(baseLevel);
                }
            }
        }
        public event Action<ulong> OnBaseLevelChanged;

        [SerializeField] private ulong currentLevel;
        public ulong CurrentLevel
        {
            get => currentLevel;
            protected set
            {
                if (currentLevel != value)
                {
                    currentLevel = value;
                    OnCurrentLevelChanged?.Invoke(currentLevel);
                }
            }
        }
        public event Action<ulong> OnCurrentLevelChanged;

        [SerializeField] private float baseSpeed;
        public float BaseSpeed
        {
            get => baseSpeed;
            protected set
            {
                if (baseSpeed != value)
                {
                    baseSpeed = value;
                    OnBaseSpeedChanged?.Invoke(baseSpeed);
                }
            }
        }
        public event Action<float> OnBaseSpeedChanged;

        [SerializeField] private float currentSpeed;
        public float CurrentSpeed
        {
            get => currentSpeed;
            protected set
            {
                if (currentSpeed != value)
                {
                    currentSpeed = value;
                    OnCurrentSpeedChanged?.Invoke(currentSpeed);
                }
            }
        }
        public event Action<float> OnCurrentSpeedChanged;

        [SerializeField] private Damage baseDamage;
        public Damage BaseDamage
        {
            get => baseDamage;
            protected set
            {
                baseDamage = value;
                OnBaseDamageChanged?.Invoke(baseDamage);
            }
        }
        public event Action<Damage> OnBaseDamageChanged;

        [SerializeField] private Damage currentDamage;
        public Damage CurrentDamage
        {
            get => currentDamage;
            protected set
            {
                currentDamage = value;
                OnCurrentDamageChanged?.Invoke(currentDamage);
            }
        }
        public event Action<Damage> OnCurrentDamageChanged;
        public bool IsAttacking;
        /// <summary>
        /// 装備しているアイテム。MobやNPCの持つアイテムはListを使った配列のように扱われる。AddやRemoveを使ってはいけない。配列を使わないのは持てるアイテムの量を増やしたいから。
        /// </summary>
        [SerializeField] List<Item> items = new();
        /// <summary>
        /// 装備アイテム。読み取り専用。Listを使っているが，基本的にAddなどでサイズを変更してはならない。
        /// </summary>
        public List<Item> Items => items;

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
        /// 装備できるアイテムの最大値
        /// </summary>
        [SerializeField] int itemCapacity;
        public int ItemCapacity
        {
            get => itemCapacity;
            private set => itemCapacity = value;
        }

        /// <summary>
        /// アイテムを持つ位置を調整
        /// </summary>
        protected Vector2 HandOffset;
        /// <summary>
        /// ドロップと確率
        /// </summary>
        [SerializeField] private List<DropItem> dropItems;

        [SerializeField] protected float Experience;
        [SerializeField] protected float ExperienceToNextLevel;


        private void Update()
        {
            // // リロード時間を減らしていく。
            // if (totalReloadTime > 0)
            //     totalReloadTime -= Time.deltaTime;

            RegenerateMP();
        }
        void RegenerateMP()
        {
            // MPが最大値に達していない場合、回復を行う
            if (CurrentMP < CurrentMaxMP)
            {
                // 毎秒mpRecoveryRate分だけMPを回復
                IncreaseCurrentMP(CurrentMPRegen * Time.deltaTime);
            }
        }

        public override void OnGet(int id)
        {
            base.OnGet(id);
            ResetItemSlots();
        }

        protected override void ResetToGeneratedStatus()
        {
            if (Data is BaseMobData mobData)
            {
                BaseTargetLayer = mobData.BasetTargetLayer;
                BaseMaxMP = mobData.BaseMaxMP;
                BaseMPRegen = mobData.BaseMPRegen;
                BaseLevel = mobData.BaseLevel;
                BaseSpeed = mobData.BaseSpeed;
                BaseDamage = mobData.BaseDamage;
            }
            base.ResetToGeneratedStatus(); // ResetToBaseを後で実行するためにこれは上の処理の後に実行する。
        }

        // すべてのステータスをBaseに戻す
        protected override void ResetToBase()
        {
            base.ResetToBase();
            CurrentTargetLayer = BaseTargetLayer;
            CurrentMaxMP = BaseMaxMP;
            CurrentMP = BaseMaxMP;
            CurrentMPRegen = BaseMPRegen;
            CurrentSpeed = BaseSpeed;
            CurrentLevel = BaseLevel;
            CurrentDamage = BaseDamage;
        }

        /// <summary>
        /// Itemsをリセットし，ItemCapacity個のnullをつめる。
        /// </summary>
        void ResetItemSlots()
        {
            if (Items == null)
            {
                Debug.LogWarning("Items is null!!!");
                return;
            }

            Items.Clear();

            for (int i = 0; i < ItemCapacity; i++)
                Items.Add(null);
        }
        /// <summary>
        /// Itemsの枠を増やす。Items.Addを使用する唯一の手段にしたい。
        /// </summary>
        void AddItemSlot()
        {
            ItemCapacity++;
            Items.Add(null);
        }

        // 経験値を追加する
        public void AddExperience(float amount)
        {
            Experience += amount;
            while (Experience >= ExperienceToNextLevel)
            {
                Experience -= ExperienceToNextLevel;
                LevelUp();
            }
        }

        // レベルアップ
        private void LevelUp()
        {
            BaseLevel++;
            ExperienceToNextLevel = Mathf.RoundToInt(ExperienceToNextLevel * 1.25f); // 次のレベルまでの経験値を増やす
            BaseMaxHP *= 1.25f;
            BaseMaxMP *= 1.25f;
            BaseMPRegen *= 1.25f;

            ResetToBase();
            // ResetStatusはステータスをBaseに戻すため，ここでバフ等を再計算する必要がある。デバフはリセットしたままでいい。
            RecalculateBuffs();

            Debug.Log("レベルアップしました！現在のレベル: " + BaseLevel);
        }

        /// <summary>
        /// バフを再計算する
        /// </summary>
        private void RecalculateBuffs()
        {
        }


        /// <summary>
        /// MPを増やすメソッド。このようにメソッドで編集すると，後でアニメーションなどをつけやすくなる
        /// </summary>
        /// <param name="additionalMP"></param>
        public void IncreaseCurrentMP(float additionalMP)
        {
            CurrentMP = math.clamp(CurrentMP + additionalMP, 0, CurrentMaxMP);
            // Debug.Log($"AdditionalMP: {additionalMP}, currentmp: {currentmp}");
            if (additionalMP > 0)
            {
                // MPが増えたときの演出。
            }
            else if (additionalMP < 0)
            {
                // MPが減った時の演出。
            }
            else
            {
                // 何も起きなかった時の演出。
            }
        }

        public void SetSelectedSlotNumber(int num)
        {
            SelectedSlotNumber = num;
        }
        public void AddSelectedSlotNumber(int num)
        {
            SetSelectedSlotNumber(SelectedSlotNumber + num);
        }

        /// <summary>
        /// 攻撃する。itemNumberで何番目のアイテムを使用するか選ぶ。shotのNextProjectiles2はそのアイテム自身を設定する。ItemModifierを持って攻撃する場合を考えて何とかしないといけない
        /// </summary>
        /// <param name="target">目標の*グローバル座標*</param>
        /// <returns>次に攻撃できるまでの時間(s)を返す</returns>
        public void Fire(Vector2 target)
        {
            if (IsDead)
                return;

            if (target == null)
                Debug.LogWarning("target is null");

            if (Items[SelectedSlotNumber] != null)
            {
                SetItemPosition(Items[SelectedSlotNumber].gameObject, target - (Vector2)transform.position);
                Shot shot = new(this, Items[SelectedSlotNumber], Items[SelectedSlotNumber].gameObject, target);
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
            Vector2 direction = shot.Target - (Vector2)transform.position;
            // Debug.Log($"direction: {direction} = shot.Target: {shot.Target} - user.transform.position: {transform.position}");
            Vector3 recoilForce = (-1) * shot.Recoil * direction.normalized;
            GetComponent<Rigidbody2D>().AddForce(recoilForce, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Colliderと重なったアイテムを手持ちに追加。拾うのは一つだけ。
        /// </summary>
        public virtual void PickupItem()
        {
            List<Collider2D> colliders = DetectNearbyColliders(new ContactFilter2D().NoFilter());

            // 検出されたコライダーの中から，Itemコンポネントを持つものを一つ見つけて手持ちに追加する
            foreach (Collider2D collider in colliders)
            {
                // Itemクラスの子クラスを持つコンポーネントがあるか確認
                if (collider.TryGetComponent(out Item itemComponent))
                {
                    // Itemsリストに追加
                    AddItemToEmptySlot(itemComponent);
                    return;
                }
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

        /// <summary>
        /// 空いているスロットにアイテムを追加。
        /// </summary>
        /// <param name="item"></param>
        protected void AddItemToEmptySlot(Item item)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null)
                {
                    SetItem(i, item);
                    return;
                }
            }
            // ここにアイテムが追加できなかったときの演出を書く。
        }

        /// <summary>
        /// スロットを指定してアイテムを追加。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public virtual void SetItem(int index, Item item)
        {
            if (item == null || index < 0 || index >= Items.Count || Items[index] == null)
            {
                // ここにアイテムが追加できなかったときの演出を書く。
                return;
            }
            item.OnPickedUp(this);
            Items[index] = item;
        }

        /// <summary>
        /// アイテムを投げる。targetはグローバル座標。
        /// </summary>
        /// <param name="itemNumber"></param>
        /// <param name="target"></param>
        public void ThrowItem(Vector2 target)
        {
            ThrowItem(Items[SelectedSlotNumber], target);
        }
        public virtual void ThrowItem(Item item, Vector2 target)
        {
            int slotNum = Items.IndexOf(item);
            if ((slotNum != -1 || (GameManager.Instance.PlayerNPCManager == this)) && item != null)
            {
                if (target == null)
                    target = transform.position;

                Shot shot = new()
                {
                    Projectiles = new List<GameObject>() { item.gameObject },
                    TargetLayer = CurrentTargetLayer,
                    Target = target,
                    Speed = (target - (Vector2)transform.position).magnitude
                };
                // Debug.Log($"Projectiles: {shot.Projectiles}");
                // Debug.Log($"TargetLayer: {shot.TargetLayer}");
                // Debug.Log($"Target: {shot.Target}");
                // Debug.Log($"Speed: {shot.Speed}");
                // Debug.Log($"shot:{shot} ");
                SetItemPosition(item.gameObject, target - (Vector2)transform.position);
                item.ThrowItem(shot);

                RemoveItem(slotNum);
            }
            else
                Debug.LogWarning("items do not contain the item or item is null");
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

        /// <summary>
        /// Itemsからアイテムを取り除く。
        /// </summary>
        /// <param name="item"></param>
        public virtual void RemoveItem(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("item is null");
                return;
            }
            int index = Items.IndexOf(item);
            RemoveItem(index);
        }
        public virtual void RemoveItem(int slotNum)
        {
            if (0 <= slotNum && slotNum < Items.Count)
            {
                // Debug.Log($"index:{index}");
                Items[slotNum].Owner = null;
                Items[slotNum] = null;
            }
        }

        public override void Die()
        {
            Drop();
            base.Die();
        }

        /// <summary>
        /// アイテムドロップの処理
        /// </summary>
        protected void Drop()
        {
            foreach (DropItem dropItem in dropItems)
            {
                if (dropItem != null && GameManager.Randoms[GameManager.RandomNames.DropItem].Value() <= dropItem.DropRate)
                {
                    GameObject droppedItem = Instantiate(dropItem.Item, transform.position, transform.rotation);
                    if (droppedItem.TryGetComponent(out Rigidbody2D rb))
                    {
                        rb.velocity = transform.GetComponent<Rigidbody2D>().velocity;
                    }
                }
            }
        }

        protected override void Release()
        {
            ResourceManager.Release((ResourceManager.MobID)ID, gameObject);
        }

        public MobData MakeMobData()
        {
            return FillMobData(new MobData());
        }

        protected MobData FillMobData(MobData mobData)
        {
            base.FillObjectData(mobData);
            mobData.MobID = (ResourceManager.MobID)ID;
            mobData.BaseTargetLayer = BaseTargetLayer;
            mobData.CurrentTargetLayer = CurrentTargetLayer;
            mobData.BaseMaxMP = BaseMaxMP;
            mobData.CurrentMaxMP = CurrentMaxMP;
            mobData.CurrentMP = CurrentMP;
            mobData.BaseMPRegen = BaseMPRegen;
            mobData.CurrentMPRegen = CurrentMPRegen;
            mobData.BaseLevel = BaseLevel;
            mobData.CurrentLevel = CurrentLevel;
            mobData.BaseSpeed = BaseSpeed;
            mobData.CurrentSpeed = CurrentSpeed;
            mobData.BaseDamage = BaseDamage;
            mobData.CurrentDamage = CurrentDamage;
            mobData.ItemCapacity = ItemCapacity;
            mobData.Experience = Experience;
            mobData.ExperienceToNextLevel = ExperienceToNextLevel;

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
            BaseTargetLayer = mobData.BaseTargetLayer;
            CurrentTargetLayer = mobData.CurrentTargetLayer;
            BaseMaxMP = mobData.BaseMaxMP;
            CurrentMaxMP = mobData.CurrentMaxMP;
            CurrentMP = mobData.CurrentMP;
            BaseMPRegen = mobData.BaseMPRegen;
            CurrentMPRegen = mobData.CurrentMPRegen;
            BaseLevel = mobData.BaseLevel;
            CurrentLevel = mobData.CurrentLevel;
            BaseSpeed = mobData.BaseSpeed;
            CurrentSpeed = mobData.CurrentSpeed;
            BaseDamage = mobData.BaseDamage;
            CurrentDamage = mobData.CurrentDamage;

            // Itemを装備させる
            ItemCapacity = mobData.ItemCapacity;
            ResetItemSlots();
            for (int i = 0; i < mobData.ItemCapacity; i++)
            {
                var itemData = mobData.Items[i];
                var itemObj = ResourceManager.Get(itemData.ItemID);
                if (itemObj != null && itemObj.TryGetComponent(out ObjectManager itemMng))
                {
                    itemMng.ApplyObjectData(itemData);
                    SetItem(i, itemMng.GetComponent<Item>());
                }
            }

            Experience = mobData.Experience;
            ExperienceToNextLevel = mobData.ExperienceToNextLevel;
        }
        public static void SpawnMob(MobData mob)
        {
            var mobObj = ResourceManager.Get(mob.MobID);
            if (mobObj == null) { Debug.LogWarning("mob is null"); return; }
            if (mobObj.TryGetComponent(out MobManager mobManager))
            {
                mobManager.ApplyMobData(mob);
            }
        }

        public override void OnStatusPowerBoost(Status status)
        {
            base.OnStatusPowerBoost(status);
            Damage damage = new()
            {
                Physical = 5,
            };
            CurrentDamage = CurrentDamage.Add(damage);
            status.RegisterExpireAction(OnStatusPowerBoostExpired);
            Debug.Log("status was registerd");
        }
        void OnStatusPowerBoostExpired()
        {
            Debug.Log("PowerBoost was expired");
            Damage damage = new()
            {
                Physical = -5,
            };
            CurrentDamage = CurrentDamage.Add(damage);
        }
    }
}
