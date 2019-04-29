using System;
using UnityEngine;

public class Keycard : AttackEntity
{
	public int accessLevel
	{
		get
		{
			Item item = this.GetItem();
			if (item == null)
			{
				return 0;
			}
			ItemModKeycard component = item.info.GetComponent<ItemModKeycard>();
			if (component == null)
			{
				return 0;
			}
			return component.accessLevel;
		}
	}

	public Keycard()
	{
	}
}