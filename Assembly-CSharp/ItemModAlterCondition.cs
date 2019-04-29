using System;

public class ItemModAlterCondition : ItemMod
{
	public float conditionChange;

	public ItemModAlterCondition()
	{
	}

	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		if (this.conditionChange >= 0f)
		{
			item.RepairCondition(this.conditionChange);
			return;
		}
		item.LoseCondition(this.conditionChange * -1f);
	}
}