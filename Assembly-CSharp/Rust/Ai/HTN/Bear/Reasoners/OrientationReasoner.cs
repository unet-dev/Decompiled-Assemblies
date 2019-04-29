using Rust.Ai.HTN;
using Rust.Ai.HTN.Bear;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Bear.Reasoners
{
	public class OrientationReasoner : INpcReasoner
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

		public OrientationReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			NpcOrientation npcOrientation;
			NpcOrientation npcOrientation1;
			BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
			if (npcContext == null)
			{
				return;
			}
			HTNAnimal hTNAnimal = npc as HTNAnimal;
			if (hTNAnimal == null)
			{
				return;
			}
			NpcOrientation npcOrientation2 = NpcOrientation.Heading;
			if (npc.IsDestroyed || hTNAnimal.IsDead())
			{
				npcOrientation2 = NpcOrientation.None;
			}
			else if (npcContext.Memory.PrimaryKnownAnimal.Animal != null)
			{
				if (npcContext.PrimaryEnemyPlayerInLineOfSight.Player == null)
				{
					npcOrientation2 = NpcOrientation.LookAtAnimal;
				}
				else if (npcContext.Memory.PrimaryKnownAnimal.SqrDistance >= npcContext.PrimaryEnemyPlayerInLineOfSight.SqrDistance)
				{
					if (npcContext.PrimaryEnemyPlayerInLineOfSight.BodyVisible)
					{
						npcOrientation1 = NpcOrientation.PrimaryTargetBody;
					}
					else
					{
						npcOrientation1 = (npcContext.PrimaryEnemyPlayerInLineOfSight.HeadVisible ? NpcOrientation.PrimaryTargetHead : NpcOrientation.LastKnownPrimaryTargetLocation);
					}
					npcOrientation2 = npcOrientation1;
				}
				else
				{
					npcOrientation2 = NpcOrientation.LookAtAnimal;
				}
			}
			else if (npcContext.PrimaryEnemyPlayerInLineOfSight.Player != null)
			{
				if (npcContext.PrimaryEnemyPlayerInLineOfSight.BodyVisible)
				{
					npcOrientation = NpcOrientation.PrimaryTargetBody;
				}
				else
				{
					npcOrientation = (npcContext.PrimaryEnemyPlayerInLineOfSight.HeadVisible ? NpcOrientation.PrimaryTargetHead : NpcOrientation.LastKnownPrimaryTargetLocation);
				}
				npcOrientation2 = npcOrientation;
			}
			else if (hTNAnimal.lastAttacker != null && hTNAnimal.lastAttackedTime > 0f && time - hTNAnimal.lastAttackedTime < 2f)
			{
				npcOrientation2 = NpcOrientation.LastAttackedDirection;
			}
			else if (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
			{
				if (npcContext.IsFact(Facts.CanHearEnemy))
				{
					npcOrientation2 = NpcOrientation.AudibleTargetDirection;
				}
			}
			else if (npcContext.GetFact(Facts.IsSearching) > 0 && npcContext.GetFact(Facts.IsNavigating) == 0)
			{
				npcOrientation2 = NpcOrientation.LookAround;
			}
			else if (npcContext.GetFact(Facts.IsIdle) <= 0)
			{
				npcOrientation2 = NpcOrientation.LastKnownPrimaryTargetLocation;
			}
			else
			{
				npcOrientation2 = (!npcContext.IsFact(Facts.CanHearEnemy) ? NpcOrientation.Heading : NpcOrientation.AudibleTargetDirection);
			}
			npcContext.OrientationType = npcOrientation2;
		}
	}
}