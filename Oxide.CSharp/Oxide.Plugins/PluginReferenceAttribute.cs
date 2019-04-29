using System;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins
{
	[AttributeUsage(AttributeTargets.Field)]
	public class PluginReferenceAttribute : Attribute
	{
		public string Name
		{
			get;
		}

		public PluginReferenceAttribute()
		{
		}

		public PluginReferenceAttribute(string name)
		{
			this.Name = name;
		}
	}
}