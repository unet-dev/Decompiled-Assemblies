using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class MaintainCoverReasoner : INpcReasoner
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

		public MaintainCoverReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			if (npcContext.IsFact(Facts.CanSeeEnemy) && (npcContext.IsFact(Facts.MaintainCover) || npcContext.IsFact(Facts.IsReloading) || npcContext.IsFact(Facts.IsApplyingMedical)))
			{
				if (npcContext.ReservedCoverPoint != null && !npcContext.ReservedCoverPoint.IsCompromised && !npcContext.IsFact(Facts.AtLocationCover) && time - npcContext.ReservedCoverTime < 1f)
				{
					return;
				}
				if (ScientistAStarDomain.AStarCanNavigateToCoverLocation.Try(CoverTactic.Retreat, npcContext))
				{
					Vector3 coverPosition = ScientistAStarDomain.AStarNavigateToCover.GetCoverPosition(CoverTactic.Retreat, npcContext);
					npcContext.Domain.SetDestination(coverPosition);
				}
			}
		}
	}
}