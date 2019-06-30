using ConVar;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SteamStatistics
{
	private BasePlayer player;

	public Dictionary<string, int> intStats = new Dictionary<string, int>();

	private Task refresh;

	public SteamStatistics(BasePlayer p)
	{
		this.player = p;
	}

	public void Add(string name, int var)
	{
		if (!SteamServer.IsValid)
		{
			return;
		}
		if (this.refresh == null || !this.refresh.IsCompleted)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("PlayerStats.Add", 0.1f))
		{
			int num = 0;
			if (!this.intStats.TryGetValue(name, out num))
			{
				num = SteamServerStats.GetInt(this.player.userID, name, 0);
				if (SteamServerStats.SetInt(this.player.userID, name, num + var))
				{
					this.intStats.Add(name, num + var);
				}
				else if (Global.developer > 0)
				{
					Debug.LogWarning(string.Concat("[STEAMWORKS] Couldn't SetUserStat: ", name));
					return;
				}
			}
			else
			{
				Dictionary<string, int> item = this.intStats;
				string str = name;
				item[str] = item[str] + var;
				SteamServerStats.SetInt(this.player.userID, name, this.intStats[name]);
			}
		}
	}

	public void Init()
	{
		if (!SteamServer.IsValid)
		{
			return;
		}
		this.refresh = SteamServerStats.RequestUserStats(this.player.userID);
		this.intStats.Clear();
	}

	public void Save()
	{
		if (!SteamServer.IsValid)
		{
			return;
		}
		SteamServerStats.StoreUserStats(this.player.userID);
	}
}