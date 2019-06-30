using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSStatsUnloaded_t
	{
		internal ulong SteamIDUser;

		internal readonly static int StructSize;

		private static Action<GSStatsUnloaded_t> actionClient;

		private static Action<GSStatsUnloaded_t> actionServer;

		static GSStatsUnloaded_t()
		{
			GSStatsUnloaded_t.StructSize = Marshal.SizeOf(typeof(GSStatsUnloaded_t));
		}

		internal static GSStatsUnloaded_t Fill(IntPtr p)
		{
			return (GSStatsUnloaded_t)Marshal.PtrToStructure(p, typeof(GSStatsUnloaded_t));
		}

		public static async Task<GSStatsUnloaded_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSStatsUnloaded_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSStatsUnloaded_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSStatsUnloaded_t.StructSize, 1108, ref flag) | flag))
					{
						nullable = new GSStatsUnloaded_t?(GSStatsUnloaded_t.Fill(intPtr));
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

		public static void Install(Action<GSStatsUnloaded_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSStatsUnloaded_t.OnClient), GSStatsUnloaded_t.StructSize, 1108, false);
				GSStatsUnloaded_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSStatsUnloaded_t.OnServer), GSStatsUnloaded_t.StructSize, 1108, true);
				GSStatsUnloaded_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSStatsUnloaded_t> action = GSStatsUnloaded_t.actionClient;
			if (action != null)
			{
				action(GSStatsUnloaded_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSStatsUnloaded_t> action = GSStatsUnloaded_t.actionServer;
			if (action != null)
			{
				action(GSStatsUnloaded_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}