using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct UserStatsReceived_t
	{
		internal ulong GameID;

		internal Steamworks.Result Result;

		internal ulong SteamIDUser;

		internal readonly static int StructSize;

		private static Action<UserStatsReceived_t> actionClient;

		private static Action<UserStatsReceived_t> actionServer;

		static UserStatsReceived_t()
		{
			UserStatsReceived_t.StructSize = Marshal.SizeOf(typeof(UserStatsReceived_t));
		}

		internal static UserStatsReceived_t Fill(IntPtr p)
		{
			return (UserStatsReceived_t)Marshal.PtrToStructure(p, typeof(UserStatsReceived_t));
		}

		public static async Task<UserStatsReceived_t?> GetResultAsync(SteamAPICall_t handle)
		{
			UserStatsReceived_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(UserStatsReceived_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, UserStatsReceived_t.StructSize, 1101, ref flag) | flag))
					{
						nullable = new UserStatsReceived_t?(UserStatsReceived_t.Fill(intPtr));
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

		public static void Install(Action<UserStatsReceived_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(UserStatsReceived_t.OnClient), UserStatsReceived_t.StructSize, 1101, false);
				UserStatsReceived_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(UserStatsReceived_t.OnServer), UserStatsReceived_t.StructSize, 1101, true);
				UserStatsReceived_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserStatsReceived_t> action = UserStatsReceived_t.actionClient;
			if (action != null)
			{
				action(UserStatsReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserStatsReceived_t> action = UserStatsReceived_t.actionServer;
			if (action != null)
			{
				action(UserStatsReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}