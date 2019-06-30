using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStoragePublishedFileDeleted_t
	{
		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<RemoteStoragePublishedFileDeleted_t> actionClient;

		private static Action<RemoteStoragePublishedFileDeleted_t> actionServer;

		static RemoteStoragePublishedFileDeleted_t()
		{
			RemoteStoragePublishedFileDeleted_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStoragePublishedFileDeleted_t) : typeof(RemoteStoragePublishedFileDeleted_t.Pack8)));
		}

		internal static RemoteStoragePublishedFileDeleted_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStoragePublishedFileDeleted_t)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileDeleted_t)) : (RemoteStoragePublishedFileDeleted_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileDeleted_t.Pack8)));
		}

		public static async Task<RemoteStoragePublishedFileDeleted_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStoragePublishedFileDeleted_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStoragePublishedFileDeleted_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStoragePublishedFileDeleted_t.StructSize, 1323, ref flag) | flag))
					{
						nullable = new RemoteStoragePublishedFileDeleted_t?(RemoteStoragePublishedFileDeleted_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStoragePublishedFileDeleted_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStoragePublishedFileDeleted_t.OnClient), RemoteStoragePublishedFileDeleted_t.StructSize, 1323, false);
				RemoteStoragePublishedFileDeleted_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStoragePublishedFileDeleted_t.OnServer), RemoteStoragePublishedFileDeleted_t.StructSize, 1323, true);
				RemoteStoragePublishedFileDeleted_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishedFileDeleted_t> action = RemoteStoragePublishedFileDeleted_t.actionClient;
			if (action != null)
			{
				action(RemoteStoragePublishedFileDeleted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishedFileDeleted_t> action = RemoteStoragePublishedFileDeleted_t.actionServer;
			if (action != null)
			{
				action(RemoteStoragePublishedFileDeleted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal AppId AppID;

			public static implicit operator RemoteStoragePublishedFileDeleted_t(RemoteStoragePublishedFileDeleted_t.Pack8 d)
			{
				RemoteStoragePublishedFileDeleted_t remoteStoragePublishedFileDeletedT = new RemoteStoragePublishedFileDeleted_t()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return remoteStoragePublishedFileDeletedT;
			}

			public static implicit operator Pack8(RemoteStoragePublishedFileDeleted_t d)
			{
				RemoteStoragePublishedFileDeleted_t.Pack8 pack8 = new RemoteStoragePublishedFileDeleted_t.Pack8()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return pack8;
			}
		}
	}
}