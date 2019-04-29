using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class IsInCoverFromTarget : BaseScorer
	{
		[ApexSerialization]
		public float CoverArcThreshold = -0.75f;

		public IsInCoverFromTarget()
		{
		}

		public override float GetScore(BaseContext ctx)
		{
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || !SingletonComponent<AiManager>.Instance.UseCover || ctx.AIAgent.AttackTarget == null)
			{
				return 0f;
			}
			return 0f;
		}
	}
}