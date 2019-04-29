using Facepunch;
using Facepunch.Math;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ConVar
{
	[Factory("chat")]
	public class Chat : ConsoleSystem
	{
		private const float textRange = 50f;

		private const float textVolumeBoost = 0.2f;

		[ClientVar]
		[ServerVar]
		public static bool enabled;

		private static List<Chat.ChatEntry> History;

		[ServerVar]
		public static bool serverlog;

		static Chat()
		{
			Chat.enabled = true;
			Chat.History = new List<Chat.ChatEntry>();
			Chat.serverlog = true;
		}

		public Chat()
		{
		}

		public static void Broadcast(string message, string username = "SERVER", string color = "#eee", ulong userid = 0L)
		{
			if (Interface.CallHook("OnServerMessage", message, username, color, userid) != null)
			{
				return;
			}
			string str = username.EscapeRichText();
			object[] objArray = new object[] { 0, null };
			objArray[1] = string.Concat(new string[] { "<color=", color, ">", str, "</color> ", message });
			ConsoleNetwork.BroadcastToAllClients("chat.add", objArray);
			Chat.ChatEntry chatEntry = new Chat.ChatEntry()
			{
				Message = message,
				UserId = userid,
				Username = username,
				Color = color,
				Time = Epoch.Current
			};
			Chat.ChatEntry chatEntry1 = chatEntry;
			Chat.History.Add(chatEntry1);
			RCon.Broadcast(RCon.LogType.Chat, chatEntry1);
		}

		[ServerUserVar]
		public static void say(ConsoleSystem.Arg arg)
		{
			if (!Chat.enabled)
			{
				arg.ReplyWith("Chat is disabled.");
				return;
			}
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper)
			{
				if (basePlayer.NextChatTime == 0f)
				{
					basePlayer.NextChatTime = UnityEngine.Time.realtimeSinceStartup - 30f;
				}
				if (basePlayer.NextChatTime > UnityEngine.Time.realtimeSinceStartup)
				{
					basePlayer.NextChatTime += 2f;
					float nextChatTime = basePlayer.NextChatTime - UnityEngine.Time.realtimeSinceStartup;
					Connection connection = basePlayer.net.connection;
					object[] objArray = new object[] { 0, null };
					float single = nextChatTime + 0.5f;
					objArray[1] = string.Concat("You're chatting too fast - try again in ", single.ToString("0"), " seconds");
					ConsoleNetwork.SendClientCommand(connection, "chat.add", objArray);
					if (nextChatTime > 120f)
					{
						basePlayer.Kick("Chatting too fast");
					}
					return;
				}
			}
			string str = arg.GetString(0, "text").Trim();
			if (str.Length > 128)
			{
				str = str.Substring(0, 128);
			}
			if (str.Length <= 0)
			{
				return;
			}
			if (str.StartsWith("/") || str.StartsWith("\\"))
			{
				if (Interface.CallHook("IOnPlayerCommand", arg) != null)
				{
					return;
				}
				return;
			}
			str = str.EscapeRichText();
			if (Interface.CallHook("IOnPlayerChat", arg) != null)
			{
				return;
			}
			if (Chat.serverlog)
			{
				ServerConsole.PrintColoured(new object[] { ConsoleColor.DarkYellow, string.Concat(basePlayer.displayName, ": "), ConsoleColor.DarkGreen, str });
				DebugEx.Log(string.Format("[CHAT] {0} : {1}", basePlayer.ToString(), str), StackTraceLogType.None);
			}
			string str1 = "#5af";
			if (basePlayer.IsAdmin)
			{
				str1 = "#af5";
			}
			if (basePlayer.IsDeveloper)
			{
				str1 = "#fa5";
			}
			string str2 = basePlayer.displayName.EscapeRichText();
			basePlayer.NextChatTime = UnityEngine.Time.realtimeSinceStartup + 1.5f;
			Chat.ChatEntry chatEntry = new Chat.ChatEntry()
			{
				Message = str,
				UserId = basePlayer.userID,
				Username = basePlayer.displayName,
				Color = str1,
				Time = Epoch.Current
			};
			Chat.ChatEntry chatEntry1 = chatEntry;
			Chat.History.Add(chatEntry1);
			RCon.Broadcast(RCon.LogType.Chat, chatEntry1);
			if (ConVar.Server.globalchat)
			{
				ConsoleNetwork.BroadcastToAllClients("chat.add2", new object[] { basePlayer.userID, str, str2, str1, 1f });
				arg.ReplyWith("");
				return;
			}
			float single1 = 2500f;
			foreach (BasePlayer basePlayer1 in BasePlayer.activePlayerList)
			{
				Vector3 vector3 = basePlayer1.transform.position - basePlayer.transform.position;
				float single2 = vector3.sqrMagnitude;
				if (single2 > single1)
				{
					continue;
				}
				ConsoleNetwork.SendClientCommand(basePlayer1.net.connection, "chat.add2", new object[] { basePlayer.userID, str, str2, str1, Mathf.Clamp01(single1 - single2 + 0.2f) });
			}
			arg.ReplyWith("");
		}

		[Help("Search the console for a particular string")]
		[ServerVar]
		public static IEnumerable<Chat.ChatEntry> search(ConsoleSystem.Arg arg)
		{
			string str = arg.GetString(0, null);
			if (str == null)
			{
				return Enumerable.Empty<Chat.ChatEntry>();
			}
			return Chat.History.Where<Chat.ChatEntry>((Chat.ChatEntry x) => {
				if (x.Message.Length >= 4096)
				{
					return false;
				}
				return x.Message.Contains(str, CompareOptions.IgnoreCase);
			});
		}

		[Help("Return the last x lines of the console. Default is 200")]
		[ServerVar]
		public static IEnumerable<Chat.ChatEntry> tail(ConsoleSystem.Arg arg)
		{
			int num = arg.GetInt(0, 200);
			int count = Chat.History.Count - num;
			if (count < 0)
			{
				count = 0;
			}
			return Chat.History.Skip<Chat.ChatEntry>(count);
		}

		public struct ChatEntry
		{
			public string Color
			{
				get;
				set;
			}

			public string Message
			{
				get;
				set;
			}

			public int Time
			{
				get;
				set;
			}

			public ulong UserId
			{
				get;
				set;
			}

			public string Username
			{
				get;
				set;
			}
		}
	}
}