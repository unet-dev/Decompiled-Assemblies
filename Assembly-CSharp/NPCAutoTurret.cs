using System;
using UnityEngine;

public class NPCAutoTurret : AutoTurret
{
	public Transform centerMuzzle;

	public Transform muzzleLeft;

	public Transform muzzleRight;

	private bool useLeftMuzzle;

	[ServerVar(Help="How many seconds until a sleeping player is considered hostile")]
	public static float sleeperhostiledelay;

	static NPCAutoTurret()
	{
		NPCAutoTurret.sleeperhostiledelay = 1200f;
	}

	public NPCAutoTurret()
	{
	}

	public override bool CheckPeekers()
	{
		return false;
	}

	public override void FireGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		muzzleToUse = this.muzzleRight;
		base.FireGun(targetPos, aimCone, muzzleToUse, target);
	}

	public override Transform GetCenterMuzzle()
	{
		return this.centerMuzzle;
	}

	public override bool HasAmmo()
	{
		return true;
	}

	protected override bool Ignore(BasePlayer player)
	{
		return player as Scientist;
	}

	public override bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return true;
	}

	public override bool IsEntityHostile(BaseCombatEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer.IsNpc)
			{
				if (basePlayer is Scientist)
				{
					return false;
				}
				return true;
			}
			if (basePlayer.IsSleeping() && basePlayer.secondsSleeping >= NPCAutoTurret.sleeperhostiledelay)
			{
				return true;
			}
		}
		return base.IsEntityHostile(ent);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.SetOnline();
		base.SetPeacekeepermode(true);
	}

	public override float TargetScanRate()
	{
		return 1.25f;
	}
}