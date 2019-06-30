using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GetUserItemVoteResult_t
	{
		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal Steamworks.Result Result;

		internal bool VotedUp;

		internal bool VotedDown;

		internal bool VoteSkipped;

		internal readonly static int StructSize;

		private static Action<GetUserItemVoteResult_t> actionClient;

		private static Action<GetUserItemVoteResult_t> actionServer;

		static GetUserItemVoteResult_t()
		{
			GetUserItemVoteResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GetUserItemVoteResult_t) : typeof(GetUserItemVoteResult_t.Pack8)));
		}

		internal static GetUserItemVoteResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GetUserItemVoteResult_t)Marshal.PtrToStructure(p, typeof(GetUserItemVoteResult_t)) : (GetUserItemVoteResult_t.Pack8)Marshal.PtrToStructure(p, typeof(GetUserItemVoteResult_t.Pack8)));
		}

		public static async Task<GetUserItemVoteResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GetUserItemVoteResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GetUserItemVoteResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GetUserItemVoteResult_t.StructSize, 3409, ref flag) | flag))
					{
						nullable = new GetUserItemVoteResult_t?(GetUserItemVoteResult_t.Fill(intPtr));
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

		public static void Install(Action<GetUserItemVoteResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GetUserItemVoteResult_t.OnClient), GetUserItemVoteResult_t.StructSize, 3409, false);
				GetUserItemVoteResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GetUserItemVoteResult_t.OnServer), GetUserItemVoteResult_t.StructSize, 3409, true);
				GetUserItemVoteResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GetUserItemVoteResult_t> action = GetUserItemVoteResult_t.actionClient;
			if (action != null)
			{
				action(GetUserItemVoteResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GetUserItemVoteResult_t> action = GetUserItemVoteResult_t.actionServer;
			if (action != null)
			{
				action(GetUserItemVoteResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal Steamworks.Result Result;

			internal bool VotedUp;

			internal bool VotedDown;

			internal bool VoteSkipped;

			public static implicit operator GetUserItemVoteResult_t(GetUserItemVoteResult_t.Pack8 d)
			{
				GetUserItemVoteResult_t getUserItemVoteResultT = new GetUserItemVoteResult_t()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					VotedUp = d.VotedUp,
					VotedDown = d.VotedDown,
					VoteSkipped = d.VoteSkipped
				};
				return getUserItemVoteResultT;
			}

			public static implicit operator Pack8(GetUserItemVoteResult_t d)
			{
				GetUserItemVoteResult_t.Pack8 pack8 = new GetUserItemVoteResult_t.Pack8()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					VotedUp = d.VotedUp,
					VotedDown = d.VotedDown,
					VoteSkipped = d.VoteSkipped
				};
				return pack8;
			}
		}
	}
}