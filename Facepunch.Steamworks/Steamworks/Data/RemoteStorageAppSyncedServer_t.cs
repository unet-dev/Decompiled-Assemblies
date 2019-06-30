using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageAppSyncedServer_t
	{
		internal AppId AppID;

		internal Steamworks.Result Result;

		internal int NumUploads;

		internal readonly static int StructSize;

		private static Action<RemoteStorageAppSyncedServer_t> actionClient;

		private static Action<RemoteStorageAppSyncedServer_t> actionServer;

		static RemoteStorageAppSyncedServer_t()
		{
			RemoteStorageAppSyncedServer_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageAppSyncedServer_t) : typeof(RemoteStorageAppSyncedServer_t.Pack8)));
		}

		internal static RemoteStorageAppSyncedServer_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageAppSyncedServer_t)Marshal.PtrToStructure(p, typeof(RemoteStorageAppSyncedServer_t)) : (RemoteStorageAppSyncedServer_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageAppSyncedServer_t.Pack8)));
		}

		public static async Task<RemoteStorageAppSyncedServer_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageAppSyncedServer_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageAppSyncedServer_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageAppSyncedServer_t.StructSize, 1302, ref flag) | flag))
					{
						nullable = new RemoteStorageAppSyncedServer_t?(RemoteStorageAppSyncedServer_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageAppSyncedServer_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageAppSyncedServer_t.OnClient), RemoteStorageAppSyncedServer_t.StructSize, 1302, false);
				RemoteStorageAppSyncedServer_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageAppSyncedServer_t.OnServer), RemoteStorageAppSyncedServer_t.StructSize, 1302, true);
				RemoteStorageAppSyncedServer_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageAppSyncedServer_t> action = RemoteStorageAppSyncedServer_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageAppSyncedServer_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageAppSyncedServer_t> action = RemoteStorageAppSyncedServer_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageAppSyncedServer_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal AppId AppID;

			internal Steamworks.Result Result;

			internal int NumUploads;

			public static implicit operator RemoteStorageAppSyncedServer_t(RemoteStorageAppSyncedServer_t.Pack8 d)
			{
				RemoteStorageAppSyncedServer_t remoteStorageAppSyncedServerT = new RemoteStorageAppSyncedServer_t()
				{
					AppID = d.AppID,
					Result = d.Result,
					NumUploads = d.NumUploads
				};
				return remoteStorageAppSyncedServerT;
			}

			public static implicit operator Pack8(RemoteStorageAppSyncedServer_t d)
			{
				RemoteStorageAppSyncedServer_t.Pack8 pack8 = new RemoteStorageAppSyncedServer_t.Pack8()
				{
					AppID = d.AppID,
					Result = d.Result,
					NumUploads = d.NumUploads
				};
				return pack8;
			}
		}
	}
}