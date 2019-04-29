using System;
using UnityEngine;

public class HumanBodyResourceDispenser : ResourceDispenser
{
	public HumanBodyResourceDispenser()
	{
	}

	public override bool OverrideOwnership(Item item, AttackEntity weapon)
	{
		if (item.info.shortname == "skull.human")
		{
			PlayerCorpse component = base.GetComponent<PlayerCorpse>();
			if (component)
			{
				item.name = string.Concat("Skull of \"", component.playerName, "\"");
				return true;
			}
		}
		return false;
	}
}