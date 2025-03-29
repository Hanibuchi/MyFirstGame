using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

namespace MyGame
{
	[Serializable]
	public class Shot //: UnityEngine.Object ←これ==nullが正しく使えないため継承しないほうがいい
	{
		public MobManager User;
		public List<Item> NextItems; // 次に使用するアイテム
		/// <summary>
		/// 発射されたプロジェクタイル。オブジェクトを直接編集するため。次のItemの発射情報（座標・角度）を伝えるためにも使う。
		/// </summary>
		public List<GameObject> Projectiles;
		/// <summary>
		/// 目標のグローバル座標。グローバルであることでUserの位置に依存しない。
		/// </summary>
		public Vector2 Target;
		public LayerMask TargetLayer;
		public Damage Damage;

		float diffusion;
		/// <summary>
		/// 拡散
		/// </summary>
		public float Diffusion
		{
			get => diffusion;
			set => diffusion = Math.Max(value, 0);
		}

		public float Speed;

		float duration;
		/// <summary>
		/// 持続時間
		/// </summary>
		public float Duration
		{
			get => duration;
			set => duration = Math.Max(value, 0);
		}

		/// <summary>
		/// サイズ。負の数を許すことで逆に大きくできる
		/// </summary>
		public float Size = 1;

		float amount;
		/// <summary>
		/// 量・数
		/// </summary>
		public float Amount
		{
			get => amount;
			set => amount = Math.Max(value, 0);
		}

		float recoil;
		/// <summary>
		/// 反動。打った方が受ける力。
		/// </summary>
		public float Recoil
		{
			get => recoil;
			set => recoil = Math.Max(value, 0);
		}

		public Shot()
		{
			NextItems = new List<Item>();
			Projectiles = new List<GameObject>();
			Target = new Vector2();
			Damage = new Damage();
		}

		public Shot(Shot shot)
		{
			SetEssentialValues(shot.User, new(shot.NextItems), new(shot.Projectiles), shot.Target);
			SetAdditionalValues(shot.Damage, shot.Diffusion, shot.Speed, shot.Duration, shot.Size, shot.Amount, shot.Recoil, shot.TargetLayer);
		}

		public Shot(MobManager user, Item nextItem, GameObject projectile, Vector2 target)
		{
			SetEssentialValues(user, new List<Item>() { nextItem }, new List<GameObject> { projectile }, target);
		}

		Shot SetEssentialValues(MobManager user, List<Item> nextItem, List<GameObject> projectiles, Vector2 target)
		{
			User = user;
			NextItems = nextItem;
			Projectiles = projectiles;
			Target = target;
			return this;
		}

		// Additionalな値をセット
		public Shot SetAdditionalValues(Damage damage, float diffusion, float speed, float duration, float size, float amount, float recoil, LayerMask targetLayer)
		{
			Damage = damage;
			Diffusion = diffusion;
			Speed = speed;
			Duration = duration;
			Size = size;
			Amount = amount;
			Recoil = recoil;
			TargetLayer = targetLayer;
			return this;
		}

		/// <summary>
		/// NextItemsを発射。shotに情報をまとめているおかげで，shotさえあればどこでも発射できる。
		/// </summary>
		/// <param name="shot"></param>
		public void FireNextItems()
		{
			foreach (Item nextItem in NextItems)
			{
				nextItem.Fire(new(this));
			}
		}
	}
}
