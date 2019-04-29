using Rust;
using System;

public class ItemPickup : DroppedItem
{
	public ItemDefinition itemDef;

	public int amount = 1;

	public ulong skinOverride;

	public ItemPickup()
	{
	}

	public override float GetDespawnDuration()
	{
		return Single.PositiveInfinity;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.IdleDestroy();
	}

	public override void Spawn()
	{
		base.Spawn();
		if (Application.isLoadingSave)
		{
			return;
		}
		Item item = ItemManager.Create(this.itemDef, this.amount, this.skinOverride);
		base.InitializeItem(item);
		item.SetWorldEntity(this);
	}
}