using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageAppSyncStatusCheck_t
	{
		internal AppId AppID;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<RemoteStorageAppSyncStatusCheck_t> actionClient;

		private static Action<RemoteStorageAppSyncStatusCheck_t> actionServer;

		static RemoteStorageAppSyncStatusCheck_t()
		{
			RemoteStorageAppSyncStatusCheck_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageAppSyncStatusCheck_t) : typeof(RemoteStorageAppSyncStatusCheck_t.Pack8)));
		}

		internal static RemoteStorageAppSyncStatusCheck_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageAppSyncStatusCheck_t)Marshal.PtrToStructure(p, typeof(RemoteStorageAppSyncStatusCheck_t)) : (RemoteStorageAppSyncStatusCheck_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageAppSyncStatusCheck_t.Pack8)));
		}

		public static async Task<RemoteStorageAppSyncStatusCheck_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageAppSyncStatusCheck_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageAppSyncStatusCheck_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageAppSyncStatusCheck_t.StructSize, 1305, ref flag) | flag))
					{
						nullable = new RemoteStorageAppSyncStatusCheck_t?(RemoteStorageAppSyncStatusCheck_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageAppSyncStatusCheck_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageAppSyncStatusCheck_t.OnClient), RemoteStorageAppSyncStatusCheck_t.StructSize, 1305, false);
				RemoteStorageAppSyncStatusCheck_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageAppSyncStatusCheck_t.OnServer), RemoteStorageAppSyncStatusCheck_t.StructSize, 1305, true);
				RemoteStorageAppSyncStatusCheck_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageAppSyncStatusCheck_t> action = RemoteStorageAppSyncStatusCheck_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageAppSyncStatusCheck_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageAppSyncStatusCheck_t> action = RemoteStorageAppSyncStatusCheck_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageAppSyncStatusCheck_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal AppId AppID;

			internal Steamworks.Result Result;

			public static implicit operator RemoteStorageAppSyncStatusCheck_t(RemoteStorageAppSyncStatusCheck_t.Pack8 d)
			{
				RemoteStorageAppSyncStatusCheck_t remoteStorageAppSyncStatusCheckT = new RemoteStorageAppSyncStatusCheck_t()
				{
					AppID = d.AppID,
					Result = d.Result
				};
				return remoteStorageAppSyncStatusCheckT;
			}

			public static implicit operator Pack8(RemoteStorageAppSyncStatusCheck_t d)
			{
				RemoteStorageAppSyncStatusCheck_t.Pack8 pack8 = new RemoteStorageAppSyncStatusCheck_t.Pack8()
				{
					AppID = d.AppID,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}