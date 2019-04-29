using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar;
using System;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
	public class HealthReasoner : INpcReasoner
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

		public HealthReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
			if (npcContext == null)
			{
				return;
			}
			float single = npc.healthFraction;
			if (single > 0.9f)
			{
				npcContext.SetFact(Facts.HealthState, HealthState.FullHealth, true, true, true);
				return;
			}
			if (single > 0.6f)
			{
				npcContext.SetFact(Facts.HealthState, HealthState.HighHealth, true, true, true);
				return;
			}
			if (single > 0.3f)
			{
				npcContext.SetFact(Facts.HealthState, HealthState.MediumHealth, true, true, true);
				return;
			}
			if (single > 0f)
			{
				npcContext.SetFact(Facts.HealthState, HealthState.LowHealth, true, true, true);
				return;
			}
			npcContext.SetFact(Facts.HealthState, HealthState.Dead, true, true, true);
		}
	}
}