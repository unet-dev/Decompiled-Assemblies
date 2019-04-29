using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Scientist;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist.Reasoners
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
			ScientistContext npcContext = npc.AiDomain.NpcContext as ScientistContext;
			if (npcContext == null)
			{
				return;
			}
			if (npcContext.IsFact(Facts.MaintainCover) || npcContext.IsFact(Facts.IsReloading) || npcContext.IsFact(Facts.IsApplyingMedical))
			{
				if (npcContext.ReservedCoverPoint != null && !npcContext.ReservedCoverPoint.IsCompromised && !npcContext.IsFact(Facts.AtLocationCover) && time - npcContext.ReservedCoverTime < 0.8f)
				{
					return;
				}
				if (!npcContext.IsFact(Facts.CanSeeEnemy) && npcContext.Body.SecondsSinceAttacked - 1f > time - npcContext.ReservedCoverTime)
				{
					return;
				}
				if (ScientistDomain.CanNavigateToCoverLocation.Try(CoverTactic.Retreat, npcContext))
				{
					Vector3 coverPosition = ScientistDomain.NavigateToCover.GetCoverPosition(CoverTactic.Retreat, npcContext);
					npcContext.Domain.SetDestination(coverPosition);
					npcContext.Body.modelState.ducked = false;
					npcContext.SetFact(Facts.IsDucking, 0, false, true, true);
					npcContext.SetFact(Facts.FirearmOrder, FirearmOrders.FireAtWill, false, true, true);
				}
			}
		}
	}
}