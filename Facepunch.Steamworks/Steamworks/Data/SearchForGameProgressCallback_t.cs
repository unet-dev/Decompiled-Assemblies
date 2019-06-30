using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SearchForGameProgressCallback_t
	{
		internal ulong LSearchID;

		internal Steamworks.Result Result;

		internal ulong LobbyID;

		internal ulong SteamIDEndedSearch;

		internal int SecondsRemainingEstimate;

		internal int CPlayersSearching;

		internal readonly static int StructSize;

		private static Action<SearchForGameProgressCallback_t> actionClient;

		private static Action<SearchForGameProgressCallback_t> actionServer;

		static SearchForGameProgressCallback_t()
		{
			SearchForGameProgressCallback_t.StructSize = Marshal.SizeOf(typeof(SearchForGameProgressCallback_t));
		}

		internal static SearchForGameProgressCallback_t Fill(IntPtr p)
		{
			return (SearchForGameProgressCallback_t)Marshal.PtrToStructure(p, typeof(SearchForGameProgressCallback_t));
		}

		public static async Task<SearchForGameProgressCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SearchForGameProgressCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SearchForGameProgressCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SearchForGameProgressCallback_t.StructSize, 5201, ref flag) | flag))
					{
						nullable = new SearchForGameProgressCallback_t?(SearchForGameProgressCallback_t.Fill(intPtr));
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

		public static void Install(Action<SearchForGameProgressCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SearchForGameProgressCallback_t.OnClient), SearchForGameProgressCallback_t.StructSize, 5201, false);
				SearchForGameProgressCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SearchForGameProgressCallback_t.OnServer), SearchForGameProgressCallback_t.StructSize, 5201, true);
				SearchForGameProgressCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SearchForGameProgressCallback_t> action = SearchForGameProgressCallback_t.actionClient;
			if (action != null)
			{
				action(SearchForGameProgressCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SearchForGameProgressCallback_t> action = SearchForGameProgressCallback_t.actionServer;
			if (action != null)
			{
				action(SearchForGameProgressCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}