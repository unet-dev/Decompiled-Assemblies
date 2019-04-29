using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonTools : MonoBehaviour
{
	public Image image;

	public ItemDefinition itemDef;

	public ItemButtonTools()
	{
	}

	public void GiveArmed()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.givearm", new object[] { this.itemDef.itemid });
	}

	public void GiveBlueprint()
	{
	}

	public void GiveSelf(int amount)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.giveid", new object[] { this.itemDef.itemid, amount });
	}
}