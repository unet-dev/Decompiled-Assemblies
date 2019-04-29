using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public enum MergeArrayHandling
	{
		Concat,
		Union,
		Replace,
		Merge
	}
}