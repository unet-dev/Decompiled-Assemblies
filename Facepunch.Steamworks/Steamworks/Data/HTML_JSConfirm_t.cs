using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_JSConfirm_t
	{
		internal uint UnBrowserHandle;

		internal string PchMessage;

		internal readonly static int StructSize;

		private static Action<HTML_JSConfirm_t> actionClient;

		private static Action<HTML_JSConfirm_t> actionServer;

		static HTML_JSConfirm_t()
		{
			HTML_JSConfirm_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_JSConfirm_t) : typeof(HTML_JSConfirm_t.Pack8)));
		}

		internal static HTML_JSConfirm_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_JSConfirm_t)Marshal.PtrToStructure(p, typeof(HTML_JSConfirm_t)) : (HTML_JSConfirm_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_JSConfirm_t.Pack8)));
		}

		public static async Task<HTML_JSConfirm_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_JSConfirm_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_JSConfirm_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_JSConfirm_t.StructSize, 4515, ref flag) | flag))
					{
						nullable = new HTML_JSConfirm_t?(HTML_JSConfirm_t.Fill(intPtr));
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

		public static void Install(Action<HTML_JSConfirm_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_JSConfirm_t.OnClient), HTML_JSConfirm_t.StructSize, 4515, false);
				HTML_JSConfirm_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_JSConfirm_t.OnServer), HTML_JSConfirm_t.StructSize, 4515, true);
				HTML_JSConfirm_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_JSConfirm_t> action = HTML_JSConfirm_t.actionClient;
			if (action != null)
			{
				action(HTML_JSConfirm_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_JSConfirm_t> action = HTML_JSConfirm_t.actionServer;
			if (action != null)
			{
				action(HTML_JSConfirm_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchMessage;

			public static implicit operator HTML_JSConfirm_t(HTML_JSConfirm_t.Pack8 d)
			{
				HTML_JSConfirm_t hTMLJSConfirmT = new HTML_JSConfirm_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchMessage = d.PchMessage
				};
				return hTMLJSConfirmT;
			}

			public static implicit operator Pack8(HTML_JSConfirm_t d)
			{
				HTML_JSConfirm_t.Pack8 pack8 = new HTML_JSConfirm_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchMessage = d.PchMessage
				};
				return pack8;
			}
		}
	}
}