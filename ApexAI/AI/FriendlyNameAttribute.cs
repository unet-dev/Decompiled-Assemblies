using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
	public sealed class FriendlyNameAttribute : Attribute
	{
		public string description
		{
			get;
			set;
		}

		public string name
		{
			get;
			private set;
		}

		public int sortOrder
		{
			get;
			set;
		}

		public FriendlyNameAttribute(string name)
		{
			this.name = name;
		}

		public FriendlyNameAttribute(string name, string description)
		{
			this.name = name;
			this.description = description;
		}
	}
}