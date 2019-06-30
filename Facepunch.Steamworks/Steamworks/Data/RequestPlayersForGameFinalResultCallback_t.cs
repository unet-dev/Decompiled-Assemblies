using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RequestPlayersForGameFinalResultCallback_t
	{
		internal Steamworks.Result Result;

		internal ulong LSearchID;

		internal ulong LUniqueGameID;

		internal readonly static int StructSize;

		private static Action<RequestPlayersForGameFinalResultCallback_t> actionClient;

		private static Action<RequestPlayersForGameFinalResultCallback_t> actionServer;

		static RequestPlayersForGameFinalResultCallback_t()
		{
			RequestPlayersForGameFinalResultCallback_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RequestPlayersForGameFinalResultCallback_t) : typeof(RequestPlayersForGameFinalResultCallback_t.Pack8)));
		}

		internal static RequestPlayersForGameFinalResultCallback_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RequestPlayersForGameFinalResultCallback_t)Marshal.PtrToStructure(p, typeof(RequestPlayersForGameFinalResultCallback_t)) : (RequestPlayersForGameFinalResultCallback_t.Pack8)Marshal.PtrToStructure(p, typeof(RequestPlayersForGameFinalResultCallback_t.Pack8)));
		}

		public static async Task<RequestPlayersForGameFinalResultCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RequestPlayersForGameFinalResultCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RequestPlayersForGameFinalResultCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RequestPlayersForGameFinalResultCallback_t.StructSize, 5213, ref flag) | flag))
					{
						nullable = new RequestPlayersForGameFinalResultCallback_t?(RequestPlayersForGameFinalResultCallback_t.Fill(intPtr));
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

		public static void Install(Action<RequestPlayersForGameFinalResultCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RequestPlayersForGameFinalResultCallback_t.OnClient), RequestPlayersForGameFinalResultCallback_t.StructSize, 5213, false);
				RequestPlayersForGameFinalResultCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RequestPlayersForGameFinalResultCallback_t.OnServer), RequestPlayersForGameFinalResultCallback_t.StructSize, 5213, true);
				RequestPlayersForGameFinalResultCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RequestPlayersForGameFinalResultCallback_t> action = RequestPlayersForGameFinalResultCallback_t.actionClient;
			if (action != null)
			{
				action(RequestPlayersForGameFinalResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RequestPlayersForGameFinalResultCallback_t> action = RequestPlayersForGameFinalResultCallback_t.actionServer;
			if (action != null)
			{
				action(RequestPlayersForGameFinalResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong LSearchID;

			internal ulong LUniqueGameID;

			public static implicit operator RequestPlayersForGameFinalResultCallback_t(RequestPlayersForGameFinalResultCallback_t.Pack8 d)
			{
				RequestPlayersForGameFinalResultCallback_t requestPlayersForGameFinalResultCallbackT = new RequestPlayersForGameFinalResultCallback_t()
				{
					Result = d.Result,
					LSearchID = d.LSearchID,
					LUniqueGameID = d.LUniqueGameID
				};
				return requestPlayersForGameFinalResultCallbackT;
			}

			public static implicit operator Pack8(RequestPlayersForGameFinalResultCallback_t d)
			{
				RequestPlayersForGameFinalResultCallback_t.Pack8 pack8 = new RequestPlayersForGameFinalResultCallback_t.Pack8()
				{
					Result = d.Result,
					LSearchID = d.LSearchID,
					LUniqueGameID = d.LUniqueGameID
				};
				return pack8;
			}
		}
	}
}