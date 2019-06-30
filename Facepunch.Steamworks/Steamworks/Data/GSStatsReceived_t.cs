using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSStatsReceived_t
	{
		internal Steamworks.Result Result;

		internal ulong SteamIDUser;

		internal readonly static int StructSize;

		private static Action<GSStatsReceived_t> actionClient;

		private static Action<GSStatsReceived_t> actionServer;

		static GSStatsReceived_t()
		{
			GSStatsReceived_t.StructSize = Marshal.SizeOf(typeof(GSStatsReceived_t));
		}

		internal static GSStatsReceived_t Fill(IntPtr p)
		{
			return (GSStatsReceived_t)Marshal.PtrToStructure(p, typeof(GSStatsReceived_t));
		}

		public static async Task<GSStatsReceived_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSStatsReceived_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSStatsReceived_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSStatsReceived_t.StructSize, 1800, ref flag) | flag))
					{
						nullable = new GSStatsReceived_t?(GSStatsReceived_t.Fill(intPtr));
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

		public static void Install(Action<GSStatsReceived_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSStatsReceived_t.OnClient), GSStatsReceived_t.StructSize, 1800, false);
				GSStatsReceived_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSStatsReceived_t.OnServer), GSStatsReceived_t.StructSize, 1800, true);
				GSStatsReceived_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSStatsReceived_t> action = GSStatsReceived_t.actionClient;
			if (action != null)
			{
				action(GSStatsReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSStatsReceived_t> action = GSStatsReceived_t.actionServer;
			if (action != null)
			{
				action(GSStatsReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}