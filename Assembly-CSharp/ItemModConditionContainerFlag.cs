using System;

public class ItemModConditionContainerFlag : ItemMod
{
	public ItemContainer.Flag flag;

	public bool requiredState;

	public ItemModConditionContainerFlag()
	{
	}

	public override bool Passes(Item item)
	{
		if (item.parent == null)
		{
			return !this.requiredState;
		}
		if (item.parent.HasFlag(this.flag))
		{
			return this.requiredState;
		}
		return !this.requiredState;
	}
}