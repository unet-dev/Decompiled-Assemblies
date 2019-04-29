using System;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	[AttributeUsage(AttributeTargets.Method)]
	public class LibraryFunction : Attribute
	{
		public string Name
		{
			get;
		}

		public LibraryFunction()
		{
		}

		public LibraryFunction(string name)
		{
			this.Name = name;
		}
	}
}