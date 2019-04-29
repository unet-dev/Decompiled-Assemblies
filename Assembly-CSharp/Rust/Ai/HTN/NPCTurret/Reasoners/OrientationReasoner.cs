using Rust.Ai.HTN;
using Rust.Ai.HTN.NPCTurret;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.NPCTurret.Reasoners
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
			NPCTurretContext npcContext = npc.AiDomain.NpcContext as NPCTurretContext;
			if (npcContext == null)
			{
				return;
			}
			HTNPlayer hTNPlayer = npc as HTNPlayer;
			if (hTNPlayer == null)
			{
				return;
			}
			NpcOrientation npcOrientation2 = NpcOrientation.Heading;
			if (npc.IsDestroyed || hTNPlayer.IsDead() || hTNPlayer.IsWounded())
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
			else if (hTNPlayer.lastAttacker != null && hTNPlayer.lastAttackedTime > 0f && time - hTNPlayer.lastAttackedTime < 2f)
			{
				npcOrientation2 = NpcOrientation.LastAttackedDirection;
			}
			else if (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null)
			{
				npcOrientation2 = NpcOrientation.LastKnownPrimaryTargetLocation;
			}
			npcContext.OrientationType = npcOrientation2;
		}
	}
}