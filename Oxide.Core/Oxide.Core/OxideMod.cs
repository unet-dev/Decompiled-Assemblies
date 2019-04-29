using Newtonsoft.Json;
using Oxide.Core.Configuration;
using Oxide.Core.Extensions;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Logging;
using Oxide.Core.Plugins;
using Oxide.Core.Plugins.Watchers;
using Oxide.Core.RemoteConsole;
using Oxide.Core.ServerConsole;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using WebSocketSharp;

namespace Oxide.Core
{
	public sealed class OxideMod
	{
		internal readonly static System.Version AssemblyVersion;

		public readonly static VersionNumber Version;

		private ExtensionManager extensionManager;

		public Oxide.Core.CommandLine CommandLine;

		private Covalence covalence;

		private Permission libperm;

		private Oxide.Core.Libraries.Timer libtimer;

		private Func<float> getTimeSinceStartup;

		private List<Action> nextTickQueue = new List<Action>();

		private List<Action> lastTickQueue = new List<Action>();

		private readonly object nextTickLock = new object();

		private Action<float> onFrame;

		private bool isInitialized;

		public Oxide.Core.RemoteConsole.RemoteConsole RemoteConsole;

		public Oxide.Core.ServerConsole.ServerConsole ServerConsole;

		private Stopwatch timer;

		private NativeDebugCallback debugCallback;

		public OxideConfig Config
		{
			get;
			private set;
		}

		public string ConfigDirectory
		{
			get;
			private set;
		}

		public string DataDirectory
		{
			get;
			private set;
		}

		public Oxide.Core.DataFileSystem DataFileSystem
		{
			get;
			private set;
		}

		public string ExtensionDirectory
		{
			get;
			private set;
		}

		public bool HasLoadedCorePlugins
		{
			get;
			private set;
		}

		public string InstanceDirectory
		{
			get;
			private set;
		}

		public bool IsShuttingDown
		{
			get;
			private set;
		}

		public string LangDirectory
		{
			get;
			private set;
		}

		public string LogDirectory
		{
			get;
			private set;
		}

		public float Now
		{
			get
			{
				return this.getTimeSinceStartup();
			}
		}

		public string PluginDirectory
		{
			get;
			private set;
		}

		public string RootDirectory
		{
			get;
			private set;
		}

		public CompoundLogger RootLogger
		{
			get;
			private set;
		}

		public PluginManager RootPluginManager
		{
			get;
			private set;
		}

		static OxideMod()
		{
			OxideMod.AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
			OxideMod.Version = new VersionNumber(OxideMod.AssemblyVersion.Major, OxideMod.AssemblyVersion.Minor, OxideMod.AssemblyVersion.Build);
		}

		public OxideMod(NativeDebugCallback debugCallback)
		{
			this.debugCallback = debugCallback;
		}

		public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
		{
			PluginManager rootPluginManager = this.RootPluginManager;
			if (rootPluginManager == null)
			{
				return null;
			}
			return rootPluginManager.CallDeprecatedHook(oldHook, newHook, expireDate, args);
		}

		public object CallHook(string hookname, params object[] args)
		{
			PluginManager rootPluginManager = this.RootPluginManager;
			if (rootPluginManager == null)
			{
				return null;
			}
			return rootPluginManager.CallHook(hookname, args);
		}

		public bool CheckConsole(bool force = false)
		{
			if (!ConsoleWindow.Check(force))
			{
				return false;
			}
			return this.Config.Console.Enabled;
		}

		public bool EnableConsole(bool force = false)
		{
			if (!this.CheckConsole(force))
			{
				return false;
			}
			this.ServerConsole = new Oxide.Core.ServerConsole.ServerConsole();
			this.ServerConsole.OnEnable();
			return true;
		}

		public IEnumerable<Extension> GetAllExtensions()
		{
			return this.extensionManager.GetAllExtensions();
		}

		public T GetLibrary<T>(string name = null)
		where T : Library
		{
			return (T)(this.extensionManager.GetLibrary(name ?? typeof(T).Name) as T);
		}

		public IEnumerable<PluginLoader> GetPluginLoaders()
		{
			return this.extensionManager.GetPluginLoaders();
		}

		public void Load()
		{
			string str;
			string str1;
			this.RootDirectory = Environment.CurrentDirectory;
			if (this.RootDirectory.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
			{
				this.RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
			}
			if (this.RootDirectory == null)
			{
				throw new Exception("RootDirectory is null");
			}
			this.InstanceDirectory = Path.Combine(this.RootDirectory, "oxide");
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
			{
				Culture = CultureInfo.InvariantCulture
			};
			this.CommandLine = new Oxide.Core.CommandLine(Environment.GetCommandLineArgs());
			if (this.CommandLine.HasVariable("oxide.directory"))
			{
				this.CommandLine.GetArgument("oxide.directory", out str, out str1);
				if (string.IsNullOrEmpty(str) || this.CommandLine.HasVariable(str))
				{
					this.InstanceDirectory = Path.Combine(this.RootDirectory, Utility.CleanPath(string.Format(str1, this.CommandLine.GetVariable(str))));
				}
			}
			this.ExtensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (this.ExtensionDirectory == null || !Directory.Exists(this.ExtensionDirectory))
			{
				throw new Exception("Could not identify extension directory");
			}
			if (!Directory.Exists(this.InstanceDirectory))
			{
				Directory.CreateDirectory(this.InstanceDirectory);
			}
			this.ConfigDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("config"));
			if (!Directory.Exists(this.ConfigDirectory))
			{
				Directory.CreateDirectory(this.ConfigDirectory);
			}
			this.DataDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("data"));
			if (!Directory.Exists(this.DataDirectory))
			{
				Directory.CreateDirectory(this.DataDirectory);
			}
			this.LangDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("lang"));
			if (!Directory.Exists(this.LangDirectory))
			{
				Directory.CreateDirectory(this.LangDirectory);
			}
			this.LogDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("logs"));
			if (!Directory.Exists(this.LogDirectory))
			{
				Directory.CreateDirectory(this.LogDirectory);
			}
			this.PluginDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("plugins"));
			if (!Directory.Exists(this.PluginDirectory))
			{
				Directory.CreateDirectory(this.PluginDirectory);
			}
			OxideMod.RegisterLibrarySearchPath(Path.Combine(this.ExtensionDirectory, (IntPtr.Size == 8 ? "x64" : "x86")));
			string str2 = Path.Combine(this.InstanceDirectory, "oxide.config.json");
			if (!File.Exists(str2))
			{
				this.Config = new OxideConfig(str2);
				this.Config.Save(null);
			}
			else
			{
				this.Config = ConfigFile.Load<OxideConfig>(str2);
			}
			if (this.CommandLine.HasVariable("nolog"))
			{
				this.LogWarning("Usage of the 'nolog' variable will prevent logging", Array.Empty<object>());
			}
			if (this.CommandLine.HasVariable("rcon.port"))
			{
				this.Config.Rcon.Port = Utility.GetNumbers(this.CommandLine.GetVariable("rcon.port"));
			}
			if (this.CommandLine.HasVariable("rcon.password"))
			{
				this.Config.Rcon.Password = this.CommandLine.GetVariable("rcon.password");
			}
			this.RootLogger = new CompoundLogger();
			this.RootLogger.AddLogger(new RotatingFileLogger()
			{
				Directory = this.LogDirectory
			});
			if (this.debugCallback != null)
			{
				this.RootLogger.AddLogger(new CallbackLogger(this.debugCallback));
			}
			this.LogInfo("Loading Oxide Core v{0}...", new object[] { OxideMod.Version });
			this.RootPluginManager = new PluginManager(this.RootLogger)
			{
				ConfigPath = this.ConfigDirectory
			};
			this.extensionManager = new ExtensionManager(this.RootLogger);
			this.DataFileSystem = new Oxide.Core.DataFileSystem(this.DataDirectory);
			ExtensionManager extensionManager = this.extensionManager;
			Covalence covalence = new Covalence();
			Covalence covalence1 = covalence;
			this.covalence = covalence;
			extensionManager.RegisterLibrary("Covalence", covalence1);
			this.extensionManager.RegisterLibrary("Global", new Global());
			this.extensionManager.RegisterLibrary("Lang", new Lang());
			ExtensionManager extensionManager1 = this.extensionManager;
			Permission permission = new Permission();
			Permission permission1 = permission;
			this.libperm = permission;
			extensionManager1.RegisterLibrary("Permission", permission1);
			this.extensionManager.RegisterLibrary("Plugins", new Oxide.Core.Libraries.Plugins(this.RootPluginManager));
			this.extensionManager.RegisterLibrary("Time", new Time());
			ExtensionManager extensionManager2 = this.extensionManager;
			Oxide.Core.Libraries.Timer timer = new Oxide.Core.Libraries.Timer();
			Oxide.Core.Libraries.Timer timer1 = timer;
			this.libtimer = timer;
			extensionManager2.RegisterLibrary("Timer", timer1);
			this.extensionManager.RegisterLibrary("WebRequests", new WebRequests());
			this.LogInfo("Loading extensions...", Array.Empty<object>());
			this.extensionManager.LoadAllExtensions(this.ExtensionDirectory);
			Cleanup.Run();
			this.covalence.Initialize();
			this.RemoteConsole = new Oxide.Core.RemoteConsole.RemoteConsole();
			Oxide.Core.RemoteConsole.RemoteConsole remoteConsole = this.RemoteConsole;
			if (remoteConsole != null)
			{
				remoteConsole.Initalize();
			}
			else
			{
			}
			if (this.getTimeSinceStartup == null)
			{
				this.timer = new Stopwatch();
				this.timer.Start();
				this.getTimeSinceStartup = () => (float)this.timer.Elapsed.TotalSeconds;
				this.LogWarning("A reliable clock is not available, falling back to a clock which may be unreliable on certain hardware", Array.Empty<object>());
			}
			foreach (Extension allExtension in this.extensionManager.GetAllExtensions())
			{
				allExtension.LoadPluginWatchers(this.PluginDirectory);
			}
			this.LogInfo("Loading plugins...", Array.Empty<object>());
			this.LoadAllPlugins(true);
			foreach (PluginChangeWatcher pluginChangeWatcher in this.extensionManager.GetPluginChangeWatchers())
			{
				pluginChangeWatcher.OnPluginSourceChanged += new PluginChangeEvent(this.watcher_OnPluginSourceChanged);
				pluginChangeWatcher.OnPluginAdded += new PluginAddEvent(this.watcher_OnPluginAdded);
				pluginChangeWatcher.OnPluginRemoved += new PluginRemoveEvent(this.watcher_OnPluginRemoved);
			}
		}

		public void LoadAllPlugins(bool init = false)
		{
			IEnumerable<PluginLoader> array = this.extensionManager.GetPluginLoaders().ToArray<PluginLoader>();
			if (!this.HasLoadedCorePlugins)
			{
				foreach (PluginLoader pluginLoader in array)
				{
					Type[] corePlugins = pluginLoader.CorePlugins;
					for (int i = 0; i < (int)corePlugins.Length; i++)
					{
						Type type = corePlugins[i];
						try
						{
							Plugin plugin = (Plugin)Activator.CreateInstance(type);
							plugin.IsCorePlugin = true;
							this.PluginLoaded(plugin);
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							this.LogException(string.Concat("Could not load core plugin ", type.Name), exception);
						}
					}
				}
				this.HasLoadedCorePlugins = true;
			}
			foreach (PluginLoader pluginLoader1 in array)
			{
				foreach (string str in pluginLoader1.ScanDirectory(this.PluginDirectory))
				{
					this.LoadPlugin(str);
				}
			}
			if (!init)
			{
				return;
			}
			float now = this.Now;
			foreach (PluginLoader pluginLoader2 in this.extensionManager.GetPluginLoaders())
			{
				while (pluginLoader2.LoadingPlugins.Count > 0)
				{
					Thread.Sleep(25);
					this.OnFrame(this.Now - now);
					now = this.Now;
				}
			}
			this.isInitialized = true;
		}

		public bool LoadExtension(string name)
		{
			string str = Path.Combine(this.ExtensionDirectory, (!name.EndsWith(".dll") ? string.Concat(name, ".dll") : name));
			if (File.Exists(str))
			{
				this.extensionManager.LoadExtension(str, false);
				return true;
			}
			this.LogError(string.Concat("Could not load extension '", name, "' (file not found)"), Array.Empty<object>());
			return false;
		}

		public bool LoadPlugin(string name)
		{
			bool flag;
			if (this.RootPluginManager.GetPlugin(name) != null)
			{
				return false;
			}
			HashSet<PluginLoader> pluginLoaders = new HashSet<PluginLoader>(
				from l in this.extensionManager.GetPluginLoaders()
				where l.ScanDirectory(this.PluginDirectory).Contains<string>(name)
				select l);
			if (pluginLoaders.Count == 0)
			{
				this.LogError("Could not load plugin '{0}' (no plugin found with that file name)", new object[] { name });
				return false;
			}
			if (pluginLoaders.Count > 1)
			{
				this.LogError("Could not load plugin '{0}' (multiple plugin with that name)", new object[] { name });
				return false;
			}
			PluginLoader pluginLoader = pluginLoaders.First<PluginLoader>();
			try
			{
				Plugin plugin = pluginLoader.Load(this.PluginDirectory, name);
				if (plugin != null)
				{
					plugin.Loader = pluginLoader;
					this.PluginLoaded(plugin);
					flag = true;
				}
				else
				{
					flag = true;
				}
			}
			catch (Exception exception)
			{
				this.LogException(string.Concat("Could not load plugin ", name), exception);
				flag = false;
			}
			return flag;
		}

		public void LogDebug(string format, params object[] args)
		{
			this.RootLogger.Write(LogType.Warning, string.Concat("[DEBUG] ", format), args);
		}

		public void LogError(string format, params object[] args)
		{
			this.RootLogger.Write(LogType.Error, format, args);
		}

		public void LogException(string message, Exception ex)
		{
			this.RootLogger.WriteException(message, ex);
		}

		public void LogInfo(string format, params object[] args)
		{
			this.RootLogger.Write(LogType.Info, format, args);
		}

		public void LogWarning(string format, params object[] args)
		{
			this.RootLogger.Write(LogType.Warning, format, args);
		}

		public void NextTick(Action callback)
		{
			lock (this.nextTickLock)
			{
				this.nextTickQueue.Add(callback);
			}
		}

		public void OnFrame(Action<float> callback)
		{
			this.onFrame += callback;
		}

		public void OnFrame(float delta)
		{
			List<Action> actions;
			if (this.nextTickQueue.Count > 0)
			{
				lock (this.nextTickLock)
				{
					actions = this.nextTickQueue;
					this.nextTickQueue = this.lastTickQueue;
					this.lastTickQueue = actions;
				}
				for (int i = 0; i < actions.Count; i++)
				{
					try
					{
						actions[i]();
					}
					catch (Exception exception)
					{
						this.LogException("Exception while calling NextTick callback", exception);
					}
				}
				actions.Clear();
			}
			this.libtimer.Update(delta);
			if (this.isInitialized)
			{
				Oxide.Core.ServerConsole.ServerConsole serverConsole = this.ServerConsole;
				if (serverConsole != null)
				{
					serverConsole.Update();
				}
				else
				{
				}
				try
				{
					Action<float> action = this.onFrame;
					if (action != null)
					{
						action(delta);
					}
					else
					{
					}
				}
				catch (Exception exception2)
				{
					Exception exception1 = exception2;
					this.LogException(string.Concat(exception1.GetType().Name, " while invoke OnFrame in extensions"), exception1);
				}
			}
		}

		public void OnSave()
		{
			this.libperm.SaveData();
		}

		public void OnShutdown()
		{
			if (!this.IsShuttingDown)
			{
				this.libperm.SaveData();
				this.IsShuttingDown = true;
				this.UnloadAllPlugins(null);
				foreach (Extension allExtension in this.extensionManager.GetAllExtensions())
				{
					allExtension.OnShutdown();
				}
				foreach (string library in this.extensionManager.GetLibraries())
				{
					this.extensionManager.GetLibrary(library).Shutdown();
				}
				Oxide.Core.RemoteConsole.RemoteConsole remoteConsole = this.RemoteConsole;
				if (remoteConsole != null)
				{
					remoteConsole.Shutdown("Server shutting down", CloseStatusCode.Normal);
				}
				else
				{
				}
				Oxide.Core.ServerConsole.ServerConsole serverConsole = this.ServerConsole;
				if (serverConsole != null)
				{
					serverConsole.OnDisable();
				}
				else
				{
				}
				this.RootLogger.Shutdown();
			}
		}

		private void plugin_OnError(Plugin sender, string message)
		{
			this.LogError("{0} v{1}: {2}", new object[] { sender.Name, sender.Version, message });
		}

		public bool PluginLoaded(Plugin plugin)
		{
			bool flag;
			plugin.OnError += new PluginError(this.plugin_OnError);
			try
			{
				PluginLoader loader = plugin.Loader;
				if (loader != null)
				{
					loader.PluginErrors.Remove(plugin.Name);
				}
				else
				{
				}
				this.RootPluginManager.AddPlugin(plugin);
				if (plugin.Loader == null || !plugin.Loader.PluginErrors.ContainsKey(plugin.Name))
				{
					plugin.IsLoaded = true;
					this.CallHook("OnPluginLoaded", new object[] { plugin });
					this.LogInfo("Loaded plugin {0} v{1} by {2}", new object[] { plugin.Title, plugin.Version, plugin.Author });
					flag = true;
				}
				else
				{
					this.UnloadPlugin(plugin.Name);
					flag = false;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (plugin.Loader != null)
				{
					plugin.Loader.PluginErrors[plugin.Name] = exception.Message;
				}
				this.LogException(string.Format("Could not initialize plugin '{0} v{1}'", plugin.Name, plugin.Version), exception);
				flag = false;
			}
			return flag;
		}

		public void RegisterEngineClock(Func<float> method)
		{
			this.getTimeSinceStartup = method;
		}

		private static void RegisterLibrarySearchPath(string path)
		{
			char pathSeparator;
			string str;
			string str1;
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
				{
					string environmentVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
					if (string.IsNullOrEmpty(environmentVariable))
					{
						str = path;
					}
					else
					{
						pathSeparator = Path.PathSeparator;
						str = string.Concat(environmentVariable, pathSeparator.ToString(), path);
					}
					Environment.SetEnvironmentVariable("PATH", str);
					OxideMod.SetDllDirectory(path);
					return;
				}
				case PlatformID.WinCE:
				case PlatformID.Xbox:
				{
					return;
				}
				case PlatformID.Unix:
				case PlatformID.MacOSX:
				{
					string environmentVariable1 = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? string.Empty;
					if (string.IsNullOrEmpty(environmentVariable1))
					{
						str1 = path;
					}
					else
					{
						pathSeparator = Path.PathSeparator;
						str1 = string.Concat(environmentVariable1, pathSeparator.ToString(), path);
					}
					Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", str1);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		public void ReloadAllPlugins(IList<string> skip = null)
		{
			Func<Plugin, bool> func = null;
			IEnumerable<Plugin> plugins = this.RootPluginManager.GetPlugins();
			Func<Plugin, bool> func1 = func;
			if (func1 == null)
			{
				Func<Plugin, bool> isCorePlugin = (Plugin p) => {
					if (p.IsCorePlugin)
					{
						return false;
					}
					if (skip == null)
					{
						return true;
					}
					return !skip.Contains(p.Name);
				};
				Func<Plugin, bool> func2 = isCorePlugin;
				func = isCorePlugin;
				func1 = func2;
			}
			Plugin[] array = plugins.Where<Plugin>(func1).ToArray<Plugin>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				this.ReloadPlugin(array[i].Name);
			}
		}

		public bool ReloadExtension(string name)
		{
			string str = Path.Combine(this.ExtensionDirectory, (!name.EndsWith(".dll") ? string.Concat(name, ".dll") : name));
			if (File.Exists(str))
			{
				this.extensionManager.ReloadExtension(str);
				return true;
			}
			this.LogError(string.Concat("Could not reload extension '", name, "' (file not found)"), Array.Empty<object>());
			return false;
		}

		public bool ReloadPlugin(string name)
		{
			string str = name;
			bool flag = false;
			string pluginDirectory = this.PluginDirectory;
			if (str.Contains("\\"))
			{
				flag = true;
				string directoryName = Path.GetDirectoryName(str);
				if (directoryName != null)
				{
					pluginDirectory = Path.Combine(pluginDirectory, directoryName);
					str = str.Substring(directoryName.Length + 1);
				}
			}
			PluginLoader pluginLoader = this.extensionManager.GetPluginLoaders().FirstOrDefault<PluginLoader>((PluginLoader l) => l.ScanDirectory(pluginDirectory).Contains<string>(str));
			if (pluginLoader != null)
			{
				pluginLoader.Reload(pluginDirectory, str);
				return true;
			}
			if (flag)
			{
				return false;
			}
			this.UnloadPlugin(str);
			this.LoadPlugin(str);
			return true;
		}

		[DllImport("kernel32", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool SetDllDirectory(string lpPathName);

		public void UnloadAllPlugins(IList<string> skip = null)
		{
			Func<Plugin, bool> func = null;
			IEnumerable<Plugin> plugins = this.RootPluginManager.GetPlugins();
			Func<Plugin, bool> func1 = func;
			if (func1 == null)
			{
				Func<Plugin, bool> isCorePlugin = (Plugin p) => {
					if (p.IsCorePlugin)
					{
						return false;
					}
					if (skip == null)
					{
						return true;
					}
					return !skip.Contains(p.Name);
				};
				Func<Plugin, bool> func2 = isCorePlugin;
				func = isCorePlugin;
				func1 = func2;
			}
			Plugin[] array = plugins.Where<Plugin>(func1).ToArray<Plugin>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				this.UnloadPlugin(array[i].Name);
			}
		}

		public bool UnloadExtension(string name)
		{
			string str = Path.Combine(this.ExtensionDirectory, (!name.EndsWith(".dll") ? string.Concat(name, ".dll") : name));
			if (File.Exists(str))
			{
				this.extensionManager.UnloadExtension(str);
				return true;
			}
			this.LogError(string.Concat("Could not unload extension '", name, "' (file not found)"), Array.Empty<object>());
			return false;
		}

		public bool UnloadPlugin(string name)
		{
			Plugin plugin = this.RootPluginManager.GetPlugin(name);
			if (plugin == null)
			{
				return false;
			}
			PluginLoader pluginLoader = this.extensionManager.GetPluginLoaders().SingleOrDefault<PluginLoader>((PluginLoader l) => l.LoadedPlugins.ContainsKey(name));
			if (pluginLoader != null)
			{
				pluginLoader.Unloading(plugin);
			}
			else
			{
			}
			this.RootPluginManager.RemovePlugin(plugin);
			if (plugin.IsLoaded)
			{
				this.CallHook("OnPluginUnloaded", new object[] { plugin });
			}
			plugin.IsLoaded = false;
			this.LogInfo("Unloaded plugin {0} v{1} by {2}", new object[] { plugin.Title, plugin.Version, plugin.Author });
			return true;
		}

		private void watcher_OnPluginAdded(string name)
		{
			this.LoadPlugin(name);
		}

		private void watcher_OnPluginRemoved(string name)
		{
			this.UnloadPlugin(name);
		}

		private void watcher_OnPluginSourceChanged(string name)
		{
			this.ReloadPlugin(name);
		}
	}
}