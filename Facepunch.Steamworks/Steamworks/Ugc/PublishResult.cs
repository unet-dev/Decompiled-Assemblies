using Steamworks;
using Steamworks.Data;
using System;

namespace Steamworks.Ugc
{
	public struct PublishResult
	{
		public Steamworks.Result Result;

		public PublishedFileId FileId;

		public bool NeedsWorkshopAgreement;

		public bool Success
		{
			get
			{
				return this.Result == Steamworks.Result.OK;
			}
		}
	}
}