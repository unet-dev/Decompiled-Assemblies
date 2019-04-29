using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Linq
{
	[Flags]
	[Preserve]
	public enum MergeNullValueHandling
	{
		Ignore,
		Merge
	}
}