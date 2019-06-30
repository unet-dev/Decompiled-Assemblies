using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_SearchResults_t
	{
		internal uint UnBrowserHandle;

		internal uint UnResults;

		internal uint UnCurrentMatch;

		internal readonly static int StructSize;

		private static Action<HTML_SearchResults_t> actionClient;

		private static Action<HTML_SearchResults_t> actionServer;

		static HTML_SearchResults_t()
		{
			HTML_SearchResults_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_SearchResults_t) : typeof(HTML_SearchResults_t.Pack8)));
		}

		internal static HTML_SearchResults_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_SearchResults_t)Marshal.PtrToStructure(p, typeof(HTML_SearchResults_t)) : (HTML_SearchResults_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_SearchResults_t.Pack8)));
		}

		public static async Task<HTML_SearchResults_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_SearchResults_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_SearchResults_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_SearchResults_t.StructSize, 4509, ref flag) | flag))
					{
						nullable = new HTML_SearchResults_t?(HTML_SearchResults_t.Fill(intPtr));
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

		public static void Install(Action<HTML_SearchResults_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_SearchResults_t.OnClient), HTML_SearchResults_t.StructSize, 4509, false);
				HTML_SearchResults_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_SearchResults_t.OnServer), HTML_SearchResults_t.StructSize, 4509, true);
				HTML_SearchResults_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_SearchResults_t> action = HTML_SearchResults_t.actionClient;
			if (action != null)
			{
				action(HTML_SearchResults_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_SearchResults_t> action = HTML_SearchResults_t.actionServer;
			if (action != null)
			{
				action(HTML_SearchResults_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal uint UnResults;

			internal uint UnCurrentMatch;

			public static implicit operator HTML_SearchResults_t(HTML_SearchResults_t.Pack8 d)
			{
				HTML_SearchResults_t hTMLSearchResultsT = new HTML_SearchResults_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnResults = d.UnResults,
					UnCurrentMatch = d.UnCurrentMatch
				};
				return hTMLSearchResultsT;
			}

			public static implicit operator Pack8(HTML_SearchResults_t d)
			{
				HTML_SearchResults_t.Pack8 pack8 = new HTML_SearchResults_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnResults = d.UnResults,
					UnCurrentMatch = d.UnCurrentMatch
				};
				return pack8;
			}
		}
	}
}