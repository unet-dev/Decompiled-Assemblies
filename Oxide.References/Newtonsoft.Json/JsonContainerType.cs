using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	internal enum JsonContainerType
	{
		None,
		Object,
		Array,
		Constructor
	}
}