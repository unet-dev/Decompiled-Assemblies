using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemModCookable : ItemMod
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition becomeOnCooked;

	public float cookTime = 30f;

	public int amountOfBecome = 1;

	public int lowTemp;

	public int highTemp;

	public bool setCookingFlag;

	public ItemModCookable()
	{
	}

	public override void OnItemCreated(Item itemcreated)
	{
		float single1 = this.cookTime;
		itemcreated.onCycle += new Action<Item, float>((Item item, float delta) => {
			float single = item.temperature;
			if (single < (float)this.lowTemp || single > (float)this.highTemp || single1 < 0f)
			{
				if (this.setCookingFlag && item.HasFlag(Item.Flag.Cooking))
				{
					item.SetFlag(Item.Flag.Cooking, false);
					item.MarkDirty();
				}
				return;
			}
			if (this.setCookingFlag && !item.HasFlag(Item.Flag.Cooking))
			{
				item.SetFlag(Item.Flag.Cooking, true);
				item.MarkDirty();
			}
			single1 -= delta;
			if (single1 > 0f)
			{
				return;
			}
			int num = item.position;
			if (item.amount <= 1)
			{
				item.Remove(0f);
			}
			else
			{
				single1 = this.cookTime;
				item.amount--;
				item.MarkDirty();
			}
			if (this.becomeOnCooked != null)
			{
				Item item1 = ItemManager.Create(this.becomeOnCooked, this.amountOfBecome, (ulong)0);
				if (item1 != null && !item1.MoveToContainer(item.parent, num, true) && !item1.MoveToContainer(item.parent, -1, true))
				{
					item1.Drop(item.parent.dropPosition, item.parent.dropVelocity, new Quaternion());
					if (item.parent.entityOwner)
					{
						BaseOven component = item.parent.entityOwner.GetComponent<BaseOven>();
						if (component != null)
						{
							component.OvenFull();
						}
					}
				}
			}
		});
	}

	public void OnValidate()
	{
		if (this.amountOfBecome < 1)
		{
			this.amountOfBecome = 1;
		}
		if (this.becomeOnCooked == null)
		{
			Debug.LogWarning(string.Concat("[ItemModCookable] becomeOnCooked is unset! [", base.name, "]"), base.gameObject);
		}
	}
}