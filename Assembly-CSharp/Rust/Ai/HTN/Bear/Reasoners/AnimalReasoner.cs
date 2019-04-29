using Rust.Ai.HTN;
using Rust.Ai.HTN.Bear;
using Rust.Ai.HTN.Reasoning;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Bear.Reasoners
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
			BearContext npcContext = npc.AiDomain.NpcContext as BearContext;
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
			if (!(animal != null) || sqrDistance >= npc.AiDefinition.Engagement.SqrMediumRange)
			{
				npcContext.SetFact(Facts.NearbyAnimal, false, true, true, true);
				return;
			}
			npcContext.Memory.RememberPrimaryAnimal(animal);
			npcContext.SetFact(Facts.NearbyAnimal, true, true, true, true);
		}
	}
}