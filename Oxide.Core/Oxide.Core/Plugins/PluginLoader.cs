using Oxide.Core;
using Oxide.Core.Libraries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Plugins
{
	public abstract class PluginLoader
	{
		public Dictionary<string, Plugin> LoadedPlugins = new Dictionary<string, Plugin>();

		public virtual Type[] CorePlugins { get; } = new Type[0];

		public virtual string FileExtension
		{
			get;
		}

		public ConcurrentHashSet<string> LoadingPlugins { get; } = new ConcurrentHashSet<string>();

		public Dictionary<string, string> PluginErrors { get; } = new Dictionary<string, string>();

		protected PluginLoader()
		{
		}

		protected virtual Plugin GetPlugin(string filename)
		{
			return null;
		}

		public virtual Plugin Load(string directory, string name)
		{
			if (this.LoadingPlugins.Contains(name))
			{
				Interface.Oxide.LogDebug("Load requested for plugin which is already loading: {0}", new object[] { name });
				return null;
			}
			string str = Path.Combine(directory, string.Concat(name, this.FileExtension));
			Plugin plugin = this.GetPlugin(str);
			this.LoadingPlugins.Add(plugin.Name);
			Interface.Oxide.NextTick(() => this.LoadPlugin(plugin, false));
			return null;
		}

		protected void LoadPlugin(Plugin plugin, bool waitingForAccess = false)
		{
			if (!File.Exists(plugin.Filename))
			{
				this.LoadingPlugins.Remove(plugin.Name);
				Interface.Oxide.LogWarning("Script no longer exists: {0}", new object[] { plugin.Name });
				return;
			}
			try
			{
				plugin.Load();
				Interface.Oxide.UnloadPlugin(plugin.Name);
				this.LoadingPlugins.Remove(plugin.Name);
				Interface.Oxide.PluginLoaded(plugin);
			}
			catch (IOException oException)
			{
				if (!waitingForAccess)
				{
					Interface.Oxide.LogWarning("Waiting for another application to stop using script: {0}", new object[] { plugin.Name });
				}
				Interface.Oxide.GetLibrary<Timer>(null).Once(0.5f, () => this.LoadPlugin(plugin, true), null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.LoadingPlugins.Remove(plugin.Name);
				Interface.Oxide.LogException(string.Concat("Failed to load plugin ", plugin.Name), exception);
			}
		}

		public virtual void Reload(string directory, string name)
		{
			Interface.Oxide.UnloadPlugin(name);
			Interface.Oxide.LoadPlugin(name);
		}

		public virtual IEnumerable<string> ScanDirectory(string directory)
		{
			PluginLoader pluginLoader = null;
			if (pluginLoader.FileExtension == null || !Directory.Exists(directory))
			{
			}
			else
			{
				FileInfo[] files = (new DirectoryInfo(directory)).GetFiles(string.Concat("*", pluginLoader.FileExtension));
				IEnumerable<FileInfo> attributes = 
					from f in (IEnumerable<FileInfo>)files
					where (f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
					select f;
				foreach (FileInfo attribute in attributes)
				{
					yield return Utility.GetFileNameWithoutExtension(attribute.FullName);
				}
			}
		}

		public virtual void Unloading(Plugin plugin)
		{
		}
	}
}