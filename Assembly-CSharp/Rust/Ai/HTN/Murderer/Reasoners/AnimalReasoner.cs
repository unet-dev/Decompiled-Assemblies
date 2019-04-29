using Rust.Ai.HTN;
using Rust.Ai.HTN.Murderer;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Murderer.Reasoners
{
	public class AnimalReasoner : INpcReasoner
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

		public AnimalReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			MurdererContext npcContext = npc.AiDomain.NpcContext as MurdererContext;
			if (npcContext == null)
			{
				return;
			}
			BaseNpc animal = null;
			float sqrDistance = Single.MaxValue;
			for (int i = 0; i < npcContext.AnimalsInRange.Count; i++)
			{
				AnimalInfo item = npcContext.AnimalsInRange[i];
				if (item.Animal != null && item.SqrDistance < sqrDistance)
				{
					sqrDistance = item.SqrDistance;
					animal = item.Animal;
				}
			}
			if (animal != null)
			{
				AttackEntity firearm = npcContext.Domain.GetFirearm();
				if (sqrDistance < npc.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm))
				{
					npcContext.Memory.RememberPrimaryAnimal(animal);
					npcContext.SetFact(Facts.NearbyAnimal, true, true, true, true);
					return;
				}
			}
			npcContext.SetFact(Facts.NearbyAnimal, false, true, true, true);
		}
	}
}