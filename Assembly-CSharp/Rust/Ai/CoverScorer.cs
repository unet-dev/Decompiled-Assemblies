using Apex.AI;
using Apex.Serialization;
using ConVar;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class CoverScorer : OptionScorerBase<CoverPoint>
	{
		[ApexSerialization]
		[Range(-1f, 1f)]
		public float coverFromPointArcThreshold = -0.8f;

		public CoverScorer()
		{
		}

		public static float Evaluate(CoverContext c, CoverPoint option, float arcThreshold)
		{
			float single;
			if (c != null)
			{
				if (option.IsReserved || option.IsCompromised)
				{
					return 0f;
				}
				Vector3 serverPosition = c.Self.Entity.ServerPosition;
				if (option.ProvidesCoverFromPoint(c.DangerPoint, arcThreshold))
				{
					Vector3 position = option.Position - serverPosition;
					Vector3 dangerPoint = c.DangerPoint - serverPosition;
					float single1 = Vector3.Dot(position.normalized, dangerPoint.normalized);
					if (CoverScorer.EvaluateAdvancement(c, option, position, dangerPoint, single1, out single))
					{
						return single;
					}
					if (CoverScorer.EvaluateRetreat(c, option, position, dangerPoint, ref single1, out single))
					{
						return single;
					}
					if (CoverScorer.EvaluateFlanking(c, option, position, dangerPoint, single1, out single))
					{
						return single;
					}
				}
			}
			return 0f;
		}

		private static bool EvaluateAdvancement(CoverContext c, CoverPoint option, Vector3 dirCover, Vector3 dirDanger, float directness, out float result)
		{
			result = 0f;
			if (directness > 0.5f && dirCover.sqrMagnitude > dirDanger.sqrMagnitude)
			{
				return false;
			}
			if (directness >= 0.5f)
			{
				float single = dirCover.sqrMagnitude;
				if (single > dirDanger.sqrMagnitude)
				{
					return false;
				}
				float single1 = directness;
				if (single1 > c.BestAdvanceValue)
				{
					if (ConVar.AI.npc_cover_use_path_distance && c.Self.IsNavRunning() && c.Self.AttackTarget != null)
					{
						NPCPlayerApex self = c.Self as NPCPlayerApex;
						if (self != null && !self.PathDistanceIsValid(c.Self.AttackTarget.ServerPosition, option.Position, false))
						{
							return false;
						}
					}
					if ((option.Position - c.DangerPoint).sqrMagnitude < single)
					{
						single1 *= 0.9f;
					}
					c.BestAdvanceValue = single1;
					c.BestAdvanceCP = option;
					result = c.BestAdvanceValue;
					return true;
				}
			}
			return false;
		}

		private static bool EvaluateFlanking(CoverContext c, CoverPoint option, Vector3 dirCover, Vector3 dirDanger, float directness, out float result)
		{
			result = 0f;
			if (directness > -0.5f && directness < 0.5f)
			{
				float single = 1f - Mathf.Abs(directness);
				if (single > c.BestFlankValue)
				{
					if (ConVar.AI.npc_cover_use_path_distance && c.Self.IsNavRunning() && c.Self.AttackTarget != null)
					{
						NPCPlayerApex self = c.Self as NPCPlayerApex;
						if (self != null && !self.PathDistanceIsValid(c.Self.AttackTarget.ServerPosition, option.Position, false))
						{
							return false;
						}
					}
					c.BestFlankValue = 0.1f - Mathf.Abs(single);
					c.BestFlankCP = option;
					result = c.BestFlankValue;
					return true;
				}
			}
			return false;
		}

		private static bool EvaluateRetreat(CoverContext c, CoverPoint option, Vector3 dirCover, Vector3 dirDanger, ref float directness, out float result)
		{
			result = 0f;
			if (directness <= -0.5f)
			{
				NPCPlayerApex self = c.Self as NPCPlayerApex;
				if (self == null)
				{
					return false;
				}
				if (dirCover.sqrMagnitude < self.MinDistanceToRetreatCover * self.MinDistanceToRetreatCover)
				{
					directness = -0.49f;
					return false;
				}
				float single = directness * -1f;
				if (single > c.BestRetreatValue)
				{
					c.BestRetreatValue = single;
					c.BestRetreatCP = option;
					result = c.BestRetreatValue;
					return true;
				}
			}
			return false;
		}

		public override float Score(IAIContext context, CoverPoint option)
		{
			return CoverScorer.Evaluate(context as CoverContext, option, this.coverFromPointArcThreshold);
		}
	}
}