using Rust.Ai.HTN;
using Rust.Ai.HTN.Murderer;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Murderer.Reasoners
{
	public class AtHomeLocationReasoner : INpcReasoner
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

		public AtHomeLocationReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			MurdererContext npcContext = npc.AiDomain.NpcContext as MurdererContext;
			if (npcContext == null)
			{
				return;
			}
			if ((npcContext.BodyPosition - npcContext.Domain.SpawnPosition).sqrMagnitude < 3f)
			{
				npcContext.SetFact(Facts.AtLocationHome, true, true, true, true);
				return;
			}
			npcContext.SetFact(Facts.AtLocationHome, false, true, true, true);
		}
	}
}