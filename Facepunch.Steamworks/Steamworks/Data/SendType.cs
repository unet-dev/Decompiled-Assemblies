using System;

namespace Steamworks.Data
{
	[Flags]
	public enum SendType
	{
		Unreliable = 0,
		NoNagle = 1,
		NoDelay = 4,
		Reliable = 8
	}
}