using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum DateParseHandling
	{
		None,
		DateTime,
		DateTimeOffset
	}
}