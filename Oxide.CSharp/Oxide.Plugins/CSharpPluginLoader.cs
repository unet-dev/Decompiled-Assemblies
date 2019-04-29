using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Oxide.Plugins
{
	public class CSharpPluginLoader : PluginLoader
	{
		public static string[] DefaultReferences;

		public static HashSet<string> PluginReferences;

		public static CSharpPluginLoader Instance;

		private static CSharpExtension extension;

		private static Dictionary<string, CompilablePlugin> plugins;

		private readonly static string[] AssemblyBlacklist;

		private List<CompilablePlugin> compilationQueue = new List<CompilablePlugin>();

		private PluginCompiler compiler;

		public override string FileExtension
		{
			get
			{
				return ".cs";
			}
		}

		static CSharpPluginLoader()
		{
			CSharpPluginLoader.DefaultReferences = new string[] { "mscorlib", "Oxide.Core", "Oxide.CSharp", "System", "System.Core", "System.Data" };
			CSharpPluginLoader.PluginReferences = new HashSet<string>(CSharpPluginLoader.DefaultReferences);
			CSharpPluginLoader.plugins = new Dictionary<string, CompilablePlugin>();
			CSharpPluginLoader.AssemblyBlacklist = new string[] { "Newtonsoft.Json", "protobuf-net", "websocket-sharp" };
		}

		public CSharpPluginLoader(CSharpExtension extension)
		{
			CSharpPluginLoader.Instance = this;
			CSharpPluginLoader.extension = extension;
			PluginCompiler.CheckCompilerBinary();
			this.compiler = new PluginCompiler();
		}

		public void CompilationRequested(CompilablePlugin plugin)
		{
			if (Compilation.Current != null)
			{
				Compilation.Current.Add(plugin);
				return;
			}
			if (this.compilationQueue.Count < 1)
			{
				Interface.Oxide.NextTick(() => {
					this.CompileAssembly(this.compilationQueue.ToArray());
					this.compilationQueue.Clear();
				});
			}
			this.compilationQueue.Add(plugin);
		}

		private void CompileAssembly(CompilablePlugin[] plugins)
		{
			this.compiler.Compile(plugins, (Compilation compilation) => {
				if (compilation.compiledAssembly != null)
				{
					if (compilation.plugins.Count > 0)
					{
						string[] array = (
							from pl in compilation.plugins
							where string.IsNullOrEmpty(pl.CompilerErrors)
							select pl.Name).ToArray<string>();
						Interface.Oxide.LogInfo(string.Format("{0} {1} compiled successfully in {2}ms", array.ToSentence<string>(), ((int)array.Length > 1 ? "were" : "was"), Math.Round((double)(compilation.duration * 1000f))), Array.Empty<object>());
					}
					foreach (CompilablePlugin plugin in compilation.plugins)
					{
						if (plugin.CompilerErrors != null)
						{
							plugin.OnCompilationFailed();
							base.PluginErrors[plugin.Name] = string.Concat("Failed to compile: ", plugin.CompilerErrors);
							Interface.Oxide.LogError(string.Concat("Error while compiling: ", plugin.CompilerErrors), Array.Empty<object>());
						}
						else
						{
							Interface.Oxide.UnloadPlugin(plugin.Name);
							plugin.OnCompilationSucceeded(compilation.compiledAssembly);
						}
					}
				}
				else
				{
					foreach (CompilablePlugin compilablePlugin in compilation.plugins)
					{
						compilablePlugin.OnCompilationFailed();
						base.PluginErrors[compilablePlugin.Name] = string.Concat("Failed to compile: ", compilablePlugin.CompilerErrors);
						Interface.Oxide.LogError(string.Concat("Error while compiling: ", compilablePlugin.CompilerErrors), Array.Empty<object>());
					}
				}
			});
		}

		public static CompilablePlugin GetCompilablePlugin(string directory, string name)
		{
			CompilablePlugin compilablePlugin;
			string str = Regex.Replace(name, "_", "");
			if (!CSharpPluginLoader.plugins.TryGetValue(str, out compilablePlugin))
			{
				compilablePlugin = new CompilablePlugin(CSharpPluginLoader.extension, CSharpPluginLoader.Instance, directory, name);
				CSharpPluginLoader.plugins[str] = compilablePlugin;
			}
			return compilablePlugin;
		}

		public override Plugin Load(string directory, string name)
		{
			CompilablePlugin compilablePlugin = CSharpPluginLoader.GetCompilablePlugin(directory, name);
			if (!compilablePlugin.IsLoading)
			{
				this.Load(compilablePlugin);
				return null;
			}
			Interface.Oxide.LogDebug(string.Concat("Load requested for plugin which is already loading: ", compilablePlugin.Name), Array.Empty<object>());
			return null;
		}

		public void Load(CompilablePlugin plugin)
		{
			Func<string, bool> func4 = null;
			Func<string, bool> func5 = null;
			Func<string, bool> func6 = null;
			Action<CSharpPlugin> action2 = null;
			plugin.Compile((bool compiled) => {
				Func<string, bool> func;
				if (!compiled)
				{
					this.PluginLoadingCompleted(plugin);
					return;
				}
				HashSet<string> requires = plugin.Requires;
				Func<string, bool> u003cu003e9_1 = func4;
				if (u003cu003e9_1 == null)
				{
					Func<string, bool> func1 = (string r) => {
						if (!this.LoadedPlugins.ContainsKey(r))
						{
							return false;
						}
						return this.LoadingPlugins.Contains(r);
					};
					func = func1;
					func4 = func1;
					u003cu003e9_1 = func;
				}
				foreach (string str in requires.Where<string>(u003cu003e9_1))
				{
					Interface.Oxide.UnloadPlugin(str);
				}
				HashSet<string> strs = plugin.Requires;
				Func<string, bool> u003cu003e9_2 = func5;
				if (u003cu003e9_2 == null)
				{
					Func<string, bool> func2 = (string r) => !this.LoadedPlugins.ContainsKey(r);
					func = func2;
					func5 = func2;
					u003cu003e9_2 = func;
				}
				IEnumerable<string> strs1 = strs.Where<string>(u003cu003e9_2);
				if (!strs1.Any<string>())
				{
					Interface.Oxide.UnloadPlugin(plugin.Name);
					CompilablePlugin compilablePlugin = plugin;
					Action<CSharpPlugin> u003cu003e9_3 = action2;
					if (u003cu003e9_3 == null)
					{
						Action<CSharpPlugin> action = (CSharpPlugin pl) => {
							if (pl != null)
							{
								this.LoadedPlugins[pl.Name] = pl;
							}
							this.PluginLoadingCompleted(plugin);
						};
						Action<CSharpPlugin> action1 = action;
						action2 = action;
						u003cu003e9_3 = action1;
					}
					compilablePlugin.LoadPlugin(u003cu003e9_3);
					return;
				}
				HashSet<string> requires1 = plugin.Requires;
				Func<string, bool> u003cu003e9_4 = func6;
				if (u003cu003e9_4 == null)
				{
					Func<string, bool> func3 = (string r) => this.LoadingPlugins.Contains(r);
					func = func3;
					func6 = func3;
					u003cu003e9_4 = func;
				}
				IEnumerable<string> strs2 = requires1.Where<string>(u003cu003e9_4);
				if (strs2.Any<string>())
				{
					Interface.Oxide.LogDebug(string.Concat(plugin.Name, " plugin is waiting for requirements to be loaded: ", strs2.ToSentence<string>()), Array.Empty<object>());
					return;
				}
				Interface.Oxide.LogError(string.Concat(plugin.Name, " plugin requires missing dependencies: ", strs1.ToSentence<string>()), Array.Empty<object>());
				this.PluginErrors[plugin.Name] = string.Concat("Missing dependencies: ", strs1.ToSentence<string>());
				this.PluginLoadingCompleted(plugin);
			});
		}

		public void OnModLoaded()
		{
			foreach (Extension allExtension in Interface.Oxide.GetAllExtensions())
			{
				if (allExtension == null || !allExtension.IsCoreExtension && !allExtension.IsGameExtension)
				{
					continue;
				}
				Assembly assembly = allExtension.GetType().Assembly;
				string name = assembly.GetName().Name;
				if (CSharpPluginLoader.AssemblyBlacklist.Contains<string>(name))
				{
					continue;
				}
				CSharpPluginLoader.PluginReferences.Add(name);
				AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
				for (int i = 0; i < (int)referencedAssemblies.Length; i++)
				{
					AssemblyName assemblyName = referencedAssemblies[i];
					if (assemblyName != null)
					{
						CSharpPluginLoader.PluginReferences.Add(assemblyName.Name);
					}
				}
			}
		}

		public void OnShutdown()
		{
			this.compiler.Shutdown();
		}

		private void PluginLoadingCompleted(CompilablePlugin plugin)
		{
			base.LoadingPlugins.Remove(plugin.Name);
			plugin.IsLoading = false;
			string[] array = base.LoadingPlugins.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				string str = array[i];
				CompilablePlugin compilablePlugin = CSharpPluginLoader.GetCompilablePlugin(plugin.Directory, str);
				if (compilablePlugin.IsLoading && compilablePlugin.Requires.Contains(plugin.Name))
				{
					this.Load(compilablePlugin);
				}
			}
		}

		public void PluginLoadingStarted(CompilablePlugin plugin)
		{
			base.LoadingPlugins.Add(plugin.Name);
			plugin.IsLoading = true;
		}

		public override void Reload(string directory, string name)
		{
			if (!Regex.Match(directory, "\\\\include\\b", RegexOptions.IgnoreCase).Success)
			{
				CompilablePlugin compilablePlugin = CSharpPluginLoader.GetCompilablePlugin(directory, name);
				if (!compilablePlugin.IsLoading)
				{
					this.Load(compilablePlugin);
					return;
				}
				Interface.Oxide.LogDebug(string.Concat("Reload requested for plugin which is already loading: ", compilablePlugin.Name), Array.Empty<object>());
				return;
			}
			name = string.Concat("Oxide.", name);
			foreach (CompilablePlugin value in CSharpPluginLoader.plugins.Values)
			{
				if (!value.References.Contains(name))
				{
					continue;
				}
				Interface.Oxide.LogInfo(string.Concat("Reloading ", value.Name, " because it references updated include file: ", name), Array.Empty<object>());
				value.LastModifiedAt = DateTime.Now;
				this.Load(value);
			}
		}

		public override IEnumerable<string> ScanDirectory(string directory)
		{
			CSharpPluginLoader cSharpPluginLoader = null;
			if (PluginCompiler.BinaryPath != null)
			{
				foreach (string str in cSharpPluginLoader.<>n__0(directory))
				{
					yield return str;
				}
			}
			else
			{
			}
		}

		public override void Unloading(Plugin pluginBase)
		{
			CSharpPlugin cSharpPlugin = pluginBase as CSharpPlugin;
			if (cSharpPlugin == null)
			{
				return;
			}
			this.LoadedPlugins.Remove(cSharpPlugin.Name);
			foreach (CompilablePlugin value in CSharpPluginLoader.plugins.Values)
			{
				if (!value.Requires.Contains(cSharpPlugin.Name))
				{
					continue;
				}
				Interface.Oxide.UnloadPlugin(value.Name);
			}
		}
	}
}