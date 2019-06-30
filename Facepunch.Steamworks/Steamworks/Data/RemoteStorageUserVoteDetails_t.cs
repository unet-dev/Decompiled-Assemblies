using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageUserVoteDetails_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal WorkshopVote Vote;

		internal readonly static int StructSize;

		private static Action<RemoteStorageUserVoteDetails_t> actionClient;

		private static Action<RemoteStorageUserVoteDetails_t> actionServer;

		static RemoteStorageUserVoteDetails_t()
		{
			RemoteStorageUserVoteDetails_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageUserVoteDetails_t) : typeof(RemoteStorageUserVoteDetails_t.Pack8)));
		}

		internal static RemoteStorageUserVoteDetails_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageUserVoteDetails_t)Marshal.PtrToStructure(p, typeof(RemoteStorageUserVoteDetails_t)) : (RemoteStorageUserVoteDetails_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageUserVoteDetails_t.Pack8)));
		}

		public static async Task<RemoteStorageUserVoteDetails_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageUserVoteDetails_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageUserVoteDetails_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageUserVoteDetails_t.StructSize, 1325, ref flag) | flag))
					{
						nullable = new RemoteStorageUserVoteDetails_t?(RemoteStorageUserVoteDetails_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageUserVoteDetails_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageUserVoteDetails_t.OnClient), RemoteStorageUserVoteDetails_t.StructSize, 1325, false);
				RemoteStorageUserVoteDetails_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageUserVoteDetails_t.OnServer), RemoteStorageUserVoteDetails_t.StructSize, 1325, true);
				RemoteStorageUserVoteDetails_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageUserVoteDetails_t> action = RemoteStorageUserVoteDetails_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageUserVoteDetails_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageUserVoteDetails_t> action = RemoteStorageUserVoteDetails_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageUserVoteDetails_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal WorkshopVote Vote;

			public static implicit operator RemoteStorageUserVoteDetails_t(RemoteStorageUserVoteDetails_t.Pack8 d)
			{
				RemoteStorageUserVoteDetails_t remoteStorageUserVoteDetailsT = new RemoteStorageUserVoteDetails_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					Vote = d.Vote
				};
				return remoteStorageUserVoteDetailsT;
			}

			public static implicit operator Pack8(RemoteStorageUserVoteDetails_t d)
			{
				RemoteStorageUserVoteDetails_t.Pack8 pack8 = new RemoteStorageUserVoteDetails_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					Vote = d.Vote
				};
				return pack8;
			}
		}
	}
}