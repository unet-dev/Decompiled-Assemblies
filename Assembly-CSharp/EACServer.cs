using ConVar;
using EasyAntiCheat.Server;
using EasyAntiCheat.Server.Cerberus;
using EasyAntiCheat.Server.Hydra;
using EasyAntiCheat.Server.Hydra.NetProtect;
using EasyAntiCheat.Server.Scout;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EACServer
{
	public static ICerberus<EasyAntiCheat.Server.Hydra.Client> playerTracker;

	public static Scout eacScout;

	private static Dictionary<EasyAntiCheat.Server.Hydra.Client, Connection> client2connection;

	private static Dictionary<Connection, EasyAntiCheat.Server.Hydra.Client> connection2client;

	private static Dictionary<Connection, ClientStatus> connection2status;

	private static EasyAntiCheatServer<EasyAntiCheat.Server.Hydra.Client> easyAntiCheat;

	static EACServer()
	{
		EACServer.client2connection = new Dictionary<EasyAntiCheat.Server.Hydra.Client, Connection>();
		EACServer.connection2client = new Dictionary<Connection, EasyAntiCheat.Server.Hydra.Client>();
		EACServer.connection2status = new Dictionary<Connection, ClientStatus>();
		EACServer.easyAntiCheat = null;
	}

	public EACServer()
	{
	}

	public static void Decrypt(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		EACServer.easyAntiCheat.NetProtect.UnprotectMessage(EACServer.GetClient(connection), src, (long)srcOffset, dst, (long)dstOffset);
	}

	public static void DoShutdown()
	{
		EACServer.client2connection.Clear();
		EACServer.connection2client.Clear();
		EACServer.connection2status.Clear();
		if (EACServer.eacScout != null)
		{
			Debug.Log("EasyAntiCheat Scout Shutting Down");
			EACServer.eacScout.Dispose();
			EACServer.eacScout = null;
		}
		if (EACServer.easyAntiCheat != null)
		{
			Debug.Log("EasyAntiCheat Server Shutting Down");
			EACServer.easyAntiCheat.Dispose();
			EACServer.easyAntiCheat = null;
		}
	}

	public static void DoStartup()
	{
		EACServer.client2connection.Clear();
		EACServer.connection2client.Clear();
		EACServer.connection2status.Clear();
		Log.SetOut(new StreamWriter(string.Concat(ConVar.Server.rootFolder, "/Log.EAC.txt"), false)
		{
			AutoFlush = true
		});
		Log.Prefix = "";
		Log.Level = EasyAntiCheat.Server.LogLevel.Info;
		EACServer.easyAntiCheat = new EasyAntiCheatServer<EasyAntiCheat.Server.Hydra.Client>(new EasyAntiCheatServer<EasyAntiCheat.Server.Hydra.Client>.ClientStatusHandler(EACServer.HandleClientUpdate), 20, ConVar.Server.hostname);
		EACServer.playerTracker = EACServer.easyAntiCheat.Cerberus;
		EACServer.playerTracker.LogGameRoundStart(World.Name, string.Empty, 0);
		EACServer.eacScout = new Scout();
	}

	public static void DoUpdate()
	{
		EasyAntiCheat.Server.Hydra.Client client;
		byte[] numArray;
		int num;
		if (EACServer.easyAntiCheat != null)
		{
			EACServer.easyAntiCheat.HandleClientUpdates();
			if (Network.Net.sv != null && Network.Net.sv.IsConnected())
			{
				while (EACServer.easyAntiCheat.PopNetworkMessage(out client, out numArray, out num))
				{
					EACServer.SendToClient(client, numArray, num);
				}
			}
		}
	}

	public static void Encrypt(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		EACServer.easyAntiCheat.NetProtect.ProtectMessage(EACServer.GetClient(connection), src, (long)srcOffset, dst, (long)dstOffset);
	}

	public static EasyAntiCheat.Server.Hydra.Client GetClient(Connection connection)
	{
		EasyAntiCheat.Server.Hydra.Client client;
		EACServer.connection2client.TryGetValue(connection, out client);
		return client;
	}

	public static Connection GetConnection(EasyAntiCheat.Server.Hydra.Client client)
	{
		Connection connection;
		EACServer.client2connection.TryGetValue(client, out connection);
		return connection;
	}

	private static void HandleClientUpdate(ClientStatusUpdate<EasyAntiCheat.Server.Hydra.Client> clientStatus)
	{
		using (TimeWarning timeWarning = TimeWarning.New("AntiCheatKickPlayer", (long)10))
		{
			EasyAntiCheat.Server.Hydra.Client client = clientStatus.Client;
			Connection connection = EACServer.GetConnection(client);
			if (connection == null)
			{
				Debug.LogError(string.Concat("EAC status update for invalid client: ", client.ClientID));
			}
			else if (!EACServer.ShouldIgnore(connection))
			{
				if (clientStatus.RequiresKick)
				{
					string message = clientStatus.Message;
					if (string.IsNullOrEmpty(message))
					{
						message = clientStatus.Status.ToString();
					}
					Debug.Log(string.Concat(new object[] { "[EAC] Kicking ", connection.userid, " (", message, ")" }));
					connection.authStatus = "eac";
					Network.Net.sv.Kick(connection, string.Concat("EAC: ", message));
					DateTime? nullable = null;
					if (clientStatus.IsBanned(out nullable))
					{
						connection.authStatus = "eacbanned";
						Interface.CallHook("OnPlayerBanned", connection, connection.authStatus);
						ConsoleNetwork.BroadcastToAllClients("chat.add", new object[] { 0, string.Concat("<color=#fff>SERVER</color> Kicking ", connection.username, " (banned by anticheat)") });
						if (!nullable.HasValue)
						{
							Entity.DeleteBy(connection.userid);
						}
					}
					EACServer.easyAntiCheat.UnregisterClient(client);
					EACServer.client2connection.Remove(client);
					EACServer.connection2client.Remove(connection);
					EACServer.connection2status.Remove(connection);
				}
				else if (clientStatus.Status == ClientStatus.ClientAuthenticatedLocal)
				{
					EACServer.OnAuthenticatedLocal(connection);
					EACServer.easyAntiCheat.SetClientNetworkState(client, false);
				}
				else if (clientStatus.Status == ClientStatus.ClientAuthenticatedRemote)
				{
					EACServer.OnAuthenticatedRemote(connection);
				}
			}
		}
	}

	public static bool IsAuthenticated(Connection connection)
	{
		ClientStatus clientStatu;
		EACServer.connection2status.TryGetValue(connection, out clientStatu);
		return clientStatu == ClientStatus.ClientAuthenticatedRemote;
	}

	private static void OnAuthenticatedLocal(Connection connection)
	{
		if (connection.authStatus == string.Empty)
		{
			connection.authStatus = "ok";
		}
		EACServer.connection2status[connection] = ClientStatus.ClientAuthenticatedLocal;
	}

	private static void OnAuthenticatedRemote(Connection connection)
	{
		EACServer.connection2status[connection] = ClientStatus.ClientAuthenticatedRemote;
	}

	public static void OnFinishLoading(Connection connection)
	{
		if (EACServer.easyAntiCheat != null)
		{
			EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(connection);
			EACServer.easyAntiCheat.SetClientNetworkState(client, true);
		}
	}

	public static void OnJoinGame(Connection connection)
	{
		if (EACServer.easyAntiCheat == null)
		{
			EACServer.OnAuthenticatedLocal(connection);
			EACServer.OnAuthenticatedRemote(connection);
		}
		else
		{
			EasyAntiCheat.Server.Hydra.Client client = EACServer.easyAntiCheat.GenerateCompatibilityClient();
			EACServer.easyAntiCheat.RegisterClient(client, connection.userid.ToString(), connection.ipaddress, connection.ownerid.ToString(), connection.username, (connection.authLevel > 0 ? PlayerRegisterFlags.PlayerRegisterFlagAdmin : PlayerRegisterFlags.PlayerRegisterFlagNone));
			EACServer.client2connection.Add(client, connection);
			EACServer.connection2client.Add(connection, client);
			EACServer.connection2status.Add(connection, ClientStatus.ClientDisconnected);
			if (EACServer.ShouldIgnore(connection))
			{
				EACServer.OnAuthenticatedLocal(connection);
				EACServer.OnAuthenticatedRemote(connection);
				return;
			}
		}
	}

	public static void OnLeaveGame(Connection connection)
	{
		if (EACServer.easyAntiCheat != null)
		{
			EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(connection);
			EACServer.easyAntiCheat.UnregisterClient(client);
			EACServer.client2connection.Remove(client);
			EACServer.connection2client.Remove(connection);
			EACServer.connection2status.Remove(connection);
		}
	}

	public static void OnMessageReceived(Message message)
	{
		if (!EACServer.connection2client.ContainsKey(message.connection))
		{
			Debug.LogError(string.Concat("EAC network packet from invalid connection: ", message.connection.userid));
			return;
		}
		EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(message.connection);
		MemoryStream memoryStream = message.read.MemoryStreamWithSize();
		EACServer.easyAntiCheat.PushNetworkMessage(client, memoryStream.GetBuffer(), (int)memoryStream.Length);
	}

	public static void OnStartLoading(Connection connection)
	{
		if (EACServer.easyAntiCheat != null)
		{
			EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(connection);
			EACServer.easyAntiCheat.SetClientNetworkState(client, false);
		}
	}

	private static void SendToClient(EasyAntiCheat.Server.Hydra.Client client, byte[] message, int messageLength)
	{
		Connection connection = EACServer.GetConnection(client);
		if (connection == null)
		{
			Debug.LogError(string.Concat("EAC network packet for invalid client: ", client.ClientID));
			return;
		}
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.EAC);
			Network.Net.sv.write.UInt32((uint)messageLength);
			Network.Net.sv.write.Write(message, 0, messageLength);
			Network.Net.sv.write.Send(new SendInfo(connection));
		}
	}

	public static bool ShouldIgnore(Connection connection)
	{
		if (!ConVar.Server.secure)
		{
			return true;
		}
		if (connection.authLevel >= 3)
		{
			return true;
		}
		return false;
	}
}