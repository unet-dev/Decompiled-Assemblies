using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct UserAchievementStored_t
	{
		internal ulong GameID;

		internal bool GroupAchievement;

		internal string AchievementName;

		internal uint CurProgress;

		internal uint MaxProgress;

		internal readonly static int StructSize;

		private static Action<UserAchievementStored_t> actionClient;

		private static Action<UserAchievementStored_t> actionServer;

		static UserAchievementStored_t()
		{
			UserAchievementStored_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(UserAchievementStored_t) : typeof(UserAchievementStored_t.Pack8)));
		}

		internal static UserAchievementStored_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (UserAchievementStored_t)Marshal.PtrToStructure(p, typeof(UserAchievementStored_t)) : (UserAchievementStored_t.Pack8)Marshal.PtrToStructure(p, typeof(UserAchievementStored_t.Pack8)));
		}

		public static async Task<UserAchievementStored_t?> GetResultAsync(SteamAPICall_t handle)
		{
			UserAchievementStored_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(UserAchievementStored_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, UserAchievementStored_t.StructSize, 1103, ref flag) | flag))
					{
						nullable = new UserAchievementStored_t?(UserAchievementStored_t.Fill(intPtr));
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

		public static void Install(Action<UserAchievementStored_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(UserAchievementStored_t.OnClient), UserAchievementStored_t.StructSize, 1103, false);
				UserAchievementStored_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(UserAchievementStored_t.OnServer), UserAchievementStored_t.StructSize, 1103, true);
				UserAchievementStored_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserAchievementStored_t> action = UserAchievementStored_t.actionClient;
			if (action != null)
			{
				action(UserAchievementStored_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserAchievementStored_t> action = UserAchievementStored_t.actionServer;
			if (action != null)
			{
				action(UserAchievementStored_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong GameID;

			internal bool GroupAchievement;

			internal string AchievementName;

			internal uint CurProgress;

			internal uint MaxProgress;

			public static implicit operator UserAchievementStored_t(UserAchievementStored_t.Pack8 d)
			{
				UserAchievementStored_t userAchievementStoredT = new UserAchievementStored_t()
				{
					GameID = d.GameID,
					GroupAchievement = d.GroupAchievement,
					AchievementName = d.AchievementName,
					CurProgress = d.CurProgress,
					MaxProgress = d.MaxProgress
				};
				return userAchievementStoredT;
			}

			public static implicit operator Pack8(UserAchievementStored_t d)
			{
				UserAchievementStored_t.Pack8 pack8 = new UserAchievementStored_t.Pack8()
				{
					GameID = d.GameID,
					GroupAchievement = d.GroupAchievement,
					AchievementName = d.AchievementName,
					CurProgress = d.CurProgress,
					MaxProgress = d.MaxProgress
				};
				return pack8;
			}
		}
	}
}