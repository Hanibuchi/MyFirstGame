using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Attack : MonoBehaviour, ISerializableComponent
{
    [SerializeField] AttackData m_attackData;
    [JsonProperty][SerializeField] Damage m_baseDamage;
    public Damage BaseDamage
    {
        get => m_baseDamage;
        protected set { m_baseDamage = value; OnBaseDamageChanged?.Invoke(m_baseDamage); }
    }
    public event Action<Damage> OnBaseDamageChanged;

    [JsonProperty][SerializeField] Damage m_damage;
    public Damage Damage
    {
        get => m_damage;
        protected set { m_damage = value; OnDamageChanged?.Invoke(m_damage); }
    }
    public event Action<Damage> OnDamageChanged;

    [JsonProperty][SerializeField] float m_damageModifier;
    public float DamageModifier
    {
        get => m_damageModifier;
        protected set { m_damageModifier = value; OnDamageModifierChanged?.Invoke(m_damageModifier); }
    }
    public event Action<float> OnDamageModifierChanged;

    [JsonProperty][SerializeField] LayerMask m_baseTargetLayer;
    public LayerMask BaseTargetLayer
    {
        get => m_baseTargetLayer;
        protected set { m_baseTargetLayer = value; OnBaseTargetLayerChanged?.Invoke(m_baseTargetLayer); }
    }
    public event Action<LayerMask> OnBaseTargetLayerChanged;

    [JsonProperty][SerializeField] LayerMask m_targetLayer;
    public LayerMask TargetLayer
    {
        get => m_targetLayer;
        protected set { m_targetLayer = value; OnTargetLayerChanged?.Invoke(m_targetLayer); }
    }
    public event Action<LayerMask> OnTargetLayerChanged;

    LevelHandler m_levelHandler;
    ItemUser _itemUser;
    public void Initialize(AttackData attackData)
    {
        m_attackData = attackData;
        if (TryGetComponent(out m_levelHandler))
        {
            m_levelHandler.OnLevelChanged += OnLevelChanged;
        }

        _itemUser = GetComponent<ItemUser>();
    }
    public void ResetToBase()
    {
        Damage = BaseDamage;
        TargetLayer = BaseTargetLayer;
    }
    /// <summary>
    /// Damageを追加する。負の値を入れると引くことも出来る。
    /// </summary>
    /// <param name="damage"></param>
    public void AddDamage(Damage damage)
    {
        Damage = Damage.Add(damage);
    }

    public void OnLevelChanged(ulong level)
    {
        BaseDamage = m_attackData.baseDamage;
        DamageModifier = m_attackData.damageModifierGrowthCurve.Function(level);
        BaseTargetLayer = m_attackData.baseTargetLayer;
    }




    /// <summary>
    /// アイテムを持つ位置を調整
    /// </summary>
    protected Vector2 HandOffset;
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

        var item = _itemUser.ItemHolder.Items[_itemUser.SelectedSlotNumber].GetItem();
        if (item != null)
        {
            if (item is MonoBehaviour mono)
            {
                SetItemPosition(mono.gameObject, target - (Vector2)transform.position);

                Shot shot = new();
                shot.SetCore(gameObject, mono.gameObject, target, TargetLayer, Damage);
                // shot.SetExtra(CurrentTargetLayer, Damage.Zero, 0, 0, 0, 0, 0, 0);

                item.FirstFire(shot);
            }
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
    /// <summary>
    /// アイテムを投げる。アイテムは所有している前提（注意）。アイテムは所有していなくてもいい。targetはグローバル座標。
    /// </summary>
    public void ThrowItem(Vector2 target)
    {
        var itemHolder = _itemUser.ItemHolder.Items[_itemUser.SelectedSlotNumber];
        itemHolder.GetItem().Drop();
        ThrowItem((Item)itemHolder.GetItem(), target);
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

        Shot shot = new()
        {
            projectiles = new List<GameObject>() { item.gameObject },
            targetLayer = TargetLayer,
            target = target,
            speed = (target - (Vector2)transform.position).magnitude
        };
        // Debug.Log($"Projectiles: {shot.Projectiles}");
        // Debug.Log($"TargetLayer: {shot.TargetLayer}");
        // Debug.Log($"Target: {shot.Target}");
        // Debug.Log($"Speed: {shot.Speed}");
        Debug.Log($"shot:{shot} ");
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
}
