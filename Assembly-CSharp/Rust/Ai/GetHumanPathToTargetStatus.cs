using Apex.Serialization;
using System;
using UnityEngine.AI;

namespace Rust.Ai
{
	public class GetHumanPathToTargetStatus : BaseScorer
	{
		[ApexSerialization]
		public NavMeshPathStatus Status;

		public GetHumanPathToTargetStatus()
		{
		}

		public static bool Evaluate(NPCHumanContext c, NavMeshPathStatus s)
		{
			byte fact = c.GetFact(NPCPlayerApex.Facts.PathToTargetStatus);
			return c.Human.ToPathStatus(fact) == s;
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (GetHumanPathToTargetStatus.Evaluate(c as NPCHumanContext, this.Status))
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