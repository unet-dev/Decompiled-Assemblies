using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTML_FileOpenDialog_t
	{
		internal uint UnBrowserHandle;

		internal string PchTitle;

		internal string PchInitialFile;

		internal readonly static int StructSize;

		private static Action<HTML_FileOpenDialog_t> actionClient;

		private static Action<HTML_FileOpenDialog_t> actionServer;

		static HTML_FileOpenDialog_t()
		{
			HTML_FileOpenDialog_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTML_FileOpenDialog_t) : typeof(HTML_FileOpenDialog_t.Pack8)));
		}

		internal static HTML_FileOpenDialog_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTML_FileOpenDialog_t)Marshal.PtrToStructure(p, typeof(HTML_FileOpenDialog_t)) : (HTML_FileOpenDialog_t.Pack8)Marshal.PtrToStructure(p, typeof(HTML_FileOpenDialog_t.Pack8)));
		}

		public static async Task<HTML_FileOpenDialog_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTML_FileOpenDialog_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTML_FileOpenDialog_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTML_FileOpenDialog_t.StructSize, 4516, ref flag) | flag))
					{
						nullable = new HTML_FileOpenDialog_t?(HTML_FileOpenDialog_t.Fill(intPtr));
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

		public static void Install(Action<HTML_FileOpenDialog_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTML_FileOpenDialog_t.OnClient), HTML_FileOpenDialog_t.StructSize, 4516, false);
				HTML_FileOpenDialog_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTML_FileOpenDialog_t.OnServer), HTML_FileOpenDialog_t.StructSize, 4516, true);
				HTML_FileOpenDialog_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_FileOpenDialog_t> action = HTML_FileOpenDialog_t.actionClient;
			if (action != null)
			{
				action(HTML_FileOpenDialog_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTML_FileOpenDialog_t> action = HTML_FileOpenDialog_t.actionServer;
			if (action != null)
			{
				action(HTML_FileOpenDialog_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint UnBrowserHandle;

			internal string PchTitle;

			internal string PchInitialFile;

			public static implicit operator HTML_FileOpenDialog_t(HTML_FileOpenDialog_t.Pack8 d)
			{
				HTML_FileOpenDialog_t hTMLFileOpenDialogT = new HTML_FileOpenDialog_t()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchTitle = d.PchTitle,
					PchInitialFile = d.PchInitialFile
				};
				return hTMLFileOpenDialogT;
			}

			public static implicit operator Pack8(HTML_FileOpenDialog_t d)
			{
				HTML_FileOpenDialog_t.Pack8 pack8 = new HTML_FileOpenDialog_t.Pack8()
				{
					UnBrowserHandle = d.UnBrowserHandle,
					PchTitle = d.PchTitle,
					PchInitialFile = d.PchInitialFile
				};
				return pack8;
			}
		}
	}
}