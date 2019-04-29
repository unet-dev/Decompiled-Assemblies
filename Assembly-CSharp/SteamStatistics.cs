using ConVar;
using Facepunch.Steamworks;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SteamStatistics
{
	private BasePlayer player;

	public Dictionary<string, int> intStats = new Dictionary<string, int>();

	private bool hasRefreshed;

	public SteamStatistics(BasePlayer p)
	{
		this.player = p;
	}

	public void Add(string name, int var)
	{
		if (Rust.Global.SteamServer == null)
		{
			return;
		}
		if (!this.hasRefreshed)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("PlayerStats.Add", 0.1f))
		{
			int num = 0;
			if (!this.intStats.TryGetValue(name, out num))
			{
				num = Rust.Global.SteamServer.Stats.GetInt(this.player.userID, name, 0);
				if (Rust.Global.SteamServer.Stats.SetInt(this.player.userID, name, num + var))
				{
					this.intStats.Add(name, num + var);
				}
				else if (ConVar.Global.developer > 0)
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
				Rust.Global.SteamServer.Stats.SetInt(this.player.userID, name, this.intStats[name]);
			}
		}
	}

	public void Init()
	{
		if (Rust.Global.SteamServer == null)
		{
			return;
		}
		Rust.Global.SteamServer.Stats.Refresh(this.player.userID, new Action<ulong, bool>(this.OnStatsRefreshed));
		this.intStats.Clear();
	}

	public void OnStatsRefreshed(ulong steamid, bool state)
	{
		this.hasRefreshed = true;
	}

	public void Save()
	{
		if (Rust.Global.SteamServer == null)
		{
			return;
		}
		Rust.Global.SteamServer.Stats.Commit(this.player.userID, null);
	}
}