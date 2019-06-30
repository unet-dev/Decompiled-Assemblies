using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageAppSyncedClient_t
	{
		internal AppId AppID;

		internal Steamworks.Result Result;

		internal int NumDownloads;

		internal readonly static int StructSize;

		private static Action<RemoteStorageAppSyncedClient_t> actionClient;

		private static Action<RemoteStorageAppSyncedClient_t> actionServer;

		static RemoteStorageAppSyncedClient_t()
		{
			RemoteStorageAppSyncedClient_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageAppSyncedClient_t) : typeof(RemoteStorageAppSyncedClient_t.Pack8)));
		}

		internal static RemoteStorageAppSyncedClient_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageAppSyncedClient_t)Marshal.PtrToStructure(p, typeof(RemoteStorageAppSyncedClient_t)) : (RemoteStorageAppSyncedClient_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageAppSyncedClient_t.Pack8)));
		}

		public static async Task<RemoteStorageAppSyncedClient_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageAppSyncedClient_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageAppSyncedClient_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageAppSyncedClient_t.StructSize, 1301, ref flag) | flag))
					{
						nullable = new RemoteStorageAppSyncedClient_t?(RemoteStorageAppSyncedClient_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageAppSyncedClient_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageAppSyncedClient_t.OnClient), RemoteStorageAppSyncedClient_t.StructSize, 1301, false);
				RemoteStorageAppSyncedClient_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageAppSyncedClient_t.OnServer), RemoteStorageAppSyncedClient_t.StructSize, 1301, true);
				RemoteStorageAppSyncedClient_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageAppSyncedClient_t> action = RemoteStorageAppSyncedClient_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageAppSyncedClient_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageAppSyncedClient_t> action = RemoteStorageAppSyncedClient_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageAppSyncedClient_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal AppId AppID;

			internal Steamworks.Result Result;

			internal int NumDownloads;

			public static implicit operator RemoteStorageAppSyncedClient_t(RemoteStorageAppSyncedClient_t.Pack8 d)
			{
				RemoteStorageAppSyncedClient_t remoteStorageAppSyncedClientT = new RemoteStorageAppSyncedClient_t()
				{
					AppID = d.AppID,
					Result = d.Result,
					NumDownloads = d.NumDownloads
				};
				return remoteStorageAppSyncedClientT;
			}

			public static implicit operator Pack8(RemoteStorageAppSyncedClient_t d)
			{
				RemoteStorageAppSyncedClient_t.Pack8 pack8 = new RemoteStorageAppSyncedClient_t.Pack8()
				{
					AppID = d.AppID,
					Result = d.Result,
					NumDownloads = d.NumDownloads
				};
				return pack8;
			}
		}
	}
}