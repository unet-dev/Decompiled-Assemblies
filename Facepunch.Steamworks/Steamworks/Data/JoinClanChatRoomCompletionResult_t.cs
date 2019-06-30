using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct JoinClanChatRoomCompletionResult_t
	{
		internal ulong SteamIDClanChat;

		internal RoomEnter ChatRoomEnterResponse;

		internal readonly static int StructSize;

		private static Action<JoinClanChatRoomCompletionResult_t> actionClient;

		private static Action<JoinClanChatRoomCompletionResult_t> actionServer;

		static JoinClanChatRoomCompletionResult_t()
		{
			JoinClanChatRoomCompletionResult_t.StructSize = Marshal.SizeOf(typeof(JoinClanChatRoomCompletionResult_t));
		}

		internal static JoinClanChatRoomCompletionResult_t Fill(IntPtr p)
		{
			return (JoinClanChatRoomCompletionResult_t)Marshal.PtrToStructure(p, typeof(JoinClanChatRoomCompletionResult_t));
		}

		public static async Task<JoinClanChatRoomCompletionResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			JoinClanChatRoomCompletionResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(JoinClanChatRoomCompletionResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, JoinClanChatRoomCompletionResult_t.StructSize, 342, ref flag) | flag))
					{
						nullable = new JoinClanChatRoomCompletionResult_t?(JoinClanChatRoomCompletionResult_t.Fill(intPtr));
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

		public static void Install(Action<JoinClanChatRoomCompletionResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(JoinClanChatRoomCompletionResult_t.OnClient), JoinClanChatRoomCompletionResult_t.StructSize, 342, false);
				JoinClanChatRoomCompletionResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(JoinClanChatRoomCompletionResult_t.OnServer), JoinClanChatRoomCompletionResult_t.StructSize, 342, true);
				JoinClanChatRoomCompletionResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<JoinClanChatRoomCompletionResult_t> action = JoinClanChatRoomCompletionResult_t.actionClient;
			if (action != null)
			{
				action(JoinClanChatRoomCompletionResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<JoinClanChatRoomCompletionResult_t> action = JoinClanChatRoomCompletionResult_t.actionServer;
			if (action != null)
			{
				action(JoinClanChatRoomCompletionResult_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}