using ConVar;
using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rust.Ai.HTN.Sensors
{
	[Serializable]
	public class EnemyPlayersInRangeSensor : INpcSensor
	{
		private static EnemyPlayersInRangeSensor.EnemyPlayerInRangeComparer _comparer;

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

		static EnemyPlayersInRangeSensor()
		{
			EnemyPlayersInRangeSensor._comparer = new EnemyPlayersInRangeSensor.EnemyPlayerInRangeComparer();
		}

		public EnemyPlayersInRangeSensor()
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
			npcContext.EnemyPlayersInRange.Sort(EnemyPlayersInRangeSensor._comparer);
		}

		public class EnemyPlayerInRangeComparer : IComparer<NpcPlayerInfo>
		{
			public EnemyPlayerInRangeComparer()
			{
			}

			public int Compare(NpcPlayerInfo a, NpcPlayerInfo b)
			{
				if (a.Player == null || b.Player == null)
				{
					return 0;
				}
				if (a.SqrDistance < 0.01f)
				{
					return -1;
				}
				if (a.SqrDistance < b.SqrDistance)
				{
					return -1;
				}
				if (a.SqrDistance > b.SqrDistance)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}