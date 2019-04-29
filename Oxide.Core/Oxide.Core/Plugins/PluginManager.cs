using Oxide.Core;
using Oxide.Core.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Core.Plugins
{
	public sealed class PluginManager
	{
		private readonly IDictionary<string, Plugin> loadedPlugins;

		private readonly IDictionary<string, IList<Plugin>> hookSubscriptions;

		private readonly Dictionary<string, float> lastDeprecatedWarningAt = new Dictionary<string, float>();

		private readonly List<string> hookConflicts = new List<string>();

		public string ConfigPath
		{
			get;
			set;
		}

		public Oxide.Core.Logging.Logger Logger
		{
			get;
			private set;
		}

		public PluginManager(Oxide.Core.Logging.Logger logger)
		{
			this.loadedPlugins = new Dictionary<string, Plugin>();
			this.hookSubscriptions = new Dictionary<string, IList<Plugin>>();
			this.Logger = logger;
		}

		public bool AddPlugin(Plugin plugin)
		{
			if (this.loadedPlugins.ContainsKey(plugin.Name))
			{
				return false;
			}
			this.loadedPlugins.Add(plugin.Name, plugin);
			plugin.HandleAddedToManager(this);
			PluginEvent pluginEvent = this.OnPluginAdded;
			if (pluginEvent != null)
			{
				pluginEvent(plugin);
			}
			else
			{
			}
			return true;
		}

		public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
		{
			IList<Plugin> plugins;
			float single;
			if (!this.hookSubscriptions.TryGetValue(oldHook, out plugins))
			{
				return null;
			}
			if (plugins.Count == 0)
			{
				return null;
			}
			if (expireDate < DateTime.Now)
			{
				return null;
			}
			float now = Interface.Oxide.Now;
			if (!this.lastDeprecatedWarningAt.TryGetValue(oldHook, out single) || now - single > 300f)
			{
				this.lastDeprecatedWarningAt[oldHook] = now;
				Interface.Oxide.LogWarning(string.Format("'{0} v{1}' is using deprecated hook '{2}', which will stop working on {3}. Please ask the author to update to '{4}'", new object[] { plugins[0].Name, plugins[0].Version, oldHook, expireDate.ToString("D"), newHook }), Array.Empty<object>());
			}
			return this.CallHook(oldHook, args);
		}

		public object CallHook(string hook, params object[] args)
		{
			IList<Plugin> plugins;
			if (!this.hookSubscriptions.TryGetValue(hook, out plugins))
			{
				return null;
			}
			if (plugins.Count == 0)
			{
				return null;
			}
			object[] objArray = ArrayPool.Get(plugins.Count);
			int num = 0;
			object obj = null;
			Plugin item = null;
			for (int i = 0; i < plugins.Count; i++)
			{
				object obj1 = plugins[i].CallHook(hook, args);
				if (obj1 != null)
				{
					objArray[i] = obj1;
					obj = obj1;
					item = plugins[i];
					num++;
				}
			}
			if (num == 0)
			{
				ArrayPool.Free(objArray);
				return null;
			}
			if (num > 1 && obj != null)
			{
				this.hookConflicts.Clear();
				for (int j = 0; j < plugins.Count; j++)
				{
					object obj2 = objArray[j];
					if (obj2 != null)
					{
						if (obj2.GetType().IsValueType)
						{
							if (!objArray[j].Equals(obj))
							{
								this.hookConflicts.Add(string.Format("{0} - {1} ({2})", plugins[j].Name, obj2, obj2.GetType().Name));
							}
						}
						else if (objArray[j] != obj)
						{
							this.hookConflicts.Add(string.Format("{0} - {1} ({2})", plugins[j].Name, obj2, obj2.GetType().Name));
						}
					}
				}
				if (this.hookConflicts.Count > 0)
				{
					this.hookConflicts.Add(string.Format("{0} ({1} ({2}))", item.Name, obj, obj.GetType().Name));
					this.Logger.Write(LogType.Warning, "Calling hook {0} resulted in a conflict between the following plugins: {1}", new object[] { hook, string.Join(", ", this.hookConflicts.ToArray()) });
				}
			}
			ArrayPool.Free(objArray);
			return obj;
		}

		public Plugin GetPlugin(string name)
		{
			Plugin plugin;
			if (!this.loadedPlugins.TryGetValue(name, out plugin))
			{
				return null;
			}
			return plugin;
		}

		public IEnumerable<Plugin> GetPlugins()
		{
			return this.loadedPlugins.Values;
		}

		public bool RemovePlugin(Plugin plugin)
		{
			if (!this.loadedPlugins.ContainsKey(plugin.Name))
			{
				return false;
			}
			this.loadedPlugins.Remove(plugin.Name);
			foreach (IList<Plugin> value in this.hookSubscriptions.Values)
			{
				if (!value.Contains(plugin))
				{
					continue;
				}
				value.Remove(plugin);
			}
			plugin.HandleRemovedFromManager(this);
			PluginEvent pluginEvent = this.OnPluginRemoved;
			if (pluginEvent != null)
			{
				pluginEvent(plugin);
			}
			else
			{
			}
			return true;
		}

		internal void SubscribeToHook(string hook, Plugin plugin)
		{
			IList<Plugin> plugins;
			if (!this.loadedPlugins.ContainsKey(plugin.Name) || !plugin.IsCorePlugin && hook.StartsWith("I"))
			{
				return;
			}
			if (!this.hookSubscriptions.TryGetValue(hook, out plugins))
			{
				plugins = new List<Plugin>();
				this.hookSubscriptions.Add(hook, plugins);
			}
			if (!plugins.Contains(plugin))
			{
				plugins.Add(plugin);
			}
		}

		internal void UnsubscribeToHook(string hook, Plugin plugin)
		{
			IList<Plugin> plugins;
			if (!this.loadedPlugins.ContainsKey(plugin.Name) || !plugin.IsCorePlugin && hook.StartsWith("I"))
			{
				return;
			}
			if (this.hookSubscriptions.TryGetValue(hook, out plugins) && plugins.Contains(plugin))
			{
				plugins.Remove(plugin);
			}
		}

		public event PluginEvent OnPluginAdded;

		public event PluginEvent OnPluginRemoved;
	}
}