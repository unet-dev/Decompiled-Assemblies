using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum DateTimeZoneHandling
	{
		Local,
		Utc,
		Unspecified,
		RoundtripKind
	}
}