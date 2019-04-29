using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System;

namespace Oxide.Plugins
{
	public class PluginTimers
	{
		private Oxide.Core.Libraries.Timer timer = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Timer>("Timer");

		private Plugin plugin;

		public PluginTimers(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void Destroy(ref Oxide.Plugins.Timer timer)
		{
			Oxide.Plugins.Timer timer1 = timer;
			if (timer1 != null)
			{
				timer1.DestroyToPool();
			}
			else
			{
			}
			timer = null;
		}

		public Oxide.Plugins.Timer Every(float interval, Action callback)
		{
			return new Oxide.Plugins.Timer(this.timer.Repeat(interval, -1, callback, this.plugin));
		}

		public Oxide.Plugins.Timer In(float seconds, Action callback)
		{
			return new Oxide.Plugins.Timer(this.timer.Once(seconds, callback, this.plugin));
		}

		public Oxide.Plugins.Timer Once(float seconds, Action callback)
		{
			return new Oxide.Plugins.Timer(this.timer.Once(seconds, callback, this.plugin));
		}

		public Oxide.Plugins.Timer Repeat(float interval, int repeats, Action callback)
		{
			return new Oxide.Plugins.Timer(this.timer.Repeat(interval, repeats, callback, this.plugin));
		}
	}
}