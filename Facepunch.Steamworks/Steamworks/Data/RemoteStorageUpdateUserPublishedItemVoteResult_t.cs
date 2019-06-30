using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageUpdateUserPublishedItemVoteResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal readonly static int StructSize;

		private static Action<RemoteStorageUpdateUserPublishedItemVoteResult_t> actionClient;

		private static Action<RemoteStorageUpdateUserPublishedItemVoteResult_t> actionServer;

		static RemoteStorageUpdateUserPublishedItemVoteResult_t()
		{
			RemoteStorageUpdateUserPublishedItemVoteResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t) : typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t.Pack8)));
		}

		internal static RemoteStorageUpdateUserPublishedItemVoteResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageUpdateUserPublishedItemVoteResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t)) : (RemoteStorageUpdateUserPublishedItemVoteResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t.Pack8)));
		}

		public static async Task<RemoteStorageUpdateUserPublishedItemVoteResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageUpdateUserPublishedItemVoteResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageUpdateUserPublishedItemVoteResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageUpdateUserPublishedItemVoteResult_t.StructSize, 1324, ref flag) | flag))
					{
						nullable = new RemoteStorageUpdateUserPublishedItemVoteResult_t?(RemoteStorageUpdateUserPublishedItemVoteResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageUpdateUserPublishedItemVoteResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnClient), RemoteStorageUpdateUserPublishedItemVoteResult_t.StructSize, 1324, false);
				RemoteStorageUpdateUserPublishedItemVoteResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageUpdateUserPublishedItemVoteResult_t.OnServer), RemoteStorageUpdateUserPublishedItemVoteResult_t.StructSize, 1324, true);
				RemoteStorageUpdateUserPublishedItemVoteResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageUpdateUserPublishedItemVoteResult_t> action = RemoteStorageUpdateUserPublishedItemVoteResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageUpdateUserPublishedItemVoteResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageUpdateUserPublishedItemVoteResult_t> action = RemoteStorageUpdateUserPublishedItemVoteResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageUpdateUserPublishedItemVoteResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			public static implicit operator RemoteStorageUpdateUserPublishedItemVoteResult_t(RemoteStorageUpdateUserPublishedItemVoteResult_t.Pack8 d)
			{
				RemoteStorageUpdateUserPublishedItemVoteResult_t remoteStorageUpdateUserPublishedItemVoteResultT = new RemoteStorageUpdateUserPublishedItemVoteResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return remoteStorageUpdateUserPublishedItemVoteResultT;
			}

			public static implicit operator Pack8(RemoteStorageUpdateUserPublishedItemVoteResult_t d)
			{
				RemoteStorageUpdateUserPublishedItemVoteResult_t.Pack8 pack8 = new RemoteStorageUpdateUserPublishedItemVoteResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return pack8;
			}
		}
	}
}