using System;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Plugins
{
	[AttributeUsage(AttributeTargets.Method)]
	public class HookMethodAttribute : Attribute
	{
		public string Name
		{
			get;
		}

		public HookMethodAttribute(string name)
		{
			this.Name = name;
		}
	}
}