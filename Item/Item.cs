using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine.Animations;
using UnityEngine.iOS;
using MyGame;

[RequireComponent(typeof(ObjectManager))]
[RequireComponent(typeof(Rigidbody2D))]
// Itemの動作を統括するコンポーネント。ItemはItemに統合された。
public class Item : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPoolable
{
	public string ID { get; private set; }
	[SerializeField] protected ItemData itemData;
	[SerializeField] MobManager owner;
	public MobManager Owner // このアイテムの持ち主
	{
		get { return owner; }
		set
		{
			if (value != null)
				OnItemOwned();
			else
				OnItemDropped();
			// Ownerに現在とは異なる値が代入された場合，次のアイテム達にもそれを伝える。
			if (value != owner)
			{
				owner = value;
				foreach (Item item in NextItems)
					if (item != null)
						item.Owner = value;
			}
		}
	}

	[SerializeField] ItemSlot itemSlot;
	public ItemSlot ItemSlot
	{
		get => itemSlot;
		private set => itemSlot = value;
	}

	[SerializeField] Item prevItem;
	public Item PrevItem
	{
		get => prevItem;
		private set => prevItem = value;
	}

	[SerializeField] List<Item> nextItems;
	/// <summary>
	/// 次に使用するアイテム
	/// </summary>
	public List<Item> NextItems
	{
		get => nextItems;
		private set => nextItems = value;
	}

	[SerializeField] int itemCapacity;
	/// <summary>
	/// 中に入れられるアイテムの最大値
	/// </summary>
	public int ItemCapacity => itemCapacity;

	[SerializeField] float reloadTime;
	/// <summary>
	/// このアイテムのリロード時間。負の数もOK
	/// </summary>
	public float ReloadTime => reloadTime;

	[SerializeField] float coolDownTime;
	/// <summary>
	/// 何秒で打てるようになるか外部に示すプロパティ。毎フレーム更新されて常に正確な時間を表す。読み取り専用
	/// </summary>
	public float CoolDownTime
	{
		get => coolDownTime;
		private set
		{
			coolDownTime = Math.Max(value, 0);
		}
	}

	[SerializeField] LayerMask targetLayer;
	/// <summary>
	/// アイテム自身が持つターゲット
	/// </summary>
	public LayerMask TargetLayer => targetLayer;

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

	[SerializeField] LayerMask throwTarget;
	/// <summary>
	/// 投げられているときのターゲットとなるレイヤーマスク
	/// </summary>
	public LayerMask ThrowTarget
	{
		get => throwTarget;
		private set => throwTarget = value;
	}

	Rigidbody2D rb;

	Vector2 DragOffset;

	/// <summary>
	/// 元の重力を保持する
	/// </summary>
	[SerializeField] float gravity;

	/// <summary>
	/// 使用するアイテムスロットのPoolID。アイテムスロットの種類はここで変える。
	/// </summary>
	/// <returns></returns>
	protected string slotID;

	ObjectManager manager;

	private void Awake()
	{
		TryGetComponent(out rb);
		gravity = rb.gravityScale;
		TryGetComponent(out manager);
	}

	public virtual void OnGet(string id)
	{
		ID = id;
		Init();
	}
	protected virtual void Init()
	{
		if (itemData == null)
		{
			Debug.LogWarning("Data is null!!!");
			return;
		}

		itemCapacity = itemData.itemCapacity;
		reloadTime = itemData.reloadTime;
		targetLayer = itemData.targetLayer;
		mp = itemData.mp;
		damage = itemData.damage;
		diffusion = itemData.diffusion;
		speed = itemData.speed;
		duration = itemData.duration;
		recoil = itemData.recoil;
		additionalSize = itemData.additionalSize;
		additionalAmount = itemData.additionalAmount;
		slotID = itemData.slotID.ToString();
	}

	public void Release()
	{
		ResourceManager.ReleaseItem(this);
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
	/// Ownerがnullでなくなった時実行される
	/// </summary>
	protected virtual void OnItemOwned()
	{

	}

	/// <summary>
	/// Ownerがnullになったとき実行される
	/// </summary>
	protected virtual void OnItemDropped()
	{

	}

	/// <summary>
	/// 投げられてる判定を返す。速度がしきい値以下になるか，投げられてから一定時間経ったらIsThrownをfalseにする
	/// </summary>
	void CheckThrownState()
	{
		if (rb.velocity.magnitude <= stopThreshold || timeSinceThrown <= 0)
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
		if (CoolDownTime != 0 || manager.IsDead)
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
	protected void ProcessReloadAndMP(Shot shot)
	{
		if (shot.User == null)
			return;

		if (MP <= shot.User.CurrentMP)
		{
			shot.User.IncreaseCurrentMP(-MP);
			IsMPSufficient = true;
		}
		else
		{
			IsMPSufficient = false;
			return;
		}

		foreach (var item in NextItems)
		{
			item.ProcessReloadAndMP(shot);
		}

		if (IsMPSufficient)
			CoolDownTime = ReloadTime;
		else
			CoolDownTime = 0;
		foreach (var item in NextItems)
		{
			CoolDownTime += item.CoolDownTime;
		}
	}

	/** アイテムを使用するメソッド。
	1. ParameterModifier（単にパラメータを編集するだけで，Fire直後に処理を終了するもの）を使用する。
	2. NextItemsのAttackのみをshot.NextItemsに写す。
	3. アタックなら発射したオブジェクトのリストをshot.Projectilesに代入する。
	4. ProjectileModifier（放射物を後から編集し，Fire直後に処理を終了するもの）を使用する。（ProjectileModifierのFire内ではshot.NextItemsを編集してはいけない）。
	5. 任意のタイミングでNextItemsを使用する。**/
	/**
	・Itemの種類によって次のItemに引き継ぐshotの部分が異なる。注:パラメータ(Damage, Speed等)
	種類, 引き継がないといけないメンバ, 引き継いではいけないメンバ, どっちでもいいメンバ
	ParameterModifier, パラメータとProjectilesとTargetとNextItems, ,　
	ProjectileModifier, パラメータとProjectilesとTargetとNextItems, , 
	Attack, Target, パラメータとProjectiles, NextItems
	・Attackの場合，放射物を発射するときに渡したshotのNextItemsは，その直後実行されるProjectileModifier.Fireによって変えられてしまう。
	→ProjectileModifierはNextItemsを使用する必要がない。したがってProjectileModifierはNextItemsを使用してはならないことにする。同様に，基本必要なければ与えられたshot.NextItemsは変更してはならない。
	・Modifierの中にAttackが入っていた場合の処理を考える。
	→暫定的な採用案：ParameterModifierは同じParameterModifierしか入れられないようにし，ProjectileModifierは種類に応じてAttackなども入れられるようにする。
	**/
	public virtual void Fire(Shot shot)
	{
	}


	/// <summary>
	/// パラメータの編集しShot.NextItemsをリセットして次に実行するAttackItemを集める。
	/// </summary>
	/// <param name="shot"></param>
	protected void ModifyParams(Shot shot)
	{
		foreach (Item nextItem in NextItems)
		{
			if (nextItem is IParameterModifier parameterModifierItem)
			{
				parameterModifierItem.EditParameters(shot);
			}
		}
	}

	/// <summary>
	/// NextItemsをセットする。
	/// </summary>
	/// <param name="shot"></param>
	protected void SetNextItems(Shot shot)
	{
		shot.NextItems = NextItems.OfType<AttackItem>().Cast<Item>().ToList();
	}

	/// <summary>
	/// ProjectileModifierを実行。shot.Projectilesが編集される。
	/// </summary>
	/// <param name="shot"></param>
	protected void ModifyProjectiles(Shot shot)
	{
		// ProjectileModifierを実行
		foreach (Item nextItem in NextItems)
		{
			if (nextItem is IProjectileModifier projectileModifier)
			{
				projectileModifier.EditProjectile(shot);
			}
		}
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
			// Debug.Log($"shot:{shot} is null");
			return;
		}

		ThrowTarget = shot.TargetLayer;
		// 参照オブジェクトの位置と回転を取得
		if (shot.Projectiles != null && shot.Projectiles.Count > 0)
		{
			gameObject.transform.position = shot.Projectiles[0].transform.position;
			gameObject.transform.rotation = shot.Projectiles[0].transform.rotation;
		}

		ShowItemAndHideUI();

		IsThrown = true;
		timeSinceThrown = throwDuration;
		rb.AddForce(gameObject.transform.up.normalized * shot.Speed, ForceMode2D.Impulse);
	}

	/// <summary>
	/// 敵にアイテムがぶつかった際の処理。
	/// </summary>
	/// <param name="objectManager"></param>
	public void MobHit(ObjectManager objectManager)
	{
		if (IsThrown && ((ThrowTarget & (1 << objectManager.gameObject.layer)) != 0))
		{
			Damage collisionDamage = new Damage()
			{
				Physical = math.pow(rb.velocity.magnitude, 2) * rb.mass / 2
			};
			objectManager.TakeDamage(collisionDamage, null, rb.velocity);
		}
	}

	public bool CanBePickedUp()
	{
		return !manager.IsDead;
	}

	/// <summary>
	/// UIを作成してInventoryに格納する。
	/// </summary>
	/// <returns></returns>
	public void AddToInventory()
	{
		Transform emptySlotTransform = UIManager.Instance.Inventory?.GetEmptySlotTransform();
		// 空いてるスロットがあるかどうか
		if (emptySlotTransform != null)
		{
			HideItemAndShowUI();
			ItemSlot.transform.SetParent(emptySlotTransform);
		}
		else
		{
			Debug.Log("Inventory Slot is full");
		}
	}

	/// <summary>
	/// このアイテムをアクティブにしUIをリリースする。
	/// </summary>
	public void ShowItemAndHideUI()
	{
		EnableComponentsOnCollected(true);
		if (ItemSlot != null)
			ItemSlot.Release();
		ItemSlot = null;
	}


	/// <summary>
	/// このアイテムを非表示にしUIを生成する。
	/// </summary>
	public ItemSlot HideItemAndShowUI()
	{
		PrepareUI();
		EnableComponentsOnCollected(false);
		return ItemSlot;
	}

	/// <summary>
	/// アイテムを取得・投棄する際にコンポネントたちを非アクティブにする。SetActiveを使わないことで，Updateなどが呼び出されるようにする。
	/// </summary>
	/// <param name="isEnable"></param>
	void EnableComponentsOnCollected(bool isEnable)
	{
		// Debug.Log("this item was enabled");
		if (TryGetComponent(out SpriteRenderer sr))
			sr.enabled = isEnable;

		GetComponent<Rigidbody2D>().simulated = isEnable;
	}

	/// <summary>
	/// アイテムスロット（UI）を準備する。アイテムを非表示にはしない。
	/// </summary>
	public void PrepareUI()
	{
		ItemSlot = ResourceManager.GetOther(slotID).GetComponent<ItemSlot>();
		Sprite itemImage = gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
		ItemSlot.Init(this, itemImage);
		foreach (Item nextItem in NextItems)
		{
			nextItem.PrepareUI();
			nextItem.ItemSlot.Move(ItemSlot.transform, NextItems.Count - 1);
		}
	}

	/// <summary>
	/// アイテムを追加する。
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	public void AddNextItem(Item item)
	{
		if (CanAddItem(item))
		{
			// Debug.Log($"item: {gameObject.name}");
			// アイテムをNextItemsに登録する前に，追加するアイテムと元持ち主の関係を断ち切る。
			item.RemovePrevRelation();
			NextItems.Add(item);
			item.Owner = Owner;
			item.PrevItem = this;
		}
		else
		{
			Debug.Log("item is full");
		}
	}

	/// <summary>
	/// オーナーが変わるとき必要な処理をする。
	/// </summary>
	/// <param name="owner">拾ったmob</param>
	public void OnChangeOwner(MobManager owner)
	{
		RemovePrevItem();
		EnableComponentsOnCollected(false);
		Owner = owner;
	}

	/// <summary>
	/// これまで持っていた持ち主アイテムや持ち主（Owner）との関係を断ち切る。
	/// </summary>
	public void RemovePrevRelation()
	{
		RemovePrevItem();
		RemovePrevOwner();
	}

	/// <summary>
	/// PrevItem（このアイテムの持ち主アイテム）との縁を断ち切る。こちらから参照できなくするだけでなく，向こうからもこちらを参照できなくする。
	/// </summary>
	void RemovePrevItem()
	{
		// Debug.Log($"prevItem:{PrevItem != null}");
		PrevItem?.RemoveNextItem(this);
	}

	/// <summary>
	/// Ownerをリセットする。こちらから参照できなくするだけでなく，向こうからもこちらを参照できなくする。
	/// </summary>
	void RemovePrevOwner()
	{
		// Debug.Log($"prevOwner:{Owner}");
		if (Owner != null)
			Owner.RemoveItem(this);
	}

	void RemoveNextItem(Item item)
	{
		if (NextItems.Contains(item))
		{
			NextItems.Remove(item);
			item.PrevItem = null;
		}
		else
		{
			Debug.LogWarning("item is not in NextItems");
		}
	}

	// アイテムを追加できるか判定する。継承してアイテムの種類ごとに判定を変える。
	public bool CanAddItem(Item item)
	{
		if (item != null && NextItems.Count < ItemCapacity)
			return true;
		else
			return false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		// マウスとオブジェクトの距離を計算
		DragOffset = transform.position - Camera.main.ScreenToWorldPoint(eventData.position);
	}

	// // ドラッグ開始時に呼ばれる
	// public void OnBeginDrag(PointerEventData eventData)
	// {
	// 	GameManager.Instance.CurrentlyDraggedItem = gameObject;
	// 	transform.position = eventData.position;
	// 	BeginDrag();

	// 	eventData.Use();
	// }

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
		if (TryGetComponent(out rb))
		{
			gravity = rb.gravityScale;
			rb.gravityScale = 0;
		}
		// rb.velocity = Vector2.zero;
		// SetAtMousePos();
	}

	// public void BeginDrag()
	// {
	// 	IsUIDragging = false;
	// 	gravity = rb.gravityScale;
	// 	rb.gravityScale = 0;
	// 	rb.velocity = Vector2.zero;
	// 	SetAtMousePos();
	// 	gameObject.SetActive(true);
	// }

	/// <summary>
	/// 必要なくても，Dragを使うにはBegin, Drag, Endの3つを必ず継承しないといけないみたい。
	/// </summary>
	/// <param name="eventData"></param>
	public void OnDrag(PointerEventData eventData)
	{
	}

	// ドラッグ中に呼ばれる
	// public void OnDrag(PointerEventData eventData)
	// {
	// 	Debug.Log($"IsUIDragging: {IsUIDragging}");

	// 	if (IsUIDragging == false)
	// 	{
	// 		MoveItem();
	// 		if (eventData.pointerDrag != gameObject)
	// 			eventData.pointerDrag = gameObject;
	// 	}
	// 	else
	// 	{
	// 		ItemSlot.SetAtMousePos();
	// 		if (eventData.pointerDrag != ItemSlot.gameObject)
	// 			eventData.pointerDrag = ItemSlot.gameObject;
	// 	}

	// 	eventData.Use();
	// }

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
		Vector3 mousePos = GameManager.Utility.GetMousePos();
		rb.AddForce((1 + AdditionalAmount) * (mousePos - transform.position));
		// SetAtMousePos();
	}

	// ドラッグ終了時に呼ばれる。どんな状況でも呼ばれないといけない。
	public void OnEndDrag(PointerEventData eventData)
	{
		// DragSystem.EndDragを実行
		DragSystem.Instance.EndDrag();

		// Debug.Log("Item OnEndDrag");
		// GameManager.Instance.CurrentlyDraggedItem = null;

		// EndDrag();

		// // if (IsUIDragging == true)
		// if (ItemSlot != null)
		// 	ItemSlot.EndDrag();

		// GameManager.Hide2ActiveFalse(gameObject);
	}

	public void EndDrag()
	{
		// Debug.Log("EndDrag");
		// 必要に応じてドラッグ終了時の処理をここに追加
		rb.gravityScale = gravity;

		DragOffset = Vector2.zero;
	}
}
