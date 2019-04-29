using System;

namespace Mono.Cecil
{
	[Flags]
	public enum ManifestResourceAttributes : uint
	{
		Public = 1,
		Private = 2,
		VisibilityMask = 7
	}
}