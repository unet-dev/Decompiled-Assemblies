using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Sensors
{
	[Serializable]
	public class PlayersDistanceSensor : INpcSensor
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

		public PlayersDistanceSensor()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			List<NpcPlayerInfo> playersInRange = npc.AiDomain.NpcContext.PlayersInRange;
			for (int i = 0; i < playersInRange.Count; i++)
			{
				NpcPlayerInfo item = playersInRange[i];
				if (item.Player == null || item.Player.transform == null || item.Player.IsDestroyed || item.Player.IsDead() || item.Player.IsWounded())
				{
					playersInRange.RemoveAt(i);
					i--;
				}
				else
				{
					Vector3 vector3 = npc.transform.position - item.Player.transform.position;
					item.SqrDistance = vector3.sqrMagnitude;
					playersInRange[i] = item;
				}
			}
		}
	}
}