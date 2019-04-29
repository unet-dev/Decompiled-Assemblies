using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum StringEscapeHandling
	{
		Default,
		EscapeNonAscii,
		EscapeHtml
	}
}