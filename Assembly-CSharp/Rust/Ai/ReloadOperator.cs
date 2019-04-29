using System;

namespace Rust.Ai
{
	public class ReloadOperator : BaseAction
	{
		public ReloadOperator()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			ReloadOperator.Reload(c as NPCHumanContext);
		}

		public static void Reload(NPCHumanContext c)
		{
			if (c == null)
			{
				return;
			}
			AttackEntity heldEntity = c.Human.GetHeldEntity() as AttackEntity;
			if (heldEntity == null)
			{
				return;
			}
			BaseProjectile baseProjectile = heldEntity as BaseProjectile;
			if (baseProjectile && baseProjectile.primaryMagazine.CanAiReload(c.Human))
			{
				baseProjectile.ServerReload();
				if (c.Human.OnReload != null)
				{
					c.Human.OnReload();
				}
			}
		}
	}
}