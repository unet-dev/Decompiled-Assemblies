using System;
using UnityEngine;

namespace Rust.Ai
{
	public class InMountRangeOfChair : BaseScorer
	{
		public InMountRangeOfChair()
		{
		}

		public override float GetScore(BaseContext context)
		{
			return InMountRangeOfChair.Test(context as NPCHumanContext);
		}

		private static float IsInRange(NPCHumanContext c, BaseMountable mountable)
		{
			Vector3 vector3 = mountable.transform.position - c.Position;
			if (vector3.y > mountable.maxMountDistance)
			{
				vector3.y -= mountable.maxMountDistance;
			}
			if (vector3.sqrMagnitude <= mountable.maxMountDistance * mountable.maxMountDistance)
			{
				return 1f;
			}
			return 0f;
		}

		public static float Test(NPCHumanContext c)
		{
			if (c.ChairTarget == null)
			{
				return 0f;
			}
			return InMountRangeOfChair.IsInRange(c, c.ChairTarget);
		}
	}
}