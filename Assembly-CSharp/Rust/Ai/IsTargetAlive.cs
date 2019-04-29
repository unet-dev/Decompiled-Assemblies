using System;
using UnityEngine;

namespace Rust.Ai
{
	public class IsTargetAlive : BaseScorer
	{
		public IsTargetAlive()
		{
		}

		public override float GetScore(BaseContext ctx)
		{
			NPCHumanContext nPCHumanContext = ctx as NPCHumanContext;
			if (nPCHumanContext == null)
			{
				return 0f;
			}
			if (!IsTargetAlive.Test(nPCHumanContext))
			{
				return 0f;
			}
			return 1f;
		}

		public static bool Test(NPCHumanContext c)
		{
			if (!(c.Human.AttackTarget != null) || c.Human.AttackTarget.IsDestroyed || (!(c.EnemyPlayer != null) || c.EnemyPlayer.IsDead()) && (!(c.EnemyNpc != null) || c.EnemyNpc.IsDead()))
			{
				return false;
			}
			Vector3 serverPosition = c.Human.AttackTarget.ServerPosition - c.Human.ServerPosition;
			return serverPosition.sqrMagnitude < c.Human.Stats.DeaggroRange * c.Human.Stats.DeaggroRange;
		}
	}
}