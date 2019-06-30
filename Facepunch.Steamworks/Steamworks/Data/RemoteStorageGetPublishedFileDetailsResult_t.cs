using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageGetPublishedFileDetailsResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal AppId CreatorAppID;

		internal AppId ConsumerAppID;

		internal string Title;

		internal string Description;

		internal ulong File;

		internal ulong PreviewFile;

		internal ulong SteamIDOwner;

		internal uint TimeCreated;

		internal uint TimeUpdated;

		internal RemoteStoragePublishedFileVisibility Visibility;

		internal bool Banned;

		internal string Tags;

		internal bool TagsTruncated;

		internal string PchFileName;

		internal int FileSize;

		internal int PreviewFileSize;

		internal string URL;

		internal WorkshopFileType FileType;

		internal bool AcceptedForUse;

		internal readonly static int StructSize;

		private static Action<RemoteStorageGetPublishedFileDetailsResult_t> actionClient;

		private static Action<RemoteStorageGetPublishedFileDetailsResult_t> actionServer;

		static RemoteStorageGetPublishedFileDetailsResult_t()
		{
			RemoteStorageGetPublishedFileDetailsResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageGetPublishedFileDetailsResult_t) : typeof(RemoteStorageGetPublishedFileDetailsResult_t.Pack8)));
		}

		internal static RemoteStorageGetPublishedFileDetailsResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageGetPublishedFileDetailsResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageGetPublishedFileDetailsResult_t)) : (RemoteStorageGetPublishedFileDetailsResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageGetPublishedFileDetailsResult_t.Pack8)));
		}

		public static async Task<RemoteStorageGetPublishedFileDetailsResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageGetPublishedFileDetailsResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageGetPublishedFileDetailsResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageGetPublishedFileDetailsResult_t.StructSize, 1318, ref flag) | flag))
					{
						nullable = new RemoteStorageGetPublishedFileDetailsResult_t?(RemoteStorageGetPublishedFileDetailsResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageGetPublishedFileDetailsResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageGetPublishedFileDetailsResult_t.OnClient), RemoteStorageGetPublishedFileDetailsResult_t.StructSize, 1318, false);
				RemoteStorageGetPublishedFileDetailsResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageGetPublishedFileDetailsResult_t.OnServer), RemoteStorageGetPublishedFileDetailsResult_t.StructSize, 1318, true);
				RemoteStorageGetPublishedFileDetailsResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageGetPublishedFileDetailsResult_t> action = RemoteStorageGetPublishedFileDetailsResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageGetPublishedFileDetailsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageGetPublishedFileDetailsResult_t> action = RemoteStorageGetPublishedFileDetailsResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageGetPublishedFileDetailsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal AppId CreatorAppID;

			internal AppId ConsumerAppID;

			internal string Title;

			internal string Description;

			internal ulong File;

			internal ulong PreviewFile;

			internal ulong SteamIDOwner;

			internal uint TimeCreated;

			internal uint TimeUpdated;

			internal RemoteStoragePublishedFileVisibility Visibility;

			internal bool Banned;

			internal string Tags;

			internal bool TagsTruncated;

			internal string PchFileName;

			internal int FileSize;

			internal int PreviewFileSize;

			internal string URL;

			internal WorkshopFileType FileType;

			internal bool AcceptedForUse;

			public static implicit operator RemoteStorageGetPublishedFileDetailsResult_t(RemoteStorageGetPublishedFileDetailsResult_t.Pack8 d)
			{
				RemoteStorageGetPublishedFileDetailsResult_t remoteStorageGetPublishedFileDetailsResultT = new RemoteStorageGetPublishedFileDetailsResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					CreatorAppID = d.CreatorAppID,
					ConsumerAppID = d.ConsumerAppID,
					Title = d.Title,
					Description = d.Description,
					File = d.File,
					PreviewFile = d.PreviewFile,
					SteamIDOwner = d.SteamIDOwner,
					TimeCreated = d.TimeCreated,
					TimeUpdated = d.TimeUpdated,
					Visibility = d.Visibility,
					Banned = d.Banned,
					Tags = d.Tags,
					TagsTruncated = d.TagsTruncated,
					PchFileName = d.PchFileName,
					FileSize = d.FileSize,
					PreviewFileSize = d.PreviewFileSize,
					URL = d.URL,
					FileType = d.FileType,
					AcceptedForUse = d.AcceptedForUse
				};
				return remoteStorageGetPublishedFileDetailsResultT;
			}

			public static implicit operator Pack8(RemoteStorageGetPublishedFileDetailsResult_t d)
			{
				RemoteStorageGetPublishedFileDetailsResult_t.Pack8 pack8 = new RemoteStorageGetPublishedFileDetailsResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					CreatorAppID = d.CreatorAppID,
					ConsumerAppID = d.ConsumerAppID,
					Title = d.Title,
					Description = d.Description,
					File = d.File,
					PreviewFile = d.PreviewFile,
					SteamIDOwner = d.SteamIDOwner,
					TimeCreated = d.TimeCreated,
					TimeUpdated = d.TimeUpdated,
					Visibility = d.Visibility,
					Banned = d.Banned,
					Tags = d.Tags,
					TagsTruncated = d.TagsTruncated,
					PchFileName = d.PchFileName,
					FileSize = d.FileSize,
					PreviewFileSize = d.PreviewFileSize,
					URL = d.URL,
					FileType = d.FileType,
					AcceptedForUse = d.AcceptedForUse
				};
				return pack8;
			}
		}
	}
}