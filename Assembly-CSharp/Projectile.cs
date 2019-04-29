using ConVar;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseMonoBehaviour
{
	public const float lifeTime = 8f;

	[Header("Attributes")]
	public Vector3 initialVelocity;

	public float drag;

	public float gravityModifier = 1f;

	public float thickness;

	[Tooltip("This projectile will raycast for this many units, and then become a projectile. This is typically done for bullets.")]
	public float initialDistance;

	[Header("Impact Rules")]
	public bool remainInWorld;

	[Range(0f, 1f)]
	public float stickProbability = 1f;

	[Range(0f, 1f)]
	public float breakProbability;

	[Range(0f, 1f)]
	public float conditionLoss;

	[Range(0f, 1f)]
	public float ricochetChance;

	public float penetrationPower = 1f;

	[Header("Damage")]
	public DamageProperties damageProperties;

	[Horizontal(2, -1)]
	public MinMax damageDistances = new MinMax(10f, 100f);

	[Horizontal(2, -1)]
	public MinMax damageMultipliers = new MinMax(1f, 0.8f);

	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	[Header("Rendering")]
	public ScaleRenderer rendererToScale;

	public ScaleRenderer firstPersonRenderer;

	public bool createDecals = true;

	[Header("Audio")]
	public SoundDefinition flybySound;

	public float flybySoundDistance = 7f;

	public SoundDefinition closeFlybySound;

	public float closeFlybyDistance = 3f;

	[Header("Tumble")]
	public float tumbleSpeed;

	public Vector3 tumbleAxis = Vector3.right;

	[Header("Swim")]
	public Vector3 swimScale;

	public Vector3 swimSpeed;

	[NonSerialized]
	public BasePlayer owner;

	[NonSerialized]
	public AttackEntity sourceWeaponPrefab;

	[NonSerialized]
	public Projectile sourceProjectilePrefab;

	[NonSerialized]
	public ItemModProjectile mod;

	[NonSerialized]
	public int projectileID;

	[NonSerialized]
	public int seed;

	[NonSerialized]
	public bool clientsideEffect;

	[NonSerialized]
	public bool clientsideAttack;

	[NonSerialized]
	public float integrity = 1f;

	[NonSerialized]
	public float maxDistance = Single.PositiveInfinity;

	[NonSerialized]
	public Projectile.Modifier modifier = Projectile.Modifier.Default;

	[NonSerialized]
	public bool invisible;

	private static uint _fleshMaterialID;

	private static uint _waterMaterialID;

	private static uint cachedWaterString;

	static Projectile()
	{
	}

	public Projectile()
	{
	}

	public void CalculateDamage(HitInfo info, Projectile.Modifier mod, float scale)
	{
		float single = this.damageMultipliers.Lerp(mod.distanceOffset + mod.distanceScale * this.damageDistances.x, mod.distanceOffset + mod.distanceScale * this.damageDistances.y, info.ProjectileDistance);
		float single1 = scale * (mod.damageOffset + mod.damageScale * single);
		foreach (DamageTypeEntry damageType in this.damageTypes)
		{
			info.damageTypes.Add(damageType.type, damageType.amount * single1);
		}
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[] { " Projectile damage: ", info.damageTypes.Total(), " (scalar=", single1, ")" }));
		}
	}

	public static uint FleshMaterialID()
	{
		if (Projectile._fleshMaterialID == 0)
		{
			Projectile._fleshMaterialID = StringPool.Get("flesh");
		}
		return Projectile._fleshMaterialID;
	}

	public static bool IsWaterMaterial(string hitMaterial)
	{
		if (Projectile.cachedWaterString == 0)
		{
			Projectile.cachedWaterString = StringPool.Get("Water");
		}
		if (StringPool.Get(hitMaterial) == Projectile.cachedWaterString)
		{
			return true;
		}
		return false;
	}

	public static uint WaterMaterialID()
	{
		if (Projectile._waterMaterialID == 0)
		{
			Projectile._waterMaterialID = StringPool.Get("Water");
		}
		return Projectile._waterMaterialID;
	}

	public struct Modifier
	{
		public float damageScale;

		public float damageOffset;

		public float distanceScale;

		public float distanceOffset;

		public static Projectile.Modifier Default;

		static Modifier()
		{
			Projectile.Modifier modifier = new Projectile.Modifier()
			{
				damageScale = 1f,
				damageOffset = 0f,
				distanceScale = 1f,
				distanceOffset = 0f
			};
			Projectile.Modifier.Default = modifier;
		}
	}
}