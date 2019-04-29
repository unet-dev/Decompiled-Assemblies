using Rust.Ai.HTN;
using Rust.Ai.HTN.ScientistJunkpile;
using Rust.Ai.HTN.ScientistJunkpile.Reasoners;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Sensors
{
	[Serializable]
	public class AnimalsInRangeSensor : INpcSensor
	{
		public const int MaxAnimals = 128;

		public static BaseNpc[] QueryResults;

		public static int QueryResultCount;

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

		static AnimalsInRangeSensor()
		{
			AnimalsInRangeSensor.QueryResults = new BaseNpc[128];
			AnimalsInRangeSensor.QueryResultCount = 0;
		}

		public AnimalsInRangeSensor()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistJunkpileDomain aiDomain = npc.AiDomain as ScientistJunkpileDomain;
			if (aiDomain == null || aiDomain.ScientistContext == null)
			{
				return;
			}
			AttackEntity firearm = aiDomain.GetFirearm();
			AnimalsInRangeSensor.QueryResultCount = BaseEntity.Query.Server.GetInSphere(npc.transform.position, npc.AiDefinition.Engagement.MediumRangeFirearm(firearm), AnimalsInRangeSensor.QueryResults, (BaseEntity entity) => {
				BaseNpc baseNpc = entity as BaseNpc;
				if (!(baseNpc == null) && baseNpc.isServer && !baseNpc.IsDestroyed && !(baseNpc.transform == null) && !baseNpc.IsDead())
				{
					return true;
				}
				return false;
			});
			List<AnimalInfo> animalsInRange = npc.AiDomain.NpcContext.AnimalsInRange;
			if (AnimalsInRangeSensor.QueryResultCount > 0)
			{
				for (int i = 0; i < AnimalsInRangeSensor.QueryResultCount; i++)
				{
					BaseNpc queryResults = AnimalsInRangeSensor.QueryResults[i];
					Vector3 vector3 = queryResults.transform.position - npc.transform.position;
					float single = vector3.sqrMagnitude;
					if (single <= npc.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm))
					{
						bool flag = false;
						int num = 0;
						while (num < animalsInRange.Count)
						{
							AnimalInfo item = animalsInRange[num];
							if (item.Animal != queryResults)
							{
								num++;
							}
							else
							{
								item.Time = time;
								item.SqrDistance = single;
								animalsInRange[num] = item;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							AnimalInfo animalInfo = new AnimalInfo()
							{
								Animal = queryResults,
								Time = time,
								SqrDistance = single
							};
							animalsInRange.Add(animalInfo);
						}
					}
				}
			}
			for (int j = 0; j < animalsInRange.Count; j++)
			{
				AnimalInfo item1 = animalsInRange[j];
				if (time - item1.Time > npc.AiDefinition.Memory.ForgetAnimalInRangeTime)
				{
					if (item1.Animal == aiDomain.ScientistContext.Memory.PrimaryKnownAnimal.Animal)
					{
						if (AnimalReasoner.IsNearby(aiDomain, item1.SqrDistance))
						{
							goto Label0;
						}
						aiDomain.ScientistContext.Memory.ForgetPrimiaryAnimal();
					}
					animalsInRange.RemoveAt(j);
					j--;
				}
			Label0:
			}
		}
	}
}