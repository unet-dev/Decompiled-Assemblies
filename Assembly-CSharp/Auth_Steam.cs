using Network;
using Steamworks;
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
		if (!SteamServer.BeginAuthSession(connection.token, connection.userid))
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
			SteamServer.EndSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "banned")
		{
			ConnectionAuth.Reject(connection, string.Concat("Auth: ", connection.authStatus));
			SteamServer.EndSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "gamebanned")
		{
			ConnectionAuth.Reject(connection, string.Concat("Steam Auth: ", connection.authStatus));
			SteamServer.EndSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "vacbanned")
		{
			ConnectionAuth.Reject(connection, string.Concat("Steam Auth: ", connection.authStatus));
			SteamServer.EndSession(connection.userid);
			yield break;
		}
		if (connection.authStatus == "ok")
		{
			SteamServer.UpdatePlayer(connection.userid, connection.username, 0);
			yield break;
		}
		ConnectionAuth.Reject(connection, string.Concat("Steam Auth Error: ", connection.authStatus));
		SteamServer.EndSession(connection.userid);
	}

	public static bool ValidateConnecting(SteamId steamid, SteamId ownerSteamID, AuthResponse response)
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
		if (response == AuthResponse.OK)
		{
			str.authStatus = "ok";
			return true;
		}
		if (response == AuthResponse.VACBanned)
		{
			str.authStatus = "vacbanned";
			return true;
		}
		if (response == AuthResponse.PublisherIssuedBan)
		{
			str.authStatus = "gamebanned";
			return true;
		}
		if (response == AuthResponse.VACCheckTimedOut)
		{
			str.authStatus = "ok";
			return true;
		}
		str.authStatus = response.ToString();
		return true;
	}
}