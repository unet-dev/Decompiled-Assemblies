using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionQueue
{
	public List<Connection> queue = new List<Connection>();

	public List<Connection> joining = new List<Connection>();

	public float nextMessageTime;

	public int Joining
	{
		get
		{
			return this.joining.Count;
		}
	}

	public int Queued
	{
		get
		{
			return this.queue.Count;
		}
	}

	public ConnectionQueue()
	{
	}

	private bool CanJumpQueue(Connection connection)
	{
		object obj = Interface.CallHook("CanBypassQueue", connection);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (DeveloperList.Contains(connection.userid))
		{
			return true;
		}
		ServerUsers.User user = ServerUsers.Get(connection.userid);
		if (user != null && user.@group == ServerUsers.UserGroup.Moderator)
		{
			return true;
		}
		if (user != null && user.@group == ServerUsers.UserGroup.Owner)
		{
			return true;
		}
		return false;
	}

	public void Cycle(int availableSlots)
	{
		if (this.queue.Count == 0)
		{
			return;
		}
		if (availableSlots - this.Joining > 0)
		{
			this.JoinGame(this.queue[0]);
		}
		this.SendMessages();
	}

	internal void Join(Connection connection)
	{
		connection.state = Connection.State.InQueue;
		this.queue.Add(connection);
		this.nextMessageTime = 0f;
		if (this.CanJumpQueue(connection))
		{
			this.JoinGame(connection);
		}
	}

	public void JoinedGame(Connection connection)
	{
		this.RemoveConnection(connection);
	}

	private void JoinGame(Connection connection)
	{
		this.queue.Remove(connection);
		connection.state = Connection.State.Welcoming;
		this.nextMessageTime = 0f;
		this.joining.Add(connection);
		SingletonComponent<ServerMgr>.Instance.JoinGame(connection);
	}

	public void RemoveConnection(Connection connection)
	{
		if (this.queue.Remove(connection))
		{
			this.nextMessageTime = 0f;
		}
		this.joining.Remove(connection);
	}

	private void SendMessage(Connection c, int position)
	{
		string empty = string.Empty;
		empty = (position <= 0 ? string.Format("YOU'RE NEXT - {1:N0} PLAYERS BEHIND YOU", position, this.queue.Count - position - 1) : string.Format("{0:N0} PLAYERS AHEAD OF YOU, {1:N0} PLAYERS BEHIND", position, this.queue.Count - position - 1));
		if (Net.sv.write.Start())
		{
			Net.sv.write.PacketID(Message.Type.Message);
			Net.sv.write.String("QUEUE");
			Net.sv.write.String(empty);
			Net.sv.write.Send(new SendInfo(c));
		}
	}

	private void SendMessages()
	{
		if (this.nextMessageTime > Time.realtimeSinceStartup)
		{
			return;
		}
		this.nextMessageTime = Time.realtimeSinceStartup + 10f;
		for (int i = 0; i < this.queue.Count; i++)
		{
			this.SendMessage(this.queue[i], i);
		}
	}

	public void SkipQueue(ulong userid)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			Connection item = this.queue[i];
			if (item.userid == userid)
			{
				this.JoinGame(item);
				return;
			}
		}
	}
}