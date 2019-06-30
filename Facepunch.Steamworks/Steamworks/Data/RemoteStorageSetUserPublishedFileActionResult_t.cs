using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageSetUserPublishedFileActionResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal WorkshopFileAction Action;

		internal readonly static int StructSize;

		private static Action<RemoteStorageSetUserPublishedFileActionResult_t> actionClient;

		private static Action<RemoteStorageSetUserPublishedFileActionResult_t> actionServer;

		static RemoteStorageSetUserPublishedFileActionResult_t()
		{
			RemoteStorageSetUserPublishedFileActionResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageSetUserPublishedFileActionResult_t) : typeof(RemoteStorageSetUserPublishedFileActionResult_t.Pack8)));
		}

		internal static RemoteStorageSetUserPublishedFileActionResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageSetUserPublishedFileActionResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageSetUserPublishedFileActionResult_t)) : (RemoteStorageSetUserPublishedFileActionResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageSetUserPublishedFileActionResult_t.Pack8)));
		}

		public static async Task<RemoteStorageSetUserPublishedFileActionResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageSetUserPublishedFileActionResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageSetUserPublishedFileActionResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageSetUserPublishedFileActionResult_t.StructSize, 1327, ref flag) | flag))
					{
						nullable = new RemoteStorageSetUserPublishedFileActionResult_t?(RemoteStorageSetUserPublishedFileActionResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageSetUserPublishedFileActionResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageSetUserPublishedFileActionResult_t.OnClient), RemoteStorageSetUserPublishedFileActionResult_t.StructSize, 1327, false);
				RemoteStorageSetUserPublishedFileActionResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageSetUserPublishedFileActionResult_t.OnServer), RemoteStorageSetUserPublishedFileActionResult_t.StructSize, 1327, true);
				RemoteStorageSetUserPublishedFileActionResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageSetUserPublishedFileActionResult_t> action = RemoteStorageSetUserPublishedFileActionResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageSetUserPublishedFileActionResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageSetUserPublishedFileActionResult_t> action = RemoteStorageSetUserPublishedFileActionResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageSetUserPublishedFileActionResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal WorkshopFileAction Action;

			public static implicit operator RemoteStorageSetUserPublishedFileActionResult_t(RemoteStorageSetUserPublishedFileActionResult_t.Pack8 d)
			{
				RemoteStorageSetUserPublishedFileActionResult_t remoteStorageSetUserPublishedFileActionResultT = new RemoteStorageSetUserPublishedFileActionResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					Action = d.Action
				};
				return remoteStorageSetUserPublishedFileActionResultT;
			}

			public static implicit operator Pack8(RemoteStorageSetUserPublishedFileActionResult_t d)
			{
				RemoteStorageSetUserPublishedFileActionResult_t.Pack8 pack8 = new RemoteStorageSetUserPublishedFileActionResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					Action = d.Action
				};
				return pack8;
			}
		}
	}
}