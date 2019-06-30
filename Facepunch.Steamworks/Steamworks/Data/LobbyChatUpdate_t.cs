using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyChatUpdate_t
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDUserChanged;

		internal ulong SteamIDMakingChange;

		internal uint GfChatMemberStateChange;

		internal readonly static int StructSize;

		private static Action<LobbyChatUpdate_t> actionClient;

		private static Action<LobbyChatUpdate_t> actionServer;

		static LobbyChatUpdate_t()
		{
			LobbyChatUpdate_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyChatUpdate_t) : typeof(LobbyChatUpdate_t.Pack8)));
		}

		internal static LobbyChatUpdate_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyChatUpdate_t)Marshal.PtrToStructure(p, typeof(LobbyChatUpdate_t)) : (LobbyChatUpdate_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyChatUpdate_t.Pack8)));
		}

		public static async Task<LobbyChatUpdate_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyChatUpdate_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyChatUpdate_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyChatUpdate_t.StructSize, 506, ref flag) | flag))
					{
						nullable = new LobbyChatUpdate_t?(LobbyChatUpdate_t.Fill(intPtr));
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

		public static void Install(Action<LobbyChatUpdate_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyChatUpdate_t.OnClient), LobbyChatUpdate_t.StructSize, 506, false);
				LobbyChatUpdate_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyChatUpdate_t.OnServer), LobbyChatUpdate_t.StructSize, 506, true);
				LobbyChatUpdate_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyChatUpdate_t> action = LobbyChatUpdate_t.actionClient;
			if (action != null)
			{
				action(LobbyChatUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyChatUpdate_t> action = LobbyChatUpdate_t.actionServer;
			if (action != null)
			{
				action(LobbyChatUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDUserChanged;

			internal ulong SteamIDMakingChange;

			internal uint GfChatMemberStateChange;

			public static implicit operator LobbyChatUpdate_t(LobbyChatUpdate_t.Pack8 d)
			{
				LobbyChatUpdate_t lobbyChatUpdateT = new LobbyChatUpdate_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDUserChanged = d.SteamIDUserChanged,
					SteamIDMakingChange = d.SteamIDMakingChange,
					GfChatMemberStateChange = d.GfChatMemberStateChange
				};
				return lobbyChatUpdateT;
			}

			public static implicit operator Pack8(LobbyChatUpdate_t d)
			{
				LobbyChatUpdate_t.Pack8 pack8 = new LobbyChatUpdate_t.Pack8()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDUserChanged = d.SteamIDUserChanged,
					SteamIDMakingChange = d.SteamIDMakingChange,
					GfChatMemberStateChange = d.GfChatMemberStateChange
				};
				return pack8;
			}
		}
	}
}