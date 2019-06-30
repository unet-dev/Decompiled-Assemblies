using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyEnter_t
	{
		internal ulong SteamIDLobby;

		internal uint GfChatPermissions;

		internal bool Locked;

		internal uint EChatRoomEnterResponse;

		internal readonly static int StructSize;

		private static Action<LobbyEnter_t> actionClient;

		private static Action<LobbyEnter_t> actionServer;

		static LobbyEnter_t()
		{
			LobbyEnter_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyEnter_t) : typeof(LobbyEnter_t.Pack8)));
		}

		internal static LobbyEnter_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyEnter_t)Marshal.PtrToStructure(p, typeof(LobbyEnter_t)) : (LobbyEnter_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyEnter_t.Pack8)));
		}

		public static async Task<LobbyEnter_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyEnter_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyEnter_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyEnter_t.StructSize, 504, ref flag) | flag))
					{
						nullable = new LobbyEnter_t?(LobbyEnter_t.Fill(intPtr));
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

		public static void Install(Action<LobbyEnter_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyEnter_t.OnClient), LobbyEnter_t.StructSize, 504, false);
				LobbyEnter_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyEnter_t.OnServer), LobbyEnter_t.StructSize, 504, true);
				LobbyEnter_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyEnter_t> action = LobbyEnter_t.actionClient;
			if (action != null)
			{
				action(LobbyEnter_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyEnter_t> action = LobbyEnter_t.actionServer;
			if (action != null)
			{
				action(LobbyEnter_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamIDLobby;

			internal uint GfChatPermissions;

			internal bool Locked;

			internal uint EChatRoomEnterResponse;

			public static implicit operator LobbyEnter_t(LobbyEnter_t.Pack8 d)
			{
				LobbyEnter_t lobbyEnterT = new LobbyEnter_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					GfChatPermissions = d.GfChatPermissions,
					Locked = d.Locked,
					EChatRoomEnterResponse = d.EChatRoomEnterResponse
				};
				return lobbyEnterT;
			}

			public static implicit operator Pack8(LobbyEnter_t d)
			{
				LobbyEnter_t.Pack8 pack8 = new LobbyEnter_t.Pack8()
				{
					SteamIDLobby = d.SteamIDLobby,
					GfChatPermissions = d.GfChatPermissions,
					Locked = d.Locked,
					EChatRoomEnterResponse = d.EChatRoomEnterResponse
				};
				return pack8;
			}
		}
	}
}