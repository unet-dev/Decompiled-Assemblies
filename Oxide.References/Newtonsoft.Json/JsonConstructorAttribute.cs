using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple=false)]
	[Preserve]
	public sealed class JsonConstructorAttribute : Attribute
	{
		public JsonConstructorAttribute()
		{
		}
	}
}