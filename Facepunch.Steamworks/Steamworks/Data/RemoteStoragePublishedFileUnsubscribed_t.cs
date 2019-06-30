using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStoragePublishedFileUnsubscribed_t
	{
		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<RemoteStoragePublishedFileUnsubscribed_t> actionClient;

		private static Action<RemoteStoragePublishedFileUnsubscribed_t> actionServer;

		static RemoteStoragePublishedFileUnsubscribed_t()
		{
			RemoteStoragePublishedFileUnsubscribed_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStoragePublishedFileUnsubscribed_t) : typeof(RemoteStoragePublishedFileUnsubscribed_t.Pack8)));
		}

		internal static RemoteStoragePublishedFileUnsubscribed_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStoragePublishedFileUnsubscribed_t)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileUnsubscribed_t)) : (RemoteStoragePublishedFileUnsubscribed_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileUnsubscribed_t.Pack8)));
		}

		public static async Task<RemoteStoragePublishedFileUnsubscribed_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStoragePublishedFileUnsubscribed_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStoragePublishedFileUnsubscribed_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStoragePublishedFileUnsubscribed_t.StructSize, 1322, ref flag) | flag))
					{
						nullable = new RemoteStoragePublishedFileUnsubscribed_t?(RemoteStoragePublishedFileUnsubscribed_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStoragePublishedFileUnsubscribed_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStoragePublishedFileUnsubscribed_t.OnClient), RemoteStoragePublishedFileUnsubscribed_t.StructSize, 1322, false);
				RemoteStoragePublishedFileUnsubscribed_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStoragePublishedFileUnsubscribed_t.OnServer), RemoteStoragePublishedFileUnsubscribed_t.StructSize, 1322, true);
				RemoteStoragePublishedFileUnsubscribed_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishedFileUnsubscribed_t> action = RemoteStoragePublishedFileUnsubscribed_t.actionClient;
			if (action != null)
			{
				action(RemoteStoragePublishedFileUnsubscribed_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishedFileUnsubscribed_t> action = RemoteStoragePublishedFileUnsubscribed_t.actionServer;
			if (action != null)
			{
				action(RemoteStoragePublishedFileUnsubscribed_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal AppId AppID;

			public static implicit operator RemoteStoragePublishedFileUnsubscribed_t(RemoteStoragePublishedFileUnsubscribed_t.Pack8 d)
			{
				RemoteStoragePublishedFileUnsubscribed_t remoteStoragePublishedFileUnsubscribedT = new RemoteStoragePublishedFileUnsubscribed_t()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return remoteStoragePublishedFileUnsubscribedT;
			}

			public static implicit operator Pack8(RemoteStoragePublishedFileUnsubscribed_t d)
			{
				RemoteStoragePublishedFileUnsubscribed_t.Pack8 pack8 = new RemoteStoragePublishedFileUnsubscribed_t.Pack8()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return pack8;
			}
		}
	}
}