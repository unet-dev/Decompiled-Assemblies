using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStoragePublishFileResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal bool UserNeedsToAcceptWorkshopLegalAgreement;

		internal readonly static int StructSize;

		private static Action<RemoteStoragePublishFileResult_t> actionClient;

		private static Action<RemoteStoragePublishFileResult_t> actionServer;

		static RemoteStoragePublishFileResult_t()
		{
			RemoteStoragePublishFileResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStoragePublishFileResult_t) : typeof(RemoteStoragePublishFileResult_t.Pack8)));
		}

		internal static RemoteStoragePublishFileResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStoragePublishFileResult_t)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishFileResult_t)) : (RemoteStoragePublishFileResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishFileResult_t.Pack8)));
		}

		public static async Task<RemoteStoragePublishFileResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStoragePublishFileResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStoragePublishFileResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStoragePublishFileResult_t.StructSize, 1309, ref flag) | flag))
					{
						nullable = new RemoteStoragePublishFileResult_t?(RemoteStoragePublishFileResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStoragePublishFileResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStoragePublishFileResult_t.OnClient), RemoteStoragePublishFileResult_t.StructSize, 1309, false);
				RemoteStoragePublishFileResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStoragePublishFileResult_t.OnServer), RemoteStoragePublishFileResult_t.StructSize, 1309, true);
				RemoteStoragePublishFileResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishFileResult_t> action = RemoteStoragePublishFileResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStoragePublishFileResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishFileResult_t> action = RemoteStoragePublishFileResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStoragePublishFileResult_t.Fill(pvParam));
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

			public static implicit operator RemoteStoragePublishFileResult_t(RemoteStoragePublishFileResult_t.Pack8 d)
			{
				RemoteStoragePublishFileResult_t remoteStoragePublishFileResultT = new RemoteStoragePublishFileResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement
				};
				return remoteStoragePublishFileResultT;
			}

			public static implicit operator Pack8(RemoteStoragePublishFileResult_t d)
			{
				RemoteStoragePublishFileResult_t.Pack8 pack8 = new RemoteStoragePublishFileResult_t.Pack8()
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