using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStoragePublishedFileSubscribed_t
	{
		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<RemoteStoragePublishedFileSubscribed_t> actionClient;

		private static Action<RemoteStoragePublishedFileSubscribed_t> actionServer;

		static RemoteStoragePublishedFileSubscribed_t()
		{
			RemoteStoragePublishedFileSubscribed_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStoragePublishedFileSubscribed_t) : typeof(RemoteStoragePublishedFileSubscribed_t.Pack8)));
		}

		internal static RemoteStoragePublishedFileSubscribed_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStoragePublishedFileSubscribed_t)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileSubscribed_t)) : (RemoteStoragePublishedFileSubscribed_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileSubscribed_t.Pack8)));
		}

		public static async Task<RemoteStoragePublishedFileSubscribed_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStoragePublishedFileSubscribed_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStoragePublishedFileSubscribed_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStoragePublishedFileSubscribed_t.StructSize, 1321, ref flag) | flag))
					{
						nullable = new RemoteStoragePublishedFileSubscribed_t?(RemoteStoragePublishedFileSubscribed_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStoragePublishedFileSubscribed_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStoragePublishedFileSubscribed_t.OnClient), RemoteStoragePublishedFileSubscribed_t.StructSize, 1321, false);
				RemoteStoragePublishedFileSubscribed_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStoragePublishedFileSubscribed_t.OnServer), RemoteStoragePublishedFileSubscribed_t.StructSize, 1321, true);
				RemoteStoragePublishedFileSubscribed_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishedFileSubscribed_t> action = RemoteStoragePublishedFileSubscribed_t.actionClient;
			if (action != null)
			{
				action(RemoteStoragePublishedFileSubscribed_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishedFileSubscribed_t> action = RemoteStoragePublishedFileSubscribed_t.actionServer;
			if (action != null)
			{
				action(RemoteStoragePublishedFileSubscribed_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal AppId AppID;

			public static implicit operator RemoteStoragePublishedFileSubscribed_t(RemoteStoragePublishedFileSubscribed_t.Pack8 d)
			{
				RemoteStoragePublishedFileSubscribed_t remoteStoragePublishedFileSubscribedT = new RemoteStoragePublishedFileSubscribed_t()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return remoteStoragePublishedFileSubscribedT;
			}

			public static implicit operator Pack8(RemoteStoragePublishedFileSubscribed_t d)
			{
				RemoteStoragePublishedFileSubscribed_t.Pack8 pack8 = new RemoteStoragePublishedFileSubscribed_t.Pack8()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return pack8;
			}
		}
	}
}