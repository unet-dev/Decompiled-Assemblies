using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemModCycle : ItemMod
{
	public ItemMod[] actions;

	public float timeBetweenCycles = 1f;

	public float timerStart;

	public bool onlyAdvanceTimerWhenPass;

	public ItemModCycle()
	{
	}

	private bool CanCycle(Item item)
	{
		ItemMod[] itemModArray = this.actions;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			if (!itemModArray[i].CanDoAction(item, item.GetOwnerPlayer()))
			{
				return false;
			}
		}
		return true;
	}

	public void CustomCycle(Item item, float delta)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		ItemMod[] itemModArray = this.actions;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].DoAction(item, ownerPlayer);
		}
	}

	public override void OnItemCreated(Item itemcreated)
	{
		float single = this.timerStart;
		itemcreated.onCycle += new Action<Item, float>((Item item, float delta) => {
			if (this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
			{
				return;
			}
			single += delta;
			if (single < this.timeBetweenCycles)
			{
				return;
			}
			single = 0f;
			if (!this.onlyAdvanceTimerWhenPass && !this.CanCycle(item))
			{
				return;
			}
			this.CustomCycle(item, delta);
		});
	}

	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null", base.gameObject);
		}
	}
}