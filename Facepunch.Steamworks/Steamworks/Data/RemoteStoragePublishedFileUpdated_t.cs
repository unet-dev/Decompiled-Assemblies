using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStoragePublishedFileUpdated_t
	{
		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal AppId AppID;

		internal ulong Unused;

		internal readonly static int StructSize;

		private static Action<RemoteStoragePublishedFileUpdated_t> actionClient;

		private static Action<RemoteStoragePublishedFileUpdated_t> actionServer;

		static RemoteStoragePublishedFileUpdated_t()
		{
			RemoteStoragePublishedFileUpdated_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStoragePublishedFileUpdated_t) : typeof(RemoteStoragePublishedFileUpdated_t.Pack8)));
		}

		internal static RemoteStoragePublishedFileUpdated_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStoragePublishedFileUpdated_t)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileUpdated_t)) : (RemoteStoragePublishedFileUpdated_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishedFileUpdated_t.Pack8)));
		}

		public static async Task<RemoteStoragePublishedFileUpdated_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStoragePublishedFileUpdated_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStoragePublishedFileUpdated_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStoragePublishedFileUpdated_t.StructSize, 1330, ref flag) | flag))
					{
						nullable = new RemoteStoragePublishedFileUpdated_t?(RemoteStoragePublishedFileUpdated_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStoragePublishedFileUpdated_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStoragePublishedFileUpdated_t.OnClient), RemoteStoragePublishedFileUpdated_t.StructSize, 1330, false);
				RemoteStoragePublishedFileUpdated_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStoragePublishedFileUpdated_t.OnServer), RemoteStoragePublishedFileUpdated_t.StructSize, 1330, true);
				RemoteStoragePublishedFileUpdated_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishedFileUpdated_t> action = RemoteStoragePublishedFileUpdated_t.actionClient;
			if (action != null)
			{
				action(RemoteStoragePublishedFileUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishedFileUpdated_t> action = RemoteStoragePublishedFileUpdated_t.actionServer;
			if (action != null)
			{
				action(RemoteStoragePublishedFileUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal AppId AppID;

			internal ulong Unused;

			public static implicit operator RemoteStoragePublishedFileUpdated_t(RemoteStoragePublishedFileUpdated_t.Pack8 d)
			{
				RemoteStoragePublishedFileUpdated_t remoteStoragePublishedFileUpdatedT = new RemoteStoragePublishedFileUpdated_t()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID,
					Unused = d.Unused
				};
				return remoteStoragePublishedFileUpdatedT;
			}

			public static implicit operator Pack8(RemoteStoragePublishedFileUpdated_t d)
			{
				RemoteStoragePublishedFileUpdated_t.Pack8 pack8 = new RemoteStoragePublishedFileUpdated_t.Pack8()
				{
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID,
					Unused = d.Unused
				};
				return pack8;
			}
		}
	}
}