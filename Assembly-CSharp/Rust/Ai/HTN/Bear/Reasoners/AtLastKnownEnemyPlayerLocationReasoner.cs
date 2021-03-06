using Rust.Ai.HTN;
using Rust.Ai.HTN.Bear;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Reasoners
{
	public class AtLastKnownEnemyPlayerLocationReasoner : INpcReasoner
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

		public AtLastKnownEnemyPlayerLocationReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
			if (npcContext == null)
			{
				return;
			}
			if (npcContext.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null && (BearDomain.BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(npcContext) - npcContext.Body.transform.position).sqrMagnitude < 1f)
			{
				npcContext.SetFact(Facts.AtLocationLastKnownLocationOfPrimaryEnemyPlayer, 1, true, true, true);
				return;
			}
			npcContext.SetFact(Facts.AtLocationLastKnownLocationOfPrimaryEnemyPlayer, 0, true, true, true);
		}
	}
}