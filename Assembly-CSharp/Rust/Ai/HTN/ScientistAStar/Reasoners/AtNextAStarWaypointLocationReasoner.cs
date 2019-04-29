using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class AtNextAStarWaypointLocationReasoner : INpcReasoner
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

		public AtNextAStarWaypointLocationReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			if (npcContext.Domain.IsAtFinalDestination())
			{
				npcContext.SetFact(Facts.AtLocationNextAStarWaypoint, 1, true, true, true);
			}
			npcContext.SetFact(Facts.AtLocationNextAStarWaypoint, 0, true, true, true);
		}
	}
}