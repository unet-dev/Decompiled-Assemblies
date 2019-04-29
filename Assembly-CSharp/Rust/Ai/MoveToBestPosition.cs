using Apex.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	[FriendlyName("Move To Best Position", "Sets a move target based on the scorers and moves towards it")]
	public class MoveToBestPosition : BaseActionWithOptions<Vector3>
	{
		public MoveToBestPosition()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			Vector3 best = base.GetBest(c, c.sampledPositions);
			if (best.sqrMagnitude == 0f)
			{
				return;
			}
			NPCHumanContext nPCHumanContext = c as NPCHumanContext;
			if (nPCHumanContext != null && nPCHumanContext.CurrentCoverVolume != null)
			{
				int num = 0;
				while (num < nPCHumanContext.sampledCoverPoints.Count)
				{
					CoverPoint item = nPCHumanContext.sampledCoverPoints[num];
					CoverPoint.CoverType coverType = nPCHumanContext.sampledCoverPointTypes[num];
					if (Vector3Ex.Distance2D(item.Position, best) >= 1f)
					{
						num++;
					}
					else
					{
						nPCHumanContext.CoverSet.Update(item, item, item);
						break;
					}
				}
			}
			c.AIAgent.UpdateDestination(best);
			c.lastSampledPosition = best;
		}
	}
}