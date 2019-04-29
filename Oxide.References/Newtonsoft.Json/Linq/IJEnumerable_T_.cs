using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public interface IJEnumerable<T> : IEnumerable<T>, IEnumerable
	where T : JToken
	{
		IJEnumerable<JToken> this[object key]
		{
			get;
		}
	}
}