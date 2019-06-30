using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct FriendsEnumerateFollowingList_t
	{
		internal Steamworks.Result Result;

		internal ulong[] GSteamID;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal readonly static int StructSize;

		private static Action<FriendsEnumerateFollowingList_t> actionClient;

		private static Action<FriendsEnumerateFollowingList_t> actionServer;

		static FriendsEnumerateFollowingList_t()
		{
			FriendsEnumerateFollowingList_t.StructSize = Marshal.SizeOf(typeof(FriendsEnumerateFollowingList_t));
		}

		internal static FriendsEnumerateFollowingList_t Fill(IntPtr p)
		{
			return (FriendsEnumerateFollowingList_t)Marshal.PtrToStructure(p, typeof(FriendsEnumerateFollowingList_t));
		}

		public static async Task<FriendsEnumerateFollowingList_t?> GetResultAsync(SteamAPICall_t handle)
		{
			FriendsEnumerateFollowingList_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(FriendsEnumerateFollowingList_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, FriendsEnumerateFollowingList_t.StructSize, 346, ref flag) | flag))
					{
						nullable = new FriendsEnumerateFollowingList_t?(FriendsEnumerateFollowingList_t.Fill(intPtr));
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

		public static void Install(Action<FriendsEnumerateFollowingList_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(FriendsEnumerateFollowingList_t.OnClient), FriendsEnumerateFollowingList_t.StructSize, 346, false);
				FriendsEnumerateFollowingList_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(FriendsEnumerateFollowingList_t.OnServer), FriendsEnumerateFollowingList_t.StructSize, 346, true);
				FriendsEnumerateFollowingList_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendsEnumerateFollowingList_t> action = FriendsEnumerateFollowingList_t.actionClient;
			if (action != null)
			{
				action(FriendsEnumerateFollowingList_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendsEnumerateFollowingList_t> action = FriendsEnumerateFollowingList_t.actionServer;
			if (action != null)
			{
				action(FriendsEnumerateFollowingList_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}