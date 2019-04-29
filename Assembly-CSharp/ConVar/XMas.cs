using System;
using UnityEngine;

namespace ConVar
{
	[Factory("xmas")]
	public class XMas : ConsoleSystem
	{
		private const string path = "assets/prefabs/misc/xmas/xmasrefill.prefab";

		[ServerVar]
		public static bool enabled;

		[ServerVar]
		public static float spawnRange;

		[ServerVar]
		public static int spawnAttempts;

		[ServerVar]
		public static int giftsPerPlayer;

		static XMas()
		{
			XMas.enabled = false;
			XMas.spawnRange = 40f;
			XMas.spawnAttempts = 5;
			XMas.giftsPerPlayer = 2;
		}

		public XMas()
		{
		}

		[ServerVar]
		public static void refill(ConsoleSystem.Arg arg)
		{
			GameManager gameManager = GameManager.server;
			Vector3 vector3 = new Vector3();
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity("assets/prefabs/misc/xmas/xmasrefill.prefab", vector3, quaternion, true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
	}
}