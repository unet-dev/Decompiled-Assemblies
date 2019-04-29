using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Serialization
{
	[AttributeUsage(AttributeTargets.Method, Inherited=false)]
	[Preserve]
	public sealed class OnErrorAttribute : Attribute
	{
		public OnErrorAttribute()
		{
		}
	}
}