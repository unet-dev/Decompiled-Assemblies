using Oxide.Core;
using System;

namespace Oxide.Core.Plugins
{
	public class PluginManagerEvent : Event<Plugin, PluginManager>
	{
		public PluginManagerEvent()
		{
		}
	}
}