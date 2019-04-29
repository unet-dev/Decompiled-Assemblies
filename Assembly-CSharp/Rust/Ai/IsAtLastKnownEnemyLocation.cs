using System;
using UnityEngine;

namespace Rust.Ai
{
	public class IsAtLastKnownEnemyLocation : BaseScorer
	{
		public IsAtLastKnownEnemyLocation()
		{
		}

		public static bool Evaluate(NPCHumanContext c)
		{
			if (c.AIAgent.AttackTarget != null && c.AIAgent.IsNavRunning())
			{
				Memory.SeenInfo info = c.Memory.GetInfo(c.AIAgent.AttackTarget);
				if (info.Entity != null)
				{
					return (info.Position - c.Position).sqrMagnitude < 4f;
				}
			}
			return false;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (IsAtLastKnownEnemyLocation.Evaluate(c as NPCHumanContext))
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			return (float)obj;
		}
	}
}