using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct StartPlaytimeTrackingResult_t
	{
		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<StartPlaytimeTrackingResult_t> actionClient;

		private static Action<StartPlaytimeTrackingResult_t> actionServer;

		static StartPlaytimeTrackingResult_t()
		{
			StartPlaytimeTrackingResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(StartPlaytimeTrackingResult_t) : typeof(StartPlaytimeTrackingResult_t.Pack8)));
		}

		internal static StartPlaytimeTrackingResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (StartPlaytimeTrackingResult_t)Marshal.PtrToStructure(p, typeof(StartPlaytimeTrackingResult_t)) : (StartPlaytimeTrackingResult_t.Pack8)Marshal.PtrToStructure(p, typeof(StartPlaytimeTrackingResult_t.Pack8)));
		}

		public static async Task<StartPlaytimeTrackingResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			StartPlaytimeTrackingResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(StartPlaytimeTrackingResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, StartPlaytimeTrackingResult_t.StructSize, 3410, ref flag) | flag))
					{
						nullable = new StartPlaytimeTrackingResult_t?(StartPlaytimeTrackingResult_t.Fill(intPtr));
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

		public static void Install(Action<StartPlaytimeTrackingResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(StartPlaytimeTrackingResult_t.OnClient), StartPlaytimeTrackingResult_t.StructSize, 3410, false);
				StartPlaytimeTrackingResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(StartPlaytimeTrackingResult_t.OnServer), StartPlaytimeTrackingResult_t.StructSize, 3410, true);
				StartPlaytimeTrackingResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<StartPlaytimeTrackingResult_t> action = StartPlaytimeTrackingResult_t.actionClient;
			if (action != null)
			{
				action(StartPlaytimeTrackingResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<StartPlaytimeTrackingResult_t> action = StartPlaytimeTrackingResult_t.actionServer;
			if (action != null)
			{
				action(StartPlaytimeTrackingResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			public static implicit operator StartPlaytimeTrackingResult_t(StartPlaytimeTrackingResult_t.Pack8 d)
			{
				return new StartPlaytimeTrackingResult_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(StartPlaytimeTrackingResult_t d)
			{
				return new StartPlaytimeTrackingResult_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}