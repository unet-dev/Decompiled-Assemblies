using Rust;
using System;
using UnityEngine;

public class ItemModProjectile : MonoBehaviour
{
	public GameObjectRef projectileObject = new GameObjectRef();

	public ItemModProjectileMod[] mods;

	public AmmoTypes ammoType;

	public int numProjectiles = 1;

	public float projectileSpread;

	public float projectileVelocity = 100f;

	public float projectileVelocitySpread;

	public bool useCurve;

	public AnimationCurve spreadScalar;

	public GameObjectRef attackEffectOverride;

	public float barrelConditionLoss;

	public string category = "bullet";

	public ItemModProjectile()
	{
	}

	public float GetAverageVelocity()
	{
		return this.projectileVelocity;
	}

	public float GetIndexedSpreadScalar(int shotIndex, int maxShots)
	{
		float single = 0f;
		if (shotIndex == -1)
		{
			single = UnityEngine.Random.Range(0f, 1f);
		}
		else
		{
			float single1 = 1f / (float)maxShots;
			single = (float)shotIndex * single1;
		}
		return this.spreadScalar.Evaluate(single);
	}

	public float GetMaxVelocity()
	{
		return this.projectileVelocity + this.projectileVelocitySpread;
	}

	public float GetMinVelocity()
	{
		return this.projectileVelocity - this.projectileVelocitySpread;
	}

	public float GetRandomVelocity()
	{
		return this.projectileVelocity + UnityEngine.Random.Range(-this.projectileVelocitySpread, this.projectileVelocitySpread);
	}

	public float GetSpreadScalar()
	{
		if (!this.useCurve)
		{
			return 1f;
		}
		return this.spreadScalar.Evaluate(UnityEngine.Random.Range(0f, 1f));
	}

	public bool IsAmmo(AmmoTypes ammo)
	{
		return (int)(this.ammoType & ammo) != 0;
	}

	public virtual void ServerProjectileHit(HitInfo info)
	{
		if (this.mods == null)
		{
			return;
		}
		ItemModProjectileMod[] itemModProjectileModArray = this.mods;
		for (int i = 0; i < (int)itemModProjectileModArray.Length; i++)
		{
			ItemModProjectileMod itemModProjectileMod = itemModProjectileModArray[i];
			if (itemModProjectileMod != null)
			{
				itemModProjectileMod.ServerProjectileHit(info);
			}
		}
	}
}