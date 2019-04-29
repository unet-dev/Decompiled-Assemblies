using ConVar;
using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Sensors
{
	[Serializable]
	public class PlayersInRangeSensor : INpcSensor
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

		static PlayersInRangeSensor()
		{
			PlayersInRangeSensor.PlayerQueryResults = new BasePlayer[128];
			PlayersInRangeSensor.PlayerQueryResultCount = 0;
		}

		public PlayersInRangeSensor()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			if (ConVar.AI.ignoreplayers || npc == null || npc.transform == null || npc.IsDestroyed || npc.AiDefinition == null)
			{
				return;
			}
			PlayersInRangeSensor.PlayerQueryResultCount = BaseEntity.Query.Server.GetPlayersInSphere(npc.transform.position, npc.AiDefinition.Sensory.VisionRange, PlayersInRangeSensor.PlayerQueryResults, (BasePlayer player) => {
				if (player == null || !player.isServer || player.IsDestroyed || player.transform == null || player.IsDead() || player.IsWounded())
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
			if (PlayersInRangeSensor.PlayerQueryResultCount > 0)
			{
				for (int i = 0; i < PlayersInRangeSensor.PlayerQueryResultCount; i++)
				{
					BasePlayer playerQueryResults = PlayersInRangeSensor.PlayerQueryResults[i];
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