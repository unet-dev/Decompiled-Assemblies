using System;
using UnityEngine;

namespace ConVar
{
	[Factory("vehicle")]
	public class vehicle : ConsoleSystem
	{
		[Help("how long until boat corpses despawn")]
		[ServerVar]
		public static float boat_corpse_seconds;

		static vehicle()
		{
			vehicle.boat_corpse_seconds = 300f;
		}

		public vehicle()
		{
		}

		[ServerUserVar]
		public static void swapseats(ConsoleSystem.Arg arg)
		{
			int num = 0;
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (basePlayer.SwapSeatCooldown())
			{
				return;
			}
			BaseMountable mounted = basePlayer.GetMounted();
			if (mounted == null)
			{
				return;
			}
			BaseVehicle component = mounted.GetComponent<BaseVehicle>();
			if (component == null)
			{
				component = mounted.VehicleParent();
			}
			if (component == null)
			{
				return;
			}
			component.SwapSeats(basePlayer, num);
		}
	}
}