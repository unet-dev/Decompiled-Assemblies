using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct StopPlaytimeTrackingResult_t
	{
		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<StopPlaytimeTrackingResult_t> actionClient;

		private static Action<StopPlaytimeTrackingResult_t> actionServer;

		static StopPlaytimeTrackingResult_t()
		{
			StopPlaytimeTrackingResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(StopPlaytimeTrackingResult_t) : typeof(StopPlaytimeTrackingResult_t.Pack8)));
		}

		internal static StopPlaytimeTrackingResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (StopPlaytimeTrackingResult_t)Marshal.PtrToStructure(p, typeof(StopPlaytimeTrackingResult_t)) : (StopPlaytimeTrackingResult_t.Pack8)Marshal.PtrToStructure(p, typeof(StopPlaytimeTrackingResult_t.Pack8)));
		}

		public static async Task<StopPlaytimeTrackingResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			StopPlaytimeTrackingResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(StopPlaytimeTrackingResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, StopPlaytimeTrackingResult_t.StructSize, 3411, ref flag) | flag))
					{
						nullable = new StopPlaytimeTrackingResult_t?(StopPlaytimeTrackingResult_t.Fill(intPtr));
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

		public static void Install(Action<StopPlaytimeTrackingResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(StopPlaytimeTrackingResult_t.OnClient), StopPlaytimeTrackingResult_t.StructSize, 3411, false);
				StopPlaytimeTrackingResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(StopPlaytimeTrackingResult_t.OnServer), StopPlaytimeTrackingResult_t.StructSize, 3411, true);
				StopPlaytimeTrackingResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<StopPlaytimeTrackingResult_t> action = StopPlaytimeTrackingResult_t.actionClient;
			if (action != null)
			{
				action(StopPlaytimeTrackingResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<StopPlaytimeTrackingResult_t> action = StopPlaytimeTrackingResult_t.actionServer;
			if (action != null)
			{
				action(StopPlaytimeTrackingResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			public static implicit operator StopPlaytimeTrackingResult_t(StopPlaytimeTrackingResult_t.Pack8 d)
			{
				return new StopPlaytimeTrackingResult_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(StopPlaytimeTrackingResult_t d)
			{
				return new StopPlaytimeTrackingResult_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}