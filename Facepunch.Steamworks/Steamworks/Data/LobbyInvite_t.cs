using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyInvite_t
	{
		internal ulong SteamIDUser;

		internal ulong SteamIDLobby;

		internal ulong GameID;

		internal readonly static int StructSize;

		private static Action<LobbyInvite_t> actionClient;

		private static Action<LobbyInvite_t> actionServer;

		static LobbyInvite_t()
		{
			LobbyInvite_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyInvite_t) : typeof(LobbyInvite_t.Pack8)));
		}

		internal static LobbyInvite_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyInvite_t)Marshal.PtrToStructure(p, typeof(LobbyInvite_t)) : (LobbyInvite_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyInvite_t.Pack8)));
		}

		public static async Task<LobbyInvite_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyInvite_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyInvite_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyInvite_t.StructSize, 503, ref flag) | flag))
					{
						nullable = new LobbyInvite_t?(LobbyInvite_t.Fill(intPtr));
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

		public static void Install(Action<LobbyInvite_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyInvite_t.OnClient), LobbyInvite_t.StructSize, 503, false);
				LobbyInvite_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyInvite_t.OnServer), LobbyInvite_t.StructSize, 503, true);
				LobbyInvite_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyInvite_t> action = LobbyInvite_t.actionClient;
			if (action != null)
			{
				action(LobbyInvite_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyInvite_t> action = LobbyInvite_t.actionServer;
			if (action != null)
			{
				action(LobbyInvite_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamIDUser;

			internal ulong SteamIDLobby;

			internal ulong GameID;

			public static implicit operator LobbyInvite_t(LobbyInvite_t.Pack8 d)
			{
				LobbyInvite_t lobbyInviteT = new LobbyInvite_t()
				{
					SteamIDUser = d.SteamIDUser,
					SteamIDLobby = d.SteamIDLobby,
					GameID = d.GameID
				};
				return lobbyInviteT;
			}

			public static implicit operator Pack8(LobbyInvite_t d)
			{
				LobbyInvite_t.Pack8 pack8 = new LobbyInvite_t.Pack8()
				{
					SteamIDUser = d.SteamIDUser,
					SteamIDLobby = d.SteamIDLobby,
					GameID = d.GameID
				};
				return pack8;
			}
		}
	}
}