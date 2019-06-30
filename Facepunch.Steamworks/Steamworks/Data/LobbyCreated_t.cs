using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyCreated_t
	{
		internal Steamworks.Result Result;

		internal ulong SteamIDLobby;

		internal readonly static int StructSize;

		private static Action<LobbyCreated_t> actionClient;

		private static Action<LobbyCreated_t> actionServer;

		static LobbyCreated_t()
		{
			LobbyCreated_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyCreated_t) : typeof(LobbyCreated_t.Pack8)));
		}

		internal static LobbyCreated_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyCreated_t)Marshal.PtrToStructure(p, typeof(LobbyCreated_t)) : (LobbyCreated_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyCreated_t.Pack8)));
		}

		public static async Task<LobbyCreated_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyCreated_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyCreated_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyCreated_t.StructSize, 513, ref flag) | flag))
					{
						nullable = new LobbyCreated_t?(LobbyCreated_t.Fill(intPtr));
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

		public static void Install(Action<LobbyCreated_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyCreated_t.OnClient), LobbyCreated_t.StructSize, 513, false);
				LobbyCreated_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyCreated_t.OnServer), LobbyCreated_t.StructSize, 513, true);
				LobbyCreated_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyCreated_t> action = LobbyCreated_t.actionClient;
			if (action != null)
			{
				action(LobbyCreated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyCreated_t> action = LobbyCreated_t.actionServer;
			if (action != null)
			{
				action(LobbyCreated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong SteamIDLobby;

			public static implicit operator LobbyCreated_t(LobbyCreated_t.Pack8 d)
			{
				LobbyCreated_t lobbyCreatedT = new LobbyCreated_t()
				{
					Result = d.Result,
					SteamIDLobby = d.SteamIDLobby
				};
				return lobbyCreatedT;
			}

			public static implicit operator Pack8(LobbyCreated_t d)
			{
				LobbyCreated_t.Pack8 pack8 = new LobbyCreated_t.Pack8()
				{
					Result = d.Result,
					SteamIDLobby = d.SteamIDLobby
				};
				return pack8;
			}
		}
	}
}