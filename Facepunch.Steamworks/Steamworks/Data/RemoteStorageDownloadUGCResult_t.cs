using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageDownloadUGCResult_t
	{
		internal Steamworks.Result Result;

		internal ulong File;

		internal AppId AppID;

		internal int SizeInBytes;

		internal string PchFileName;

		internal ulong SteamIDOwner;

		internal readonly static int StructSize;

		private static Action<RemoteStorageDownloadUGCResult_t> actionClient;

		private static Action<RemoteStorageDownloadUGCResult_t> actionServer;

		static RemoteStorageDownloadUGCResult_t()
		{
			RemoteStorageDownloadUGCResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageDownloadUGCResult_t) : typeof(RemoteStorageDownloadUGCResult_t.Pack8)));
		}

		internal static RemoteStorageDownloadUGCResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageDownloadUGCResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageDownloadUGCResult_t)) : (RemoteStorageDownloadUGCResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageDownloadUGCResult_t.Pack8)));
		}

		public static async Task<RemoteStorageDownloadUGCResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageDownloadUGCResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageDownloadUGCResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageDownloadUGCResult_t.StructSize, 1317, ref flag) | flag))
					{
						nullable = new RemoteStorageDownloadUGCResult_t?(RemoteStorageDownloadUGCResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageDownloadUGCResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageDownloadUGCResult_t.OnClient), RemoteStorageDownloadUGCResult_t.StructSize, 1317, false);
				RemoteStorageDownloadUGCResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageDownloadUGCResult_t.OnServer), RemoteStorageDownloadUGCResult_t.StructSize, 1317, true);
				RemoteStorageDownloadUGCResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageDownloadUGCResult_t> action = RemoteStorageDownloadUGCResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageDownloadUGCResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageDownloadUGCResult_t> action = RemoteStorageDownloadUGCResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageDownloadUGCResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong File;

			internal AppId AppID;

			internal int SizeInBytes;

			internal string PchFileName;

			internal ulong SteamIDOwner;

			public static implicit operator RemoteStorageDownloadUGCResult_t(RemoteStorageDownloadUGCResult_t.Pack8 d)
			{
				RemoteStorageDownloadUGCResult_t remoteStorageDownloadUGCResultT = new RemoteStorageDownloadUGCResult_t()
				{
					Result = d.Result,
					File = d.File,
					AppID = d.AppID,
					SizeInBytes = d.SizeInBytes,
					PchFileName = d.PchFileName,
					SteamIDOwner = d.SteamIDOwner
				};
				return remoteStorageDownloadUGCResultT;
			}

			public static implicit operator Pack8(RemoteStorageDownloadUGCResult_t d)
			{
				RemoteStorageDownloadUGCResult_t.Pack8 pack8 = new RemoteStorageDownloadUGCResult_t.Pack8()
				{
					Result = d.Result,
					File = d.File,
					AppID = d.AppID,
					SizeInBytes = d.SizeInBytes,
					PchFileName = d.PchFileName,
					SteamIDOwner = d.SteamIDOwner
				};
				return pack8;
			}
		}
	}
}