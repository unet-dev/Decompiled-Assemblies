using Rust.Ai.HTN;
using Rust.Ai.HTN.Murderer;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Murderer.Reasoners
{
	public class ReturnHomeReasoner : INpcReasoner
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

		public ReturnHomeReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			NavMeshHit navMeshHit;
			MurdererContext npcContext = npc.AiDomain.NpcContext as MurdererContext;
			if (npcContext == null)
			{
				return;
			}
			if (!npcContext.IsFact(Facts.AtLocationHome) && !npcContext.IsFact(Facts.IsReturningHome) && !npcContext.IsFact(Facts.IsNavigating) && !npcContext.IsFact(Facts.IsWaiting))
			{
				if (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null || time - npcContext.Memory.PrimaryKnownEnemyPlayer.Time > npcContext.Body.AiDefinition.Memory.NoSeeReturnToSpawnTime)
				{
					npcContext.SetFact(Facts.IsReturningHome, true, true, true, true);
					if (NavMesh.SamplePosition(npcContext.Domain.SpawnPosition, out navMeshHit, 1f, npcContext.Domain.NavAgent.areaMask))
					{
						npcContext.Domain.SetDestination(navMeshHit.position, false);
						return;
					}
				}
			}
			else if (npcContext.IsFact(Facts.CanSeeEnemy) || npcContext.IsFact(Facts.CanHearEnemy) || time - npcContext.Body.lastAttackedTime < 2f || npcContext.IsFact(Facts.AtLocationHome))
			{
				npcContext.SetFact(Facts.IsReturningHome, false, true, true, true);
			}
		}
	}
}