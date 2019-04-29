using System;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute
	{
		public string[] Commands
		{
			get;
		}

		public CommandAttribute(params string[] commands)
		{
			this.Commands = commands;
		}
	}
}