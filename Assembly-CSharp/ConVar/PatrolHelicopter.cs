using System;
using UnityEngine;

namespace ConVar
{
	[Factory("heli")]
	public class PatrolHelicopter : ConsoleSystem
	{
		private const string path = "assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab";

		[ServerVar]
		public static float lifetimeMinutes;

		[ServerVar]
		public static int guns;

		[ServerVar]
		public static float bulletDamageScale;

		[ServerVar]
		public static float bulletAccuracy;

		static PatrolHelicopter()
		{
			PatrolHelicopter.lifetimeMinutes = 15f;
			PatrolHelicopter.guns = 1;
			PatrolHelicopter.bulletDamageScale = 1f;
			PatrolHelicopter.bulletAccuracy = 2f;
		}

		public PatrolHelicopter()
		{
		}

		[ServerVar]
		public static void call(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			Debug.Log("Helicopter inbound");
			GameManager gameManager = GameManager.server;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}

		[ServerVar]
		public static void calltome(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log(string.Concat("heli called to : ", basePlayer.transform.position));
			GameManager gameManager = GameManager.server;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.GetComponent<PatrolHelicopterAI>().SetInitialDestination(basePlayer.transform.position + new Vector3(0f, 10f, 0f), 0.25f);
				baseEntity.Spawn();
			}
		}

		[ServerVar]
		public static void drop(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log(string.Concat("heli called to : ", basePlayer.transform.position));
			GameManager gameManager = GameManager.server;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.GetComponent<PatrolHelicopterAI>().SetInitialDestination(basePlayer.transform.position + new Vector3(0f, 10f, 0f), 0f);
				baseEntity.Spawn();
			}
		}

		[ServerVar]
		public static void strafe(ConsoleSystem.Arg arg)
		{
			RaycastHit raycastHit;
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			PatrolHelicopterAI patrolHelicopterAI = PatrolHelicopterAI.heliInstance;
			if (patrolHelicopterAI == null)
			{
				Debug.Log("no heli instance");
				return;
			}
			if (!UnityEngine.Physics.Raycast(basePlayer.eyes.HeadRay(), out raycastHit, 1000f, 1218652417))
			{
				Debug.Log("strafe ray missed");
				return;
			}
			Debug.Log(string.Concat("strafing :", raycastHit.point));
			patrolHelicopterAI.interestZoneOrigin = raycastHit.point;
			patrolHelicopterAI.ExitCurrentState();
			patrolHelicopterAI.State_Strafe_Enter(raycastHit.point, false);
		}

		[ServerVar]
		public static void testpuzzle(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			bool isDeveloper = basePlayer.IsDeveloper;
		}
	}
}