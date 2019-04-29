using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Core.Plugins.Watchers
{
	public abstract class PluginChangeWatcher
	{
		protected PluginChangeWatcher()
		{
		}

		protected void FirePluginAdded(string name)
		{
			PluginAddEvent pluginAddEvent = this.OnPluginAdded;
			if (pluginAddEvent == null)
			{
				return;
			}
			pluginAddEvent(name);
		}

		protected void FirePluginRemoved(string name)
		{
			PluginRemoveEvent pluginRemoveEvent = this.OnPluginRemoved;
			if (pluginRemoveEvent == null)
			{
				return;
			}
			pluginRemoveEvent(name);
		}

		protected void FirePluginSourceChanged(string name)
		{
			PluginChangeEvent pluginChangeEvent = this.OnPluginSourceChanged;
			if (pluginChangeEvent == null)
			{
				return;
			}
			pluginChangeEvent(name);
		}

		public event PluginAddEvent OnPluginAdded;

		public event PluginRemoveEvent OnPluginRemoved;

		public event PluginChangeEvent OnPluginSourceChanged;
	}
}