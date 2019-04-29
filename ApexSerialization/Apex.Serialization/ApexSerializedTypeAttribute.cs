using System;

namespace Apex.Serialization
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public sealed class ApexSerializedTypeAttribute : Attribute
	{
		public ApexSerializedTypeAttribute()
		{
		}
	}
}