using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum MetadataPropertyHandling
	{
		Default,
		ReadAhead,
		Ignore
	}
}