using System;

public class Wolf : BaseAnimalNPC
{
	[ServerVar(Help="Population active on the server, per square km")]
	public static float Population;

	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat | BaseEntity.TraitFlag.Water;
		}
	}

	static Wolf()
	{
		Wolf.Population = 2f;
	}

	public Wolf()
	{
	}

	public override string Categorize()
	{
		return "Wolf";
	}

	public override bool WantsToEat(BaseEntity best)
	{
		if (best.HasTrait(BaseEntity.TraitFlag.Alive))
		{
			return false;
		}
		if (best.HasTrait(BaseEntity.TraitFlag.Meat))
		{
			return true;
		}
		return base.WantsToEat(best);
	}
}