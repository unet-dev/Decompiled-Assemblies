using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SearchForGameResultCallback_t
	{
		internal ulong LSearchID;

		internal Steamworks.Result Result;

		internal int CountPlayersInGame;

		internal int CountAcceptedGame;

		internal ulong SteamIDHost;

		internal bool FinalCallback;

		internal readonly static int StructSize;

		private static Action<SearchForGameResultCallback_t> actionClient;

		private static Action<SearchForGameResultCallback_t> actionServer;

		static SearchForGameResultCallback_t()
		{
			SearchForGameResultCallback_t.StructSize = Marshal.SizeOf(typeof(SearchForGameResultCallback_t));
		}

		internal static SearchForGameResultCallback_t Fill(IntPtr p)
		{
			return (SearchForGameResultCallback_t)Marshal.PtrToStructure(p, typeof(SearchForGameResultCallback_t));
		}

		public static async Task<SearchForGameResultCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SearchForGameResultCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SearchForGameResultCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SearchForGameResultCallback_t.StructSize, 5202, ref flag) | flag))
					{
						nullable = new SearchForGameResultCallback_t?(SearchForGameResultCallback_t.Fill(intPtr));
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

		public static void Install(Action<SearchForGameResultCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SearchForGameResultCallback_t.OnClient), SearchForGameResultCallback_t.StructSize, 5202, false);
				SearchForGameResultCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SearchForGameResultCallback_t.OnServer), SearchForGameResultCallback_t.StructSize, 5202, true);
				SearchForGameResultCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SearchForGameResultCallback_t> action = SearchForGameResultCallback_t.actionClient;
			if (action != null)
			{
				action(SearchForGameResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SearchForGameResultCallback_t> action = SearchForGameResultCallback_t.actionServer;
			if (action != null)
			{
				action(SearchForGameResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}