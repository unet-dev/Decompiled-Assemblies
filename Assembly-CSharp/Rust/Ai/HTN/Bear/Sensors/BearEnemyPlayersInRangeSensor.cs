using ConVar;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Bear.Sensors
{
	[Serializable]
	public class BearEnemyPlayersInRangeSensor : INpcSensor
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

		public BearEnemyPlayersInRangeSensor()
		{
		}

		protected virtual bool EvaluatePlayer(BaseNpcContext context, IHTNAgent npc, NpcPlayerInfo player, float time)
		{
			if (player.Player.Family == npc.Family)
			{
				return false;
			}
			context.EnemyPlayersInRange.Add(player);
			return true;
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			if (ConVar.AI.ignoreplayers)
			{
				return;
			}
			BaseNpcContext npcContext = npc.AiDomain.NpcContext;
			npcContext.EnemyPlayersInRange.Clear();
			for (int i = 0; i < npcContext.PlayersInRange.Count; i++)
			{
				NpcPlayerInfo item = npcContext.PlayersInRange[i];
				if (!npcContext.BaseMemory.ShouldRemoveOnPlayerForgetTimeout(time, item))
				{
					this.EvaluatePlayer(npcContext, npc, item, time);
				}
				else
				{
					npcContext.PlayersInRange.RemoveAt(i);
					i--;
				}
			}
		}
	}
}