using Rust.Ai.HTN;
using Rust.Ai.HTN.Murderer;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Murderer.Reasoners
{
	public class FireTacticReasoner : INpcReasoner
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

		public FireTacticReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			MurdererContext npcContext = npc.AiDomain.NpcContext as MurdererContext;
			if (npcContext == null)
			{
				return;
			}
			HTNPlayer hTNPlayer = npc as HTNPlayer;
			if (hTNPlayer == null)
			{
				return;
			}
			FireTactic fireTactic = FireTactic.Single;
			AttackEntity heldEntity = hTNPlayer.GetHeldEntity() as AttackEntity;
			if (heldEntity)
			{
				BaseProjectile baseProjectile = heldEntity as BaseProjectile;
				float sqrDistance = Single.MaxValue;
				if (npcContext.PrimaryEnemyPlayerInLineOfSight.Player != null)
				{
					sqrDistance = npcContext.PrimaryEnemyPlayerInLineOfSight.SqrDistance;
				}
				if (heldEntity.attackLengthMin < 0f || sqrDistance > npcContext.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(baseProjectile))
				{
					fireTactic = (heldEntity.attackLengthMin < 0f || sqrDistance > npcContext.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(baseProjectile) ? FireTactic.Single : FireTactic.Burst);
				}
				else
				{
					fireTactic = FireTactic.FullAuto;
				}
			}
			npcContext.SetFact(Facts.FireTactic, fireTactic, true, true, true);
		}
	}
}