using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_CanGoBackAndForward_t
	{
		internal uint UnBrowserHandle;

		internal bool BCanGoBack;

		internal bool BCanGoForward;

		internal readonly static int StructSize;

		private static Action<HTML_CanGoBackAndForward_t> actionClient;

		private static Action<HTML_CanGoBackAndForward_t> actionServer;

		static HTML_CanGoBackAndForward_t()
		{
			HTML_CanGoBackAndForward_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_CanGoBackAndForward_t) : typeof(HTML_CanGoBackAndForward_t.Pack8)));
		}

		internal static HTML_CanGoBackAndForward_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_CanGoBackAndForward_t)Marshal.PtrToStructure(p, typeof(HTML_CanGoBackAndForward_t)) : (HTML_CanGoBackAndForward_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_CanGoBackAndForward_t.Pack8)));
		}

		public static async Task<HTML_CanGoBackAndForward_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_CanGoBackAndForward_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_CanGoBackAndForward_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_CanGoBackAndForward_t.StructSize, 4510, ref flag) | flag))
					{
						nullable = new HTML_CanGoBackAndForward_t?(HTML_CanGoBackAndForward_t.Fill(intPtr));
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

		public static void Install(Action<HTML_CanGoBackAndForward_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_CanGoBackAndForward_t.OnClient), HTML_CanGoBackAndForward_t.StructSize, 4510, false);
				HTML_CanGoBackAndForward_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_CanGoBackAndForward_t.OnServer), HTML_CanGoBackAndForward_t.StructSize, 4510, true);
				HTML_CanGoBackAndForward_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_CanGoBackAndForward_t> action = HTML_CanGoBackAndForward_t.actionClient;
			if (action != null)
			{
				action(HTML_CanGoBackAndForward_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_CanGoBackAndForward_t> action = HTML_CanGoBackAndForward_t.actionServer;
			if (action != null)
			{
				action(HTML_CanGoBackAndForward_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal bool BCanGoBack;

			internal bool BCanGoForward;

			public static implicit operator HTML_CanGoBackAndForward_t(HTML_CanGoBackAndForward_t.Pack8 d)
			{
				HTML_CanGoBackAndForward_t hTMLCanGoBackAndForwardT = new HTML_CanGoBackAndForward_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					BCanGoBack = d.BCanGoBack,
					BCanGoForward = d.BCanGoForward
				};
				return hTMLCanGoBackAndForwardT;
			}

			public static implicit operator Pack8(HTML_CanGoBackAndForward_t d)
			{
				HTML_CanGoBackAndForward_t.Pack8 pack8 = new HTML_CanGoBackAndForward_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					BCanGoBack = d.BCanGoBack,
					BCanGoForward = d.BCanGoForward
				};
				return pack8;
			}
		}
	}
}