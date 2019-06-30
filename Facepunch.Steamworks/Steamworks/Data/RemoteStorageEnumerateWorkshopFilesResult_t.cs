using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageEnumerateWorkshopFilesResult_t
	{
		internal Steamworks.Result Result;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal PublishedFileId[] GPublishedFileId;

		internal float[] GScore;

		internal Steamworks.AppId AppId;

		internal uint StartIndex;

		internal readonly static int StructSize;

		private static Action<RemoteStorageEnumerateWorkshopFilesResult_t> actionClient;

		private static Action<RemoteStorageEnumerateWorkshopFilesResult_t> actionServer;

		static RemoteStorageEnumerateWorkshopFilesResult_t()
		{
			RemoteStorageEnumerateWorkshopFilesResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageEnumerateWorkshopFilesResult_t) : typeof(RemoteStorageEnumerateWorkshopFilesResult_t.Pack8)));
		}

		internal static RemoteStorageEnumerateWorkshopFilesResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageEnumerateWorkshopFilesResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateWorkshopFilesResult_t)) : (RemoteStorageEnumerateWorkshopFilesResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumerateWorkshopFilesResult_t.Pack8)));
		}

		public static async Task<RemoteStorageEnumerateWorkshopFilesResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageEnumerateWorkshopFilesResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageEnumerateWorkshopFilesResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageEnumerateWorkshopFilesResult_t.StructSize, 1319, ref flag) | flag))
					{
						nullable = new RemoteStorageEnumerateWorkshopFilesResult_t?(RemoteStorageEnumerateWorkshopFilesResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageEnumerateWorkshopFilesResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageEnumerateWorkshopFilesResult_t.OnClient), RemoteStorageEnumerateWorkshopFilesResult_t.StructSize, 1319, false);
				RemoteStorageEnumerateWorkshopFilesResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageEnumerateWorkshopFilesResult_t.OnServer), RemoteStorageEnumerateWorkshopFilesResult_t.StructSize, 1319, true);
				RemoteStorageEnumerateWorkshopFilesResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumerateWorkshopFilesResult_t> action = RemoteStorageEnumerateWorkshopFilesResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageEnumerateWorkshopFilesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumerateWorkshopFilesResult_t> action = RemoteStorageEnumerateWorkshopFilesResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageEnumerateWorkshopFilesResult_t.Fill(pvParam));
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

			internal float[] GScore;

			internal Steamworks.AppId AppId;

			internal uint StartIndex;

			public static implicit operator RemoteStorageEnumerateWorkshopFilesResult_t(RemoteStorageEnumerateWorkshopFilesResult_t.Pack8 d)
			{
				RemoteStorageEnumerateWorkshopFilesResult_t remoteStorageEnumerateWorkshopFilesResultT = new RemoteStorageEnumerateWorkshopFilesResult_t()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId,
					GScore = d.GScore,
					AppId = d.AppId,
					StartIndex = d.StartIndex
				};
				return remoteStorageEnumerateWorkshopFilesResultT;
			}

			public static implicit operator Pack8(RemoteStorageEnumerateWorkshopFilesResult_t d)
			{
				RemoteStorageEnumerateWorkshopFilesResult_t.Pack8 pack8 = new RemoteStorageEnumerateWorkshopFilesResult_t.Pack8()
				{
					Result = d.Result,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId,
					GScore = d.GScore,
					AppId = d.AppId,
					StartIndex = d.StartIndex
				};
				return pack8;
			}
		}
	}
}