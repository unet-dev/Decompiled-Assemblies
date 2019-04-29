using System;

public class PlayerStatistics
{
	public SteamStatistics steam;

	public ServerStatistics server;

	public CombatLog combat;

	public PlayerStatistics(BasePlayer player)
	{
		this.steam = new SteamStatistics(player);
		this.server = new ServerStatistics(player);
		this.combat = new CombatLog(player);
	}

	public void Add(string name, int val, Stats stats = 1)
	{
		if ((int)(stats & Stats.Steam) != 0)
		{
			this.steam.Add(name, val);
		}
		if ((int)(stats & Stats.Server) != 0)
		{
			this.server.Add(name, val);
		}
	}

	public void Init()
	{
		this.steam.Init();
		this.server.Init();
		this.combat.Init();
	}

	public void Save()
	{
		this.steam.Save();
		this.server.Save();
		this.combat.Save();
	}
}