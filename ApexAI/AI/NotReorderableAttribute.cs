using System;

namespace Apex.AI
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=true)]
	public sealed class NotReorderableAttribute : Attribute
	{
		public NotReorderableAttribute()
		{
		}
	}
}