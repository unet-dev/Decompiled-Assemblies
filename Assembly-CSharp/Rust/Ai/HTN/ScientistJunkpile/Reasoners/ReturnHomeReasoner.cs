using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.AI;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
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
			ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
			if (npcContext == null)
			{
				return;
			}
			if (!npcContext.IsFact(Facts.IsReturningHome))
			{
				if ((npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null || time - npcContext.Memory.PrimaryKnownEnemyPlayer.Time > npcContext.Body.AiDefinition.Memory.NoSeeReturnToSpawnTime) && npcContext.Domain.SqrDistanceToSpawn() > npcContext.Domain.SqrMovementRadius)
				{
					npcContext.SetFact(Facts.IsReturningHome, true, true, true, true);
					if (NavMesh.SamplePosition(npcContext.Domain.SpawnPosition, out navMeshHit, 1f, npcContext.Domain.NavAgent.areaMask))
					{
						npcContext.Domain.SetDestination(navMeshHit.position);
						return;
					}
				}
			}
			else if (npcContext.IsFact(Facts.CanSeeEnemy) || time - npcContext.Body.lastAttackedTime < 2f || npcContext.IsFact(Facts.AtLocationHome))
			{
				npcContext.SetFact(Facts.IsReturningHome, false, true, true, true);
			}
		}
	}
}