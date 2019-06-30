using System;

public class BaseAnimalNPC : BaseNpc
{
	public string deathStatName = "";

	public BaseAnimalNPC()
	{
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (hitInfo != null)
		{
			BasePlayer initiatorPlayer = hitInfo.InitiatorPlayer;
			if (initiatorPlayer != null)
			{
				initiatorPlayer.GiveAchievement("KILL_ANIMAL");
				if (!string.IsNullOrEmpty(this.deathStatName))
				{
					initiatorPlayer.stats.Add(this.deathStatName, 1, Stats.Steam);
					initiatorPlayer.stats.Save();
				}
			}
		}
		base.OnKilled(null);
	}
}