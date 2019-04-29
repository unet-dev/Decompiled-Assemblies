using System;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	[AttributeUsage(AttributeTargets.Property)]
	public class LibraryProperty : Attribute
	{
		public string Name
		{
			get;
			private set;
		}

		public LibraryProperty()
		{
		}

		public LibraryProperty(string name)
		{
			this.Name = name;
		}
	}
}