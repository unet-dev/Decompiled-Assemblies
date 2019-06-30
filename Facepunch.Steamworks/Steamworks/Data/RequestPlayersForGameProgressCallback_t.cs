using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RequestPlayersForGameProgressCallback_t
	{
		internal Steamworks.Result Result;

		internal ulong LSearchID;

		internal readonly static int StructSize;

		private static Action<RequestPlayersForGameProgressCallback_t> actionClient;

		private static Action<RequestPlayersForGameProgressCallback_t> actionServer;

		static RequestPlayersForGameProgressCallback_t()
		{
			RequestPlayersForGameProgressCallback_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RequestPlayersForGameProgressCallback_t) : typeof(RequestPlayersForGameProgressCallback_t.Pack8)));
		}

		internal static RequestPlayersForGameProgressCallback_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RequestPlayersForGameProgressCallback_t)Marshal.PtrToStructure(p, typeof(RequestPlayersForGameProgressCallback_t)) : (RequestPlayersForGameProgressCallback_t.Pack8)Marshal.PtrToStructure(p, typeof(RequestPlayersForGameProgressCallback_t.Pack8)));
		}

		public static async Task<RequestPlayersForGameProgressCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RequestPlayersForGameProgressCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RequestPlayersForGameProgressCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RequestPlayersForGameProgressCallback_t.StructSize, 5211, ref flag) | flag))
					{
						nullable = new RequestPlayersForGameProgressCallback_t?(RequestPlayersForGameProgressCallback_t.Fill(intPtr));
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

		public static void Install(Action<RequestPlayersForGameProgressCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RequestPlayersForGameProgressCallback_t.OnClient), RequestPlayersForGameProgressCallback_t.StructSize, 5211, false);
				RequestPlayersForGameProgressCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RequestPlayersForGameProgressCallback_t.OnServer), RequestPlayersForGameProgressCallback_t.StructSize, 5211, true);
				RequestPlayersForGameProgressCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RequestPlayersForGameProgressCallback_t> action = RequestPlayersForGameProgressCallback_t.actionClient;
			if (action != null)
			{
				action(RequestPlayersForGameProgressCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RequestPlayersForGameProgressCallback_t> action = RequestPlayersForGameProgressCallback_t.actionServer;
			if (action != null)
			{
				action(RequestPlayersForGameProgressCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong LSearchID;

			public static implicit operator RequestPlayersForGameProgressCallback_t(RequestPlayersForGameProgressCallback_t.Pack8 d)
			{
				RequestPlayersForGameProgressCallback_t requestPlayersForGameProgressCallbackT = new RequestPlayersForGameProgressCallback_t()
				{
					Result = d.Result,
					LSearchID = d.LSearchID
				};
				return requestPlayersForGameProgressCallbackT;
			}

			public static implicit operator Pack8(RequestPlayersForGameProgressCallback_t d)
			{
				RequestPlayersForGameProgressCallback_t.Pack8 pack8 = new RequestPlayersForGameProgressCallback_t.Pack8()
				{
					Result = d.Result,
					LSearchID = d.LSearchID
				};
				return pack8;
			}
		}
	}
}