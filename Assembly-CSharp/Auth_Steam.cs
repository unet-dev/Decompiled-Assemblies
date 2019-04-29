using Facepunch.Steamworks;
using Network;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Auth_Steam
{
	internal static List<Connection> waitingList;

	static Auth_Steam()
	{
		Auth_Steam.waitingList = new List<Connection>();
	}

	public static IEnumerator Run(Connection connection)
	{
		connection.authStatus = "";
		if (!Global.SteamServer.Auth.StartSession(connection.token, connection.userid))
		{
			ConnectionAuth.Reject(connection, "Steam Auth Failed");
			yield break;
		}
		Auth_Steam.waitingList.Add(connection);
		Stopwatch stopwatch = Stopwatch.StartNew();
		while (stopwatch.Elapsed.TotalSeconds < 30 && connection.active && !(connection.authStatus != ""))
		{
			yield return null;
		}
		Auth_Steam.waitingList.Remove(connection);
		if (!connection.active)
		{
			yield break;
		}
		if (connection.authStatus.Length == 0)
		{
			ConnectionAuth.Reject(connection, "Steam Auth Timeout");
			Global.SteamServer.Auth.EndSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "banned")
		{
			ConnectionAuth.Reject(connection, string.Concat("Auth: ", connection.authStatus));
			Global.SteamServer.Auth.EndSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "gamebanned")
		{
			ConnectionAuth.Reject(connection, string.Concat("Steam Auth: ", connection.authStatus));
			Global.SteamServer.Auth.EndSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "vacbanned")
		{
			ConnectionAuth.Reject(connection, string.Concat("Steam Auth: ", connection.authStatus));
			Global.SteamServer.Auth.EndSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "ok")
		{
			Global.SteamServer.UpdatePlayer(connection.userid, connection.username, 0);
			yield break;
		}
		ConnectionAuth.Reject(connection, string.Concat("Steam Auth Error: ", connection.authStatus));
		Global.SteamServer.Auth.EndSession(connection.userid);
	}

	public static bool ValidateConnecting(ulong steamid, ulong ownerSteamID, ServerAuth.Status response)
	{
		Connection str = Auth_Steam.waitingList.Find((Connection x) => x.userid == steamid);
		if (str == null)
		{
			return false;
		}
		str.ownerid = ownerSteamID;
		if (ServerUsers.Is(ownerSteamID, ServerUsers.UserGroup.Banned) || ServerUsers.Is(steamid, ServerUsers.UserGroup.Banned))
		{
			str.authStatus = "banned";
			return true;
		}
		if (response == ServerAuth.Status.OK)
		{
			str.authStatus = "ok";
			return true;
		}
		if (response == ServerAuth.Status.VACBanned)
		{
			str.authStatus = "vacbanned";
			return true;
		}
		if (response == ServerAuth.Status.PublisherIssuedBan)
		{
			str.authStatus = "gamebanned";
			return true;
		}
		if (response == ServerAuth.Status.VACCheckTimedOut)
		{
			str.authStatus = "ok";
			return true;
		}
		str.authStatus = response.ToString();
		return true;
	}
}