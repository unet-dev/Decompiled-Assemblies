using Network;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;

public class HitInfo
{
	public BaseEntity Initiator;

	public BaseEntity WeaponPrefab;

	public AttackEntity Weapon;

	public bool DoHitEffects = true;

	public bool DoDecals = true;

	public bool IsPredicting;

	public bool UseProtection = true;

	public Connection Predicted;

	public bool DidHit;

	public BaseEntity HitEntity;

	public uint HitBone;

	public uint HitPart;

	public uint HitMaterial;

	public Vector3 HitPositionWorld;

	public Vector3 HitPositionLocal;

	public Vector3 HitNormalWorld;

	public Vector3 HitNormalLocal;

	public Vector3 PointStart;

	public Vector3 PointEnd;

	public int ProjectileID;

	public float ProjectileDistance;

	public Vector3 ProjectileVelocity;

	public Projectile ProjectilePrefab;

	public PhysicMaterial material;

	public DamageProperties damageProperties;

	public DamageTypeList damageTypes = new DamageTypeList();

	public bool CanGather;

	public bool DidGather;

	public float gatherScale = 1f;

	public Vector3 attackNormal
	{
		get
		{
			return (this.PointEnd - this.PointStart).normalized;
		}
	}

	public HitArea boneArea
	{
		get
		{
			if (this.HitEntity == null)
			{
				return HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot;
			}
			BaseCombatEntity hitEntity = this.HitEntity as BaseCombatEntity;
			if (hitEntity == null)
			{
				return HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot;
			}
			return hitEntity.SkeletonLookup(this.HitBone);
		}
	}

	public string boneName
	{
		get
		{
			Translate.Phrase phrase = this.bonePhrase;
			if (phrase == null)
			{
				return "N/A";
			}
			return phrase.english;
		}
	}

	public Translate.Phrase bonePhrase
	{
		get
		{
			if (this.HitEntity == null)
			{
				return null;
			}
			BaseCombatEntity hitEntity = this.HitEntity as BaseCombatEntity;
			if (hitEntity == null)
			{
				return null;
			}
			if (hitEntity.skeletonProperties == null)
			{
				return null;
			}
			SkeletonProperties.BoneProperty boneProperty = hitEntity.skeletonProperties.FindBone(this.HitBone);
			if (boneProperty == null)
			{
				return null;
			}
			return boneProperty.name;
		}
	}

	public bool hasDamage
	{
		get
		{
			return this.damageTypes.Total() > 0f;
		}
	}

	public BasePlayer InitiatorPlayer
	{
		get
		{
			if (!this.Initiator)
			{
				return null;
			}
			return this.Initiator.ToPlayer();
		}
	}

	public bool isHeadshot
	{
		get
		{
			if (this.HitEntity == null)
			{
				return false;
			}
			BaseCombatEntity hitEntity = this.HitEntity as BaseCombatEntity;
			if (hitEntity == null)
			{
				return false;
			}
			if (hitEntity.skeletonProperties == null)
			{
				return false;
			}
			SkeletonProperties.BoneProperty boneProperty = hitEntity.skeletonProperties.FindBone(this.HitBone);
			if (boneProperty == null)
			{
				return false;
			}
			return boneProperty.area == HitArea.Head;
		}
	}

	public HitInfo()
	{
	}

	public HitInfo(BaseEntity attacker, BaseEntity target, DamageType type, float damageAmount, Vector3 vhitPosition)
	{
		this.Initiator = attacker;
		this.HitEntity = target;
		this.HitPositionWorld = vhitPosition;
		if (attacker != null)
		{
			this.PointStart = attacker.transform.position;
		}
		this.damageTypes.Add(type, damageAmount);
	}

	public HitInfo(BaseEntity attacker, BaseEntity target, DamageType type, float damageAmount) : this(attacker, target, type, damageAmount, target.transform.position)
	{
	}

	public Vector3 HitPositionOnRay()
	{
		return this.PositionOnRay(this.HitPositionWorld);
	}

	public bool IsNaNOrInfinity()
	{
		if (this.PointStart.IsNaNOrInfinity())
		{
			return true;
		}
		if (this.PointEnd.IsNaNOrInfinity())
		{
			return true;
		}
		if (this.HitPositionWorld.IsNaNOrInfinity())
		{
			return true;
		}
		if (this.HitPositionLocal.IsNaNOrInfinity())
		{
			return true;
		}
		if (this.HitNormalWorld.IsNaNOrInfinity())
		{
			return true;
		}
		if (this.HitNormalLocal.IsNaNOrInfinity())
		{
			return true;
		}
		if (this.ProjectileVelocity.IsNaNOrInfinity())
		{
			return true;
		}
		if (float.IsNaN(this.ProjectileDistance))
		{
			return true;
		}
		if (float.IsInfinity(this.ProjectileDistance))
		{
			return true;
		}
		return false;
	}

	public bool IsProjectile()
	{
		return this.ProjectileID != 0;
	}

	public void LoadFromAttack(Attack attack, bool serverSide)
	{
		this.HitEntity = null;
		this.PointStart = attack.pointStart;
		this.PointEnd = attack.pointEnd;
		if (attack.hitID > 0)
		{
			this.DidHit = true;
			if (serverSide)
			{
				this.HitEntity = BaseNetworkable.serverEntities.Find(attack.hitID) as BaseEntity;
			}
			if (this.HitEntity)
			{
				this.HitBone = attack.hitBone;
				this.HitPart = attack.hitPartID;
			}
		}
		this.DidHit = true;
		this.HitPositionLocal = attack.hitPositionLocal;
		this.HitPositionWorld = attack.hitPositionWorld;
		this.HitNormalLocal = attack.hitNormalLocal.normalized;
		this.HitNormalWorld = attack.hitNormalWorld.normalized;
		this.HitMaterial = attack.hitMaterialID;
	}

	public Vector3 PositionOnRay(Vector3 position)
	{
		RaycastHit raycastHit;
		Ray ray = new Ray(this.PointStart, this.attackNormal);
		if (this.ProjectilePrefab == null)
		{
			return ray.ClosestPoint(position);
		}
		Sphere sphere = new Sphere(position, this.ProjectilePrefab.thickness);
		if (!sphere.Trace(ray, out raycastHit, Single.PositiveInfinity))
		{
			return position;
		}
		return raycastHit.point;
	}
}