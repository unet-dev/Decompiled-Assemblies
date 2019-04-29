using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Scientist;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist.Reasoners
{
	public class EnemyRangeReasoner : INpcReasoner
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

		public EnemyRangeReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistContext npcContext = npc.AiDomain.NpcContext as ScientistContext;
			if (npcContext == null)
			{
				return;
			}
			if (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
			{
				npcContext.SetFact(Facts.EnemyRange, EnemyRange.OutOfRange, true, true, true);
			}
			Vector3 lastKnownPosition = npcContext.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition - npcContext.BodyPosition;
			float single = lastKnownPosition.sqrMagnitude;
			AttackEntity firearm = npcContext.Domain.GetFirearm();
			if (single <= npcContext.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm))
			{
				npcContext.SetFact(Facts.EnemyRange, EnemyRange.CloseRange, true, true, true);
				return;
			}
			if (single <= npcContext.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm))
			{
				npcContext.SetFact(Facts.EnemyRange, EnemyRange.MediumRange, true, true, true);
				return;
			}
			if (single <= npcContext.Body.AiDefinition.Engagement.SqrLongRangeFirearm(firearm))
			{
				npcContext.SetFact(Facts.EnemyRange, EnemyRange.LongRange, true, true, true);
				return;
			}
			npcContext.SetFact(Facts.EnemyRange, EnemyRange.OutOfRange, true, true, true);
		}
	}
}