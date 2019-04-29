using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class BestPlayerDistance : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		public BestPlayerDistance()
		{
		}

		public static void Evaluate(IAIAgent self, Vector3 optionPosition, out float distanceSqr, out float aggroRangeSqr)
		{
			Vector3 vector3 = optionPosition - self.Entity.ServerPosition;
			aggroRangeSqr = self.GetActiveAggressionRangeSqr();
			distanceSqr = Mathf.Min(vector3.sqrMagnitude, aggroRangeSqr);
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			float single;
			float single1;
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext == null)
			{
				playerTargetContext.DistanceSqr[playerTargetContext.CurrentOptionsIndex] = -1f;
				return 0f;
			}
			BestPlayerDistance.Evaluate(playerTargetContext.Self, option.ServerPosition, out single, out single1);
			playerTargetContext.DistanceSqr[playerTargetContext.CurrentOptionsIndex] = single;
			return (1f - single / single1) * this.score;
		}
	}
}