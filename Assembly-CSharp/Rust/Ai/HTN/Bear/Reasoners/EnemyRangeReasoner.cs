using Rust.Ai.HTN;
using Rust.Ai.HTN.Bear;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Reasoners
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
			BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
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
			if (single <= npcContext.Body.AiDefinition.Engagement.SqrCloseRange)
			{
				npcContext.SetFact(Facts.EnemyRange, EnemyRange.CloseRange, true, true, true);
				return;
			}
			if (single <= npcContext.Body.AiDefinition.Engagement.SqrMediumRange)
			{
				npcContext.SetFact(Facts.EnemyRange, EnemyRange.MediumRange, true, true, true);
				return;
			}
			if (single <= npcContext.Body.AiDefinition.Engagement.SqrLongRange)
			{
				npcContext.SetFact(Facts.EnemyRange, EnemyRange.LongRange, true, true, true);
				return;
			}
			npcContext.SetFact(Facts.EnemyRange, EnemyRange.OutOfRange, true, true, true);
		}
	}
}