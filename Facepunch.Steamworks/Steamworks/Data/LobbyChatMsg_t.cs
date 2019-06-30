using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LobbyChatMsg_t
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDUser;

		internal byte ChatEntryType;

		internal uint ChatID;

		internal readonly static int StructSize;

		private static Action<LobbyChatMsg_t> actionClient;

		private static Action<LobbyChatMsg_t> actionServer;

		static LobbyChatMsg_t()
		{
			LobbyChatMsg_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LobbyChatMsg_t) : typeof(LobbyChatMsg_t.Pack8)));
		}

		internal static LobbyChatMsg_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LobbyChatMsg_t)Marshal.PtrToStructure(p, typeof(LobbyChatMsg_t)) : (LobbyChatMsg_t.Pack8)Marshal.PtrToStructure(p, typeof(LobbyChatMsg_t.Pack8)));
		}

		public static async Task<LobbyChatMsg_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LobbyChatMsg_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LobbyChatMsg_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LobbyChatMsg_t.StructSize, 507, ref flag) | flag))
					{
						nullable = new LobbyChatMsg_t?(LobbyChatMsg_t.Fill(intPtr));
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

		public static void Install(Action<LobbyChatMsg_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LobbyChatMsg_t.OnClient), LobbyChatMsg_t.StructSize, 507, false);
				LobbyChatMsg_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LobbyChatMsg_t.OnServer), LobbyChatMsg_t.StructSize, 507, true);
				LobbyChatMsg_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyChatMsg_t> action = LobbyChatMsg_t.actionClient;
			if (action != null)
			{
				action(LobbyChatMsg_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LobbyChatMsg_t> action = LobbyChatMsg_t.actionServer;
			if (action != null)
			{
				action(LobbyChatMsg_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamIDLobby;

			internal ulong SteamIDUser;

			internal byte ChatEntryType;

			internal uint ChatID;

			public static implicit operator LobbyChatMsg_t(LobbyChatMsg_t.Pack8 d)
			{
				LobbyChatMsg_t lobbyChatMsgT = new LobbyChatMsg_t()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDUser = d.SteamIDUser,
					ChatEntryType = d.ChatEntryType,
					ChatID = d.ChatID
				};
				return lobbyChatMsgT;
			}

			public static implicit operator Pack8(LobbyChatMsg_t d)
			{
				LobbyChatMsg_t.Pack8 pack8 = new LobbyChatMsg_t.Pack8()
				{
					SteamIDLobby = d.SteamIDLobby,
					SteamIDUser = d.SteamIDUser,
					ChatEntryType = d.ChatEntryType,
					ChatID = d.ChatID
				};
				return pack8;
			}
		}
	}
}