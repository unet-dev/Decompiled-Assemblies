using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_JSAlert_t
	{
		internal uint UnBrowserHandle;

		internal string PchMessage;

		internal readonly static int StructSize;

		private static Action<HTML_JSAlert_t> actionClient;

		private static Action<HTML_JSAlert_t> actionServer;

		static HTML_JSAlert_t()
		{
			HTML_JSAlert_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_JSAlert_t) : typeof(HTML_JSAlert_t.Pack8)));
		}

		internal static HTML_JSAlert_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_JSAlert_t)Marshal.PtrToStructure(p, typeof(HTML_JSAlert_t)) : (HTML_JSAlert_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_JSAlert_t.Pack8)));
		}

		public static async Task<HTML_JSAlert_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_JSAlert_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_JSAlert_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_JSAlert_t.StructSize, 4514, ref flag) | flag))
					{
						nullable = new HTML_JSAlert_t?(HTML_JSAlert_t.Fill(intPtr));
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

		public static void Install(Action<HTML_JSAlert_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_JSAlert_t.OnClient), HTML_JSAlert_t.StructSize, 4514, false);
				HTML_JSAlert_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_JSAlert_t.OnServer), HTML_JSAlert_t.StructSize, 4514, true);
				HTML_JSAlert_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_JSAlert_t> action = HTML_JSAlert_t.actionClient;
			if (action != null)
			{
				action(HTML_JSAlert_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_JSAlert_t> action = HTML_JSAlert_t.actionServer;
			if (action != null)
			{
				action(HTML_JSAlert_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchMessage;

			public static implicit operator HTML_JSAlert_t(HTML_JSAlert_t.Pack8 d)
			{
				HTML_JSAlert_t hTMLJSAlertT = new HTML_JSAlert_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchMessage = d.PchMessage
				};
				return hTMLJSAlertT;
			}

			public static implicit operator Pack8(HTML_JSAlert_t d)
			{
				HTML_JSAlert_t.Pack8 pack8 = new HTML_JSAlert_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchMessage = d.PchMessage
				};
				return pack8;
			}
		}
	}
}