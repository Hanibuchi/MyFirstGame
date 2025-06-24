using System;
using System.Collections;

using MyGame;
using UnityEngine;

public class TestAttackItem : Item, IAttackItem
{
	[SerializeField] protected string projectileID;



	protected override void Init()
	{
		base.Init();
		if (m_itemData is AttackItemData attackItemData)
			projectileID = attackItemData.projectileID;
		else
			Debug.LogWarning("Data should be AttackItemData!!!");
	}

	public override void Fire(Shot shot)
	{
		((IAttackItem)this).Attack(shot);
	}

	public bool CanAttack(Shot shot)
	{
		return IsMPSufficient;
	}


	/// <summary>
	/// 次のアイテムを発射するとき実行されるメソッド。
	/// </summary>
	/// <param name="shot"></param>
	public void NextAttack(Shot shot)
	{
		foreach (var attack in shot.NextAttackMethods)
		{
			attack(CreateNextShot(shot));
		}
	}

	public Shot CreateNextShot(Shot shot)
	{
		var newShot = new Shot();
		return newShot.CopyCore(shot);
	}

	/// <summary>
	/// Amountを適用。Projectileの量を増やす。この中で必ずGenerateAndInitProjectileを実行しなければならない。
	/// </summary>
	/// <param name="referenceObject"></param>
	/// <param name="shot"></param>
	public void ApplyAmount(Shot shot, Action<Shot> nextMethod)
	{
		float amount = shot.amount;
		do
		{
			nextMethod.Invoke(shot);
			amount -= 1f;
		} while (amount >= 0);
	}


	/// <summary>
	/// 放射物を生成し，初期化。
	/// </summary>
	/// <param name="referenceObj"></param>
	/// <param name="shot"></param>
	public void GenerateAndInitProjectile(Shot shot)
	{
		foreach (var generateProjectile in shot.generateProjectileMethods)
		{
			var projectile = generateProjectile.Invoke(shot);
			shot.projectiles.Add(projectile);
		}
	}

	/// <summary>
	/// ProjectileをreferenceObjectの向きと方向へ投げる。
	/// </summary>
	/// <param name="shot"></param>
	/// <returns></returns>
	public virtual GameObject GenerateProjectile(Shot shot)
	{
		Debug.Log($"AttackItem, GenerateProjectile, shot: {shot}");

		// 参照オブジェクトの位置と回転を取得
		Transform referenceTransform = shot.referenceObject.transform;
		float aimingErrorAngle = Random.Randoms[RandomName.Diffusion.ToString()].NormalDistribution() * shot.diffusion;

		GameObject nextProjectileObj = ResourceManager.Instance.GetProjectile(projectileID);
		nextProjectileObj.transform.SetPositionAndRotation(referenceTransform.position, referenceTransform.rotation * Quaternion.Euler(0, 0, aimingErrorAngle));
		// Debug.Log("projectile thrown");
		if (nextProjectileObj.TryGetComponent(out Rigidbody2D rb))
		{
			rb.velocity = Vector2.zero;
			rb.AddForce(nextProjectileObj.transform.up * shot.speed, ForceMode2D.Impulse);
		}// 生成したオブジェクトを加速。

		Projectile projectile = nextProjectileObj.GetComponent<Projectile>();

		projectile.Launch(shot);

		return projectile.gameObject;
	}

	/// <summary>
	/// サイズを適用
	/// </summary>
	/// <param name="nextProjectile"></param>
	/// <param name="shot"></param>
	public void ApplySize(Shot shot)
	{
		if (shot.size == 0) // サイズを適用
		{
			Debug.LogWarning("shot.size may be wrong");
			return;
		}
		foreach (var projectile in shot.projectiles)
			projectile.transform.localScale *= shot.size;
	}

	/// <summary>
	/// 反動の実装。これだけ編集することができる。
	/// </summary>
	/// <param name="user"></param>
	/// <param name="shot"></param>
	public void ApplyRecoil(Shot shot)
	{
		shot.user?.GetComponent<KnockbackHandler>()?.Knockback(shot.recoil, (Vector2)transform.position - shot.target);
	}
}
