using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_FinishedRequest_t
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal string PchPageTitle;

		internal readonly static int StructSize;

		private static Action<HTML_FinishedRequest_t> actionClient;

		private static Action<HTML_FinishedRequest_t> actionServer;

		static HTML_FinishedRequest_t()
		{
			HTML_FinishedRequest_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_FinishedRequest_t) : typeof(HTML_FinishedRequest_t.Pack8)));
		}

		internal static HTML_FinishedRequest_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_FinishedRequest_t)Marshal.PtrToStructure(p, typeof(HTML_FinishedRequest_t)) : (HTML_FinishedRequest_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_FinishedRequest_t.Pack8)));
		}

		public static async Task<HTML_FinishedRequest_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_FinishedRequest_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_FinishedRequest_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_FinishedRequest_t.StructSize, 4506, ref flag) | flag))
					{
						nullable = new HTML_FinishedRequest_t?(HTML_FinishedRequest_t.Fill(intPtr));
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

		public static void Install(Action<HTML_FinishedRequest_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_FinishedRequest_t.OnClient), HTML_FinishedRequest_t.StructSize, 4506, false);
				HTML_FinishedRequest_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_FinishedRequest_t.OnServer), HTML_FinishedRequest_t.StructSize, 4506, true);
				HTML_FinishedRequest_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_FinishedRequest_t> action = HTML_FinishedRequest_t.actionClient;
			if (action != null)
			{
				action(HTML_FinishedRequest_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_FinishedRequest_t> action = HTML_FinishedRequest_t.actionServer;
			if (action != null)
			{
				action(HTML_FinishedRequest_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchURL;

			internal string PchPageTitle;

			public static implicit operator HTML_FinishedRequest_t(HTML_FinishedRequest_t.Pack8 d)
			{
				HTML_FinishedRequest_t hTMLFinishedRequestT = new HTML_FinishedRequest_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL,
					PchPageTitle = d.PchPageTitle
				};
				return hTMLFinishedRequestT;
			}

			public static implicit operator Pack8(HTML_FinishedRequest_t d)
			{
				HTML_FinishedRequest_t.Pack8 pack8 = new HTML_FinishedRequest_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL,
					PchPageTitle = d.PchPageTitle
				};
				return pack8;
			}
		}
	}
}