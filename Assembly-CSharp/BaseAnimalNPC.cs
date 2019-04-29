using System;

public class BaseAnimalNPC : BaseNpc
{
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
			}
		}
		base.OnKilled(null);
	}
}