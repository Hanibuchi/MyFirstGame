using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(ObjectManager))]
[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IItem, IPartyRegistrationHandler, IMemberRegistrationHandler, IChildItemUIRefresher, IItemTypeProvider
{
	[SerializeField] protected ItemData m_itemData;
	ItemSlot m_itemSlotUI;

	[SerializeReference] IChildItemHolder _childItemHolder = new ChildItemHolder();
	public virtual IChildItemHolder ItemHolder => _childItemHolder;



	[SerializeField] float m_reloadTime;
	/// <summary>
	/// このアイテムのリロード時間。負の数もOK
	/// </summary>
	public float ReloadTime => m_reloadTime;

	[SerializeField] float m_coolDownTime;
	/// <summary>
	/// 何秒で打てるようになるか外部に示すプロパティ。毎フレーム更新されて常に正確な時間を表す。読み取り専用
	/// </summary>
	public float CoolDownTime { get => m_coolDownTime; private set { m_coolDownTime = Math.Max(value, 0); } }

	[SerializeField] LayerMask m_targetLayer;
	/// <summary>
	/// アイテム自身が持つターゲット
	/// </summary>
	public LayerMask TargetLayer => m_targetLayer;

	/// <summary>
	/// MP。負の数もOK
	/// </summary>
	[SerializeField] float mp;
	public float MP => mp;

	[SerializeField] Damage damage; // Item自体の攻撃力。
	public Damage Damage => damage;

	[SerializeField] float diffusion;
	public float Diffusion => diffusion;

	[SerializeField] float speed;
	public float Speed => speed;

	[SerializeField] float duration;
	public float Duration => duration;

	[SerializeField] float recoil;
	public float Recoil => recoil;

	[SerializeField] float additionalSize;
	public float AdditionalSize => additionalSize;

	[SerializeField] float additionalAmount;
	public float AdditionalAmount => additionalAmount;

	[SerializeField] bool isMPSufficient;
	public bool IsMPSufficient
	{
		get => isMPSufficient;
		private set => isMPSufficient = value;
	} // MPが足りているか。FirstFireで設定する

	/// <summary>
	/// 投げられているかどうかを表す。trueのとき，敵にダメージを与える。
	/// </summary>
	[SerializeField] bool isThrown;
	public bool IsThrown
	{
		get => isThrown;
		private set => isThrown = value;
	}

	/// <summary>
	/// 投げられた判定を止める速度。この速度未満になるとisThrownがfalseになる
	/// </summary>
	[SerializeField] float stopThreshold = 0.5f;

	/// <summary>
	/// 投げた判定の持続時間。この時間が経過したらisThrownはfalseになる
	/// </summary>
	[SerializeField] float throwDuration = 0.5f;

	[SerializeField] float timeSinceThrown = 0;

	[SerializeField] LayerMask m_throwTarget;
	/// <summary>
	/// 投げられているときのターゲットとなるレイヤーマスク
	/// </summary>
	public LayerMask ThrowTarget
	{
		get => m_throwTarget;
		private set => m_throwTarget = value;
	}

	Rigidbody2D m_rb;

	Vector2 m_dragOffset;

	/// <summary>
	/// 元の重力を保持する
	/// </summary>
	[SerializeField] float m_gravity;

	/// <summary>
	/// 使用するアイテムスロットのPoolID。アイテムスロットの種類はここで変える。
	/// </summary>
	/// <returns></returns>
	protected string m_slotID;

	DeathHandler m_deathHandler;

	private void Awake()
	{
		TryGetComponent(out m_rb);
		m_gravity = m_rb.gravityScale;
		m_deathHandler = GetComponent<DeathHandler>();
		if (TryGetComponent(out PoolableResourceComponent poolableResourceComponent))
		{
			poolableResourceComponent.ReleaseCallback += Init;
		}
		Init();
	}

	protected virtual void Init()
	{
		if (m_itemData == null)
		{
			Debug.LogWarning("Data is null!!!");
			return;
		}

		m_reloadTime = m_itemData.reloadTime;
		m_targetLayer = m_itemData.targetLayer;
		mp = m_itemData.mp;
		damage = m_itemData.damage;
		diffusion = m_itemData.diffusion;
		speed = m_itemData.speed;
		duration = m_itemData.duration;
		recoil = m_itemData.recoil;
		additionalSize = m_itemData.size;
		additionalAmount = m_itemData.amount;
		m_slotID = m_itemData.slotID.ToString();

		_childItemHolder.SetItem(this);
		_childItemHolder.SetPartyRegistrationHandler(this);
		_childItemHolder.SetMemberRegistrationHandler(this);
		_childItemHolder.SetChildItemUIRefresher(this);
		_childItemHolder.SetItemTypeProvider(this);
		_childItemHolder.SetItemCapacityData(m_itemData.itemCapacityData);
		_childItemHolder.ResetItems();
	}

	/// <summary>
	/// NextItemsを走査して必要なメソッドをShotに登録する。
	/// </summary>
	/// <param name="shot"></param>
	public void RegisterNextItemsToShot(Shot shot)
	{
		foreach (ChildItemHolder itemHolder in ItemHolder.Items)
		{
			IItem nextItem = itemHolder.GetItem();
			if (nextItem is IParameterModifierItem parameterModifierItem)
			{
				shot.EditParameters += parameterModifierItem.EditParameters;
			}
			if (nextItem is IProjectileModifierItem projectileModifierItem)
			{
				shot.EditProjectiles += projectileModifierItem.EditProjectiles;
			}
			if (nextItem is IAttackItem attackItem)
			{
				shot.NextAttackMethods.Add(attackItem.Attack);
			}
		}
	}
	/// <summary>
	/// このアイテム固有のExtrasをセットする。
	/// </summary>
	/// <param name="shot"></param>
	public void SetBaseExtras(Shot shot)
	{
		shot.targetLayer = shot.userTargetLayer | TargetLayer;
		shot.damage = shot.userDamage.Add(Damage);
		shot.diffusion = Diffusion;
		shot.speed = Speed;
		shot.duration = Duration;
		shot.size = AdditionalSize;
		shot.amount = AdditionalAmount;
		shot.recoil = Recoil;
	}

	void Update()
	{
		if (IsThrown)
			CheckThrownState();
		if (timeSinceThrown > 0)
			timeSinceThrown -= Time.deltaTime;
		if (CoolDownTime > 0)
			CoolDownTime -= Time.deltaTime;
	}


	/// <summary>
	/// 投げられてる判定を返す。速度がしきい値以下になるか，投げられてから一定時間経ったらIsThrownをfalseにする
	/// </summary>
	void CheckThrownState()
	{
		if (m_rb.velocity.magnitude <= stopThreshold || timeSinceThrown <= 0)
		{
			IsThrown = false;
		}
	}

	/// <summary>
	/// 最初に実行するFire。
	/// </summary>
	/// <param name="shot"></param>
	public virtual void FirstFire(Shot shot)
	{
		if (CoolDownTime != 0 || m_deathHandler.IsDead)
			return;

		ProcessReloadAndMP(shot); // MPとリロードの設定

		if (!IsMPSufficient)
			return;

		Fire(shot);
	}

	// public void test()
	// {
	// 	Shot shot = new Shot(){
	// 		User = GameManager.Instance.PlayerManager
	// 	};
	// 	ProcessReloadAndMP(shot);
	// }
	/// <summary>
	/// MPとリロード時間の初期設定をする。アイテムを走査してMPが足りるアイテムのIsMPSufficientをtrue，そうでないものをfalseにする。trueのもののリロード時間の総和を計算してCoolDownTimeに入れる。深さ優先探索。mpは行きがけ，reloadは帰りがけ。
	/// </summary>
	/// <param name="shot"></param>
	public void ProcessReloadAndMP(Shot shot)
	{

		if (shot.user != null && shot.user.TryGetComponent(out Mana mana) && MP <= mana.MP)
		{
			mana.ChangeMP(-MP);
			IsMPSufficient = true;
		}
		else
		{
			IsMPSufficient = false;
			return;
		}

		foreach (var itemHolder in ItemHolder.Items)
		{
			var item = itemHolder.GetItem();
			item?.ProcessReloadAndMP(shot);
		}

		if (IsMPSufficient)
			CoolDownTime = ReloadTime;
		else
			CoolDownTime = 0;
		foreach (var itemHolder in ItemHolder.Items)
		{
			var item = itemHolder.GetItem();
			if (item != null)
				CoolDownTime += item.CoolDownTime;
		}
	}

	// /** アイテムを使用するメソッド。
	// 1. ParameterModifier（単にパラメータを編集するだけで，Fire直後に処理を終了するもの）を使用する。
	// 2. NextItemsのAttackのみをshot.NextItemsに写す。
	// 3. アタックなら発射したオブジェクトのリストをshot.Projectilesに代入する。
	// 4. ProjectileModifier（放射物を後から編集し，Fire直後に処理を終了するもの）を使用する。（ProjectileModifierのFire内ではshot.NextItemsを編集してはいけない）。
	// 5. 任意のタイミングでNextItemsを使用する。**/
	// /**
	// ・Itemの種類によって次のItemに引き継ぐshotの部分が異なる。注:パラメータ(Damage, Speed等)
	// 種類, 引き継がないといけないメンバ, 引き継いではいけないメンバ, どっちでもいいメンバ
	// ParameterModifier, パラメータとProjectilesとTargetとNextItems, ,　
	// ProjectileModifier, パラメータとProjectilesとTargetとNextItems, , 
	// Attack, Target, パラメータとProjectiles, NextItems
	// ・Attackの場合，放射物を発射するときに渡したshotのNextItemsは，その直後実行されるProjectileModifier.Fireによって変えられてしまう。
	// →ProjectileModifierはNextItemsを使用する必要がない。したがってProjectileModifierはNextItemsを使用してはならないことにする。同様に，基本必要なければ与えられたshot.NextItemsは変更してはならない。
	// ・Modifierの中にAttackが入っていた場合の処理を考える。
	// →暫定的な採用案：ParameterModifierは同じParameterModifierしか入れられないようにし，ProjectileModifierは種類に応じてAttackなども入れられるようにする。
	// **/

	/// <summary>
	/// このアイテムが使用されたときに呼び出されるメソッド。AttackItemならAttack，それ以外はそのうち決める（投げるとか面白いかも）。
	/// </summary>
	/// <param name="shot"></param>
	public virtual void Fire(Shot shot)
	{
	}

	/// <summary>
	/// このアイテムをreferenceObjectの向きと方向へ投げる。shotはProjectiles, TargetLayer, Speedしか使わない。
	/// </summary>
	/// <param name="shot">Projectiles, TargetLayer, Speedしか使わない。</param>
	// アイテムの投射について。このメソッドでisThrownをtrueに，throwTargetをshot.TargetLayerにする。OncollisionEnter2Dで当たったオブジェクトにダメージを与える。isThrownがtrueのときUpdateで速度を観察し，大きさが一定値以下ならisThrownをfalseにする。
	public void ThrowItem(Shot shot)
	{
		if (shot == null)
		{
			Debug.Log($"shot:{shot} is null");
			return;
		}

		ThrowTarget = shot.targetLayer;
		// 参照オブジェクトの位置と回転を取得
		if (shot.projectiles != null && shot.projectiles.Count > 0)
		{
			gameObject.transform.position = shot.projectiles[0].transform.position;
			gameObject.transform.rotation = shot.projectiles[0].transform.rotation;
		}

		IsThrown = true;
		timeSinceThrown = throwDuration;
		m_rb.AddForce(gameObject.transform.up.normalized * shot.speed, ForceMode2D.Impulse);
	}

	/// <summary>
	/// 敵にアイテムがぶつかった際の処理。
	/// </summary>
	/// <param name="objectManager"></param>
	public void MobHit(Health health)
	{
		if (IsThrown && ((ThrowTarget & (1 << health.gameObject.layer)) != 0))
		{
			Damage collisionDamage = new()
			{
				physical = math.pow(m_rb.velocity.magnitude, 2) * m_rb.mass / 2
			};
			health.TakeDamage(collisionDamage, null, m_rb.velocity);
		}
	}

	/// <summary>
	/// Pickupされることができるかどうか。Itemなど壊れてる時などにfalseを返す。
	/// </summary>
	/// <returns></returns>
	public bool CanBePickedUp()
	{
		return !m_deathHandler.IsDead;
	}



	public void OnRegistered(IPartyModifier partyModifier)
	{
		// ここにパーティに登録されたときの処理を書く。
	}
	public void OnUnregistered(IPartyModifier partyModifier)
	{
	}
	public void OnRegistered(IMemberModifier memberModifier)
	{
		// ここにメンバーに登録されたときの処理を書く。
	}
	public void OnUnregistered(IMemberModifier memberModifier)
	{
	}


	// ここからnew

	// アイテムを追加できるか判定する。継承してアイテムの種類ごとに判定を変える。

	/// <summary>
	/// 直下のItemsのUIをリフレッシュする。もし直下のItemのItemSlotがnullの場合のみそのItemのUIをrefreshする。
	/// </summary>
	public void RefreshUI()
	{
		InitItemSlotUI();
		m_itemSlotUI.UpdateItemSlotUI(ItemHolder.Items, ItemHolder.IsBag);
	}
	/// <summary>
	/// m_itemSlotUIを返す。nullのときもそのまま返す。
	/// </summary>
	/// <returns></returns>
	public ItemSlot GetItemSlotUI()
	{
		return m_itemSlotUI;
	}
	/// <summary>
	/// m_itemSlotUIを生成して取得する。すでにあるなら無視される。
	/// </summary>
	/// <returns></returns>
	protected virtual void InitItemSlotUI()
	{
		if (m_itemSlotUI == null)
		{
			m_itemSlotUI = ResourceManager.Instance.GetOther(m_slotID).GetComponent<ItemSlot>();
			m_itemSlotUI.Init(this, gameObject.GetComponentInChildren<SpriteRenderer>().sprite);
			m_itemSlotUI.SetItem(this);
		}
	}

	/// <summary>
	/// ItemSlotUIがリリースされるときに呼び出される。
	/// </summary>
	public void OnReleaseItemSlotUI()
	{
		m_itemSlotUI = null;
	}
	// ここまでnew





	/// <summary>
	/// アイテムを取得・投棄する際にコンポネントたちを非アクティブにする。SetActiveを使わないことで，Updateなどが呼び出されるようにする。
	/// </summary>
	/// <param name="isEnable"></param>
	public void EnableComponentsOnCollected(bool isEnable)
	{
		// Debug.Log("this item was enabled");
		if (TryGetComponent(out SpriteRenderer sr))
			sr.enabled = isEnable;

		GetComponent<Rigidbody2D>().simulated = isEnable;
	}




	public void Drop()
	{
		ItemHolder.ClearPrevRelation();
		EnableComponentsOnCollected(true);
		if (m_itemSlotUI != null && m_itemSlotUI.TryGetComponent(out PoolableResourceComponent poolable))
		{
			poolable.Release();
		}
	}





	public void OnPointerDown(PointerEventData eventData)
	{
		// マウスとオブジェクトの距離を計算
		m_dragOffset = transform.position - GameManager.Utility.GetMousePos();
	}

	/// <summary>
	/// ドラッグ開始時に呼ばれる
	/// </summary>
	/// <param name="eventData"></param>
	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log("on begin drag");
		DragSystem.Instance.ItemBeginDrag(this);

		eventData.Use();
	}

	public void BeginDrag()
	{
		m_gravity = m_rb.gravityScale;
		m_rb.gravityScale = 0;
		m_rb.velocity = Vector2.zero;
	}

	/// <summary>
	/// 必要なくても，Dragを使うにはBegin, Drag, Endの3つを必ず継承しないといけないみたい。
	/// </summary>
	/// <param name="eventData"></param>
	public void OnDrag(PointerEventData eventData)
	{
	}

	/// <summary>
	/// Itemの位置をマウスの位置にする。
	/// </summary>
	public void SetAtMousePos()
	{
		// Debug.Log($"pointer Current pos: {Pointer.current.position.ReadValue()}");
		Vector3 mousePos = GameManager.Utility.GetMousePos();
		transform.position = mousePos;
	}

	/// <summary>
	/// Itemをマウスで動かすときの処理。これをrb.MovePositionを使った方法にするとプレイヤーをひっかけてマウスで動かせるようになる。
	/// </summary>
	public virtual void MoveItem()
	{
		Vector2 targetPoint = (Vector2)GameManager.Utility.GetMousePos() + m_dragOffset;
		m_rb.AddForce((1 + AdditionalAmount) * (targetPoint - (Vector2)transform.position));
		// SetAtMousePos();
	}

	// ドラッグ終了時に呼ばれる。どんな状況でも呼ばれないといけない。
	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("Item OnEndDrag");
		DragSystem.Instance.EndDrag();
	}

	public void EndDrag()
	{
		Debug.Log("EndDrag");
		// 必要に応じてドラッグ終了時の処理をここに追加
		m_rb.gravityScale = m_gravity;
		if (ItemHolder.ParentItemHolder != null)
			ItemHolder.ParentItemHolder.GetItem().RefreshUI();
	}

	/// <summary>
	/// このアイテムを追加するのに失敗したとき呼び出されるメソッド。親がいない場合は投げる。
	/// </summary>
	/// <returns></returns>
	public void OnAddItemFailed()
	{
		ItemHolder.ClearPrevRelation();
		var attack = GameManager.Instance.Player?.GetComponent<Attack>();
		attack?.ThrowItem(this);
	}

	public ItemType GetItemType()
	{
		ItemType type = ItemType.None;

		if (this is IAttackItem)
			type |= ItemType.Attack;
		if (this is IParameterModifierItem)
			type |= ItemType.ParameterModifier;
		if (this is IProjectileModifierItem)
			type |= ItemType.ProjectileModifier;

		return type;
	}
}
