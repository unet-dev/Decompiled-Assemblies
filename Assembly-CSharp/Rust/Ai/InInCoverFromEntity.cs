using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class InInCoverFromEntity : WeightedScorerBase<BaseEntity>
	{
		[ApexSerialization]
		public float CoverArcThreshold = -0.75f;

		public InInCoverFromEntity()
		{
		}

		public override float GetScore(BaseContext ctx, BaseEntity option)
		{
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || !SingletonComponent<AiManager>.Instance.UseCover || ctx.AIAgent.AttackTarget == null)
			{
				return 0f;
			}
			return 0f;
		}
	}
}