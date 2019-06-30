using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageAppSyncProgress_t
	{
		internal string CurrentFile;

		internal AppId AppID;

		internal uint BytesTransferredThisChunk;

		internal double DAppPercentComplete;

		internal bool Uploading;

		internal readonly static int StructSize;

		private static Action<RemoteStorageAppSyncProgress_t> actionClient;

		private static Action<RemoteStorageAppSyncProgress_t> actionServer;

		static RemoteStorageAppSyncProgress_t()
		{
			RemoteStorageAppSyncProgress_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageAppSyncProgress_t) : typeof(RemoteStorageAppSyncProgress_t.Pack8)));
		}

		internal static RemoteStorageAppSyncProgress_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageAppSyncProgress_t)Marshal.PtrToStructure(p, typeof(RemoteStorageAppSyncProgress_t)) : (RemoteStorageAppSyncProgress_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageAppSyncProgress_t.Pack8)));
		}

		public static async Task<RemoteStorageAppSyncProgress_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageAppSyncProgress_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageAppSyncProgress_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageAppSyncProgress_t.StructSize, 1303, ref flag) | flag))
					{
						nullable = new RemoteStorageAppSyncProgress_t?(RemoteStorageAppSyncProgress_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageAppSyncProgress_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageAppSyncProgress_t.OnClient), RemoteStorageAppSyncProgress_t.StructSize, 1303, false);
				RemoteStorageAppSyncProgress_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageAppSyncProgress_t.OnServer), RemoteStorageAppSyncProgress_t.StructSize, 1303, true);
				RemoteStorageAppSyncProgress_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageAppSyncProgress_t> action = RemoteStorageAppSyncProgress_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageAppSyncProgress_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageAppSyncProgress_t> action = RemoteStorageAppSyncProgress_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageAppSyncProgress_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal string CurrentFile;

			internal AppId AppID;

			internal uint BytesTransferredThisChunk;

			internal double DAppPercentComplete;

			internal bool Uploading;

			public static implicit operator RemoteStorageAppSyncProgress_t(RemoteStorageAppSyncProgress_t.Pack8 d)
			{
				RemoteStorageAppSyncProgress_t remoteStorageAppSyncProgressT = new RemoteStorageAppSyncProgress_t()
				{
					CurrentFile = d.CurrentFile,
					AppID = d.AppID,
					BytesTransferredThisChunk = d.BytesTransferredThisChunk,
					DAppPercentComplete = d.DAppPercentComplete,
					Uploading = d.Uploading
				};
				return remoteStorageAppSyncProgressT;
			}

			public static implicit operator Pack8(RemoteStorageAppSyncProgress_t d)
			{
				RemoteStorageAppSyncProgress_t.Pack8 pack8 = new RemoteStorageAppSyncProgress_t.Pack8()
				{
					CurrentFile = d.CurrentFile,
					AppID = d.AppID,
					BytesTransferredThisChunk = d.BytesTransferredThisChunk,
					DAppPercentComplete = d.DAppPercentComplete,
					Uploading = d.Uploading
				};
				return pack8;
			}
		}
	}
}