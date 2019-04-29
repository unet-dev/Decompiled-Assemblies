using Rust.Ai.HTN;
using Rust.Ai.HTN.Bear;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Bear.Sensors
{
	[Serializable]
	public class BearEnemyPlayersLineOfSightSensor : INpcSensor
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

		public BearEnemyPlayersLineOfSightSensor()
		{
		}

		public static Ray AimAtBody(IHTNAgent npc, ref NpcPlayerInfo info)
		{
			HTNPlayer hTNPlayer = npc as HTNPlayer;
			if (hTNPlayer != null)
			{
				return BearEnemyPlayersLineOfSightSensor.AimAtBody(hTNPlayer, ref info);
			}
			HTNAnimal hTNAnimal = npc as HTNAnimal;
			if (hTNAnimal != null)
			{
				return BearEnemyPlayersLineOfSightSensor.AimAtBody(hTNAnimal, ref info);
			}
			return new Ray();
		}

		public static Ray AimAtBody(HTNPlayer npc, ref NpcPlayerInfo info)
		{
			Vector3 vector3 = npc.eyes.position;
			Vector3 vector31 = info.Player.CenterPoint() - npc.CenterPoint();
			return new Ray(vector3, vector31.normalized);
		}

		public static Ray AimAtBody(HTNAnimal npc, ref NpcPlayerInfo info)
		{
			Vector3 vector3 = npc.CenterPoint();
			Vector3 vector31 = info.Player.CenterPoint() - npc.CenterPoint();
			return new Ray(vector3, vector31.normalized);
		}

		public static Ray AimAtHead(IHTNAgent npc, ref NpcPlayerInfo info)
		{
			HTNPlayer hTNPlayer = npc as HTNPlayer;
			if (hTNPlayer != null)
			{
				return BearEnemyPlayersLineOfSightSensor.AimAtHead(hTNPlayer, ref info);
			}
			HTNAnimal hTNAnimal = npc as HTNAnimal;
			if (hTNAnimal != null)
			{
				return BearEnemyPlayersLineOfSightSensor.AimAtHead(hTNAnimal, ref info);
			}
			return new Ray();
		}

		public static Ray AimAtHead(HTNPlayer npc, ref NpcPlayerInfo info)
		{
			Vector3 vector3 = npc.eyes.position;
			Vector3 player = info.Player.eyes.position - npc.CenterPoint();
			return new Ray(vector3, player.normalized);
		}

		public static Ray AimAtHead(HTNAnimal npc, ref NpcPlayerInfo info)
		{
			Vector3 vector3 = npc.CenterPoint();
			Vector3 player = info.Player.eyes.position - npc.CenterPoint();
			return new Ray(vector3, player.normalized);
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			BearEnemyPlayersLineOfSightSensor.TickStatic(npc);
		}

		public static void TickLineOfSightTest(IHTNAgent npc, ref NpcPlayerInfo info)
		{
			BearDomain aiDomain = npc.AiDomain as BearDomain;
			if (aiDomain == null)
			{
				return;
			}
			bool flag = aiDomain.BearContext.IsFact(Facts.IsStandingUp);
			info.HeadVisible = false;
			info.BodyVisible = false;
			if (info.SqrDistance < aiDomain.BearDefinition.SqrAggroRange(flag) && info.ForwardDotDir > npc.AiDefinition.Sensory.FieldOfViewRadians)
			{
				float single = aiDomain.BearDefinition.AggroRange(flag);
				Ray ray = BearEnemyPlayersLineOfSightSensor.AimAtBody(npc, ref info);
				if (info.Player.IsVisible(ray, single))
				{
					info.BodyVisible = true;
					npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Add(info);
					return;
				}
				ray = BearEnemyPlayersLineOfSightSensor.AimAtHead(npc, ref info);
				if (info.Player.IsVisible(ray, single))
				{
					info.HeadVisible = true;
					npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Add(info);
				}
			}
		}

		public static void TickStatic(IHTNAgent npc)
		{
			npc.AiDomain.NpcContext.EnemyPlayersInLineOfSight.Clear();
			List<NpcPlayerInfo> enemyPlayersInRange = npc.AiDomain.NpcContext.EnemyPlayersInRange;
			for (int i = 0; i < enemyPlayersInRange.Count; i++)
			{
				NpcPlayerInfo item = enemyPlayersInRange[i];
				if (item.Player == null || item.Player.transform == null || item.Player.IsDestroyed || item.Player.IsDead())
				{
					enemyPlayersInRange.RemoveAt(i);
					i--;
				}
				else
				{
					BearEnemyPlayersLineOfSightSensor.TickLineOfSightTest(npc, ref item);
				}
			}
		}
	}
}