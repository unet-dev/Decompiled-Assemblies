using System;

public class Bear : BaseAnimalNPC
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

	static Bear()
	{
		Bear.Population = 2f;
	}

	public Bear()
	{
	}

	public override string Categorize()
	{
		return "Bear";
	}

	public override bool WantsToEat(BaseEntity best)
	{
		if (best.HasTrait(BaseEntity.TraitFlag.Alive))
		{
			return false;
		}
		return base.WantsToEat(best);
	}
}