using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameConnectedFriendChatMsg_t
	{
		internal ulong SteamIDUser;

		internal int MessageID;

		internal readonly static int StructSize;

		private static Action<GameConnectedFriendChatMsg_t> actionClient;

		private static Action<GameConnectedFriendChatMsg_t> actionServer;

		static GameConnectedFriendChatMsg_t()
		{
			GameConnectedFriendChatMsg_t.StructSize = Marshal.SizeOf(typeof(GameConnectedFriendChatMsg_t));
		}

		internal static GameConnectedFriendChatMsg_t Fill(IntPtr p)
		{
			return (GameConnectedFriendChatMsg_t)Marshal.PtrToStructure(p, typeof(GameConnectedFriendChatMsg_t));
		}

		public static async Task<GameConnectedFriendChatMsg_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameConnectedFriendChatMsg_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameConnectedFriendChatMsg_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameConnectedFriendChatMsg_t.StructSize, 343, ref flag) | flag))
					{
						nullable = new GameConnectedFriendChatMsg_t?(GameConnectedFriendChatMsg_t.Fill(intPtr));
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

		public static void Install(Action<GameConnectedFriendChatMsg_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameConnectedFriendChatMsg_t.OnClient), GameConnectedFriendChatMsg_t.StructSize, 343, false);
				GameConnectedFriendChatMsg_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameConnectedFriendChatMsg_t.OnServer), GameConnectedFriendChatMsg_t.StructSize, 343, true);
				GameConnectedFriendChatMsg_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameConnectedFriendChatMsg_t> action = GameConnectedFriendChatMsg_t.actionClient;
			if (action != null)
			{
				action(GameConnectedFriendChatMsg_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameConnectedFriendChatMsg_t> action = GameConnectedFriendChatMsg_t.actionServer;
			if (action != null)
			{
				action(GameConnectedFriendChatMsg_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}