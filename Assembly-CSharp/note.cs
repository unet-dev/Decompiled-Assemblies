using Facepunch.Extend;
using System;
using UnityEngine;

[Factory("note")]
public class note : ConsoleSystem
{
	public note()
	{
	}

	[ServerUserVar]
	public static void update(ConsoleSystem.Arg arg)
	{
		uint num = arg.GetUInt(0, 0);
		string str = arg.GetString(1, "");
		Item item = arg.Player().inventory.FindItemUID(num);
		if (item == null)
		{
			return;
		}
		item.text = str.Truncate(1024, null);
		item.MarkDirty();
	}
}