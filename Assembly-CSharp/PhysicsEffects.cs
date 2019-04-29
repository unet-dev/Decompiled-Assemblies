using ConVar;
using System;
using UnityEngine;

public class PhysicsEffects : MonoBehaviour
{
	public BaseEntity entity;

	public SoundDefinition physImpactSoundDef;

	public float minTimeBetweenEffects = 0.25f;

	public float minDistBetweenEffects = 0.1f;

	public float hardnessScale = 1f;

	public float lowMedThreshold = 0.4f;

	public float medHardThreshold = 0.7f;

	public float enableDelay = 0.1f;

	public LayerMask ignoreLayers;

	private float lastEffectPlayed;

	private float enabledAt = Single.PositiveInfinity;

	private float ignoreImpactThreshold = 0.02f;

	private Vector3 lastCollisionPos;

	public PhysicsEffects()
	{
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!ConVar.Physics.sendeffects)
		{
			return;
		}
		if (UnityEngine.Time.time < this.enabledAt + this.enableDelay)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEffectPlayed + this.minTimeBetweenEffects)
		{
			return;
		}
		if ((1 << (collision.gameObject.layer & 31) & this.ignoreLayers) != 0)
		{
			return;
		}
		float single = collision.relativeVelocity.magnitude;
		single = single * 0.055f * this.hardnessScale;
		if (single <= this.ignoreImpactThreshold)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, this.lastCollisionPos) < this.minDistBetweenEffects && this.lastEffectPlayed != 0f)
		{
			return;
		}
		if (this.entity != null)
		{
			this.entity.SignalBroadcast(BaseEntity.Signal.PhysImpact, single.ToString(), null);
		}
		this.lastEffectPlayed = UnityEngine.Time.time;
		this.lastCollisionPos = base.transform.position;
	}

	public void OnEnable()
	{
		this.enabledAt = UnityEngine.Time.time;
	}
}