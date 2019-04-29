using ConVar;
using System;

public class BuildingGradeDecay : Decay
{
	public BuildingGrade.Enum decayGrade;

	public BuildingGradeDecay()
	{
	}

	public override float GetDecayDelay(BaseEntity entity)
	{
		return base.GetDecayDelay(this.decayGrade);
	}

	public override float GetDecayDuration(BaseEntity entity)
	{
		return base.GetDecayDuration(this.decayGrade);
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