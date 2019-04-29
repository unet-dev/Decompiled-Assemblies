using System;
using UnityEngine;

public class ScaleParticleSystem : ScaleRenderer
{
	public ParticleSystem pSystem;

	public bool scaleGravity;

	[NonSerialized]
	private float startSize;

	[NonSerialized]
	private float startLifeTime;

	[NonSerialized]
	private float startSpeed;

	[NonSerialized]
	private float startGravity;

	public ScaleParticleSystem()
	{
	}

	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		this.startGravity = this.pSystem.gravityModifier;
		this.startSpeed = this.pSystem.startSpeed;
		this.startSize = this.pSystem.startSize;
		this.startLifeTime = this.pSystem.startLifetime;
	}

	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.pSystem.startSize = this.startSize * scale;
		this.pSystem.startLifetime = this.startLifeTime * scale;
		this.pSystem.startSpeed = this.startSpeed * scale;
		this.pSystem.gravityModifier = this.startGravity * scale;
	}
}