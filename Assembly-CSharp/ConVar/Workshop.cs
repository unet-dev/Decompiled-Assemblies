using Facepunch.Steamworks;
using Rust;
using System;

namespace ConVar
{
	[Factory("workshop")]
	public class Workshop : ConsoleSystem
	{
		public Workshop()
		{
		}

		[ServerVar]
		public static void print_approved_skins(ConsoleSystem.Arg arg)
		{
			if (Rust.Global.SteamServer != null && Rust.Global.SteamServer.Inventory.Definitions != null)
			{
				TextTable textTable = new TextTable();
				textTable.AddColumn("name");
				textTable.AddColumn("itemshortname");
				textTable.AddColumn("workshopid");
				textTable.AddColumn("workshopdownload");
				Facepunch.Steamworks.Inventory.Definition[] definitions = Rust.Global.SteamServer.Inventory.Definitions;
				for (int i = 0; i < (int)definitions.Length; i++)
				{
					Facepunch.Steamworks.Inventory.Definition definition = definitions[i];
					string name = definition.Name;
					string stringProperty = definition.GetStringProperty("itemshortname");
					string str = definition.GetStringProperty("workshopid");
					string stringProperty1 = definition.GetStringProperty("workshopdownload");
					textTable.AddRow(new string[] { name, stringProperty, str, stringProperty1 });
				}
				arg.ReplyWith(textTable.ToString());
			}
		}
	}
}