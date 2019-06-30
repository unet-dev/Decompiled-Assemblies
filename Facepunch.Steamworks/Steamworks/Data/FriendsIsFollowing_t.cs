using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct FriendsIsFollowing_t
	{
		internal Steamworks.Result Result;

		internal ulong SteamID;

		internal bool IsFollowing;

		internal readonly static int StructSize;

		private static Action<FriendsIsFollowing_t> actionClient;

		private static Action<FriendsIsFollowing_t> actionServer;

		static FriendsIsFollowing_t()
		{
			FriendsIsFollowing_t.StructSize = Marshal.SizeOf(typeof(FriendsIsFollowing_t));
		}

		internal static FriendsIsFollowing_t Fill(IntPtr p)
		{
			return (FriendsIsFollowing_t)Marshal.PtrToStructure(p, typeof(FriendsIsFollowing_t));
		}

		public static async Task<FriendsIsFollowing_t?> GetResultAsync(SteamAPICall_t handle)
		{
			FriendsIsFollowing_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(FriendsIsFollowing_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, FriendsIsFollowing_t.StructSize, 345, ref flag) | flag))
					{
						nullable = new FriendsIsFollowing_t?(FriendsIsFollowing_t.Fill(intPtr));
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

		public static void Install(Action<FriendsIsFollowing_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(FriendsIsFollowing_t.OnClient), FriendsIsFollowing_t.StructSize, 345, false);
				FriendsIsFollowing_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(FriendsIsFollowing_t.OnServer), FriendsIsFollowing_t.StructSize, 345, true);
				FriendsIsFollowing_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendsIsFollowing_t> action = FriendsIsFollowing_t.actionClient;
			if (action != null)
			{
				action(FriendsIsFollowing_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendsIsFollowing_t> action = FriendsIsFollowing_t.actionServer;
			if (action != null)
			{
				action(FriendsIsFollowing_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}