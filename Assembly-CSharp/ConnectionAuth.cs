using Network;
using Oxide.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConnectionAuth : MonoBehaviour
{
	[NonSerialized]
	public static List<Connection> m_AuthConnection;

	static ConnectionAuth()
	{
		ConnectionAuth.m_AuthConnection = new List<Connection>();
	}

	public ConnectionAuth()
	{
	}

	public void Approve(Connection connection)
	{
		ConnectionAuth.m_AuthConnection.Remove(connection);
		SingletonComponent<ServerMgr>.Instance.connectionQueue.Join(connection);
	}

	public IEnumerator AuthorisationRoutine(Connection connection)
	{
		ConnectionAuth connectionAuth = null;
		yield return connectionAuth.StartCoroutine(Auth_Steam.Run(connection));
		yield return connectionAuth.StartCoroutine(Auth_EAC.Run(connection));
		if (connection.rejected || !connection.active)
		{
			yield break;
		}
		BasePlayer basePlayer = BasePlayer.FindByID(connection.userid);
		if (basePlayer && basePlayer.net.connection != null)
		{
			ConnectionAuth.Reject(connection, "You are already connected as a player!");
			yield break;
		}
		connectionAuth.Approve(connection);
	}

	public bool IsConnected(ulong iSteamID)
	{
		if (BasePlayer.FindByID(iSteamID))
		{
			return true;
		}
		return ConnectionAuth.m_AuthConnection.Any<Connection>((Connection item) => item.userid == iSteamID);
	}

	public static void OnDisconnect(Connection connection)
	{
		ConnectionAuth.m_AuthConnection.Remove(connection);
	}

	public void OnNewConnection(Connection connection)
	{
		connection.connected = false;
		if (connection.token == null || (int)connection.token.Length < 32)
		{
			ConnectionAuth.Reject(connection, "Invalid Token");
			return;
		}
		if (connection.userid == 0)
		{
			ConnectionAuth.Reject(connection, "Invalid SteamID");
			return;
		}
		if (connection.protocol != 2163)
		{
			if (!DeveloperList.Contains(connection.userid))
			{
				ConnectionAuth.Reject(connection, "Incompatible Version");
				return;
			}
			DebugEx.Log(string.Concat("Not kicking ", connection.userid, " for incompatible protocol (is a developer)"), StackTraceLogType.None);
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Banned))
		{
			ConnectionAuth.Reject(connection, "You are banned from this server");
			return;
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Moderator))
		{
			DebugEx.Log(string.Concat(connection.ToString(), " has auth level 1"), StackTraceLogType.None);
			connection.authLevel = 1;
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Owner))
		{
			DebugEx.Log(string.Concat(connection.ToString(), " has auth level 2"), StackTraceLogType.None);
			connection.authLevel = 2;
		}
		if (DeveloperList.Contains(connection.userid))
		{
			DebugEx.Log(string.Concat(connection.ToString(), " is a developer"), StackTraceLogType.None);
			connection.authLevel = 3;
		}
		if (this.IsConnected(connection.userid))
		{
			ConnectionAuth.Reject(connection, "You are already connected!");
			return;
		}
		if (Interface.CallHook("IOnUserApprove", connection) != null)
		{
			return;
		}
		ConnectionAuth.m_AuthConnection.Add(connection);
		base.StartCoroutine(this.AuthorisationRoutine(connection));
	}

	public static void Reject(Connection connection, string strReason)
	{
		DebugEx.Log(string.Concat(connection.ToString(), " Rejecting connection - ", strReason), StackTraceLogType.None);
		Net.sv.Kick(connection, strReason);
		ConnectionAuth.m_AuthConnection.Remove(connection);
	}
}