using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : TimedExplosive
{
	public float smokeDuration = 45f;

	public GameObjectRef smokeEffectPrefab;

	public GameObjectRef igniteSound;

	public SoundPlayer soundLoop;

	private GameObject smokeEffectInstance;

	public static List<SmokeGrenade> activeGrenades;

	public float fieldMin = 5f;

	public float fieldMax = 8f;

	protected bool killing;

	static SmokeGrenade()
	{
		SmokeGrenade.activeGrenades = new List<SmokeGrenade>();
	}

	public SmokeGrenade()
	{
	}

	public void CheckForWater()
	{
		if (this.WaterFactor() >= 0.5f)
		{
			this.FinishUp();
		}
	}

	public override void DestroyShared()
	{
		SmokeGrenade.activeGrenades.Remove(this);
		base.DestroyShared();
	}

	public override void Explode()
	{
		if (this.WaterFactor() >= 0.5f)
		{
			this.FinishUp();
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		base.Invoke(new Action(this.FinishUp), this.smokeDuration);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.InvalidateNetworkCache();
		base.SendNetworkUpdateImmediate(false);
		SmokeGrenade.activeGrenades.Add(this);
		Sensation sensation = new Sensation()
		{
			Type = SensationType.Explosion,
			Position = this.creatorEntity.transform.position,
			Radius = this.explosionRadius * 17f,
			DamagePotential = 0f,
			InitiatorPlayer = this.creatorEntity as BasePlayer,
			Initiator = this.creatorEntity
		};
		Sense.Stimulate(sensation);
	}

	public void FinishUp()
	{
		if (this.killing)
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
		this.killing = true;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.CheckForWater), 1f, 1f);
	}
}