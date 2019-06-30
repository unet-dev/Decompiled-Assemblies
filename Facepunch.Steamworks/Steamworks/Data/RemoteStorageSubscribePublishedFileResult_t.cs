using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageSubscribePublishedFileResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal readonly static int StructSize;

		private static Action<RemoteStorageSubscribePublishedFileResult_t> actionClient;

		private static Action<RemoteStorageSubscribePublishedFileResult_t> actionServer;

		static RemoteStorageSubscribePublishedFileResult_t()
		{
			RemoteStorageSubscribePublishedFileResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageSubscribePublishedFileResult_t) : typeof(RemoteStorageSubscribePublishedFileResult_t.Pack8)));
		}

		internal static RemoteStorageSubscribePublishedFileResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageSubscribePublishedFileResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageSubscribePublishedFileResult_t)) : (RemoteStorageSubscribePublishedFileResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageSubscribePublishedFileResult_t.Pack8)));
		}

		public static async Task<RemoteStorageSubscribePublishedFileResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageSubscribePublishedFileResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageSubscribePublishedFileResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageSubscribePublishedFileResult_t.StructSize, 1313, ref flag) | flag))
					{
						nullable = new RemoteStorageSubscribePublishedFileResult_t?(RemoteStorageSubscribePublishedFileResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageSubscribePublishedFileResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageSubscribePublishedFileResult_t.OnClient), RemoteStorageSubscribePublishedFileResult_t.StructSize, 1313, false);
				RemoteStorageSubscribePublishedFileResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageSubscribePublishedFileResult_t.OnServer), RemoteStorageSubscribePublishedFileResult_t.StructSize, 1313, true);
				RemoteStorageSubscribePublishedFileResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageSubscribePublishedFileResult_t> action = RemoteStorageSubscribePublishedFileResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageSubscribePublishedFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageSubscribePublishedFileResult_t> action = RemoteStorageSubscribePublishedFileResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageSubscribePublishedFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			public static implicit operator RemoteStorageSubscribePublishedFileResult_t(RemoteStorageSubscribePublishedFileResult_t.Pack8 d)
			{
				RemoteStorageSubscribePublishedFileResult_t remoteStorageSubscribePublishedFileResultT = new RemoteStorageSubscribePublishedFileResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return remoteStorageSubscribePublishedFileResultT;
			}

			public static implicit operator Pack8(RemoteStorageSubscribePublishedFileResult_t d)
			{
				RemoteStorageSubscribePublishedFileResult_t.Pack8 pack8 = new RemoteStorageSubscribePublishedFileResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return pack8;
			}
		}
	}
}