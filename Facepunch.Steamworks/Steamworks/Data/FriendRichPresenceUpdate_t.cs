using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct FriendRichPresenceUpdate_t
	{
		internal ulong SteamIDFriend;

		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<FriendRichPresenceUpdate_t> actionClient;

		private static Action<FriendRichPresenceUpdate_t> actionServer;

		static FriendRichPresenceUpdate_t()
		{
			FriendRichPresenceUpdate_t.StructSize = Marshal.SizeOf(typeof(FriendRichPresenceUpdate_t));
		}

		internal static FriendRichPresenceUpdate_t Fill(IntPtr p)
		{
			return (FriendRichPresenceUpdate_t)Marshal.PtrToStructure(p, typeof(FriendRichPresenceUpdate_t));
		}

		public static async Task<FriendRichPresenceUpdate_t?> GetResultAsync(SteamAPICall_t handle)
		{
			FriendRichPresenceUpdate_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(FriendRichPresenceUpdate_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, FriendRichPresenceUpdate_t.StructSize, 336, ref flag) | flag))
					{
						nullable = new FriendRichPresenceUpdate_t?(FriendRichPresenceUpdate_t.Fill(intPtr));
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

		public static void Install(Action<FriendRichPresenceUpdate_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(FriendRichPresenceUpdate_t.OnClient), FriendRichPresenceUpdate_t.StructSize, 336, false);
				FriendRichPresenceUpdate_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(FriendRichPresenceUpdate_t.OnServer), FriendRichPresenceUpdate_t.StructSize, 336, true);
				FriendRichPresenceUpdate_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendRichPresenceUpdate_t> action = FriendRichPresenceUpdate_t.actionClient;
			if (action != null)
			{
				action(FriendRichPresenceUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendRichPresenceUpdate_t> action = FriendRichPresenceUpdate_t.actionServer;
			if (action != null)
			{
				action(FriendRichPresenceUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}