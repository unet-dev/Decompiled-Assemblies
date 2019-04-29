using ConVar;
using System;

public class DeployableDecay : Decay
{
	public float decayDelay = 8f;

	public float decayDuration = 8f;

	public DeployableDecay()
	{
	}

	public override float GetDecayDelay(BaseEntity entity)
	{
		return this.decayDelay * 60f * 60f;
	}

	public override float GetDecayDuration(BaseEntity entity)
	{
		return this.decayDuration * 60f * 60f;
	}

	public override bool ShouldDecay(BaseEntity entity)
	{
		if (ConVar.Decay.upkeep)
		{
			return true;
		}
		return entity.IsOutside();
	}
}