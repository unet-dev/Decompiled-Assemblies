using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Sensors
{
	[Serializable]
	public class AnimalDistanceSensor : INpcSensor
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

		public AnimalDistanceSensor()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			List<AnimalInfo> animalsInRange = npc.AiDomain.NpcContext.AnimalsInRange;
			for (int i = 0; i < animalsInRange.Count; i++)
			{
				AnimalInfo item = animalsInRange[i];
				if (item.Animal == null || item.Animal.transform == null || item.Animal.IsDestroyed || item.Animal.IsDead())
				{
					animalsInRange.RemoveAt(i);
					i--;
				}
				else
				{
					Vector3 vector3 = npc.transform.position - item.Animal.transform.position;
					item.SqrDistance = vector3.sqrMagnitude;
					animalsInRange[i] = item;
				}
			}
		}
	}
}