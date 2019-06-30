using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct DeleteItemResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal readonly static int StructSize;

		private static Action<DeleteItemResult_t> actionClient;

		private static Action<DeleteItemResult_t> actionServer;

		static DeleteItemResult_t()
		{
			DeleteItemResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(DeleteItemResult_t) : typeof(DeleteItemResult_t.Pack8)));
		}

		internal static DeleteItemResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (DeleteItemResult_t)Marshal.PtrToStructure(p, typeof(DeleteItemResult_t)) : (DeleteItemResult_t.Pack8)Marshal.PtrToStructure(p, typeof(DeleteItemResult_t.Pack8)));
		}

		public static async Task<DeleteItemResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			DeleteItemResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(DeleteItemResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, DeleteItemResult_t.StructSize, 3417, ref flag) | flag))
					{
						nullable = new DeleteItemResult_t?(DeleteItemResult_t.Fill(intPtr));
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

		public static void Install(Action<DeleteItemResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(DeleteItemResult_t.OnClient), DeleteItemResult_t.StructSize, 3417, false);
				DeleteItemResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(DeleteItemResult_t.OnServer), DeleteItemResult_t.StructSize, 3417, true);
				DeleteItemResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<DeleteItemResult_t> action = DeleteItemResult_t.actionClient;
			if (action != null)
			{
				action(DeleteItemResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<DeleteItemResult_t> action = DeleteItemResult_t.actionServer;
			if (action != null)
			{
				action(DeleteItemResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			public static implicit operator DeleteItemResult_t(DeleteItemResult_t.Pack8 d)
			{
				DeleteItemResult_t deleteItemResultT = new DeleteItemResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return deleteItemResultT;
			}

			public static implicit operator Pack8(DeleteItemResult_t d)
			{
				DeleteItemResult_t.Pack8 pack8 = new DeleteItemResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId
				};
				return pack8;
			}
		}
	}
}