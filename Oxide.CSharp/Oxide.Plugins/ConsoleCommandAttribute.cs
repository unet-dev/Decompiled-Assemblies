using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleCommandAttribute : Attribute
	{
		public string Command
		{
			get;
			private set;
		}

		public ConsoleCommandAttribute(string command)
		{
			this.Command = (command.Contains<char>('.') ? command : string.Concat("global.", command));
		}
	}
}