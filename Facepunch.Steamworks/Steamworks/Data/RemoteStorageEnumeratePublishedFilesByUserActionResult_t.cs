using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageEnumeratePublishedFilesByUserActionResult_t
	{
		internal Steamworks.Result Result;

		internal WorkshopFileAction Action;

		internal int ResultsReturned;

		internal int TotalResultCount;

		internal PublishedFileId[] GPublishedFileId;

		internal uint[] GRTimeUpdated;

		internal readonly static int StructSize;

		private static Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t> actionClient;

		private static Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t> actionServer;

		static RemoteStorageEnumeratePublishedFilesByUserActionResult_t()
		{
			RemoteStorageEnumeratePublishedFilesByUserActionResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t) : typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Pack8)));
		}

		internal static RemoteStorageEnumeratePublishedFilesByUserActionResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageEnumeratePublishedFilesByUserActionResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t)) : (RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Pack8)));
		}

		public static async Task<RemoteStorageEnumeratePublishedFilesByUserActionResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageEnumeratePublishedFilesByUserActionResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageEnumeratePublishedFilesByUserActionResult_t.StructSize, 1328, ref flag) | flag))
					{
						nullable = new RemoteStorageEnumeratePublishedFilesByUserActionResult_t?(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnClient), RemoteStorageEnumeratePublishedFilesByUserActionResult_t.StructSize, 1328, false);
				RemoteStorageEnumeratePublishedFilesByUserActionResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.OnServer), RemoteStorageEnumeratePublishedFilesByUserActionResult_t.StructSize, 1328, true);
				RemoteStorageEnumeratePublishedFilesByUserActionResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t> action = RemoteStorageEnumeratePublishedFilesByUserActionResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageEnumeratePublishedFilesByUserActionResult_t> action = RemoteStorageEnumeratePublishedFilesByUserActionResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal WorkshopFileAction Action;

			internal int ResultsReturned;

			internal int TotalResultCount;

			internal PublishedFileId[] GPublishedFileId;

			internal uint[] GRTimeUpdated;

			public static implicit operator RemoteStorageEnumeratePublishedFilesByUserActionResult_t(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Pack8 d)
			{
				RemoteStorageEnumeratePublishedFilesByUserActionResult_t remoteStorageEnumeratePublishedFilesByUserActionResultT = new RemoteStorageEnumeratePublishedFilesByUserActionResult_t()
				{
					Result = d.Result,
					Action = d.Action,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId,
					GRTimeUpdated = d.GRTimeUpdated
				};
				return remoteStorageEnumeratePublishedFilesByUserActionResultT;
			}

			public static implicit operator Pack8(RemoteStorageEnumeratePublishedFilesByUserActionResult_t d)
			{
				RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Pack8 pack8 = new RemoteStorageEnumeratePublishedFilesByUserActionResult_t.Pack8()
				{
					Result = d.Result,
					Action = d.Action,
					ResultsReturned = d.ResultsReturned,
					TotalResultCount = d.TotalResultCount,
					GPublishedFileId = d.GPublishedFileId,
					GRTimeUpdated = d.GRTimeUpdated
				};
				return pack8;
			}
		}
	}
}