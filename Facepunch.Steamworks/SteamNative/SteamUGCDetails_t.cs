using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamUGCDetails_t
	{
		internal ulong PublishedFileId;

		internal SteamNative.Result Result;

		internal WorkshopFileType FileType;

		internal uint CreatorAppID;

		internal uint ConsumerAppID;

		internal string Title;

		internal string Description;

		internal ulong SteamIDOwner;

		internal uint TimeCreated;

		internal uint TimeUpdated;

		internal uint TimeAddedToUserList;

		internal RemoteStoragePublishedFileVisibility Visibility;

		internal bool Banned;

		internal bool AcceptedForUse;

		internal bool TagsTruncated;

		internal string Tags;

		internal ulong File;

		internal ulong PreviewFile;

		internal string PchFileName;

		internal int FileSize;

		internal int PreviewFileSize;

		internal string URL;

		internal uint VotesUp;

		internal uint VotesDown;

		internal float Score;

		internal uint NumChildren;

		internal static SteamUGCDetails_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamUGCDetails_t)Marshal.PtrToStructure(p, typeof(SteamUGCDetails_t));
			}
			return (SteamUGCDetails_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamUGCDetails_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamUGCDetails_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamUGCDetails_t));
		}

		internal struct PackSmall
		{
			internal ulong PublishedFileId;

			internal SteamNative.Result Result;

			internal WorkshopFileType FileType;

			internal uint CreatorAppID;

			internal uint ConsumerAppID;

			internal string Title;

			internal string Description;

			internal ulong SteamIDOwner;

			internal uint TimeCreated;

			internal uint TimeUpdated;

			internal uint TimeAddedToUserList;

			internal RemoteStoragePublishedFileVisibility Visibility;

			internal bool Banned;

			internal bool AcceptedForUse;

			internal bool TagsTruncated;

			internal string Tags;

			internal ulong File;

			internal ulong PreviewFile;

			internal string PchFileName;

			internal int FileSize;

			internal int PreviewFileSize;

			internal string URL;

			internal uint VotesUp;

			internal uint VotesDown;

			internal float Score;

			internal uint NumChildren;

			public static implicit operator SteamUGCDetails_t(SteamUGCDetails_t.PackSmall d)
			{
				SteamUGCDetails_t steamUGCDetailsT = new SteamUGCDetails_t()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					FileType = d.FileType,
					CreatorAppID = d.CreatorAppID,
					ConsumerAppID = d.ConsumerAppID,
					Title = d.Title,
					Description = d.Description,
					SteamIDOwner = d.SteamIDOwner,
					TimeCreated = d.TimeCreated,
					TimeUpdated = d.TimeUpdated,
					TimeAddedToUserList = d.TimeAddedToUserList,
					Visibility = d.Visibility,
					Banned = d.Banned,
					AcceptedForUse = d.AcceptedForUse,
					TagsTruncated = d.TagsTruncated,
					Tags = d.Tags,
					File = d.File,
					PreviewFile = d.PreviewFile,
					PchFileName = d.PchFileName,
					FileSize = d.FileSize,
					PreviewFileSize = d.PreviewFileSize,
					URL = d.URL,
					VotesUp = d.VotesUp,
					VotesDown = d.VotesDown,
					Score = d.Score,
					NumChildren = d.NumChildren
				};
				return steamUGCDetailsT;
			}
		}
	}
}