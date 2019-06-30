using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_SetCursor_t
	{
		internal uint UnBrowserHandle;

		internal uint EMouseCursor;

		internal readonly static int StructSize;

		private static Action<HTML_SetCursor_t> actionClient;

		private static Action<HTML_SetCursor_t> actionServer;

		static HTML_SetCursor_t()
		{
			HTML_SetCursor_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_SetCursor_t) : typeof(HTML_SetCursor_t.Pack8)));
		}

		internal static HTML_SetCursor_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_SetCursor_t)Marshal.PtrToStructure(p, typeof(HTML_SetCursor_t)) : (HTML_SetCursor_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_SetCursor_t.Pack8)));
		}

		public static async Task<HTML_SetCursor_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_SetCursor_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_SetCursor_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_SetCursor_t.StructSize, 4522, ref flag) | flag))
					{
						nullable = new HTML_SetCursor_t?(HTML_SetCursor_t.Fill(intPtr));
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

		public static void Install(Action<HTML_SetCursor_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_SetCursor_t.OnClient), HTML_SetCursor_t.StructSize, 4522, false);
				HTML_SetCursor_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_SetCursor_t.OnServer), HTML_SetCursor_t.StructSize, 4522, true);
				HTML_SetCursor_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_SetCursor_t> action = HTML_SetCursor_t.actionClient;
			if (action != null)
			{
				action(HTML_SetCursor_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_SetCursor_t> action = HTML_SetCursor_t.actionServer;
			if (action != null)
			{
				action(HTML_SetCursor_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal uint EMouseCursor;

			public static implicit operator HTML_SetCursor_t(HTML_SetCursor_t.Pack8 d)
			{
				HTML_SetCursor_t hTMLSetCursorT = new HTML_SetCursor_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					EMouseCursor = d.EMouseCursor
				};
				return hTMLSetCursorT;
			}

			public static implicit operator Pack8(HTML_SetCursor_t d)
			{
				HTML_SetCursor_t.Pack8 pack8 = new HTML_SetCursor_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					EMouseCursor = d.EMouseCursor
				};
				return pack8;
			}
		}
	}
}