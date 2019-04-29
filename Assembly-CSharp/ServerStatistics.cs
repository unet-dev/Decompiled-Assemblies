using System;
using System.Collections.Generic;

public class ServerStatistics
{
	private BasePlayer player;

	private ServerStatistics.Storage storage;

	private static Dictionary<ulong, ServerStatistics.Storage> players;

	static ServerStatistics()
	{
		ServerStatistics.players = new Dictionary<ulong, ServerStatistics.Storage>();
	}

	public ServerStatistics(BasePlayer player)
	{
		this.player = player;
	}

	public void Add(string name, int val)
	{
		if (this.storage != null)
		{
			this.storage.Add(name, val);
		}
	}

	public static ServerStatistics.Storage Get(ulong id)
	{
		ServerStatistics.Storage storage;
		if (ServerStatistics.players.TryGetValue(id, out storage))
		{
			return storage;
		}
		storage = new ServerStatistics.Storage();
		ServerStatistics.players.Add(id, storage);
		return storage;
	}

	public void Init()
	{
		this.storage = ServerStatistics.Get(this.player.userID);
	}

	public void Save()
	{
	}

	public class Storage
	{
		private Dictionary<string, int> dict;

		public Storage()
		{
		}

		public void Add(string name, int val)
		{
			if (!this.dict.ContainsKey(name))
			{
				this.dict.Add(name, val);
				return;
			}
			Dictionary<string, int> item = this.dict;
			string str = name;
			item[str] = item[str] + val;
		}

		public int Get(string name)
		{
			int num;
			this.dict.TryGetValue(name, out num);
			return num;
		}
	}
}