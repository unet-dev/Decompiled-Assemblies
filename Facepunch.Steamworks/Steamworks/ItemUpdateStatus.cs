using System;

namespace Steamworks
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