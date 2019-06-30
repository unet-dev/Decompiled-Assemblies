using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageGetPublishedItemVoteDetailsResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal int VotesFor;

		internal int VotesAgainst;

		internal int Reports;

		internal float FScore;

		internal readonly static int StructSize;

		private static Action<RemoteStorageGetPublishedItemVoteDetailsResult_t> actionClient;

		private static Action<RemoteStorageGetPublishedItemVoteDetailsResult_t> actionServer;

		static RemoteStorageGetPublishedItemVoteDetailsResult_t()
		{
			RemoteStorageGetPublishedItemVoteDetailsResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t) : typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t.Pack8)));
		}

		internal static RemoteStorageGetPublishedItemVoteDetailsResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageGetPublishedItemVoteDetailsResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t)) : (RemoteStorageGetPublishedItemVoteDetailsResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t.Pack8)));
		}

		public static async Task<RemoteStorageGetPublishedItemVoteDetailsResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageGetPublishedItemVoteDetailsResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageGetPublishedItemVoteDetailsResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageGetPublishedItemVoteDetailsResult_t.StructSize, 1320, ref flag) | flag))
					{
						nullable = new RemoteStorageGetPublishedItemVoteDetailsResult_t?(RemoteStorageGetPublishedItemVoteDetailsResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageGetPublishedItemVoteDetailsResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnClient), RemoteStorageGetPublishedItemVoteDetailsResult_t.StructSize, 1320, false);
				RemoteStorageGetPublishedItemVoteDetailsResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageGetPublishedItemVoteDetailsResult_t.OnServer), RemoteStorageGetPublishedItemVoteDetailsResult_t.StructSize, 1320, true);
				RemoteStorageGetPublishedItemVoteDetailsResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageGetPublishedItemVoteDetailsResult_t> action = RemoteStorageGetPublishedItemVoteDetailsResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageGetPublishedItemVoteDetailsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageGetPublishedItemVoteDetailsResult_t> action = RemoteStorageGetPublishedItemVoteDetailsResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageGetPublishedItemVoteDetailsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal int VotesFor;

			internal int VotesAgainst;

			internal int Reports;

			internal float FScore;

			public static implicit operator RemoteStorageGetPublishedItemVoteDetailsResult_t(RemoteStorageGetPublishedItemVoteDetailsResult_t.Pack8 d)
			{
				RemoteStorageGetPublishedItemVoteDetailsResult_t remoteStorageGetPublishedItemVoteDetailsResultT = new RemoteStorageGetPublishedItemVoteDetailsResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					VotesFor = d.VotesFor,
					VotesAgainst = d.VotesAgainst,
					Reports = d.Reports,
					FScore = d.FScore
				};
				return remoteStorageGetPublishedItemVoteDetailsResultT;
			}

			public static implicit operator Pack8(RemoteStorageGetPublishedItemVoteDetailsResult_t d)
			{
				RemoteStorageGetPublishedItemVoteDetailsResult_t.Pack8 pack8 = new RemoteStorageGetPublishedItemVoteDetailsResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					VotesFor = d.VotesFor,
					VotesAgainst = d.VotesAgainst,
					Reports = d.Reports,
					FScore = d.FScore
				};
				return pack8;
			}
		}
	}
}