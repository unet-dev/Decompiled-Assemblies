using ConVar;
using Facepunch.Extend;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ServerUsers
{
	private static Dictionary<ulong, ServerUsers.User> users;

	static ServerUsers()
	{
		ServerUsers.users = new Dictionary<ulong, ServerUsers.User>();
	}

	public static string BanListString(bool bHeader = false)
	{
		IEnumerable<ServerUsers.User> all = ServerUsers.GetAll(ServerUsers.UserGroup.Banned);
		string str = "";
		if (bHeader)
		{
			if (all.Count<ServerUsers.User>() == 0)
			{
				return "ID filter list: empty\n";
			}
			if (all.Count<ServerUsers.User>() != 1)
			{
				int num = all.Count<ServerUsers.User>();
				str = string.Concat("ID filter list: ", num.ToString(), " entries\n");
			}
			else
			{
				str = "ID filter list: 1 entry\n";
			}
		}
		int num1 = 1;
		foreach (ServerUsers.User user in all)
		{
			str = string.Concat(new string[] { str, num1.ToString(), " ", user.steamid.ToString(), " : permanent\n" });
			num1++;
		}
		return str;
	}

	public static string BanListStringEx()
	{
		string str = "";
		int num = 1;
		foreach (ServerUsers.User all in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
		{
			str = string.Concat(new string[] { str, num.ToString(), " ", all.steamid.ToString(), " ", all.username.QuoteSafe(), " ", all.notes.QuoteSafe(), "\n" });
			num++;
		}
		return str;
	}

	public static void Clear()
	{
		ServerUsers.users.Clear();
	}

	public static ServerUsers.User Get(ulong uid)
	{
		ServerUsers.User user = null;
		if (ServerUsers.users.TryGetValue(uid, out user))
		{
			return user;
		}
		return null;
	}

	public static IEnumerable<ServerUsers.User> GetAll(ServerUsers.UserGroup group)
	{
		return 
			from x in ServerUsers.users
			where x.Value.@group == group
			select x.Value;
	}

	public static bool Is(ulong uid, ServerUsers.UserGroup group)
	{
		ServerUsers.User user = ServerUsers.Get(uid);
		if (user == null)
		{
			return false;
		}
		return user.@group == group;
	}

	public static void Load()
	{
		ConsoleSystem.Option server;
		ServerUsers.Clear();
		string serverFolder = Server.GetServerFolder("cfg");
		if (File.Exists(string.Concat(serverFolder, "/bans.cfg")))
		{
			string str = File.ReadAllText(string.Concat(serverFolder, "/bans.cfg"));
			if (!string.IsNullOrEmpty(str))
			{
				Debug.Log(string.Concat("Running ", serverFolder, "/bans.cfg"));
				server = ConsoleSystem.Option.Server;
				ConsoleSystem.RunFile(server.Quiet(), str);
			}
		}
		if (File.Exists(string.Concat(serverFolder, "/users.cfg")))
		{
			string str1 = File.ReadAllText(string.Concat(serverFolder, "/users.cfg"));
			if (!string.IsNullOrEmpty(str1))
			{
				Debug.Log(string.Concat("Running ", serverFolder, "/users.cfg"));
				server = ConsoleSystem.Option.Server;
				ConsoleSystem.RunFile(server.Quiet(), str1);
			}
		}
	}

	public static void Remove(ulong uid)
	{
		if (!ServerUsers.users.ContainsKey(uid))
		{
			return;
		}
		Interface.CallHook("IOnServerUsersRemove", uid);
		ServerUsers.users.Remove(uid);
	}

	public static void Save()
	{
		string serverFolder = Server.GetServerFolder("cfg");
		string str = "";
		foreach (ServerUsers.User all in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
		{
			if (all.notes == "EAC")
			{
				continue;
			}
			str = string.Concat(new string[] { str, "banid ", all.steamid.ToString(), " ", all.username.QuoteSafe(), " ", all.notes.QuoteSafe(), "\r\n" });
		}
		File.WriteAllText(string.Concat(serverFolder, "/bans.cfg"), str);
		string str1 = "";
		foreach (ServerUsers.User user in ServerUsers.GetAll(ServerUsers.UserGroup.Owner))
		{
			str1 = string.Concat(new string[] { str1, "ownerid ", user.steamid.ToString(), " ", user.username.QuoteSafe(), " ", user.notes.QuoteSafe(), "\r\n" });
		}
		foreach (ServerUsers.User all1 in ServerUsers.GetAll(ServerUsers.UserGroup.Moderator))
		{
			str1 = string.Concat(new string[] { str1, "moderatorid ", all1.steamid.ToString(), " ", all1.username.QuoteSafe(), " ", all1.notes.QuoteSafe(), "\r\n" });
		}
		File.WriteAllText(string.Concat(serverFolder, "/users.cfg"), str1);
	}

	public static void Set(ulong uid, ServerUsers.UserGroup group, string username, string notes)
	{
		ServerUsers.Remove(uid);
		ServerUsers.User user = new ServerUsers.User()
		{
			steamid = uid,
			@group = group,
			username = username,
			notes = notes
		};
		Interface.CallHook("IOnServerUsersSet", uid, group, username, notes);
		ServerUsers.users.Add(uid, user);
	}

	public class User
	{
		public ulong steamid;

		[JsonConverter(typeof(StringEnumConverter))]
		public ServerUsers.UserGroup @group;

		public string username;

		public string notes;

		public User()
		{
		}
	}

	public enum UserGroup
	{
		None,
		Owner,
		Moderator,
		Banned
	}
}