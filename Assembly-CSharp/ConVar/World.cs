using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConVar
{
	[Factory("world")]
	public class World : ConsoleSystem
	{
		[ClientVar]
		[ServerVar]
		public static bool cache;

		static World()
		{
			World.cache = true;
		}

		public World()
		{
		}

		[ClientVar]
		public static void monuments(ConsoleSystem.Arg arg)
		{
			if (!TerrainMeta.Path)
			{
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("name");
			textTable.AddColumn("pos");
			foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
			{
				string[] str = new string[] { monument.Type.ToString(), monument.name, null };
				str[2] = monument.transform.position.ToString();
				textTable.AddRow(str);
			}
			arg.ReplyWith(textTable.ToString());
		}
	}
}