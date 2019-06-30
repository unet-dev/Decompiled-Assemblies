using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GameConnectedClanChatMsg_t
	{
		internal ulong SteamIDClanChat;

		internal ulong SteamIDUser;

		internal int MessageID;

		internal readonly static int StructSize;

		private static Action<GameConnectedClanChatMsg_t> actionClient;

		private static Action<GameConnectedClanChatMsg_t> actionServer;

		static GameConnectedClanChatMsg_t()
		{
			GameConnectedClanChatMsg_t.StructSize = Marshal.SizeOf(typeof(GameConnectedClanChatMsg_t));
		}

		internal static GameConnectedClanChatMsg_t Fill(IntPtr p)
		{
			return (GameConnectedClanChatMsg_t)Marshal.PtrToStructure(p, typeof(GameConnectedClanChatMsg_t));
		}

		public static async Task<GameConnectedClanChatMsg_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GameConnectedClanChatMsg_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GameConnectedClanChatMsg_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GameConnectedClanChatMsg_t.StructSize, 338, ref flag) | flag))
					{
						nullable = new GameConnectedClanChatMsg_t?(GameConnectedClanChatMsg_t.Fill(intPtr));
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

		public static void Install(Action<GameConnectedClanChatMsg_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GameConnectedClanChatMsg_t.OnClient), GameConnectedClanChatMsg_t.StructSize, 338, false);
				GameConnectedClanChatMsg_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GameConnectedClanChatMsg_t.OnServer), GameConnectedClanChatMsg_t.StructSize, 338, true);
				GameConnectedClanChatMsg_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameConnectedClanChatMsg_t> action = GameConnectedClanChatMsg_t.actionClient;
			if (action != null)
			{
				action(GameConnectedClanChatMsg_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GameConnectedClanChatMsg_t> action = GameConnectedClanChatMsg_t.actionServer;
			if (action != null)
			{
				action(GameConnectedClanChatMsg_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}