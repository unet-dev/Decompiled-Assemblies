using System;

namespace Mono.Unix
{
	[Flags]
	public enum FileSpecialAttributes
	{
		Sticky = 512,
		SetGroupId = 1024,
		SetUserId = 2048
	}
}