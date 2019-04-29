using System;

namespace Rust.Ai
{
	public sealed class ReloadWeapon : BaseAction
	{
		public ReloadWeapon()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			BasePlayer aIAgent = c.AIAgent as BasePlayer;
			if (aIAgent != null)
			{
				AttackEntity heldEntity = aIAgent.GetHeldEntity() as AttackEntity;
				if (heldEntity != null)
				{
					BaseProjectile baseProjectile = heldEntity as BaseProjectile;
					if (baseProjectile)
					{
						baseProjectile.ServerReload();
					}
				}
			}
		}
	}
}