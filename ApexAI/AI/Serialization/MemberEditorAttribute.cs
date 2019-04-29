using System;

namespace Apex.AI.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=true)]
	public abstract class MemberEditorAttribute : Attribute
	{
		protected MemberEditorAttribute()
		{
		}
	}
}