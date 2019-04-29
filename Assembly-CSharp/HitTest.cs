using System;
using UnityEngine;

public class HitTest
{
	public HitTest.Type type;

	public Ray AttackRay;

	public float Radius;

	public float Forgiveness;

	public float MaxDistance;

	public RaycastHit RayHit;

	public bool MultiHit;

	public bool BestHit;

	public bool DidHit;

	public DamageProperties damageProperties;

	public GameObject gameObject;

	public Collider collider;

	public BaseEntity ignoreEntity;

	public BaseEntity HitEntity;

	public Vector3 HitPoint;

	public Vector3 HitNormal;

	public float HitDistance;

	public Transform HitTransform;

	public uint HitPart;

	public string HitMaterial;

	public HitTest()
	{
	}

	public void Clear()
	{
		this.type = HitTest.Type.Generic;
		this.AttackRay = new Ray();
		this.Radius = 0f;
		this.Forgiveness = 0f;
		this.MaxDistance = 0f;
		this.RayHit = new RaycastHit();
		this.MultiHit = false;
		this.BestHit = false;
		this.DidHit = false;
		this.damageProperties = null;
		this.gameObject = null;
		this.collider = null;
		this.ignoreEntity = null;
		this.HitEntity = null;
		this.HitPoint = new Vector3();
		this.HitNormal = new Vector3();
		this.HitDistance = 0f;
		this.HitTransform = null;
		this.HitPart = 0;
		this.HitMaterial = null;
	}

	public Vector3 HitNormalWorld()
	{
		if (this.HitEntity == null)
		{
			return this.HitNormal;
		}
		Transform hitTransform = this.HitTransform;
		if (!hitTransform)
		{
			hitTransform = this.HitEntity.transform;
		}
		return hitTransform.TransformDirection(this.HitNormal);
	}

	public Vector3 HitPointWorld()
	{
		if (this.HitEntity == null)
		{
			return this.HitPoint;
		}
		Transform hitTransform = this.HitTransform;
		if (!hitTransform)
		{
			hitTransform = this.HitEntity.transform;
		}
		return hitTransform.TransformPoint(this.HitPoint);
	}

	public enum Type
	{
		Generic,
		ProjectileEffect,
		Projectile,
		MeleeAttack,
		Use
	}
}