using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageEnumerateUserSharedWorkshopFilesResult_t
	{
		internal Steamworks.Result Result;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal PublishedFileId[] GPublishedFileId;

		internal readonly static int StructSize;

		private static Action<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t> actionClient;

		private static Action<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t> actionServer;

		static RemoteStorageEnumerateUserSharedWorkshopFilesResult_t()
		{
			RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t) : typeof(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Pack8)));
		}

		internal static RemoteStorageEnumerateUserSharedWorkshopFilesResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageEnumerateUserSharedWorkshopFilesResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t)) : (RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Pack8)));
		}

		public static async Task<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageEnumerateUserSharedWorkshopFilesResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.StructSize, 1326, ref flag) | flag))
					{
						nullable = new RemoteStorageEnumerateUserSharedWorkshopFilesResult_t?(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.OnClient), RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.StructSize, 1326, false);
				RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.OnServer), RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.StructSize, 1326, true);
				RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t> action = RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t> action = RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal int ResultsReturned;

			internal int TotalResultCount;

			internal PublishedFileId[] GPublishedFileId;

			public static implicit operator RemoteStorageEnumerateUserSharedWorkshopFilesResult_t(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Pack8 d)
			{
				RemoteStorageEnumerateUserSharedWorkshopFilesResult_t remoteStorageEnumerateUserSharedWorkshopFilesResultT = new RemoteStorageEnumerateUserSharedWorkshopFilesResult_t()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId
				};
				return remoteStorageEnumerateUserSharedWorkshopFilesResultT;
			}

			public static implicit operator Pack8(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t d)
			{
				RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Pack8 pack8 = new RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.Pack8()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId
				};
				return pack8;
			}
		}
	}
}