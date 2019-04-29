using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class BestPlayerDirection : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		public BestPlayerDirection()
		{
		}

		public static bool Evaluate(IAIAgent self, Vector3 optionPosition, out Vector3 dir, out float dot)
		{
			dir = optionPosition - self.Entity.ServerPosition;
			NPCPlayerApex nPCPlayerApex = self as NPCPlayerApex;
			if (nPCPlayerApex == null)
			{
				dir.Normalize();
				dot = Vector3.Dot(dir, self.Entity.transform.forward);
				if (dot < self.GetStats.VisionCone)
				{
					dot = -1f;
					return false;
				}
			}
			else
			{
				if (nPCPlayerApex.ToEnemyRangeEnum(dir.sqrMagnitude) == NPCPlayerApex.EnemyRangeEnum.CloseAttackRange)
				{
					dot = 1f;
					dir.Normalize();
					return true;
				}
				dir.Normalize();
				dot = Vector3.Dot(dir, nPCPlayerApex.eyes.BodyForward());
				if (dot < self.GetStats.VisionCone)
				{
					dot = -1f;
					return false;
				}
			}
			return true;
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			Vector3 vector3;
			float single;
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext == null || !BestPlayerDirection.Evaluate(playerTargetContext.Self, option.ServerPosition, out vector3, out single))
			{
				playerTargetContext.Direction[playerTargetContext.CurrentOptionsIndex] = Vector3.zero;
				playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex] = -1f;
				return 0f;
			}
			playerTargetContext.Direction[playerTargetContext.CurrentOptionsIndex] = vector3;
			playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex] = single;
			return (single + 1f) * 0.5f * this.score;
		}
	}
}