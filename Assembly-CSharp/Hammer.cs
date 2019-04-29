using Oxide.Core;
using System;

public class Hammer : BaseMelee
{
	public Hammer()
	{
	}

	public override bool CanHit(HitTest info)
	{
		if (info.HitEntity == null)
		{
			return false;
		}
		if (info.HitEntity is BasePlayer)
		{
			return false;
		}
		return info.HitEntity is BaseCombatEntity;
	}

	public override void DoAttackShared(HitInfo info)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		BaseCombatEntity hitEntity = info.HitEntity as BaseCombatEntity;
		if (hitEntity != null)
		{
			if (Interface.CallHook("OnHammerHit", ownerPlayer, info) != null)
			{
				return;
			}
			if (ownerPlayer != null && base.isServer)
			{
				using (TimeWarning timeWarning = TimeWarning.New("DoRepair", (long)50))
				{
					hitEntity.DoRepair(ownerPlayer);
				}
			}
		}
		if (base.isServer)
		{
			Effect.server.ImpactEffect(info);
			return;
		}
		Effect.client.ImpactEffect(info);
	}
}