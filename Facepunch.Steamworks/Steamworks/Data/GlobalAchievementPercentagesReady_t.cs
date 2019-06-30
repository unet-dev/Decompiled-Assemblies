using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GlobalAchievementPercentagesReady_t
	{
		internal ulong GameID;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<GlobalAchievementPercentagesReady_t> actionClient;

		private static Action<GlobalAchievementPercentagesReady_t> actionServer;

		static GlobalAchievementPercentagesReady_t()
		{
			GlobalAchievementPercentagesReady_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GlobalAchievementPercentagesReady_t) : typeof(GlobalAchievementPercentagesReady_t.Pack8)));
		}

		internal static GlobalAchievementPercentagesReady_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GlobalAchievementPercentagesReady_t)Marshal.PtrToStructure(p, typeof(GlobalAchievementPercentagesReady_t)) : (GlobalAchievementPercentagesReady_t.Pack8)Marshal.PtrToStructure(p, typeof(GlobalAchievementPercentagesReady_t.Pack8)));
		}

		public static async Task<GlobalAchievementPercentagesReady_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GlobalAchievementPercentagesReady_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GlobalAchievementPercentagesReady_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GlobalAchievementPercentagesReady_t.StructSize, 1110, ref flag) | flag))
					{
						nullable = new GlobalAchievementPercentagesReady_t?(GlobalAchievementPercentagesReady_t.Fill(intPtr));
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

		public static void Install(Action<GlobalAchievementPercentagesReady_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GlobalAchievementPercentagesReady_t.OnClient), GlobalAchievementPercentagesReady_t.StructSize, 1110, false);
				GlobalAchievementPercentagesReady_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GlobalAchievementPercentagesReady_t.OnServer), GlobalAchievementPercentagesReady_t.StructSize, 1110, true);
				GlobalAchievementPercentagesReady_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GlobalAchievementPercentagesReady_t> action = GlobalAchievementPercentagesReady_t.actionClient;
			if (action != null)
			{
				action(GlobalAchievementPercentagesReady_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GlobalAchievementPercentagesReady_t> action = GlobalAchievementPercentagesReady_t.actionServer;
			if (action != null)
			{
				action(GlobalAchievementPercentagesReady_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong GameID;

			internal Steamworks.Result Result;

			public static implicit operator GlobalAchievementPercentagesReady_t(GlobalAchievementPercentagesReady_t.Pack8 d)
			{
				GlobalAchievementPercentagesReady_t globalAchievementPercentagesReadyT = new GlobalAchievementPercentagesReady_t()
				{
					GameID = d.GameID,
					Result = d.Result
				};
				return globalAchievementPercentagesReadyT;
			}

			public static implicit operator Pack8(GlobalAchievementPercentagesReady_t d)
			{
				GlobalAchievementPercentagesReady_t.Pack8 pack8 = new GlobalAchievementPercentagesReady_t.Pack8()
				{
					GameID = d.GameID,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}