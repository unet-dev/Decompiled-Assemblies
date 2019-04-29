using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
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

		public static bool IsNearby(ScientistJunkpileDomain domain, float sqrDistance)
		{
			AttackEntity firearm = domain.GetFirearm();
			return sqrDistance < domain.ScientistDefinition.Engagement.SqrCloseRangeFirearm(firearm) + 4f;
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
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
			if (!(animal != null) || !AnimalReasoner.IsNearby(npcContext.Domain, sqrDistance))
			{
				npcContext.SetFact(Facts.NearbyAnimal, false, true, true, true);
				return;
			}
			npcContext.Memory.RememberPrimaryAnimal(animal);
			npcContext.SetFact(Facts.NearbyAnimal, true, true, true, true);
		}
	}
}