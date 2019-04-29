using System;
using UnityEngine;

namespace ConVar
{
	[Factory("supply")]
	public class Supply : ConsoleSystem
	{
		private const string path = "assets/prefabs/npc/cargo plane/cargo_plane.prefab";

		public Supply()
		{
		}

		[ServerVar]
		public static void call(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			Debug.Log("Supply Drop Inbound");
			GameManager gameManager = GameManager.server;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", vector3, quaternion, true);
			if (baseEntity)
			{
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
			Debug.Log("Supply Drop Inbound");
			GameManager gameManager = GameManager.server;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.GetComponent<CargoPlane>().InitDropPosition(basePlayer.transform.position + new Vector3(0f, 10f, 0f));
				baseEntity.Spawn();
			}
		}
	}
}