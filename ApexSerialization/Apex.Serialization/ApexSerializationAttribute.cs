using System;
using System.Runtime.CompilerServices;

namespace Apex.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=true)]
	public sealed class ApexSerializationAttribute : Attribute
	{
		public object defaultValue
		{
			get;
			set;
		}

		public bool hideInEditor
		{
			get;
			set;
		}

		public ApexSerializationAttribute()
		{
		}
	}
}