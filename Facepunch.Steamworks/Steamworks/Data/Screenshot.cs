using Steamworks;
using System;

namespace Steamworks.Data
{
	public struct Screenshot
	{
		internal ScreenshotHandle Value;

		public bool SetLocation(string location)
		{
			return SteamScreenshots.Internal.SetLocation(this.Value, location);
		}

		public bool TagPublishedFile(PublishedFileId file)
		{
			return SteamScreenshots.Internal.TagPublishedFile(this.Value, file);
		}

		public bool TagUser(SteamId user)
		{
			return SteamScreenshots.Internal.TagUser(this.Value, user);
		}
	}
}