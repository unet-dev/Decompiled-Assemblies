using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_HorizontalScroll_t
	{
		internal uint UnBrowserHandle;

		internal uint UnScrollMax;

		internal uint UnScrollCurrent;

		internal float FlPageScale;

		internal bool BVisible;

		internal uint UnPageSize;

		internal readonly static int StructSize;

		private static Action<HTML_HorizontalScroll_t> actionClient;

		private static Action<HTML_HorizontalScroll_t> actionServer;

		static HTML_HorizontalScroll_t()
		{
			HTML_HorizontalScroll_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_HorizontalScroll_t) : typeof(HTML_HorizontalScroll_t.Pack8)));
		}

		internal static HTML_HorizontalScroll_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_HorizontalScroll_t)Marshal.PtrToStructure(p, typeof(HTML_HorizontalScroll_t)) : (HTML_HorizontalScroll_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_HorizontalScroll_t.Pack8)));
		}

		public static async Task<HTML_HorizontalScroll_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_HorizontalScroll_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_HorizontalScroll_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_HorizontalScroll_t.StructSize, 4511, ref flag) | flag))
					{
						nullable = new HTML_HorizontalScroll_t?(HTML_HorizontalScroll_t.Fill(intPtr));
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

		public static void Install(Action<HTML_HorizontalScroll_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_HorizontalScroll_t.OnClient), HTML_HorizontalScroll_t.StructSize, 4511, false);
				HTML_HorizontalScroll_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_HorizontalScroll_t.OnServer), HTML_HorizontalScroll_t.StructSize, 4511, true);
				HTML_HorizontalScroll_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_HorizontalScroll_t> action = HTML_HorizontalScroll_t.actionClient;
			if (action != null)
			{
				action(HTML_HorizontalScroll_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_HorizontalScroll_t> action = HTML_HorizontalScroll_t.actionServer;
			if (action != null)
			{
				action(HTML_HorizontalScroll_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal uint UnScrollMax;

			internal uint UnScrollCurrent;

			internal float FlPageScale;

			internal bool BVisible;

			internal uint UnPageSize;

			public static implicit operator HTML_HorizontalScroll_t(HTML_HorizontalScroll_t.Pack8 d)
			{
				HTML_HorizontalScroll_t hTMLHorizontalScrollT = new HTML_HorizontalScroll_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnScrollMax = d.UnScrollMax,
					UnScrollCurrent = d.UnScrollCurrent,
					FlPageScale = d.FlPageScale,
					BVisible = d.BVisible,
					UnPageSize = d.UnPageSize
				};
				return hTMLHorizontalScrollT;
			}

			public static implicit operator Pack8(HTML_HorizontalScroll_t d)
			{
				HTML_HorizontalScroll_t.Pack8 pack8 = new HTML_HorizontalScroll_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					UnScrollMax = d.UnScrollMax,
					UnScrollCurrent = d.UnScrollCurrent,
					FlPageScale = d.FlPageScale,
					BVisible = d.BVisible,
					UnPageSize = d.UnPageSize
				};
				return pack8;
			}
		}
	}
}