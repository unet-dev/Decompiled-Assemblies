using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Logging;
using Oxide.Core.Plugins;
using Oxide.Core.Plugins.Watchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Extensions
{
	public sealed class ExtensionManager
	{
		private IList<Extension> extensions;

		private const string extSearchPattern = "Oxide.*.dll";

		private IList<PluginLoader> pluginloaders;

		private IDictionary<string, Library> libraries;

		private IList<PluginChangeWatcher> changewatchers;

		public CompoundLogger Logger
		{
			get;
			private set;
		}

		public ExtensionManager(CompoundLogger logger)
		{
			this.Logger = logger;
			this.extensions = new List<Extension>();
			this.pluginloaders = new List<PluginLoader>();
			this.libraries = new Dictionary<string, Library>();
			this.changewatchers = new List<PluginChangeWatcher>();
		}

		public IEnumerable<Extension> GetAllExtensions()
		{
			return this.extensions;
		}

		public Extension GetExtension(string name)
		{
			Extension extension;
			try
			{
				extension = this.extensions.Single<Extension>((Extension e) => e.Name == name);
			}
			catch (Exception exception)
			{
				extension = null;
			}
			return extension;
		}

		public IEnumerable<string> GetLibraries()
		{
			return this.libraries.Keys;
		}

		public Library GetLibrary(string name)
		{
			Library library;
			if (this.libraries.TryGetValue(name, out library))
			{
				return library;
			}
			return null;
		}

		public IEnumerable<PluginChangeWatcher> GetPluginChangeWatchers()
		{
			return this.changewatchers;
		}

		public IEnumerable<PluginLoader> GetPluginLoaders()
		{
			return this.pluginloaders;
		}

		public bool IsExtensionPresent(string name)
		{
			return this.extensions.Any<Extension>((Extension e) => e.Name == name);
		}

		public void LoadAllExtensions(string directory)
		{
			List<string> strs = new List<string>();
			List<string> strs1 = new List<string>();
			List<string> strs2 = new List<string>();
			string[] strArrays = new string[] { "Oxide.CSharp", "Oxide.JavaScript", "Oxide.Lua", "Oxide.MySql", "Oxide.Python", "Oxide.SQLite", "Oxide.Unity" };
			string[] strArrays1 = new string[] { "Oxide.Blackwake", "Oxide.Blockstorm", "Oxide.FortressCraft", "Oxide.FromTheDepths", "Oxide.GangBeasts", "Oxide.Hurtworld", "Oxide.InterstellarRift", "Oxide.MedievalEngineers", "Oxide.Nomad", "Oxide.PlanetExplorers", "Oxide.ReignOfKings", "Oxide.Rust", "Oxide.RustLegacy", "Oxide.SavageLands", "Oxide.SevenDaysToDie", "Oxide.SpaceEngineers", "Oxide.TheForest", "Oxide.Terraria", "Oxide.Unturned" };
			string[] files = Directory.GetFiles(directory, "Oxide.*.dll");
			foreach (string str in files.Where<string>((string e) => {
				if (e.EndsWith("Oxide.Core.dll"))
				{
					return false;
				}
				return !e.EndsWith("Oxide.References.dll");
			}))
			{
				if (str.Contains("Oxide.Core.") && Array.IndexOf<string>(files, str.Replace(".Core", "")) != -1)
				{
					Cleanup.Add(str);
				}
				else if (str.Contains("Oxide.Ext.") && Array.IndexOf<string>(files, str.Replace(".Ext", "")) != -1)
				{
					Cleanup.Add(str);
				}
				else if (str.Contains("Oxide.Game."))
				{
					Cleanup.Add(str);
				}
				else if (strArrays.Contains<string>(str.Basename(null)))
				{
					strs.Add(str);
				}
				else if (!strArrays1.Contains<string>(str.Basename(null)))
				{
					strs2.Add(str);
				}
				else
				{
					strs1.Add(str);
				}
			}
			foreach (string str1 in strs)
			{
				this.LoadExtension(Path.Combine(directory, str1), true);
			}
			foreach (string str2 in strs1)
			{
				this.LoadExtension(Path.Combine(directory, str2), true);
			}
			foreach (string str3 in strs2)
			{
				this.LoadExtension(Path.Combine(directory, str3), true);
			}
			Extension[] array = this.extensions.ToArray<Extension>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				Extension extension = array[i];
				try
				{
					extension.OnModLoad();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.extensions.Remove(extension);
					this.Logger.WriteException(string.Format("Failed OnModLoad extension {0} v{1}", extension.Name, extension.Version), exception);
					RemoteLogger.Exception(string.Format("Failed OnModLoad extension {0} v{1}", extension.Name, extension.Version), exception);
				}
			}
		}

		public void LoadExtension(string filename, bool forced)
		{
			string fileNameWithoutExtension = Utility.GetFileNameWithoutExtension(filename);
			if (this.extensions.Any<Extension>((Extension x) => x.Filename == filename))
			{
				this.Logger.Write(LogType.Error, string.Concat("Failed to load extension '", fileNameWithoutExtension, "': extension already loaded."), Array.Empty<object>());
				return;
			}
			try
			{
				Assembly assembly = Assembly.Load(File.ReadAllBytes(filename));
				Type type = typeof(Extension);
				Type type1 = null;
				Type[] exportedTypes = assembly.GetExportedTypes();
				int num = 0;
				while (num < (int)exportedTypes.Length)
				{
					Type type2 = exportedTypes[num];
					if (!type.IsAssignableFrom(type2))
					{
						num++;
					}
					else
					{
						type1 = type2;
						break;
					}
				}
				if (type1 != null)
				{
					Extension extension = Activator.CreateInstance(type1, new object[] { this }) as Extension;
					if (extension != null)
					{
						if (!forced)
						{
							if (extension.IsCoreExtension || extension.IsGameExtension)
							{
								this.Logger.Write(LogType.Error, string.Concat("Failed to load extension '", fileNameWithoutExtension, "': you may not hotload Core or Game extensions."), Array.Empty<object>());
								return;
							}
							else if (!extension.SupportsReloading)
							{
								this.Logger.Write(LogType.Error, string.Concat("Failed to load extension '", fileNameWithoutExtension, "': this extension does not support reloading."), Array.Empty<object>());
								return;
							}
						}
						extension.Filename = filename;
						extension.Load();
						this.extensions.Add(extension);
						this.Logger.Write(LogType.Info, "Loaded extension {0} v{1} by {2}", new object[] { extension.Name, extension.Version, extension.Author });
					}
				}
				else
				{
					this.Logger.Write(LogType.Error, "Failed to load extension {0} ({1})", new object[] { fileNameWithoutExtension, "Specified assembly does not implement an Extension class" });
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Logger.WriteException(string.Concat("Failed to load extension ", fileNameWithoutExtension), exception);
				RemoteLogger.Exception(string.Concat("Failed to load extension ", fileNameWithoutExtension), exception);
			}
		}

		public void RegisterLibrary(string name, Library library)
		{
			if (!this.libraries.ContainsKey(name))
			{
				this.libraries[name] = library;
				return;
			}
			Interface.Oxide.LogError(string.Concat("An extension tried to register an already registered library: ", name), Array.Empty<object>());
		}

		public void RegisterPluginChangeWatcher(PluginChangeWatcher watcher)
		{
			this.changewatchers.Add(watcher);
		}

		public void RegisterPluginLoader(PluginLoader loader)
		{
			this.pluginloaders.Add(loader);
		}

		public void ReloadExtension(string filename)
		{
			string fileNameWithoutExtension = Utility.GetFileNameWithoutExtension(filename);
			Extension extension = this.extensions.SingleOrDefault<Extension>((Extension x) => Utility.GetFileNameWithoutExtension(x.Filename) == fileNameWithoutExtension);
			if (extension == null)
			{
				this.LoadExtension(filename, false);
				return;
			}
			if (extension.IsCoreExtension || extension.IsGameExtension)
			{
				this.Logger.Write(LogType.Error, string.Concat("Failed to unload extension '", fileNameWithoutExtension, "': you may not unload Core or Game extensions."), Array.Empty<object>());
				return;
			}
			if (extension.SupportsReloading)
			{
				this.UnloadExtension(filename);
				this.LoadExtension(filename, false);
				return;
			}
			this.Logger.Write(LogType.Error, string.Concat("Failed to reload extension '", fileNameWithoutExtension, "': this extension doesn't support reloading."), Array.Empty<object>());
		}

		public void UnloadExtension(string filename)
		{
			string fileNameWithoutExtension = Utility.GetFileNameWithoutExtension(filename);
			Extension extension = this.extensions.SingleOrDefault<Extension>((Extension x) => x.Filename == filename);
			if (extension == null)
			{
				this.Logger.Write(LogType.Error, string.Concat("Failed to unload extension '", fileNameWithoutExtension, "': extension not loaded."), Array.Empty<object>());
				return;
			}
			if (extension.IsCoreExtension || extension.IsGameExtension)
			{
				this.Logger.Write(LogType.Error, string.Concat("Failed to unload extension '", fileNameWithoutExtension, "': you may not unload Core or Game extensions."), Array.Empty<object>());
				return;
			}
			if (!extension.SupportsReloading)
			{
				this.Logger.Write(LogType.Error, string.Concat("Failed to unload extension '", fileNameWithoutExtension, "': this extension doesn't support reloading."), Array.Empty<object>());
				return;
			}
			extension.Unload();
			this.extensions.Remove(extension);
			this.Logger.Write(LogType.Info, string.Format("Unloaded extension {0} v{1} by {2}", extension.Name, extension.Version, extension.Author), Array.Empty<object>());
		}
	}
}