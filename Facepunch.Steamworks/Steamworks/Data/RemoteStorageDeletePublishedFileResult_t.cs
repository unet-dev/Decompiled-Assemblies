using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageDeletePublishedFileResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal readonly static int StructSize;

		private static Action<RemoteStorageDeletePublishedFileResult_t> actionClient;

		private static Action<RemoteStorageDeletePublishedFileResult_t> actionServer;

		static RemoteStorageDeletePublishedFileResult_t()
		{
			RemoteStorageDeletePublishedFileResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageDeletePublishedFileResult_t) : typeof(RemoteStorageDeletePublishedFileResult_t.Pack8)));
		}

		internal static RemoteStorageDeletePublishedFileResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageDeletePublishedFileResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageDeletePublishedFileResult_t)) : (RemoteStorageDeletePublishedFileResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageDeletePublishedFileResult_t.Pack8)));
		}

		public static async Task<RemoteStorageDeletePublishedFileResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageDeletePublishedFileResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageDeletePublishedFileResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageDeletePublishedFileResult_t.StructSize, 1311, ref flag) | flag))
					{
						nullable = new RemoteStorageDeletePublishedFileResult_t?(RemoteStorageDeletePublishedFileResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageDeletePublishedFileResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageDeletePublishedFileResult_t.OnClient), RemoteStorageDeletePublishedFileResult_t.StructSize, 1311, false);
				RemoteStorageDeletePublishedFileResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageDeletePublishedFileResult_t.OnServer), RemoteStorageDeletePublishedFileResult_t.StructSize, 1311, true);
				RemoteStorageDeletePublishedFileResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageDeletePublishedFileResult_t> action = RemoteStorageDeletePublishedFileResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageDeletePublishedFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageDeletePublishedFileResult_t> action = RemoteStorageDeletePublishedFileResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageDeletePublishedFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			public static implicit operator RemoteStorageDeletePublishedFileResult_t(RemoteStorageDeletePublishedFileResult_t.Pack8 d)
			{
				RemoteStorageDeletePublishedFileResult_t remoteStorageDeletePublishedFileResultT = new RemoteStorageDeletePublishedFileResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return remoteStorageDeletePublishedFileResultT;
			}

			public static implicit operator Pack8(RemoteStorageDeletePublishedFileResult_t d)
			{
				RemoteStorageDeletePublishedFileResult_t.Pack8 pack8 = new RemoteStorageDeletePublishedFileResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return pack8;
			}
		}
	}
}