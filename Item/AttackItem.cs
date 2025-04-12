using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using MyGame;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AttackItem : Item
{
	[SerializeField] string projectileID;

	// 音声ファイル
	public AudioClip LaunchSound;
	// 発射時の音声再生用AudioSource
	public AudioSource AudioSource;

	protected override void OnItemOwned()
	{
		base.OnItemOwned();
		// LoadProjectile();
	}

	protected override void OnItemDropped()
	{
		base.OnItemDropped();
		// ReleaseAsset();
	}

	protected override void Init()
	{
		base.Init();
		if (itemData is AttackItemData attackItemData)
			projectileID = attackItemData.projectileID;
		else
			Debug.LogWarning("Data should be AttackItemData!!!");
	}


	/** AttackItemを使用するメソッド。
shot.AdditionalDamage, shot.AdditionalDiffusion, shot.AdditionalSpeed, shot.AdditionalDurationを初期化
↓
NextItemsのAttackのみをshot.NextItemsに写す。
↓
ParameterModifier（単にパラメータを編集するだけで，Fire直後に処理を終了するもの）を使用する。
↓
発射したオブジェクトのリストをshot.Projectilesに代入する。
↓
ProjectileModifier（放射物を後から編集し，Fire直後に処理を終了するもの）を使用する。（ProjectileModifierのFire内ではshot.NextItemsを編集してはいけない）。
↓
任意のタイミングでNextItemsを使用する。**/
	public override void Fire(Shot _shot)
	{
		Shot shot = new(_shot); // 渡されたshotを編集してしまうのを防ぐ。

		if (!IsMPSufficient) // マナが足りなかったらリターン。
			return;

		MobManager user = shot.User;

		// Additionalな値をリセットし，武器自体の値を入れる。
		shot.SetAdditionalValues(Damage.Add(
			user != null ? user.CurrentDamage : new Damage()),
			Diffusion,
			Speed,
			Duration,
			AdditionalSize,
			AdditionalAmount,
			Recoil,
			user != null ? TargetLayer | user.CurrentTargetLayer : TargetLayer
			);

		ModifyParams(shot);

		SetNextItems(shot);

		// Recoil(反動)の実装
		ApplyRecoil(shot);

		// 前のProjectiles(prevProjectiles)の座標と方向に，当たり判定を持つ放射物を生成しshot.Projectilesに代入。
		var prevProjectiles = shot.Projectiles;
		shot.Projectiles = new List<GameObject>();
		foreach (GameObject referenceObject in prevProjectiles)
		{
			ApplyAmount(referenceObject, shot);
		}

		ModifyProjectiles(shot);
	}

	/// <summary>
	/// 反動の実装。これだけ編集することができる。
	/// </summary>
	/// <param name="user"></param>
	/// <param name="shot"></param>
	void ApplyRecoil(Shot shot)
	{
		shot.User?.ApplyRecoil(shot);
	}

	/// <summary>
	/// Amountを適用。Projectileの量を増やす。この中で必ずGenerateAndInitProjectileを実行しなければならない。
	/// </summary>
	/// <param name="referenceObject"></param>
	/// <param name="shot"></param>
	void ApplyAmount(GameObject referenceObject, Shot shot)
	{
		float amount = shot.Amount;
		do
		{
			GenerateAndInitProjectile(referenceObject, shot);
			amount -= 1;
		} while (amount >= 0);
	}

	/// <summary>
	/// 放射物を生成し，初期化。
	/// </summary>
	/// <param name="referenceObj"></param>
	/// <param name="shot"></param>
	void GenerateAndInitProjectile(GameObject referenceObj, Shot shot)
	{
		Projectile nextProjectile = ThrowProjectile(referenceObj, shot);
		ApplySize(nextProjectile.gameObject, shot);

		shot.Projectiles.Add(nextProjectile.gameObject); // ここでもとのshotを編集できるようにしなければならない。コピーとかしちゃダメ。
	}

	/// <summary>
	/// ProjectileをreferenceObjectの向きと方向へ投げる。
	/// </summary>
	/// <param name="referenceObject"></param>
	/// <param name="speed"></param>
	/// <param name="diffusion"></param>
	/// <returns></returns>
	Projectile ThrowProjectile(GameObject referenceObject, Shot shot)
	{
		// Debug.Log("throw Projectile was called");

		// 参照オブジェクトの位置と回転を取得
		Transform referenceTransform = referenceObject.transform;
		float aimingErrorAngle = GameManager.Randoms[GameManager.RandomNames.Diffusion].NormalDistribution() * shot.Diffusion;

		GameObject nextProjectileObj = ResourceManager.GetProjectile(projectileID);
		nextProjectileObj.transform.SetPositionAndRotation(referenceTransform.position, referenceTransform.rotation * Quaternion.Euler(0, 0, aimingErrorAngle));
		// Debug.Log("projectile thrown");
		if (nextProjectileObj.TryGetComponent(out Rigidbody2D rb))
		{
			rb.velocity = Vector2.zero;
			rb.AddForce(nextProjectileObj.transform.up * shot.Speed, ForceMode2D.Impulse);
		}// 生成したオブジェクトを加速。

		Projectile projectile = nextProjectileObj.GetComponent<Projectile>();

		projectile.Launch(new(shot));

		return projectile;
	}

	/// <summary>
	/// サイズを適用
	/// </summary>
	/// <param name="nextProjectile"></param>
	/// <param name="shot"></param>
	void ApplySize(GameObject nextProjectile, Shot shot)
	{
		if (shot.Size != 0) // サイズを適用
			nextProjectile.transform.localScale = new Vector3(shot.Size, shot.Size, shot.Size);
	}



	/// <summary>
	/// Projectileをロードする。発射の前に実行しなければならない。
	/// </summary>
	// public void LoadProjectile()
	// {
	// 	if (projectileReference == null)
	// 	{
	// 		Debug.LogWarning("ProjectileReference is null");
	// 		return;
	// 	}

	// 	projectileReference.LoadAssetAsync<GameObject>().Completed += OnProjectileLoaded;
	// }

	// public void ReleaseAsset()
	// {
	// 	if (loadedProjectile != null)
	// 	{
	// 		projectileReference.ReleaseAsset();
	// 		loadedProjectile = null;
	// 	}
	// 	ProjectilePool.Clear();
	// }

	// void OnProjectileLoaded(AsyncOperationHandle<GameObject> obj)
	// {
	// 	if (obj.Status == AsyncOperationStatus.Succeeded)
	// 	{
	// 		loadedProjectile = obj.Result;
	// 		ProjectilePool ??= new(loadedProjectile, 100, 200);
	// 	}
	// 	else
	// 		Debug.LogWarning("Failed to load Asset");
	// }

	// 発射地点で音を再生するメソッド
	public void PlayLaunchSoundAtPoint()
	{
		// 音声を再生する
		if (LaunchSound != null)
		{
			// 空のオブジェクトを生成して音声再生
			GameObject audioObject = new("LaunchSound");
			audioObject.transform.position = transform.position;

			AudioSource audioSource = audioObject.AddComponent<AudioSource>();
			// 現在のゲームオブジェクトにアタッチされているAudioSourceを取得する
			if (TryGetComponent<AudioSource>(out var originalAudioSource))
			{
				// コピー元のAudioSourceの設定をコピー先のAudioSourceにコピーする
				CopyAudioSourceSettings(originalAudioSource, audioSource);
			}

			audioSource.clip = LaunchSound;
			audioSource.playOnAwake = false;
			audioSource.Play();

			// 音声再生が終了したらオブジェクトを破棄する
			Destroy(audioObject, LaunchSound.length);
		}
	}

	// AudioSourceの設定をコピー。
	void CopyAudioSourceSettings(AudioSource original, AudioSource copy)
	{
		copy.clip = original.clip;
		copy.volume = original.volume;
		copy.pitch = original.pitch;
		copy.loop = original.loop;
		// 他の設定も必要に応じてコピーする
	}
}
