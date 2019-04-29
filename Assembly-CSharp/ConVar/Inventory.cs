using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConVar
{
	[Factory("inventory")]
	public class Inventory : ConsoleSystem
	{
		public Inventory()
		{
		}

		[ServerUserVar]
		public static void endloot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			basePlayer.inventory.loot.Clear();
		}

		[ServerVar]
		public static void give(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item num = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1);
			if (num == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int num1 = arg.GetInt(1, 1);
			num.amount = num1;
			num.conditionNormalized = arg.GetFloat(2, 1f);
			num.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(num, null))
			{
				num.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[] { num.info.itemid, num1 });
			Debug.Log(string.Concat(new object[] { "giving ", basePlayer.displayName, " ", num1, " x ", num.info.displayName.english }));
			Chat.Broadcast(string.Concat(new object[] { basePlayer.displayName, " gave themselves ", num1, " x ", num.info.displayName.english }), "SERVER", "#eee", (ulong)0);
		}

		[ServerVar]
		public static void giveall(ConsoleSystem.Arg arg)
		{
			Item num = null;
			string str = "SERVER";
			if (arg.Player() != null)
			{
				str = arg.Player().displayName;
			}
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				num = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1);
				if (num != null)
				{
					num.amount = arg.GetInt(1, 1);
					num.OnVirginSpawn();
					if (basePlayer.inventory.GiveItem(num, null))
					{
						basePlayer.Command("note.inv", new object[] { num.info.itemid, num.amount });
						Debug.Log(string.Concat(new object[] { " [ServerVar] giving ", basePlayer.displayName, " ", num.amount, " x ", num.info.displayName.english }));
					}
					else
					{
						num.Remove(0f);
						arg.ReplyWith("Couldn't give item (inventory full?)");
					}
				}
				else
				{
					arg.ReplyWith("Invalid Item!");
					return;
				}
			}
			if (num != null)
			{
				Chat.Broadcast(string.Concat(new object[] { str, " gave everyone ", num.amount, " x ", num.info.displayName.english }), "SERVER", "#eee", (ulong)0);
			}
		}

		[ServerVar]
		public static void givearm(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item num = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, (ulong)0);
			if (num == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			num.amount = arg.GetInt(1, 1);
			num.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(num, basePlayer.inventory.containerBelt))
			{
				num.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[] { num.info.itemid, num.amount });
			Debug.Log(string.Concat(new object[] { " [ServerVar] giving ", basePlayer.displayName, " ", num.amount, " x ", num.info.displayName.english }));
			Chat.Broadcast(string.Concat(new object[] { basePlayer.displayName, " gave themselves ", num.amount, " x ", num.info.displayName.english }), "SERVER", "#eee", (ulong)0);
		}

		[ServerVar]
		public static void giveid(ConsoleSystem.Arg arg)
		{
			string str = "SERVER";
			if (arg.Player() != null)
			{
				str = arg.Player().displayName;
			}
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item num = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, (ulong)0);
			if (num == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			num.amount = arg.GetInt(1, 1);
			num.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(num, null))
			{
				num.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[] { num.info.itemid, num.amount });
			Debug.Log(string.Concat(new object[] { " [ServerVar] giving ", basePlayer.displayName, " ", num.amount, " x ", num.info.displayName.english }));
			Chat.Broadcast(string.Concat(new object[] { str, " gave ", basePlayer.displayName, " ", num.amount, " x ", num.info.displayName.english }), "SERVER", "#eee", (ulong)0);
		}

		[ServerVar]
		public static void giveto(ConsoleSystem.Arg arg)
		{
			string str = "SERVER";
			if (arg.Player() != null)
			{
				str = arg.Player().displayName;
			}
			BasePlayer basePlayer = BasePlayer.Find(arg.GetString(0, ""));
			if (basePlayer == null)
			{
				arg.ReplyWith("Couldn't find player!");
				return;
			}
			Item num = ItemManager.CreateByPartialName(arg.GetString(1, ""), 1);
			if (num == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			num.amount = arg.GetInt(2, 1);
			num.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(num, null))
			{
				num.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[] { num.info.itemid, num.amount });
			Debug.Log(string.Concat(new object[] { " [ServerVar] giving ", basePlayer.displayName, " ", num.amount, " x ", num.info.displayName.english }));
			Chat.Broadcast(string.Concat(new object[] { str, " gave ", basePlayer.displayName, " ", num.amount, " x ", num.info.displayName.english }), "SERVER", "#eee", (ulong)0);
		}

		[ServerUserVar]
		public static void lighttoggle(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			basePlayer.LightToggle();
		}

		[ServerVar]
		public static void resetbp(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			basePlayer.blueprints.Reset();
		}

		[ServerVar]
		public static void unlockall(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			basePlayer.blueprints.UnlockAll();
		}
	}
}