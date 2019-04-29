using ConVar;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Sensors
{
	[Serializable]
	public class BearPlayersInRangeSensor : INpcSensor
	{
		public const int MaxPlayers = 128;

		public static BasePlayer[] PlayerQueryResults;

		public static int PlayerQueryResultCount;

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

		static BearPlayersInRangeSensor()
		{
			BearPlayersInRangeSensor.PlayerQueryResults = new BasePlayer[128];
			BearPlayersInRangeSensor.PlayerQueryResultCount = 0;
		}

		public BearPlayersInRangeSensor()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			if (ConVar.AI.ignoreplayers)
			{
				return;
			}
			BearPlayersInRangeSensor.PlayerQueryResultCount = BaseEntity.Query.Server.GetPlayersInSphere(npc.transform.position, npc.AiDefinition.Sensory.VisionRange, BearPlayersInRangeSensor.PlayerQueryResults, (BasePlayer player) => {
				if (player == null || !player.isServer || player.IsDestroyed || player.transform == null || player.IsDead())
				{
					return false;
				}
				if (player.IsSleeping() && player.secondsSleeping < NPCAutoTurret.sleeperhostiledelay)
				{
					return false;
				}
				return true;
			});
			List<NpcPlayerInfo> playersInRange = npc.AiDomain.NpcContext.PlayersInRange;
			if (BearPlayersInRangeSensor.PlayerQueryResultCount > 0)
			{
				for (int i = 0; i < BearPlayersInRangeSensor.PlayerQueryResultCount; i++)
				{
					BasePlayer playerQueryResults = BearPlayersInRangeSensor.PlayerQueryResults[i];
					HTNPlayer hTNPlayer = npc as HTNPlayer;
					if ((!(hTNPlayer != null) || !(playerQueryResults == hTNPlayer)) && (playerQueryResults.transform.position - npc.transform.position).sqrMagnitude <= npc.AiDefinition.Sensory.SqrVisionRange)
					{
						bool flag = false;
						int num = 0;
						while (num < playersInRange.Count)
						{
							NpcPlayerInfo item = playersInRange[num];
							if (item.Player != playerQueryResults)
							{
								num++;
							}
							else
							{
								item.Time = time;
								playersInRange[num] = item;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							NpcPlayerInfo npcPlayerInfo = new NpcPlayerInfo()
							{
								Player = playerQueryResults,
								Time = time
							};
							playersInRange.Add(npcPlayerInfo);
						}
					}
				}
			}
			for (int j = 0; j < playersInRange.Count; j++)
			{
				NpcPlayerInfo item1 = playersInRange[j];
				if (time - item1.Time > npc.AiDefinition.Memory.ForgetInRangeTime && npc.AiDomain.NpcContext.BaseMemory.ShouldRemoveOnPlayerForgetTimeout(time, item1))
				{
					playersInRange.RemoveAt(j);
					j--;
				}
			}
		}
	}
}