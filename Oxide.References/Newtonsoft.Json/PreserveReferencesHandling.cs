using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Flags]
	[Preserve]
	public enum PreserveReferencesHandling
	{
		None,
		Objects,
		Arrays,
		All
	}
}