using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyDataUpdate_t
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDMember;

		internal byte Success;

		internal readonly static int StructSize;

		private static Action<LobbyDataUpdate_t> actionClient;

		private static Action<LobbyDataUpdate_t> actionServer;

		static LobbyDataUpdate_t()
		{
			LobbyDataUpdate_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyDataUpdate_t) : typeof(LobbyDataUpdate_t.Pack8)));
		}

		internal static LobbyDataUpdate_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyDataUpdate_t)Marshal.PtrToStructure(p, typeof(LobbyDataUpdate_t)) : (LobbyDataUpdate_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyDataUpdate_t.Pack8)));
		}

		public static async Task<LobbyDataUpdate_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyDataUpdate_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyDataUpdate_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyDataUpdate_t.StructSize, 505, ref flag) | flag))
					{
						nullable = new LobbyDataUpdate_t?(LobbyDataUpdate_t.Fill(intPtr));
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

		public static void Install(Action<LobbyDataUpdate_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyDataUpdate_t.OnClient), LobbyDataUpdate_t.StructSize, 505, false);
				LobbyDataUpdate_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyDataUpdate_t.OnServer), LobbyDataUpdate_t.StructSize, 505, true);
				LobbyDataUpdate_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyDataUpdate_t> action = LobbyDataUpdate_t.actionClient;
			if (action != null)
			{
				action(LobbyDataUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyDataUpdate_t> action = LobbyDataUpdate_t.actionServer;
			if (action != null)
			{
				action(LobbyDataUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDMember;

			internal byte Success;

			public static implicit operator LobbyDataUpdate_t(LobbyDataUpdate_t.Pack8 d)
			{
				LobbyDataUpdate_t lobbyDataUpdateT = new LobbyDataUpdate_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDMember = d.SteamIDMember,
					Success = d.Success
				};
				return lobbyDataUpdateT;
			}

			public static implicit operator Pack8(LobbyDataUpdate_t d)
			{
				LobbyDataUpdate_t.Pack8 pack8 = new LobbyDataUpdate_t.Pack8()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDMember = d.SteamIDMember,
					Success = d.Success
				};
				return pack8;
			}
		}
	}
}