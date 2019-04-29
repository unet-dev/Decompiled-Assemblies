using System;

namespace Oxide.Plugins
{
	public class PluginLoadFailure : Exception
	{
		public PluginLoadFailure(string reason)
		{
		}
	}
}