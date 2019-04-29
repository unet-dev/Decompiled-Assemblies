using System;

namespace Apex.Serialization
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SerializationOverrideAttribute : Attribute
	{
		public SerializationOverrideAttribute()
		{
		}
	}
}