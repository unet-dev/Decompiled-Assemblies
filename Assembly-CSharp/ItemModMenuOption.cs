using System;
using UnityEngine;

public class ItemModMenuOption : ItemMod
{
	public string commandName;

	public ItemMod actionTarget;

	public BaseEntity.Menu.Option option;

	[Tooltip("If true, this is the command that will run when an item is 'selected' on the toolbar")]
	public bool isPrimaryOption = true;

	public ItemModMenuOption()
	{
	}

	private void OnValidate()
	{
		if (this.actionTarget == null)
		{
			Debug.LogWarning("ItemModMenuOption: actionTarget is null!", base.gameObject);
		}
		if (string.IsNullOrEmpty(this.commandName))
		{
			Debug.LogWarning("ItemModMenuOption: commandName can't be empty!", base.gameObject);
		}
		if (this.option.icon == null)
		{
			Debug.LogWarning(string.Concat("No icon set for ItemModMenuOption ", base.gameObject.name), base.gameObject);
		}
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command != this.commandName)
		{
			return;
		}
		if (!this.actionTarget.CanDoAction(item, player))
		{
			return;
		}
		this.actionTarget.DoAction(item, player);
	}
}