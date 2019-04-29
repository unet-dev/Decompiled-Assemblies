using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public sealed class AICategoryAttribute : Attribute
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

		public AICategoryAttribute(string name)
		{
			this.name = name;
		}

		public AICategoryAttribute(string name, int sortOrder)
		{
			this.name = name;
			this.sortOrder = sortOrder;
		}
	}
}