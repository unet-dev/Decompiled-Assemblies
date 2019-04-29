using System;
using UnityEngine;

namespace ConVar
{
	[Factory("craft")]
	public class Craft : ConsoleSystem
	{
		[ServerVar]
		public static bool instant;

		static Craft()
		{
		}

		public Craft()
		{
		}

		[ServerUserVar]
		public static void @add(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			int num = args.GetInt(0, 0);
			int num1 = args.GetInt(1, 1);
			int num2 = (int)args.GetUInt64(2, (ulong)0);
			if (num1 < 1)
			{
				return;
			}
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(num);
			if (itemDefinition == null)
			{
				args.ReplyWith("Item not found");
				return;
			}
			ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(itemDefinition);
			if (!itemBlueprint)
			{
				args.ReplyWith("Blueprint not found");
				return;
			}
			if (!itemBlueprint.userCraftable)
			{
				args.ReplyWith("Item is not craftable");
				return;
			}
			if (!basePlayer.blueprints.CanCraft(num, num2))
			{
				num2 = 0;
				if (!basePlayer.blueprints.CanCraft(num, num2))
				{
					args.ReplyWith("You can't craft this item");
					return;
				}
				args.ReplyWith("You don't have permission to use this skin, so crafting unskinned");
			}
			if (basePlayer.inventory.crafting.CraftItem(itemBlueprint, basePlayer, null, num1, num2, null))
			{
				return;
			}
			args.ReplyWith("Couldn't craft!");
		}

		[ServerUserVar]
		public static void cancel(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			int num = args.GetInt(0, 0);
			basePlayer.inventory.crafting.CancelBlueprint(num);
		}

		[ServerUserVar]
		public static void canceltask(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			int num = args.GetInt(0, 0);
			if (basePlayer.inventory.crafting.CancelTask(num, true))
			{
				return;
			}
			args.ReplyWith("Couldn't cancel task!");
		}
	}
}