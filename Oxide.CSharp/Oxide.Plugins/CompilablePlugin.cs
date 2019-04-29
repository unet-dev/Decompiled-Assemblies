using Oxide.Core;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins
{
	public class CompilablePlugin : CompilableFile
	{
		private static object compileLock;

		public Oxide.Plugins.CompiledAssembly LastGoodAssembly;

		public bool IsLoading;

		static CompilablePlugin()
		{
			CompilablePlugin.compileLock = new object();
		}

		public CompilablePlugin(CSharpExtension extension, CSharpPluginLoader loader, string directory, string name) : base(extension, loader, directory, name)
		{
		}

		protected override void InitFailed(string message = null)
		{
			base.InitFailed(message);
			if (this.LastGoodAssembly == null)
			{
				Interface.Oxide.LogInfo("No previous version to rollback plugin: {0}", new object[] { this.ScriptName });
				return;
			}
			if (this.CompiledAssembly == this.LastGoodAssembly)
			{
				Interface.Oxide.LogInfo("Previous version of plugin failed to load: {0}", new object[] { this.ScriptName });
				return;
			}
			Interface.Oxide.LogInfo("Rolling back plugin to last good version: {0}", new object[] { this.ScriptName });
			this.CompiledAssembly = this.LastGoodAssembly;
			this.CompilerErrors = null;
			this.LoadPlugin(null);
		}

		internal void LoadPlugin(Action<CSharpPlugin> callback = null)
		{
			if (this.CompiledAssembly == null)
			{
				Interface.Oxide.LogError("Load called before a compiled assembly exists: {0}", new object[] { this.Name });
				return;
			}
			this.LoadCallback = callback;
			this.CompiledAssembly.LoadAssembly((bool loaded) => {
				CSharpPlugin watcher;
				if (!loaded)
				{
					Action<CSharpPlugin> action = callback;
					if (action == null)
					{
						return;
					}
					action(null);
					return;
				}
				if (this.CompilerErrors != null)
				{
					this.InitFailed(string.Concat("Unable to load ", this.ScriptName, ". ", this.CompilerErrors));
					return;
				}
				Type type = this.CompiledAssembly.LoadedAssembly.GetType(string.Concat("Oxide.Plugins.", this.Name));
				if (type == null)
				{
					this.InitFailed(string.Concat("Unable to find main plugin class: ", this.Name));
					return;
				}
				try
				{
					watcher = Activator.CreateInstance(type) as CSharpPlugin;
					goto Label0;
				}
				catch (MissingMethodException missingMethodException)
				{
					this.InitFailed(string.Concat("Main plugin class should not have a constructor defined: ", this.Name));
				}
				catch (TargetInvocationException targetInvocationException)
				{
					Exception innerException = targetInvocationException.InnerException;
					this.InitFailed(string.Concat("Unable to load ", this.ScriptName, ". ", innerException.ToString()));
				}
				catch (Exception exception)
				{
					this.InitFailed(string.Concat("Unable to load ", this.ScriptName, ". ", exception.ToString()));
				}
				return;
			Label0:
				if (watcher == null)
				{
					this.InitFailed(string.Concat("Plugin assembly failed to load: ", this.ScriptName));
					return;
				}
				if (!watcher.SetPluginInfo(this.ScriptName, this.ScriptPath))
				{
					this.InitFailed(null);
					return;
				}
				watcher.Watcher = this.Extension.Watcher;
				watcher.Loader = this.Loader;
				if (!Interface.Oxide.PluginLoaded(watcher))
				{
					this.InitFailed(null);
					return;
				}
				if (!this.CompiledAssembly.IsBatch)
				{
					this.LastGoodAssembly = this.CompiledAssembly;
				}
				Action<CSharpPlugin> action1 = callback;
				if (action1 == null)
				{
					return;
				}
				action1(watcher);
			});
		}

		protected override void OnCompilationRequested()
		{
			this.Loader.CompilationRequested(this);
		}

		internal override void OnCompilationStarted()
		{
			base.OnCompilationStarted();
			foreach (Plugin plugin in Interface.Oxide.RootPluginManager.GetPlugins())
			{
				if (!(plugin is CSharpPlugin))
				{
					continue;
				}
				CompilablePlugin compilablePlugin = CSharpPluginLoader.GetCompilablePlugin(this.Directory, plugin.Name);
				if (!compilablePlugin.Requires.Contains(this.Name))
				{
					continue;
				}
				compilablePlugin.CompiledAssembly = null;
				this.Loader.Load(compilablePlugin);
			}
		}

		protected override void OnLoadingStarted()
		{
			this.Loader.PluginLoadingStarted(this);
		}
	}
}