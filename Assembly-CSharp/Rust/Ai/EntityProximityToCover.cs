using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class EntityProximityToCover : BaseScorer
	{
		[ApexSerialization]
		public float MaxDistance = 20f;

		[ApexSerialization]
		public ProximityToCover.CoverType _coverType;

		[ApexSerialization]
		public AnimationCurve Response = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		public EntityProximityToCover()
		{
		}

		public override float GetScore(BaseContext ctx)
		{
			float single;
			NPCHumanContext nPCHumanContext = ctx as NPCHumanContext;
			if (nPCHumanContext != null)
			{
				CoverPoint closestCover = ProximityToCover.GetClosestCover(nPCHumanContext, nPCHumanContext.Position, this.MaxDistance, this._coverType, out single);
				if (closestCover != null)
				{
					return this.Response.Evaluate(single / this.MaxDistance) * closestCover.Score;
				}
			}
			return 0f;
		}
	}
}