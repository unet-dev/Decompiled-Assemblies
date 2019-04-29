using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal enum ParseResult
	{
		None,
		Success,
		Overflow,
		Invalid
	}
}