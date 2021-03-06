using Rust.Ai.HTN;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Sensors
{
	[Serializable]
	public class BearPlayersViewAngleSensor : INpcSensor
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

		public BearPlayersViewAngleSensor()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			List<NpcPlayerInfo> playersInRange = npc.AiDomain.NpcContext.PlayersInRange;
			for (int i = 0; i < playersInRange.Count; i++)
			{
				NpcPlayerInfo item = playersInRange[i];
				if (item.Player == null || item.Player.transform == null || item.Player.IsDestroyed || item.Player.IsDead())
				{
					playersInRange.RemoveAt(i);
					i--;
				}
				else
				{
					Vector3 vector3 = npc.transform.position - item.Player.transform.position;
					Vector3 vector31 = vector3.normalized;
					item.ForwardDotDir = Vector3.Dot(-npc.transform.forward, vector31);
					playersInRange[i] = item;
				}
			}
		}
	}
}