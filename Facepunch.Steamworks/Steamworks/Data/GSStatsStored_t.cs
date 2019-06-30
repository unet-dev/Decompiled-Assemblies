using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSStatsStored_t
	{
		internal Steamworks.Result Result;

		internal ulong SteamIDUser;

		internal readonly static int StructSize;

		private static Action<GSStatsStored_t> actionClient;

		private static Action<GSStatsStored_t> actionServer;

		static GSStatsStored_t()
		{
			GSStatsStored_t.StructSize = Marshal.SizeOf(typeof(GSStatsStored_t));
		}

		internal static GSStatsStored_t Fill(IntPtr p)
		{
			return (GSStatsStored_t)Marshal.PtrToStructure(p, typeof(GSStatsStored_t));
		}

		public static async Task<GSStatsStored_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSStatsStored_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSStatsStored_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSStatsStored_t.StructSize, 1801, ref flag) | flag))
					{
						nullable = new GSStatsStored_t?(GSStatsStored_t.Fill(intPtr));
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

		public static void Install(Action<GSStatsStored_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSStatsStored_t.OnClient), GSStatsStored_t.StructSize, 1801, false);
				GSStatsStored_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSStatsStored_t.OnServer), GSStatsStored_t.StructSize, 1801, true);
				GSStatsStored_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSStatsStored_t> action = GSStatsStored_t.actionClient;
			if (action != null)
			{
				action(GSStatsStored_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSStatsStored_t> action = GSStatsStored_t.actionServer;
			if (action != null)
			{
				action(GSStatsStored_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}