using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RequestPlayersForGameResultCallback_t
	{
		internal Steamworks.Result Result;

		internal ulong LSearchID;

		internal ulong SteamIDPlayerFound;

		internal ulong SteamIDLobby;

		internal PlayerAcceptState_t PlayerAcceptState;

		internal int PlayerIndex;

		internal int TotalPlayersFound;

		internal int TotalPlayersAcceptedGame;

		internal int SuggestedTeamIndex;

		internal ulong LUniqueGameID;

		internal readonly static int StructSize;

		private static Action<RequestPlayersForGameResultCallback_t> actionClient;

		private static Action<RequestPlayersForGameResultCallback_t> actionServer;

		static RequestPlayersForGameResultCallback_t()
		{
			RequestPlayersForGameResultCallback_t.StructSize = Marshal.SizeOf(typeof(RequestPlayersForGameResultCallback_t));
		}

		internal static RequestPlayersForGameResultCallback_t Fill(IntPtr p)
		{
			return (RequestPlayersForGameResultCallback_t)Marshal.PtrToStructure(p, typeof(RequestPlayersForGameResultCallback_t));
		}

		public static async Task<RequestPlayersForGameResultCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RequestPlayersForGameResultCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RequestPlayersForGameResultCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RequestPlayersForGameResultCallback_t.StructSize, 5212, ref flag) | flag))
					{
						nullable = new RequestPlayersForGameResultCallback_t?(RequestPlayersForGameResultCallback_t.Fill(intPtr));
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

		public static void Install(Action<RequestPlayersForGameResultCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RequestPlayersForGameResultCallback_t.OnClient), RequestPlayersForGameResultCallback_t.StructSize, 5212, false);
				RequestPlayersForGameResultCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RequestPlayersForGameResultCallback_t.OnServer), RequestPlayersForGameResultCallback_t.StructSize, 5212, true);
				RequestPlayersForGameResultCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RequestPlayersForGameResultCallback_t> action = RequestPlayersForGameResultCallback_t.actionClient;
			if (action != null)
			{
				action(RequestPlayersForGameResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RequestPlayersForGameResultCallback_t> action = RequestPlayersForGameResultCallback_t.actionServer;
			if (action != null)
			{
				action(RequestPlayersForGameResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}