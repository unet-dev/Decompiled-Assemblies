using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum Required
	{
		Default,
		AllowNull,
		Always,
		DisallowNull
	}
}