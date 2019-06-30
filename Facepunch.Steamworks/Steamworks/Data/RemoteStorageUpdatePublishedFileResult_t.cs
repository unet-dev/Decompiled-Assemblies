using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageUpdatePublishedFileResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal bool UserNeedsToAcceptWorkshopLegalAgreement;

		internal readonly static int StructSize;

		private static Action<RemoteStorageUpdatePublishedFileResult_t> actionClient;

		private static Action<RemoteStorageUpdatePublishedFileResult_t> actionServer;

		static RemoteStorageUpdatePublishedFileResult_t()
		{
			RemoteStorageUpdatePublishedFileResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageUpdatePublishedFileResult_t) : typeof(RemoteStorageUpdatePublishedFileResult_t.Pack8)));
		}

		internal static RemoteStorageUpdatePublishedFileResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageUpdatePublishedFileResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageUpdatePublishedFileResult_t)) : (RemoteStorageUpdatePublishedFileResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageUpdatePublishedFileResult_t.Pack8)));
		}

		public static async Task<RemoteStorageUpdatePublishedFileResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageUpdatePublishedFileResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageUpdatePublishedFileResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageUpdatePublishedFileResult_t.StructSize, 1316, ref flag) | flag))
					{
						nullable = new RemoteStorageUpdatePublishedFileResult_t?(RemoteStorageUpdatePublishedFileResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageUpdatePublishedFileResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageUpdatePublishedFileResult_t.OnClient), RemoteStorageUpdatePublishedFileResult_t.StructSize, 1316, false);
				RemoteStorageUpdatePublishedFileResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageUpdatePublishedFileResult_t.OnServer), RemoteStorageUpdatePublishedFileResult_t.StructSize, 1316, true);
				RemoteStorageUpdatePublishedFileResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageUpdatePublishedFileResult_t> action = RemoteStorageUpdatePublishedFileResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageUpdatePublishedFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageUpdatePublishedFileResult_t> action = RemoteStorageUpdatePublishedFileResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageUpdatePublishedFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal bool UserNeedsToAcceptWorkshopLegalAgreement;

			public static implicit operator RemoteStorageUpdatePublishedFileResult_t(RemoteStorageUpdatePublishedFileResult_t.Pack8 d)
			{
				RemoteStorageUpdatePublishedFileResult_t remoteStorageUpdatePublishedFileResultT = new RemoteStorageUpdatePublishedFileResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement
				};
				return remoteStorageUpdatePublishedFileResultT;
			}

			public static implicit operator Pack8(RemoteStorageUpdatePublishedFileResult_t d)
			{
				RemoteStorageUpdatePublishedFileResult_t.Pack8 pack8 = new RemoteStorageUpdatePublishedFileResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement
				};
				return pack8;
			}
		}
	}
}