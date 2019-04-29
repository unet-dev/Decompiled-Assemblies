using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum ReferenceLoopHandling
	{
		Error,
		Ignore,
		Serialize
	}
}