using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageEnumerateUserPublishedFilesResult_t
	{
		internal Steamworks.Result Result;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal PublishedFileId[] GPublishedFileId;

		internal readonly static int StructSize;

		private static Action<RemoteStorageEnumerateUserPublishedFilesResult_t> actionClient;

		private static Action<RemoteStorageEnumerateUserPublishedFilesResult_t> actionServer;

		static RemoteStorageEnumerateUserPublishedFilesResult_t()
		{
			RemoteStorageEnumerateUserPublishedFilesResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageEnumerateUserPublishedFilesResult_t) : typeof(RemoteStorageEnumerateUserPublishedFilesResult_t.Pack8)));
		}

		internal static RemoteStorageEnumerateUserPublishedFilesResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageEnumerateUserPublishedFilesResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserPublishedFilesResult_t)) : (RemoteStorageEnumerateUserPublishedFilesResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserPublishedFilesResult_t.Pack8)));
		}

		public static async Task<RemoteStorageEnumerateUserPublishedFilesResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageEnumerateUserPublishedFilesResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageEnumerateUserPublishedFilesResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageEnumerateUserPublishedFilesResult_t.StructSize, 1312, ref flag) | flag))
					{
						nullable = new RemoteStorageEnumerateUserPublishedFilesResult_t?(RemoteStorageEnumerateUserPublishedFilesResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageEnumerateUserPublishedFilesResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageEnumerateUserPublishedFilesResult_t.OnClient), RemoteStorageEnumerateUserPublishedFilesResult_t.StructSize, 1312, false);
				RemoteStorageEnumerateUserPublishedFilesResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageEnumerateUserPublishedFilesResult_t.OnServer), RemoteStorageEnumerateUserPublishedFilesResult_t.StructSize, 1312, true);
				RemoteStorageEnumerateUserPublishedFilesResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumerateUserPublishedFilesResult_t> action = RemoteStorageEnumerateUserPublishedFilesResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageEnumerateUserPublishedFilesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumerateUserPublishedFilesResult_t> action = RemoteStorageEnumerateUserPublishedFilesResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageEnumerateUserPublishedFilesResult_t.Fill(pvParam));
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

			public static implicit operator RemoteStorageEnumerateUserPublishedFilesResult_t(RemoteStorageEnumerateUserPublishedFilesResult_t.Pack8 d)
			{
				RemoteStorageEnumerateUserPublishedFilesResult_t remoteStorageEnumerateUserPublishedFilesResultT = new RemoteStorageEnumerateUserPublishedFilesResult_t()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId
				};
				return remoteStorageEnumerateUserPublishedFilesResultT;
			}

			public static implicit operator Pack8(RemoteStorageEnumerateUserPublishedFilesResult_t d)
			{
				RemoteStorageEnumerateUserPublishedFilesResult_t.Pack8 pack8 = new RemoteStorageEnumerateUserPublishedFilesResult_t.Pack8()
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