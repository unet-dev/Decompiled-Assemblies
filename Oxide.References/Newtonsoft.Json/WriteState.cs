using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum WriteState
	{
		Error,
		Closed,
		Object,
		Array,
		Constructor,
		Property,
		Start
	}
}