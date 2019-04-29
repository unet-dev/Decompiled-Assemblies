using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class IsClosestPlayerWithinDistance : BaseScorer
	{
		[ApexSerialization]
		private float distance = 4f;

		public IsClosestPlayerWithinDistance()
		{
		}

		public override float GetScore(BaseContext ctx)
		{
			NPCHumanContext nPCHumanContext = ctx as NPCHumanContext;
			if (nPCHumanContext == null)
			{
				return 0f;
			}
			if (!IsClosestPlayerWithinDistance.Test(nPCHumanContext, this.distance))
			{
				return 0f;
			}
			return 1f;
		}

		public static bool Test(NPCHumanContext c, float distance)
		{
			if (c == null || !(c.ClosestPlayer != null))
			{
				return false;
			}
			Vector3 serverPosition = c.ClosestPlayer.ServerPosition - c.Position;
			return serverPosition.sqrMagnitude < distance * distance;
		}
	}
}