using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Flags]
	[Preserve]
	public enum DefaultValueHandling
	{
		Include,
		Ignore,
		Populate,
		IgnoreAndPopulate
	}
}