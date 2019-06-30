using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct FriendsGetFollowerCount_t
	{
		internal Steamworks.Result Result;

		internal ulong SteamID;

		internal int Count;

		internal readonly static int StructSize;

		private static Action<FriendsGetFollowerCount_t> actionClient;

		private static Action<FriendsGetFollowerCount_t> actionServer;

		static FriendsGetFollowerCount_t()
		{
			FriendsGetFollowerCount_t.StructSize = Marshal.SizeOf(typeof(FriendsGetFollowerCount_t));
		}

		internal static FriendsGetFollowerCount_t Fill(IntPtr p)
		{
			return (FriendsGetFollowerCount_t)Marshal.PtrToStructure(p, typeof(FriendsGetFollowerCount_t));
		}

		public static async Task<FriendsGetFollowerCount_t?> GetResultAsync(SteamAPICall_t handle)
		{
			FriendsGetFollowerCount_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(FriendsGetFollowerCount_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, FriendsGetFollowerCount_t.StructSize, 344, ref flag) | flag))
					{
						nullable = new FriendsGetFollowerCount_t?(FriendsGetFollowerCount_t.Fill(intPtr));
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

		public static void Install(Action<FriendsGetFollowerCount_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(FriendsGetFollowerCount_t.OnClient), FriendsGetFollowerCount_t.StructSize, 344, false);
				FriendsGetFollowerCount_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(FriendsGetFollowerCount_t.OnServer), FriendsGetFollowerCount_t.StructSize, 344, true);
				FriendsGetFollowerCount_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendsGetFollowerCount_t> action = FriendsGetFollowerCount_t.actionClient;
			if (action != null)
			{
				action(FriendsGetFollowerCount_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendsGetFollowerCount_t> action = FriendsGetFollowerCount_t.actionServer;
			if (action != null)
			{
				action(FriendsGetFollowerCount_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}