using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct UserStatsStored_t
	{
		internal ulong GameID;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<UserStatsStored_t> actionClient;

		private static Action<UserStatsStored_t> actionServer;

		static UserStatsStored_t()
		{
			UserStatsStored_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(UserStatsStored_t) : typeof(UserStatsStored_t.Pack8)));
		}

		internal static UserStatsStored_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (UserStatsStored_t)Marshal.PtrToStructure(p, typeof(UserStatsStored_t)) : (UserStatsStored_t.Pack8)Marshal.PtrToStructure(p, typeof(UserStatsStored_t.Pack8)));
		}

		public static async Task<UserStatsStored_t?> GetResultAsync(SteamAPICall_t handle)
		{
			UserStatsStored_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(UserStatsStored_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, UserStatsStored_t.StructSize, 1102, ref flag) | flag))
					{
						nullable = new UserStatsStored_t?(UserStatsStored_t.Fill(intPtr));
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

		public static void Install(Action<UserStatsStored_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(UserStatsStored_t.OnClient), UserStatsStored_t.StructSize, 1102, false);
				UserStatsStored_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(UserStatsStored_t.OnServer), UserStatsStored_t.StructSize, 1102, true);
				UserStatsStored_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserStatsStored_t> action = UserStatsStored_t.actionClient;
			if (action != null)
			{
				action(UserStatsStored_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserStatsStored_t> action = UserStatsStored_t.actionServer;
			if (action != null)
			{
				action(UserStatsStored_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong GameID;

			internal Steamworks.Result Result;

			public static implicit operator UserStatsStored_t(UserStatsStored_t.Pack8 d)
			{
				UserStatsStored_t userStatsStoredT = new UserStatsStored_t()
				{
					GameID = d.GameID,
					Result = d.Result
				};
				return userStatsStoredT;
			}

			public static implicit operator Pack8(UserStatsStored_t d)
			{
				UserStatsStored_t.Pack8 pack8 = new UserStatsStored_t.Pack8()
				{
					GameID = d.GameID,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}