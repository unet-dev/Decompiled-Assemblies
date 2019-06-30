using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GlobalStatsReceived_t
	{
		internal ulong GameID;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<GlobalStatsReceived_t> actionClient;

		private static Action<GlobalStatsReceived_t> actionServer;

		static GlobalStatsReceived_t()
		{
			GlobalStatsReceived_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GlobalStatsReceived_t) : typeof(GlobalStatsReceived_t.Pack8)));
		}

		internal static GlobalStatsReceived_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GlobalStatsReceived_t)Marshal.PtrToStructure(p, typeof(GlobalStatsReceived_t)) : (GlobalStatsReceived_t.Pack8)Marshal.PtrToStructure(p, typeof(GlobalStatsReceived_t.Pack8)));
		}

		public static async Task<GlobalStatsReceived_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GlobalStatsReceived_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GlobalStatsReceived_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GlobalStatsReceived_t.StructSize, 1112, ref flag) | flag))
					{
						nullable = new GlobalStatsReceived_t?(GlobalStatsReceived_t.Fill(intPtr));
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

		public static void Install(Action<GlobalStatsReceived_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GlobalStatsReceived_t.OnClient), GlobalStatsReceived_t.StructSize, 1112, false);
				GlobalStatsReceived_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GlobalStatsReceived_t.OnServer), GlobalStatsReceived_t.StructSize, 1112, true);
				GlobalStatsReceived_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GlobalStatsReceived_t> action = GlobalStatsReceived_t.actionClient;
			if (action != null)
			{
				action(GlobalStatsReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GlobalStatsReceived_t> action = GlobalStatsReceived_t.actionServer;
			if (action != null)
			{
				action(GlobalStatsReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong GameID;

			internal Steamworks.Result Result;

			public static implicit operator GlobalStatsReceived_t(GlobalStatsReceived_t.Pack8 d)
			{
				GlobalStatsReceived_t globalStatsReceivedT = new GlobalStatsReceived_t()
				{
					GameID = d.GameID,
					Result = d.Result
				};
				return globalStatsReceivedT;
			}

			public static implicit operator Pack8(GlobalStatsReceived_t d)
			{
				GlobalStatsReceived_t.Pack8 pack8 = new GlobalStatsReceived_t.Pack8()
				{
					GameID = d.GameID,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}