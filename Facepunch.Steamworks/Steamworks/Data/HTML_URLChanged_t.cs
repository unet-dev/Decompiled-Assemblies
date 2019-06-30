using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_URLChanged_t
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal string PchPostData;

		internal bool BIsRedirect;

		internal string PchPageTitle;

		internal bool BNewNavigation;

		internal readonly static int StructSize;

		private static Action<HTML_URLChanged_t> actionClient;

		private static Action<HTML_URLChanged_t> actionServer;

		static HTML_URLChanged_t()
		{
			HTML_URLChanged_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_URLChanged_t) : typeof(HTML_URLChanged_t.Pack8)));
		}

		internal static HTML_URLChanged_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_URLChanged_t)Marshal.PtrToStructure(p, typeof(HTML_URLChanged_t)) : (HTML_URLChanged_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_URLChanged_t.Pack8)));
		}

		public static async Task<HTML_URLChanged_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_URLChanged_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_URLChanged_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_URLChanged_t.StructSize, 4505, ref flag) | flag))
					{
						nullable = new HTML_URLChanged_t?(HTML_URLChanged_t.Fill(intPtr));
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

		public static void Install(Action<HTML_URLChanged_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_URLChanged_t.OnClient), HTML_URLChanged_t.StructSize, 4505, false);
				HTML_URLChanged_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_URLChanged_t.OnServer), HTML_URLChanged_t.StructSize, 4505, true);
				HTML_URLChanged_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_URLChanged_t> action = HTML_URLChanged_t.actionClient;
			if (action != null)
			{
				action(HTML_URLChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_URLChanged_t> action = HTML_URLChanged_t.actionServer;
			if (action != null)
			{
				action(HTML_URLChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchURL;

			internal string PchPostData;

			internal bool BIsRedirect;

			internal string PchPageTitle;

			internal bool BNewNavigation;

			public static implicit operator HTML_URLChanged_t(HTML_URLChanged_t.Pack8 d)
			{
				HTML_URLChanged_t hTMLURLChangedT = new HTML_URLChanged_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL,
					PchPostData = d.PchPostData,
					BIsRedirect = d.BIsRedirect,
					PchPageTitle = d.PchPageTitle,
					BNewNavigation = d.BNewNavigation
				};
				return hTMLURLChangedT;
			}

			public static implicit operator Pack8(HTML_URLChanged_t d)
			{
				HTML_URLChanged_t.Pack8 pack8 = new HTML_URLChanged_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchURL = d.PchURL,
					PchPostData = d.PchPostData,
					BIsRedirect = d.BIsRedirect,
					PchPageTitle = d.PchPageTitle,
					BNewNavigation = d.BNewNavigation
				};
				return pack8;
			}
		}
	}
}