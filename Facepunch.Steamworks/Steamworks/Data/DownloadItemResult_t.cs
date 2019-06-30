using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct DownloadItemResult_t
	{
		internal AppId AppID;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<DownloadItemResult_t> actionClient;

		private static Action<DownloadItemResult_t> actionServer;

		static DownloadItemResult_t()
		{
			DownloadItemResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(DownloadItemResult_t) : typeof(DownloadItemResult_t.Pack8)));
		}

		internal static DownloadItemResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (DownloadItemResult_t)Marshal.PtrToStructure(p, typeof(DownloadItemResult_t)) : (DownloadItemResult_t.Pack8)Marshal.PtrToStructure(p, typeof(DownloadItemResult_t.Pack8)));
		}

		public static async Task<DownloadItemResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			DownloadItemResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(DownloadItemResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, DownloadItemResult_t.StructSize, 3406, ref flag) | flag))
					{
						nullable = new DownloadItemResult_t?(DownloadItemResult_t.Fill(intPtr));
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

		public static void Install(Action<DownloadItemResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(DownloadItemResult_t.OnClient), DownloadItemResult_t.StructSize, 3406, false);
				DownloadItemResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(DownloadItemResult_t.OnServer), DownloadItemResult_t.StructSize, 3406, true);
				DownloadItemResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<DownloadItemResult_t> action = DownloadItemResult_t.actionClient;
			if (action != null)
			{
				action(DownloadItemResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<DownloadItemResult_t> action = DownloadItemResult_t.actionServer;
			if (action != null)
			{
				action(DownloadItemResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal AppId AppID;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal Steamworks.Result Result;

			public static implicit operator DownloadItemResult_t(DownloadItemResult_t.Pack8 d)
			{
				DownloadItemResult_t downloadItemResultT = new DownloadItemResult_t()
				{
					AppID = d.AppID,
					PublishedFileId = d.PublishedFileId,
					Result = d.Result
				};
				return downloadItemResultT;
			}

			public static implicit operator Pack8(DownloadItemResult_t d)
			{
				DownloadItemResult_t.Pack8 pack8 = new DownloadItemResult_t.Pack8()
				{
					AppID = d.AppID,
					PublishedFileId = d.PublishedFileId,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}