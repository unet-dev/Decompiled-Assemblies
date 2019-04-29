using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=true)]
	public sealed class MemberCategoryAttribute : Attribute
	{
		public string name
		{
			get;
			private set;
		}

		public int sortOrder
		{
			get;
			private set;
		}

		public MemberCategoryAttribute(string name)
		{
			this.name = name;
		}

		public MemberCategoryAttribute(string name, int sortOrder)
		{
			this.name = name;
			this.sortOrder = sortOrder;
		}
	}
}