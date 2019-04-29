using ConVar;
using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class CoverPointsReasoner : INpcReasoner
	{
		public float LastTickTime
		{
			get;
			set;
		}

		public float TickFrequency
		{
			get;
			set;
		}

		public CoverPointsReasoner()
		{
		}

		private static bool EvaluateAdvancement(IHTNAgent npc, ScientistAStarContext c, ref float bestScore, ref BaseNpcMemory.EnemyPlayerInfo enemyInfo, CoverPoint option, Vector3 dirCover, Vector3 dirDanger, float directness)
		{
			if (directness >= 0.2f)
			{
				float single = dirCover.sqrMagnitude;
				if (single > dirDanger.sqrMagnitude || single < 0.5f)
				{
					return false;
				}
				float position = (option.Position - enemyInfo.LastKnownPosition).sqrMagnitude;
				float allowedCoverRangeSqr = c.Domain.GetAllowedCoverRangeSqr();
				float single1 = directness + (allowedCoverRangeSqr - single) / allowedCoverRangeSqr + option.Score + (position < single ? 1f : 0f);
				if (single1 > bestScore)
				{
					if (ConVar.AI.npc_cover_use_path_distance && npc != null && !c.Domain.PathDistanceIsValid(enemyInfo.LastKnownPosition, option.Position, false))
					{
						return false;
					}
					if ((option.Position - enemyInfo.LastKnownPosition).sqrMagnitude < single)
					{
						single1 *= 0.9f;
					}
					bestScore = single1;
					c.BestAdvanceCover = option;
					return true;
				}
			}
			return false;
		}

		private static bool EvaluateFlanking(IHTNAgent npc, ScientistAStarContext c, ref float bestScore, ref BaseNpcMemory.EnemyPlayerInfo enemyInfo, CoverPoint option, Vector3 dirCover, Vector3 dirDanger, float directness)
		{
			if (directness > -0.2f && directness < 0.2f)
			{
				float single = dirCover.sqrMagnitude;
				float allowedCoverRangeSqr = c.Domain.GetAllowedCoverRangeSqr();
				float single1 = (0.2f - Mathf.Abs(directness)) / 0.2f + (allowedCoverRangeSqr - single) / allowedCoverRangeSqr + option.Score;
				if (single1 > bestScore)
				{
					if (ConVar.AI.npc_cover_use_path_distance && npc != null && !c.Domain.PathDistanceIsValid(enemyInfo.LastKnownPosition, option.Position, false))
					{
						return false;
					}
					bestScore = 0.1f - Mathf.Abs(single1);
					c.BestFlankCover = option;
					return true;
				}
			}
			return false;
		}

		private static bool EvaluateRetreat(IHTNAgent npc, ScientistAStarContext c, ref float bestScore, ref BaseNpcMemory.EnemyPlayerInfo enemyInfo, CoverPoint option, Vector3 dirCover, Vector3 dirDanger, ref float directness)
		{
			float single = dirCover.sqrMagnitude;
			if (directness <= -0.2f)
			{
				float allowedCoverRangeSqr = c.Domain.GetAllowedCoverRangeSqr();
				float single1 = directness * -1f + (allowedCoverRangeSqr - single) / allowedCoverRangeSqr + option.Score;
				if (single1 > bestScore)
				{
					bestScore = single1;
					c.BestRetreatCover = option;
					return true;
				}
			}
			return false;
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			npcContext.SetFact(Facts.HasNearbyCover, (npcContext.CoverPoints.Count > 0 ? 1 : 0), true, true, true);
			if (!npcContext.IsFact(Facts.HasEnemyTarget))
			{
				npcContext.ReserveCoverPoint(null);
				return;
			}
			HTNPlayer hTNPlayer = npc as HTNPlayer;
			if (hTNPlayer == null)
			{
				return;
			}
			float single = 0f;
			float single1 = 0f;
			float single2 = 0f;
			foreach (CoverPoint coverPoint in npcContext.CoverPoints)
			{
				if (coverPoint.IsCompromised || coverPoint.IsReserved && !coverPoint.ReservedFor.EqualNetID(hTNPlayer))
				{
					continue;
				}
				float single3 = -0.8f;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = npcContext.Memory.PrimaryKnownEnemyPlayer;
				if (!coverPoint.ProvidesCoverFromPoint(primaryKnownEnemyPlayer.LastKnownPosition, single3))
				{
					continue;
				}
				Vector3 position = coverPoint.Position - npc.BodyPosition;
				Vector3 lastKnownPosition = primaryKnownEnemyPlayer.LastKnownPosition - npc.BodyPosition;
				float single4 = Vector3.Dot(position.normalized, lastKnownPosition.normalized);
				if (single < 1f)
				{
					CoverPointsReasoner.EvaluateAdvancement(npc, npcContext, ref single, ref primaryKnownEnemyPlayer, coverPoint, position, lastKnownPosition, single4);
				}
				if (single2 < 1f)
				{
					CoverPointsReasoner.EvaluateRetreat(npc, npcContext, ref single2, ref primaryKnownEnemyPlayer, coverPoint, position, lastKnownPosition, ref single4);
				}
				if (single1 >= 1f)
				{
					continue;
				}
				CoverPointsReasoner.EvaluateFlanking(npc, npcContext, ref single1, ref primaryKnownEnemyPlayer, coverPoint, position, lastKnownPosition, single4);
			}
		}
	}
}