using System;

public class ItemModSwitchFlag : ItemMod
{
	public Item.Flag flag;

	public bool state;

	public ItemModSwitchFlag()
	{
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		if (item.HasFlag(this.flag) == this.state)
		{
			return;
		}
		item.SetFlag(this.flag, this.state);
		item.MarkDirty();
	}
}