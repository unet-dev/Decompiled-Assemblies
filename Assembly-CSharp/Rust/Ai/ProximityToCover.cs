using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class ProximityToCover : WeightedScorerBase<Vector3>
	{
		[ApexSerialization]
		public float MaxDistance = 20f;

		[ApexSerialization]
		public ProximityToCover.CoverType _coverType;

		[ApexSerialization]
		public AnimationCurve Response = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		public ProximityToCover()
		{
		}

		internal static CoverPoint GetClosestCover(NPCHumanContext c, Vector3 point, float MaxDistance, ProximityToCover.CoverType _coverType, out float bestDistance)
		{
			bestDistance = MaxDistance;
			CoverPoint coverPoint = null;
			for (int i = 0; i < c.sampledCoverPoints.Count; i++)
			{
				CoverPoint item = c.sampledCoverPoints[i];
				CoverPoint.CoverType coverType = c.sampledCoverPointTypes[i];
				if ((_coverType != ProximityToCover.CoverType.Full || coverType == CoverPoint.CoverType.Full) && (_coverType != ProximityToCover.CoverType.Partial || coverType == CoverPoint.CoverType.Partial))
				{
					float single = Vector3.Distance(item.Position, point);
					if (single < bestDistance)
					{
						bestDistance = single;
						coverPoint = item;
					}
				}
			}
			return coverPoint;
		}

		public override float GetScore(BaseContext ctx, Vector3 option)
		{
			float single;
			NPCHumanContext nPCHumanContext = ctx as NPCHumanContext;
			if (nPCHumanContext != null)
			{
				CoverPoint closestCover = ProximityToCover.GetClosestCover(nPCHumanContext, option, this.MaxDistance, this._coverType, out single);
				if (closestCover != null)
				{
					return this.Response.Evaluate(single / this.MaxDistance) * closestCover.Score;
				}
			}
			return 0f;
		}

		public enum CoverType
		{
			All,
			Full,
			Partial
		}
	}
}