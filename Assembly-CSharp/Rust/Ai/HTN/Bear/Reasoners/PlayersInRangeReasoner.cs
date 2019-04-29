using Rust.Ai.HTN;
using Rust.Ai.HTN.Bear;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Bear.Reasoners
{
	public class PlayersInRangeReasoner : INpcReasoner
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

		public PlayersInRangeReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
			if (npcContext == null)
			{
				return;
			}
			npcContext.SetFact(Facts.HasPlayersInRange, npcContext.PlayersInRange.Count > 0, true, true, true);
		}
	}
}