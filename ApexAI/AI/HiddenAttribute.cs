using System;

namespace Apex.AI
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
	public sealed class HiddenAttribute : Attribute
	{
		public HiddenAttribute()
		{
		}
	}
}