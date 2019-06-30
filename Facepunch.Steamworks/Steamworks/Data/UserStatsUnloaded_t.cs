using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct UserStatsUnloaded_t
	{
		internal ulong SteamIDUser;

		internal readonly static int StructSize;

		private static Action<UserStatsUnloaded_t> actionClient;

		private static Action<UserStatsUnloaded_t> actionServer;

		static UserStatsUnloaded_t()
		{
			UserStatsUnloaded_t.StructSize = Marshal.SizeOf(typeof(UserStatsUnloaded_t));
		}

		internal static UserStatsUnloaded_t Fill(IntPtr p)
		{
			return (UserStatsUnloaded_t)Marshal.PtrToStructure(p, typeof(UserStatsUnloaded_t));
		}

		public static async Task<UserStatsUnloaded_t?> GetResultAsync(SteamAPICall_t handle)
		{
			UserStatsUnloaded_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(UserStatsUnloaded_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, UserStatsUnloaded_t.StructSize, 1108, ref flag) | flag))
					{
						nullable = new UserStatsUnloaded_t?(UserStatsUnloaded_t.Fill(intPtr));
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

		public static void Install(Action<UserStatsUnloaded_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(UserStatsUnloaded_t.OnClient), UserStatsUnloaded_t.StructSize, 1108, false);
				UserStatsUnloaded_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(UserStatsUnloaded_t.OnServer), UserStatsUnloaded_t.StructSize, 1108, true);
				UserStatsUnloaded_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserStatsUnloaded_t> action = UserStatsUnloaded_t.actionClient;
			if (action != null)
			{
				action(UserStatsUnloaded_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserStatsUnloaded_t> action = UserStatsUnloaded_t.actionServer;
			if (action != null)
			{
				action(UserStatsUnloaded_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}