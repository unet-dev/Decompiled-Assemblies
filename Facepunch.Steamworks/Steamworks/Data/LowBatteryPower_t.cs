using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LowBatteryPower_t
	{
		internal byte MinutesBatteryLeft;

		internal readonly static int StructSize;

		private static Action<LowBatteryPower_t> actionClient;

		private static Action<LowBatteryPower_t> actionServer;

		static LowBatteryPower_t()
		{
			LowBatteryPower_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LowBatteryPower_t) : typeof(LowBatteryPower_t.Pack8)));
		}

		internal static LowBatteryPower_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LowBatteryPower_t)Marshal.PtrToStructure(p, typeof(LowBatteryPower_t)) : (LowBatteryPower_t.Pack8)Marshal.PtrToStructure(p, typeof(LowBatteryPower_t.Pack8)));
		}

		public static async Task<LowBatteryPower_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LowBatteryPower_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LowBatteryPower_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LowBatteryPower_t.StructSize, 702, ref flag) | flag))
					{
						nullable = new LowBatteryPower_t?(LowBatteryPower_t.Fill(intPtr));
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

		public static void Install(Action<LowBatteryPower_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LowBatteryPower_t.OnClient), LowBatteryPower_t.StructSize, 702, false);
				LowBatteryPower_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LowBatteryPower_t.OnServer), LowBatteryPower_t.StructSize, 702, true);
				LowBatteryPower_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LowBatteryPower_t> action = LowBatteryPower_t.actionClient;
			if (action != null)
			{
				action(LowBatteryPower_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LowBatteryPower_t> action = LowBatteryPower_t.actionServer;
			if (action != null)
			{
				action(LowBatteryPower_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal byte MinutesBatteryLeft;

			public static implicit operator LowBatteryPower_t(LowBatteryPower_t.Pack8 d)
			{
				return new LowBatteryPower_t()
				{
					MinutesBatteryLeft = d.MinutesBatteryLeft
				};
			}

			public static implicit operator Pack8(LowBatteryPower_t d)
			{
				return new LowBatteryPower_t.Pack8()
				{
					MinutesBatteryLeft = d.MinutesBatteryLeft
				};
			}
		}
	}
}