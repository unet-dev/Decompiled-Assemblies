using ConVar;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Bear.Sensors
{
	[Serializable]
	public class BearPlayersOutsideRangeSensor : INpcSensor
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

		public BearPlayersOutsideRangeSensor()
		{
		}

		protected virtual bool EvaluatePlayer(BaseNpcContext context, IHTNAgent npc, ref NpcPlayerInfo player, float time)
		{
			if (player.Player.Family == npc.Family)
			{
				return false;
			}
			List<NpcPlayerInfo> playersInRange = npc.AiDomain.NpcContext.PlayersInRange;
			bool flag = false;
			int num = 0;
			while (num < playersInRange.Count)
			{
				if (playersInRange[num].Player != player.Player)
				{
					num++;
				}
				else
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				playersInRange.Add(player);
			}
			return true;
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			if (ConVar.AI.ignoreplayers)
			{
				return;
			}
			BaseNpcContext npcContext = npc.AiDomain.NpcContext;
			for (int i = 0; i < npcContext.PlayersOutsideDetectionRange.Count; i++)
			{
				NpcPlayerInfo item = npcContext.PlayersOutsideDetectionRange[i];
				if (!npcContext.BaseMemory.ShouldRemoveOnPlayerForgetTimeout(time, item))
				{
					this.EvaluatePlayer(npcContext, npc, ref item, time);
				}
				else
				{
					npcContext.PlayersOutsideDetectionRange.RemoveAt(i);
					i--;
				}
			}
		}
	}
}