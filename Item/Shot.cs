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
	// Core。これらは共通の情報。複数回発射されても変わらない。主にユーザー固有の情報。
	public GameObject user;
	public LayerMask userTargetLayer;
	/// <summary>
	/// 目標のグローバル座標。グローバルであることでuserの位置に依存しない。
	/// </summary>
	public Vector2 target;
	// ユーザー固有のダメージ。アイテム固有のダメージに足し算される。
	public Damage userDamage;
	// ユーザーのレベルに依存する値。ダメージに掛け算される。
	public float userDamageModifier;


	// Extras。これらはshotごとに変わる。主にアイテム固有の情報。
	public GameObject referenceObject;
	// ユーザーのターゲットレイヤーとアイテムのそれも加えたターゲットレイヤー
	public LayerMask targetLayer;
	// ユーザーのダメージとアイテムのダメージを加算したダメージ。
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

	public List<Action<Shot>> NextAttackMethods = new(); // 使用時、個別にshotを生成して渡したいためListにしている。
	/// <summary>
	/// 次のアイテムを発射する際実行する。☆実行する前に，shot.referenceObjectに発射したい位置と向きのオブジェクトを入れる。
	/// </summary>
	public Action<Shot> NextAttack;


	public Shot()
	{
	}

	/// <summary>
	/// 引数のshotのCoreな情報をコピーする
	/// </summary>
	/// <param name="shot"></param>
	/// <returns></returns>
	public Shot CopyCore(Shot shot)
	{
		this.user = shot.user;
		this.userTargetLayer = shot.userTargetLayer;
		this.target = shot.target;
		this.userDamage = shot.userDamage;
		this.userDamageModifier = shot.userDamageModifier;
		return this;
	}

	public override string ToString()
	{
		return $"Shot: user: {user}, referenceObj: {referenceObject}, target: {target}, baseTargetLayer: {userTargetLayer}, userDamageRate: {userDamage}, targetLayer: {targetLayer}, damage: {damage}, diffusion: {diffusion}, speed: {speed}, duration: {duration}, size: {size}, amount: {amount}, recoil: {recoil}";
	}
}
