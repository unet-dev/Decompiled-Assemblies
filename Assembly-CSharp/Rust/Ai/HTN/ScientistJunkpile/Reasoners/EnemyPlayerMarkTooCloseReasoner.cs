using ConVar;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
	public class EnemyPlayerMarkTooCloseReasoner : INpcReasoner
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

		public EnemyPlayerMarkTooCloseReasoner()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
			if (npcContext == null)
			{
				return;
			}
			float npcJunkpileDistAggroGate = ConVar.AI.npc_junkpile_dist_aggro_gate * ConVar.AI.npc_junkpile_dist_aggro_gate;
			for (int i = 0; i < npc.AiDomain.NpcContext.EnemyPlayersInRange.Count; i++)
			{
				NpcPlayerInfo item = npc.AiDomain.NpcContext.EnemyPlayersInRange[i];
				if (!(item.Player == null) && !(item.Player.transform == null))
				{
					if (Mathf.Approximately(item.SqrDistance, 0f))
					{
						Vector3 player = item.Player.transform.position - npc.BodyPosition;
						item.SqrDistance = player.sqrMagnitude;
						npc.AiDomain.NpcContext.EnemyPlayersInRange[i] = item;
					}
					if (item.SqrDistance < npcJunkpileDistAggroGate)
					{
						npcContext.Memory.MarkEnemy(item.Player);
					}
				}
			}
		}
	}
}