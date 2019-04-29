using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Scientist;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist.Reasoners
{
	public class ExplosivesReasoner : INpcReasoner
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

		public ExplosivesReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistContext npcContext = npc.AiDomain.NpcContext as ScientistContext;
			if (npcContext == null)
			{
				return;
			}
			for (int i = 0; i < npcContext.Memory.KnownTimedExplosives.Count; i++)
			{
				BaseNpcMemory.EntityOfInterestInfo item = npcContext.Memory.KnownTimedExplosives[i];
				if (item.Entity != null)
				{
					AttackEntity firearm = npcContext.Domain.GetFirearm();
					if ((item.Entity.transform.position - npcContext.BodyPosition).sqrMagnitude < npcContext.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm))
					{
						npcContext.SetFact(Facts.NearbyExplosives, true, true, true, true);
						npcContext.IncrementFact(Facts.Alertness, 2, true, true, true);
						return;
					}
				}
			}
			npcContext.SetFact(Facts.NearbyExplosives, false, true, true, true);
		}
	}
}