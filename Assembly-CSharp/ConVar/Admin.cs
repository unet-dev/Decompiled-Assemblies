using Facepunch;
using Facepunch.Extend;
using Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ConVar
{
	[Factory("global")]
	public class Admin : ConsoleSystem
	{
		public Admin()
		{
		}

		[ServerVar]
		public static void ban(ConsoleSystem.Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			ServerUsers.User user = ServerUsers.Get(player.userID);
			if (user != null && user.@group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Concat("User ", player.userID, " is already banned"));
				return;
			}
			string str = arg.GetString(1, "No Reason Given");
			ServerUsers.Set(player.userID, ServerUsers.UserGroup.Banned, player.displayName, str);
			string str1 = "";
			if (player.IsConnected && player.net.connection.ownerid != player.net.connection.userid)
			{
				str1 = string.Concat(str1, " and also banned ownerid ", player.net.connection.ownerid);
				ServerUsers.Set(player.net.connection.ownerid, ServerUsers.UserGroup.Banned, player.displayName, arg.GetString(1, string.Concat("Family share owner of ", player.net.connection.userid)));
			}
			ServerUsers.Save();
			arg.ReplyWith(string.Concat(new object[] { "Kickbanned User: ", player.userID, " - ", player.displayName, str1 }));
			Chat.Broadcast(string.Concat(new string[] { "Kickbanning ", player.displayName, " (", str, ")" }), "SERVER", "#eee", (ulong)0);
			Network.Net.sv.Kick(player.net.connection, string.Concat("Banned: ", str));
		}

		[ServerVar]
		public static void banid(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, (ulong)0);
			string str = arg.GetString(1, "unnamed");
			string str1 = arg.GetString(2, "no reason");
			if (num < 70000000000000000L)
			{
				arg.ReplyWith(string.Concat("This doesn't appear to be a 64bit steamid: ", num));
				return;
			}
			ServerUsers.User user = ServerUsers.Get(num);
			if (user != null && user.@group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Concat("User ", num, " is already banned"));
				return;
			}
			ServerUsers.Set(num, ServerUsers.UserGroup.Banned, str, str1);
			arg.ReplyWith(string.Concat(new object[] { "Banned User: ", num, " - ", str }));
		}

		[ServerVar(Help="List of banned users (sourceds compat)")]
		public static void banlist(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListString(false));
		}

		[ServerVar(Help="List of banned users - shows reasons and usernames")]
		public static void banlistex(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListStringEx());
		}

		[ServerVar(Help="List of banned users")]
		public static ServerUsers.User[] Bans()
		{
			return ServerUsers.GetAll(ServerUsers.UserGroup.Banned).ToArray<ServerUsers.User>();
		}

		[ServerVar(Help="Get information about this build")]
		public static BuildInfo BuildInfo()
		{
			return BuildInfo.Current;
		}

		[ServerVar]
		public static void clientperf(ConsoleSystem.Arg arg)
		{
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.ClientRPCPlayer(null, basePlayer, "GetPerformanceReport");
			}
		}

		[ServerVar]
		public static void entid(ConsoleSystem.Arg arg)
		{
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(arg.GetUInt(1, 0)) as BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			if (baseEntity is BasePlayer)
			{
				return;
			}
			string str = arg.GetString(0, "");
			if (arg.Player() != null)
			{
				Debug.Log(string.Concat(new object[] { "[ENTCMD] ", arg.Player().displayName, "/", arg.Player().userID, " used *", str, "* on ent: ", baseEntity.name }));
			}
			if (str == "kill")
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
				return;
			}
			if (str == "lock")
			{
				baseEntity.SetFlag(BaseEntity.Flags.Locked, true, false, true);
				return;
			}
			if (str == "unlock")
			{
				baseEntity.SetFlag(BaseEntity.Flags.Locked, false, false, true);
				return;
			}
			if (str == "debug")
			{
				baseEntity.SetFlag(BaseEntity.Flags.Debugging, true, false, true);
				return;
			}
			if (str == "undebug")
			{
				baseEntity.SetFlag(BaseEntity.Flags.Debugging, false, false, true);
				return;
			}
			if (str != "who")
			{
				arg.ReplyWith("Unknown command");
				return;
			}
			arg.ReplyWith(string.Concat("Owner ID: ", baseEntity.OwnerID));
		}

		[ServerVar]
		public static void kick(ConsoleSystem.Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			string str = arg.GetString(1, "no reason given");
			arg.ReplyWith(string.Concat("Kicked: ", player.displayName));
			Chat.Broadcast(string.Concat(new string[] { "Kicking ", player.displayName, " (", str, ")" }), "SERVER", "#eee", (ulong)0);
			player.Kick(string.Concat("Kicked: ", arg.GetString(1, "No Reason Given")));
		}

		[ServerVar]
		public static void kickall(ConsoleSystem.Arg arg)
		{
			BasePlayer[] array = BasePlayer.activePlayerList.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				array[i].Kick(string.Concat("Kicked: ", arg.GetString(1, "No Reason Given")));
			}
		}

		[ServerVar(Help="List of banned users, by ID (sourceds compat)")]
		public static void listid(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListString(true));
		}

		[ServerVar]
		public static void moderatorid(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, (ulong)0);
			string str = arg.GetString(1, "unnamed");
			string str1 = arg.GetString(2, "no reason");
			if (num < 70000000000000000L)
			{
				arg.ReplyWith(string.Concat("This doesn't appear to be a 64bit steamid: ", num));
				return;
			}
			ServerUsers.User user = ServerUsers.Get(num);
			if (user != null && user.@group == ServerUsers.UserGroup.Moderator)
			{
				arg.ReplyWith(string.Concat("User ", num, " is already a Moderator"));
				return;
			}
			ServerUsers.Set(num, ServerUsers.UserGroup.Moderator, str, str1);
			arg.ReplyWith(string.Concat(new object[] { "Added moderator ", str, ", steamid ", num }));
		}

		[ServerVar]
		public static void mutechat(ConsoleSystem.Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			player.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, true);
		}

		[ServerVar]
		public static void mutevoice(ConsoleSystem.Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			player.SetPlayerFlag(BasePlayer.PlayerFlags.VoiceMuted, true);
		}

		[ServerVar]
		public static void ownerid(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, (ulong)0);
			string str = arg.GetString(1, "unnamed");
			string str1 = arg.GetString(2, "no reason");
			if (num < 70000000000000000L)
			{
				arg.ReplyWith(string.Concat("This doesn't appear to be a 64bit steamid: ", num));
				return;
			}
			ServerUsers.User user = ServerUsers.Get(num);
			if (user != null && user.@group == ServerUsers.UserGroup.Owner)
			{
				arg.ReplyWith(string.Concat("User ", num, " is already an Owner"));
				return;
			}
			ServerUsers.Set(num, ServerUsers.UserGroup.Owner, str, str1);
			arg.ReplyWith(string.Concat(new object[] { "Added owner ", str, ", steamid ", num }));
		}

		[ServerVar(Help="Get a list of players")]
		public static Admin.PlayerInfo[] playerlist()
		{
			return (
				from x in BasePlayer.activePlayerList
				select new Admin.PlayerInfo()
				{
					SteamID = x.userID.ToString(),
					OwnerSteamID = x.OwnerID.ToString(),
					DisplayName = x.displayName,
					Ping = Network.Net.sv.GetAveragePing(x.net.connection),
					Address = x.net.connection.ipaddress,
					ConnectedSeconds = (int)x.net.connection.GetSecondsConnected(),
					VoiationLevel = x.violationLevel,
					Health = x.Health()
				}).ToArray<Admin.PlayerInfo>();
		}

		[ServerVar(Help="Print out currently connected clients etc")]
		public static void players(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("ping");
			textTable.AddColumn("snap");
			textTable.AddColumn("updt");
			textTable.AddColumn("posi");
			textTable.AddColumn("dist");
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				string userIDString = basePlayer.UserIDString;
				string str = basePlayer.displayName.ToString();
				if (str.Length >= 14)
				{
					str = string.Concat(str.Substring(0, 14), "..");
				}
				string str1 = str;
				int averagePing = Network.Net.sv.GetAveragePing(basePlayer.net.connection);
				string str2 = averagePing.ToString();
				string str3 = basePlayer.GetQueuedUpdateCount(BasePlayer.NetworkQueue.Update).ToString();
				string str4 = basePlayer.GetQueuedUpdateCount(BasePlayer.NetworkQueue.UpdateDistance).ToString();
				textTable.AddRow(new string[] { userIDString, str1, str2, string.Empty, str3, string.Empty, str4 });
			}
			arg.ReplyWith(textTable.ToString());
		}

		[ServerVar]
		public static void removemoderator(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, (ulong)0);
			if (num < 70000000000000000L)
			{
				arg.ReplyWith(string.Concat("This doesn't appear to be a 64bit steamid: ", num));
				return;
			}
			ServerUsers.User user = ServerUsers.Get(num);
			if (user == null || user.@group != ServerUsers.UserGroup.Moderator)
			{
				arg.ReplyWith(string.Concat("User ", num, " isn't a moderator"));
				return;
			}
			ServerUsers.Remove(num);
			arg.ReplyWith(string.Concat("Removed Moderator: ", num));
		}

		[ServerVar]
		public static void removeowner(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, (ulong)0);
			if (num < 70000000000000000L)
			{
				arg.ReplyWith(string.Concat("This doesn't appear to be a 64bit steamid: ", num));
				return;
			}
			ServerUsers.User user = ServerUsers.Get(num);
			if (user == null || user.@group != ServerUsers.UserGroup.Owner)
			{
				arg.ReplyWith(string.Concat("User ", num, " isn't an owner"));
				return;
			}
			ServerUsers.Remove(num);
			arg.ReplyWith(string.Concat("Removed Owner: ", num));
		}

		[ServerVar(Help="Sends a message in chat")]
		public static void say(ConsoleSystem.Arg arg)
		{
			Chat.Broadcast(arg.FullString, "SERVER", "#eee", (ulong)0);
		}

		[ServerVar(Help="Get a list of information about the server")]
		public static Admin.ServerInfoOutput ServerInfo()
		{
			Admin.ServerInfoOutput serverInfoOutput = new Admin.ServerInfoOutput()
			{
				Hostname = ConVar.Server.hostname,
				MaxPlayers = ConVar.Server.maxplayers,
				Players = BasePlayer.activePlayerList.Count,
				Queued = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued,
				Joining = SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining,
				EntityCount = BaseNetworkable.serverEntities.Count,
				GameTime = (TOD_Sky.Instance != null ? TOD_Sky.Instance.Cycle.DateTime.ToString() : DateTime.UtcNow.ToString()),
				Uptime = (int)UnityEngine.Time.realtimeSinceStartup,
				Map = ConVar.Server.level,
				Framerate = (float)Performance.report.frameRate,
				Memory = (int)Performance.report.memoryAllocations,
				Collections = (int)Performance.report.memoryCollections,
				NetworkIn = (Network.Net.sv == null ? 0 : (int)Network.Net.sv.GetStat(null, NetworkPeer.StatTypeLong.BytesReceived_LastSecond)),
				NetworkOut = (Network.Net.sv == null ? 0 : (int)Network.Net.sv.GetStat(null, NetworkPeer.StatTypeLong.BytesSent_LastSecond)),
				Restarting = SingletonComponent<ServerMgr>.Instance.Restarting,
				SaveCreatedTime = SaveRestore.SaveCreatedTime.ToString()
			};
			return serverInfoOutput;
		}

		[ServerVar]
		public static void skipqueue(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, (ulong)0);
			if (num >= 70000000000000000L)
			{
				SingletonComponent<ServerMgr>.Instance.connectionQueue.SkipQueue(num);
				return;
			}
			arg.ReplyWith(string.Concat("This doesn't appear to be a 64bit steamid: ", num));
		}

		[ServerVar(Help="Print out stats of currently connected clients")]
		public static void stats(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("time");
			textTable.AddColumn("kills");
			textTable.AddColumn("deaths");
			textTable.AddColumn("suicides");
			textTable.AddColumn("player");
			textTable.AddColumn("building");
			textTable.AddColumn("entity");
			Action<ulong, string> action = (ulong id, string name) => {
				ServerStatistics.Storage storage = ServerStatistics.Get(id);
				string shortString = TimeSpan.FromSeconds((double)storage.Get("time")).ToShortString();
				string str = storage.Get("kill_player").ToString();
				string str1 = (storage.Get("deaths") - storage.Get("death_suicide")).ToString();
				string str2 = storage.Get("death_suicide").ToString();
				string str3 = storage.Get("hit_player_direct_los").ToString();
				string str4 = storage.Get("hit_player_indirect_los").ToString();
				string str5 = storage.Get("hit_building_direct_los").ToString();
				string str6 = storage.Get("hit_building_indirect_los").ToString();
				string str7 = storage.Get("hit_entity_direct_los").ToString();
				string str8 = storage.Get("hit_entity_indirect_los").ToString();
				textTable.AddRow(new string[] { id.ToString(), name, shortString, str, str1, str2, string.Concat(str3, " / ", str4), string.Concat(str5, " / ", str6), string.Concat(str7, " / ", str8) });
			};
			ulong num = arg.GetUInt64(0, (ulong)0);
			if (num != 0)
			{
				string str9 = "N/A";
				BasePlayer basePlayer = BasePlayer.activePlayerList.Find((BasePlayer p) => p.userID == num);
				if (basePlayer)
				{
					str9 = basePlayer.GetSubName(32).QuoteSafe();
				}
				action(num, str9);
			}
			else
			{
				string str10 = arg.GetString(0, "");
				foreach (BasePlayer basePlayer1 in BasePlayer.activePlayerList)
				{
					try
					{
						if (basePlayer1.IsValid())
						{
							string str11 = basePlayer1.GetSubName(32).QuoteSafe();
							if (str10.Length <= 0 || str11.Contains(str10, CompareOptions.IgnoreCase))
							{
								action(basePlayer1.userID, str11);
							}
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						textTable.AddRow(new string[] { basePlayer1.UserIDString, exception.Message.QuoteSafe() });
					}
				}
			}
			arg.ReplyWith(textTable.ToString());
		}

		[ServerVar(Help="Print out currently connected clients")]
		public static void status(ConsoleSystem.Arg arg)
		{
			int averagePing;
			string str = arg.GetString(0, "");
			string empty = string.Empty;
			if (str.Length == 0)
			{
				empty = string.Concat(empty, "hostname: ", ConVar.Server.hostname, "\n");
				averagePing = 2163;
				empty = string.Concat(empty, "version : ", averagePing.ToString(), " secure (secure mode enabled, connected to Steam3)\n");
				empty = string.Concat(empty, "map     : ", ConVar.Server.level, "\n");
				empty = string.Concat(new object[] { empty, "players : ", BasePlayer.activePlayerList.Count<BasePlayer>(), " (", ConVar.Server.maxplayers, " max) (", SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued, " queued) (", SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining, " joining)\n\n" });
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("ping");
			textTable.AddColumn("connected");
			textTable.AddColumn("addr");
			textTable.AddColumn("owner");
			textTable.AddColumn("violation");
			textTable.AddColumn("kicks");
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				try
				{
					if (basePlayer.IsValid())
					{
						string userIDString = basePlayer.UserIDString;
						if (basePlayer.net.connection != null)
						{
							string str1 = basePlayer.net.connection.ownerid.ToString();
							string str2 = basePlayer.GetSubName(32).QuoteSafe();
							averagePing = Network.Net.sv.GetAveragePing(basePlayer.net.connection);
							string str3 = averagePing.ToString();
							string str4 = basePlayer.net.connection.ipaddress;
							string str5 = basePlayer.violationLevel.ToString("0.0");
							string str6 = basePlayer.GetAntiHackKicks().ToString();
							if (!arg.IsAdmin && !arg.IsRcon)
							{
								str4 = "xx.xxx.xx.xxx";
							}
							float secondsConnected = basePlayer.net.connection.GetSecondsConnected();
							string str7 = string.Concat(secondsConnected.ToString(), "s");
							if (str.Length <= 0 || str2.Contains(str, CompareOptions.IgnoreCase) || userIDString.Contains(str) || str1.Contains(str) || str4.Contains(str))
							{
								TextTable textTable1 = textTable;
								string[] strArrays = new string[] { userIDString, str2, str3, str7, str4, null, null, null };
								strArrays[5] = (str1 == userIDString ? string.Empty : str1);
								strArrays[6] = str5;
								strArrays[7] = str6;
								textTable1.AddRow(strArrays);
							}
						}
						else
						{
							textTable.AddRow(new string[] { userIDString, "NO CONNECTION" });
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					textTable.AddRow(new string[] { basePlayer.UserIDString, exception.Message.QuoteSafe() });
				}
			}
			arg.ReplyWith(string.Concat(empty, textTable.ToString()));
		}

		[ServerVar]
		public static void unban(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, (ulong)0);
			if (num < 70000000000000000L)
			{
				arg.ReplyWith(string.Concat("This doesn't appear to be a 64bit steamid: ", num));
				return;
			}
			ServerUsers.User user = ServerUsers.Get(num);
			if (user == null || user.@group != ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Concat("User ", num, " isn't banned"));
				return;
			}
			ServerUsers.Remove(num);
			arg.ReplyWith(string.Concat("Unbanned User: ", num));
		}

		[ServerVar]
		public static void unmutechat(ConsoleSystem.Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			player.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, false);
		}

		[ServerVar]
		public static void unmutevoice(ConsoleSystem.Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			player.SetPlayerFlag(BasePlayer.PlayerFlags.VoiceMuted, false);
		}

		[ServerVar(Help="Show user info for players on server.")]
		public static void users(ConsoleSystem.Arg arg)
		{
			string str = "<slot:userid:\"name\">\n";
			int num = 0;
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				str = string.Concat(new object[] { str, basePlayer.userID, ":\"", basePlayer.displayName, "\"\n" });
				num++;
			}
			str = string.Concat(str, num.ToString(), "users\n");
			arg.ReplyWith(str);
		}

		public struct PlayerInfo
		{
			public string SteamID;

			public string OwnerSteamID;

			public string DisplayName;

			public int Ping;

			public string Address;

			public int ConnectedSeconds;

			public float VoiationLevel;

			public float CurrentLevel;

			public float UnspentXp;

			public float Health;
		}

		public struct ServerInfoOutput
		{
			public string Hostname;

			public int MaxPlayers;

			public int Players;

			public int Queued;

			public int Joining;

			public int EntityCount;

			public string GameTime;

			public int Uptime;

			public string Map;

			public float Framerate;

			public int Memory;

			public int Collections;

			public int NetworkIn;

			public int NetworkOut;

			public bool Restarting;

			public string SaveCreatedTime;
		}
	}
}