using Steamworks;
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
			if (!SteamServer.IsValid)
			{
				return;
			}
			if (SteamInventory.Definitions == null)
			{
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("name");
			textTable.AddColumn("itemshortname");
			textTable.AddColumn("workshopid");
			textTable.AddColumn("workshopdownload");
			InventoryDef[] definitions = SteamInventory.Definitions;
			for (int i = 0; i < (int)definitions.Length; i++)
			{
				InventoryDef inventoryDef = definitions[i];
				string name = inventoryDef.Name;
				string property = inventoryDef.GetProperty("itemshortname");
				string str = inventoryDef.GetProperty("workshopid");
				string property1 = inventoryDef.GetProperty("workshopdownload");
				textTable.AddRow(new string[] { name, property, str, property1 });
			}
			arg.ReplyWith(textTable.ToString());
		}
	}
}