using System;

namespace SteamNative
{
	internal enum ItemUpdateStatus
	{
		Invalid,
		PreparingConfig,
		PreparingContent,
		UploadingContent,
		UploadingPreviewFile,
		CommittingChanges
	}
}