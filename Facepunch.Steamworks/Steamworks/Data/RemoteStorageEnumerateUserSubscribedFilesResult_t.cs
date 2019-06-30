using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageEnumerateUserSubscribedFilesResult_t
	{
		internal Steamworks.Result Result;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal PublishedFileId[] GPublishedFileId;

		internal uint[] GRTimeSubscribed;

		internal readonly static int StructSize;

		private static Action<RemoteStorageEnumerateUserSubscribedFilesResult_t> actionClient;

		private static Action<RemoteStorageEnumerateUserSubscribedFilesResult_t> actionServer;

		static RemoteStorageEnumerateUserSubscribedFilesResult_t()
		{
			RemoteStorageEnumerateUserSubscribedFilesResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t) : typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t.Pack8)));
		}

		internal static RemoteStorageEnumerateUserSubscribedFilesResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageEnumerateUserSubscribedFilesResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t)) : (RemoteStorageEnumerateUserSubscribedFilesResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t.Pack8)));
		}

		public static async Task<RemoteStorageEnumerateUserSubscribedFilesResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageEnumerateUserSubscribedFilesResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageEnumerateUserSubscribedFilesResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageEnumerateUserSubscribedFilesResult_t.StructSize, 1314, ref flag) | flag))
					{
						nullable = new RemoteStorageEnumerateUserSubscribedFilesResult_t?(RemoteStorageEnumerateUserSubscribedFilesResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageEnumerateUserSubscribedFilesResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnClient), RemoteStorageEnumerateUserSubscribedFilesResult_t.StructSize, 1314, false);
				RemoteStorageEnumerateUserSubscribedFilesResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageEnumerateUserSubscribedFilesResult_t.OnServer), RemoteStorageEnumerateUserSubscribedFilesResult_t.StructSize, 1314, true);
				RemoteStorageEnumerateUserSubscribedFilesResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumerateUserSubscribedFilesResult_t> action = RemoteStorageEnumerateUserSubscribedFilesResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageEnumerateUserSubscribedFilesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumerateUserSubscribedFilesResult_t> action = RemoteStorageEnumerateUserSubscribedFilesResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageEnumerateUserSubscribedFilesResult_t.Fill(pvParam));
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

			internal uint[] GRTimeSubscribed;

			public static implicit operator RemoteStorageEnumerateUserSubscribedFilesResult_t(RemoteStorageEnumerateUserSubscribedFilesResult_t.Pack8 d)
			{
				RemoteStorageEnumerateUserSubscribedFilesResult_t remoteStorageEnumerateUserSubscribedFilesResultT = new RemoteStorageEnumerateUserSubscribedFilesResult_t()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId,
					GRTimeSubscribed = d.GRTimeSubscribed
				};
				return remoteStorageEnumerateUserSubscribedFilesResultT;
			}

			public static implicit operator Pack8(RemoteStorageEnumerateUserSubscribedFilesResult_t d)
			{
				RemoteStorageEnumerateUserSubscribedFilesResult_t.Pack8 pack8 = new RemoteStorageEnumerateUserSubscribedFilesResult_t.Pack8()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId,
					GRTimeSubscribed = d.GRTimeSubscribed
				};
				return pack8;
			}
		}
	}
}