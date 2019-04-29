using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class DistanceFromSpawnToTargetInRange : BaseScorer
	{
		[ApexSerialization]
		private NPCPlayerApex.EnemyRangeEnum range;

		public DistanceFromSpawnToTargetInRange()
		{
		}

		public static bool Evaluate(NPCHumanContext c, NPCPlayerApex.EnemyRangeEnum range)
		{
			if (c == null || c.Human.AttackTarget == null)
			{
				return false;
			}
			Memory.SeenInfo info = c.Memory.GetInfo(c.Human.AttackTarget);
			if (info.Entity == null)
			{
				return false;
			}
			float position = (info.Position - c.Human.SpawnPosition).sqrMagnitude;
			NPCPlayerApex.EnemyRangeEnum enemyRangeEnum = c.Human.ToEnemyRangeEnum(position);
			if (enemyRangeEnum == range)
			{
				return true;
			}
			if (enemyRangeEnum < range)
			{
				return true;
			}
			return false;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (DistanceFromSpawnToTargetInRange.Evaluate(c as NPCHumanContext, this.range))
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