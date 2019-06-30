using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageUnsubscribePublishedFileResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal readonly static int StructSize;

		private static Action<RemoteStorageUnsubscribePublishedFileResult_t> actionClient;

		private static Action<RemoteStorageUnsubscribePublishedFileResult_t> actionServer;

		static RemoteStorageUnsubscribePublishedFileResult_t()
		{
			RemoteStorageUnsubscribePublishedFileResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageUnsubscribePublishedFileResult_t) : typeof(RemoteStorageUnsubscribePublishedFileResult_t.Pack8)));
		}

		internal static RemoteStorageUnsubscribePublishedFileResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageUnsubscribePublishedFileResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageUnsubscribePublishedFileResult_t)) : (RemoteStorageUnsubscribePublishedFileResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageUnsubscribePublishedFileResult_t.Pack8)));
		}

		public static async Task<RemoteStorageUnsubscribePublishedFileResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageUnsubscribePublishedFileResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageUnsubscribePublishedFileResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageUnsubscribePublishedFileResult_t.StructSize, 1315, ref flag) | flag))
					{
						nullable = new RemoteStorageUnsubscribePublishedFileResult_t?(RemoteStorageUnsubscribePublishedFileResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageUnsubscribePublishedFileResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageUnsubscribePublishedFileResult_t.OnClient), RemoteStorageUnsubscribePublishedFileResult_t.StructSize, 1315, false);
				RemoteStorageUnsubscribePublishedFileResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageUnsubscribePublishedFileResult_t.OnServer), RemoteStorageUnsubscribePublishedFileResult_t.StructSize, 1315, true);
				RemoteStorageUnsubscribePublishedFileResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageUnsubscribePublishedFileResult_t> action = RemoteStorageUnsubscribePublishedFileResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageUnsubscribePublishedFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageUnsubscribePublishedFileResult_t> action = RemoteStorageUnsubscribePublishedFileResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageUnsubscribePublishedFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			public static implicit operator RemoteStorageUnsubscribePublishedFileResult_t(RemoteStorageUnsubscribePublishedFileResult_t.Pack8 d)
			{
				RemoteStorageUnsubscribePublishedFileResult_t remoteStorageUnsubscribePublishedFileResultT = new RemoteStorageUnsubscribePublishedFileResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return remoteStorageUnsubscribePublishedFileResultT;
			}

			public static implicit operator Pack8(RemoteStorageUnsubscribePublishedFileResult_t d)
			{
				RemoteStorageUnsubscribePublishedFileResult_t.Pack8 pack8 = new RemoteStorageUnsubscribePublishedFileResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return pack8;
			}
		}
	}
}