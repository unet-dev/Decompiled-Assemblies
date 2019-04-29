using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ConsoleNetwork
{
	public static void BroadcastToAllClients(string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		Network.Net.sv.write.Start();
		Network.Net.sv.write.PacketID(Message.Type.ConsoleCommand);
		Network.Net.sv.write.String(ConsoleSystem.BuildCommand(strCommand, args));
		Network.Net.sv.write.Send(new SendInfo(Network.Net.sv.connections));
	}

	internal static void Init()
	{
	}

	internal static void OnClientCommand(Message packet)
	{
		if (packet.read.unread > ConVar.Server.maxcommandpacketsize)
		{
			Debug.LogWarning("Dropping client command due to size");
			return;
		}
		string str = packet.read.String();
		if (packet.connection == null || !packet.connection.connected)
		{
			Debug.LogWarning(string.Concat("Client without connection tried to run command: ", str));
			return;
		}
		ConsoleSystem.Option server = ConsoleSystem.Option.Server;
		server = server.FromConnection(packet.connection);
		string str1 = ConsoleSystem.Run(server.Quiet(), str, Array.Empty<object>());
		if (!string.IsNullOrEmpty(str1))
		{
			ConsoleNetwork.SendClientReply(packet.connection, str1);
		}
	}

	public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		Network.Net.sv.write.Start();
		Network.Net.sv.write.PacketID(Message.Type.ConsoleCommand);
		Network.Net.sv.write.String(ConsoleSystem.BuildCommand(strCommand, args));
		Network.Net.sv.write.Send(new SendInfo(cn));
	}

	public static void SendClientCommand(List<Connection> cn, string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		Network.Net.sv.write.Start();
		Network.Net.sv.write.PacketID(Message.Type.ConsoleCommand);
		Network.Net.sv.write.String(ConsoleSystem.BuildCommand(strCommand, args));
		Network.Net.sv.write.Send(new SendInfo(cn));
	}

	internal static void SendClientReply(Connection cn, string strCommand)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		Network.Net.sv.write.Start();
		Network.Net.sv.write.PacketID(Message.Type.ConsoleMessage);
		Network.Net.sv.write.String(strCommand);
		Network.Net.sv.write.Send(new SendInfo(cn));
	}
}