using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Sensors
{
	[Serializable]
	public class EnemyPlayersLineOfSightSensor : INpcSensor
	{
		public float LastTickTime
		{
			get;
			set;
		}

		public int MaxVisible
		{
			get;
			set;
		}

		public float TickFrequency
		{
			get;
			set;
		}

		public EnemyPlayersLineOfSightSensor()
		{
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			EnemyPlayersLineOfSightSensor.TickStatic(npc, (this.MaxVisible > 0 ? this.MaxVisible : 3));
		}

		public static bool TickLineOfSightTest(IHTNAgent npc, ref NpcPlayerInfo info)
		{
			info.HeadVisible = false;
			info.BodyVisible = false;
			Vector3 player = info.Player.transform.position - npc.transform.position;
			Vector3 eyeRotation = npc.EyeRotation * Vector3.forward;
			float single = player.sqrMagnitude;
			float single1 = Vector3.Dot(eyeRotation, player.normalized);
			if (single < npc.AiDefinition.Engagement.SqrAggroRange && single1 > npc.AiDefinition.Sensory.FieldOfViewRadians)
			{
				if (info.Player.IsVisible(npc.EyePosition, info.Player.CenterPoint(), npc.AiDefinition.Engagement.AggroRange))
				{
					info.BodyVisible = true;
					npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Add(info);
				}
				else if (info.Player.IsVisible(npc.EyePosition, info.Player.eyes.position, npc.AiDefinition.Engagement.AggroRange))
				{
					info.HeadVisible = true;
					npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Add(info);
				}
			}
			if (info.HeadVisible)
			{
				return true;
			}
			return info.BodyVisible;
		}

		public static void TickStatic(IHTNAgent npc, int maxVisible = 3)
		{
			npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Clear();
			int num = 0;
			List<NpcPlayerInfo> enemyPlayersInRange = npc.AiDomain.NpcContext.EnemyPlayersInRange;
			for (int i = 0; i < enemyPlayersInRange.Count; i++)
			{
				NpcPlayerInfo item = enemyPlayersInRange[i];
				if (item.Player == null || item.Player.transform == null || item.Player.IsDestroyed || item.Player.IsDead() || item.Player.IsWounded())
				{
					enemyPlayersInRange.RemoveAt(i);
					i--;
				}
				else if (EnemyPlayersLineOfSightSensor.TickLineOfSightTest(npc, ref item))
				{
					num++;
					if (num >= maxVisible)
					{
						return;
					}
				}
			}
		}
	}
}