using Oxide.Core.Plugins;
using System;

namespace Oxide.Core.Libraries.Covalence
{
	public interface ICommandSystem
	{
		void RegisterCommand(string command, Plugin plugin, CommandCallback callback);

		void UnregisterCommand(string command, Plugin plugin);
	}
}