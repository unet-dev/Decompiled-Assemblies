using System;
using UnityEngine;

namespace ConVar
{
	[Factory("bradley")]
	public class Bradley : ConsoleSystem
	{
		[ServerVar]
		public static float respawnDelayMinutes;

		[ServerVar]
		public static float respawnDelayVariance;

		[ServerVar]
		public static bool enabled;

		static Bradley()
		{
			Bradley.respawnDelayMinutes = 60f;
			Bradley.respawnDelayVariance = 1f;
			Bradley.enabled = true;
		}

		public Bradley()
		{
		}

		[ServerVar]
		public static void quickrespawn(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			BradleySpawner bradleySpawner = BradleySpawner.singleton;
			if (bradleySpawner == null)
			{
				Debug.LogWarning("No Spawner");
				return;
			}
			if (bradleySpawner.spawned)
			{
				bradleySpawner.spawned.Kill(BaseNetworkable.DestroyMode.None);
			}
			bradleySpawner.spawned = null;
			bradleySpawner.DoRespawn();
		}
	}
}