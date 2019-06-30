using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SetUserItemVoteResult_t
	{
		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal Steamworks.Result Result;

		internal bool VoteUp;

		internal readonly static int StructSize;

		private static Action<SetUserItemVoteResult_t> actionClient;

		private static Action<SetUserItemVoteResult_t> actionServer;

		static SetUserItemVoteResult_t()
		{
			SetUserItemVoteResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SetUserItemVoteResult_t) : typeof(SetUserItemVoteResult_t.Pack8)));
		}

		internal static SetUserItemVoteResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SetUserItemVoteResult_t)Marshal.PtrToStructure(p, typeof(SetUserItemVoteResult_t)) : (SetUserItemVoteResult_t.Pack8)Marshal.PtrToStructure(p, typeof(SetUserItemVoteResult_t.Pack8)));
		}

		public static async Task<SetUserItemVoteResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SetUserItemVoteResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SetUserItemVoteResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SetUserItemVoteResult_t.StructSize, 3408, ref flag) | flag))
					{
						nullable = new SetUserItemVoteResult_t?(SetUserItemVoteResult_t.Fill(intPtr));
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

		public static void Install(Action<SetUserItemVoteResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SetUserItemVoteResult_t.OnClient), SetUserItemVoteResult_t.StructSize, 3408, false);
				SetUserItemVoteResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SetUserItemVoteResult_t.OnServer), SetUserItemVoteResult_t.StructSize, 3408, true);
				SetUserItemVoteResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SetUserItemVoteResult_t> action = SetUserItemVoteResult_t.actionClient;
			if (action != null)
			{
				action(SetUserItemVoteResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SetUserItemVoteResult_t> action = SetUserItemVoteResult_t.actionServer;
			if (action != null)
			{
				action(SetUserItemVoteResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal Steamworks.Result Result;

			internal bool VoteUp;

			public static implicit operator SetUserItemVoteResult_t(SetUserItemVoteResult_t.Pack8 d)
			{
				SetUserItemVoteResult_t setUserItemVoteResultT = new SetUserItemVoteResult_t()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					VoteUp = d.VoteUp
				};
				return setUserItemVoteResultT;
			}

			public static implicit operator Pack8(SetUserItemVoteResult_t d)
			{
				SetUserItemVoteResult_t.Pack8 pack8 = new SetUserItemVoteResult_t.Pack8()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					VoteUp = d.VoteUp
				};
				return pack8;
			}
		}
	}
}