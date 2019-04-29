using System;

public class Stag : BaseAnimalNPC
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

	static Stag()
	{
		Stag.Population = 3f;
	}

	public Stag()
	{
	}

	public override string Categorize()
	{
		return "Stag";
	}

	public override bool WantsToEat(BaseEntity best)
	{
		if (best.HasTrait(BaseEntity.TraitFlag.Alive))
		{
			return false;
		}
		if (best.HasTrait(BaseEntity.TraitFlag.Meat))
		{
			return false;
		}
		CollectibleEntity collectibleEntity = best as CollectibleEntity;
		if (collectibleEntity != null)
		{
			ItemAmount[] itemAmountArray = collectibleEntity.itemList;
			for (int i = 0; i < (int)itemAmountArray.Length; i++)
			{
				if (itemAmountArray[i].itemDef.category == ItemCategory.Food)
				{
					return true;
				}
			}
		}
		return base.WantsToEat(best);
	}
}