using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct SteamUGCDetails_t
	{
		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal Steamworks.Result Result;

		internal WorkshopFileType FileType;

		internal AppId CreatorAppID;

		internal AppId ConsumerAppID;

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

		internal static SteamUGCDetails_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamUGCDetails_t)Marshal.PtrToStructure(p, typeof(SteamUGCDetails_t)) : (SteamUGCDetails_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamUGCDetails_t.Pack8)));
		}

		public struct Pack8
		{
			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal Steamworks.Result Result;

			internal WorkshopFileType FileType;

			internal AppId CreatorAppID;

			internal AppId ConsumerAppID;

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

			public static implicit operator SteamUGCDetails_t(SteamUGCDetails_t.Pack8 d)
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

			public static implicit operator Pack8(SteamUGCDetails_t d)
			{
				SteamUGCDetails_t.Pack8 pack8 = new SteamUGCDetails_t.Pack8()
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
				return pack8;
			}
		}
	}
}