using System;

public class ItemModConditionHasFlag : ItemMod
{
	public Item.Flag flag;

	public bool requiredState;

	public ItemModConditionHasFlag()
	{
	}

	public override bool Passes(Item item)
	{
		return item.HasFlag(this.flag) == this.requiredState;
	}
}