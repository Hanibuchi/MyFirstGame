using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using MyGame;

[Serializable]
public class Shot //: UnityEngine.Object ←これ==nullが正しく使えないため継承しないほうがいい
{
	// Core
	public MobManager mobMan; // 廃止予定
	public GameObject user;
	public GameObject referenceObject;
	/// <summary>
	/// 目標のグローバル座標。グローバルであることでuserの位置に依存しない。
	/// </summary>
	public LayerMask baseTargetLayer;
	public Vector2 target;
	public Damage userDamageRate;


	// Extras
	public LayerMask targetLayer;
	public Damage damage;
	/// <summary>
	/// 拡散
	/// </summary>
	public float diffusion;
	public float speed;
	/// <summary>
	/// 持続時間
	/// </summary>
	public float duration;
	/// <summary>
	/// サイズ。負の数を許すことで逆に大きくできる
	/// </summary>
	public float size = 1;
	public float amount;
	/// <summary>
	/// 反動。打った方が受ける力。
	/// </summary>
	public float recoil;


	// Other
	/// <summary>
	/// 発射されたプロジェクタイル。オブジェクトを直接編集するため。次のItemの発射情報（座標・角度）を伝えるためにも使う。
	/// </summary>
	public List<GameObject> projectiles = new();
	public Action<Shot> EditParameters;
	public delegate GameObject GenerateProjectile(Shot shot);
	public List<GenerateProjectile> generateProjectileMethods = new();
	public Action<Shot> EditProjectiles;
	public Action<Shot> OnDestroyed;
	public Action<Shot> OnHit;
	public Action<Shot> OnTimeout;
	public List<Action<Shot>> NextAttackMethods = new();
	/// <summary>
	/// 次のアイテムを発射する際実行する。☆実行する前に，shot.referenceObjectに発射したい位置と向きのオブジェクトを入れる。
	/// </summary>
	public Action<Shot> NextAttack;


	public Shot()
	{
	}

	public Shot(Shot shot)
	{
		SetCore(shot.mobMan, shot.referenceObject, shot.target, shot.baseTargetLayer, shot.userDamageRate);
		SetExtras(shot.targetLayer, shot.damage, shot.diffusion, shot.speed, shot.duration, shot.size, shot.amount, shot.recoil);
	}

	/// <summary>
	/// 引数のshotのCoreな情報をコピーする
	/// </summary>
	/// <param name="shot"></param>
	/// <returns></returns>
	public Shot CopyCore(Shot shot)
	{
		return SetCore(shot.mobMan, shot.referenceObject, shot.target, shot.baseTargetLayer, shot.userDamageRate);
	}

	public Shot SetCore(MobManager user, GameObject referenceObject, Vector2 target, LayerMask baseTargetLayer, Damage userDamageRate)
	{
		this.mobMan = user;
		this.referenceObject = referenceObject;
		this.baseTargetLayer = baseTargetLayer;
		this.target = target;
		this.userDamageRate = userDamageRate;
		return this;
	}

	// Additionalな値をセット
	public Shot SetExtras(LayerMask targetLayer, Damage damage, float diffusion, float speed, float duration, float size, float amount, float recoil)
	{
		this.targetLayer = targetLayer;
		this.damage = damage;
		this.diffusion = diffusion;
		this.speed = speed;
		this.duration = duration;
		this.size = size;
		this.amount = amount;
		this.recoil = recoil;
		return this;
	}

	public override string ToString()
	{
		return $"Shot: user: {mobMan}, referenceObj: {referenceObject}, target: {target}, baseTargetLayer: {baseTargetLayer}, userDamageRate: {userDamageRate}, targetLayer: {targetLayer}, damage: {damage}, diffusion: {diffusion}, speed: {speed}, duration: {duration}, size: {size}, amount: {amount}, recoil: {recoil}";
	}
}
