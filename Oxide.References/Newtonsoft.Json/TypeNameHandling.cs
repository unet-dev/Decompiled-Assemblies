using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Flags]
	[Preserve]
	public enum TypeNameHandling
	{
		None,
		Objects,
		Arrays,
		All,
		Auto
	}
}