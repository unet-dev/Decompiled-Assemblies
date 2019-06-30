using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct AvailableBeaconLocationsUpdated_t
	{
		internal readonly static int StructSize;

		private static Action<AvailableBeaconLocationsUpdated_t> actionClient;

		private static Action<AvailableBeaconLocationsUpdated_t> actionServer;

		static AvailableBeaconLocationsUpdated_t()
		{
			AvailableBeaconLocationsUpdated_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(AvailableBeaconLocationsUpdated_t) : typeof(AvailableBeaconLocationsUpdated_t.Pack8)));
		}

		internal static AvailableBeaconLocationsUpdated_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (AvailableBeaconLocationsUpdated_t)Marshal.PtrToStructure(p, typeof(AvailableBeaconLocationsUpdated_t)) : (AvailableBeaconLocationsUpdated_t.Pack8)Marshal.PtrToStructure(p, typeof(AvailableBeaconLocationsUpdated_t.Pack8)));
		}

		public static async Task<AvailableBeaconLocationsUpdated_t?> GetResultAsync(SteamAPICall_t handle)
		{
			AvailableBeaconLocationsUpdated_t? nullable;
			bool flag = false;
			while (!SteamUtils.IsCallComplete(handle, out flag))
			{
				await Task.Delay(1);
				if ((SteamClient.IsValid ? false : !SteamServer.IsValid))
				{
					nullable = null;
					return nullable;
				}
			}
			if (!flag)
			{
				IntPtr intPtr = Marshal.AllocHGlobal(AvailableBeaconLocationsUpdated_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, AvailableBeaconLocationsUpdated_t.StructSize, 5305, ref flag) | flag))
					{
						nullable = new AvailableBeaconLocationsUpdated_t?(AvailableBeaconLocationsUpdated_t.Fill(intPtr));
					}
					else
					{
						nullable = null;
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static void Install(Action<AvailableBeaconLocationsUpdated_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(AvailableBeaconLocationsUpdated_t.OnClient), AvailableBeaconLocationsUpdated_t.StructSize, 5305, false);
				AvailableBeaconLocationsUpdated_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(AvailableBeaconLocationsUpdated_t.OnServer), AvailableBeaconLocationsUpdated_t.StructSize, 5305, true);
				AvailableBeaconLocationsUpdated_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AvailableBeaconLocationsUpdated_t> action = AvailableBeaconLocationsUpdated_t.actionClient;
			if (action != null)
			{
				action(AvailableBeaconLocationsUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AvailableBeaconLocationsUpdated_t> action = AvailableBeaconLocationsUpdated_t.actionServer;
			if (action != null)
			{
				action(AvailableBeaconLocationsUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator AvailableBeaconLocationsUpdated_t(AvailableBeaconLocationsUpdated_t.Pack8 d)
			{
				return new AvailableBeaconLocationsUpdated_t();
			}

			public static implicit operator Pack8(AvailableBeaconLocationsUpdated_t d)
			{
				return new AvailableBeaconLocationsUpdated_t.Pack8();
			}
		}
	}
}