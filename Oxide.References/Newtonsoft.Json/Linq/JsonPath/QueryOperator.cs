using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal enum QueryOperator
	{
		None,
		Equals,
		NotEquals,
		Exists,
		LessThan,
		LessThanOrEquals,
		GreaterThan,
		GreaterThanOrEquals,
		And,
		Or
	}
}