using ObjectStream.Data;
using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Oxide.Plugins
{
	internal class Compilation
	{
		public static Compilation Current;

		internal int id;

		internal string name;

		internal Action<Compilation> callback;

		internal ConcurrentHashSet<CompilablePlugin> queuedPlugins;

		internal HashSet<CompilablePlugin> plugins = new HashSet<CompilablePlugin>();

		internal float startedAt;

		internal float endedAt;

		internal Hash<string, CompilerFile> references = new Hash<string, CompilerFile>();

		internal HashSet<string> referencedPlugins = new HashSet<string>();

		internal CompiledAssembly compiledAssembly;

		private string includePath;

		private string[] extensionNames;

		private string gameExtensionNamespace;

		private readonly string gameExtensionName;

		private readonly string gameExtensionBranch;

		internal float duration
		{
			get
			{
				return this.endedAt - this.startedAt;
			}
		}

		internal Compilation(int id, Action<Compilation> callback, CompilablePlugin[] plugins)
		{
			string upper;
			string @namespace;
			string str;
			this.id = id;
			this.callback = callback;
			this.queuedPlugins = new ConcurrentHashSet<CompilablePlugin>(plugins);
			if (Compilation.Current == null)
			{
				Compilation.Current = this;
			}
			CompilablePlugin[] compilablePluginArray = plugins;
			for (int i = 0; i < (int)compilablePluginArray.Length; i++)
			{
				CompilablePlugin compilablePlugin = compilablePluginArray[i];
				compilablePlugin.CompilerErrors = null;
				compilablePlugin.OnCompilationStarted();
			}
			this.includePath = Path.Combine(Interface.Oxide.PluginDirectory, "include");
			this.extensionNames = (
				from ext in Interface.Oxide.GetAllExtensions()
				select ext.Name).ToArray<string>();
			Extension extension = Interface.Oxide.GetAllExtensions().SingleOrDefault<Extension>((Extension ext) => ext.IsGameExtension);
			if (extension != null)
			{
				upper = extension.Name.ToUpper();
			}
			else
			{
				upper = null;
			}
			this.gameExtensionName = upper;
			if (extension != null)
			{
				@namespace = extension.GetType().Namespace;
			}
			else
			{
				@namespace = null;
			}
			this.gameExtensionNamespace = @namespace;
			if (extension != null)
			{
				string branch = extension.Branch;
				if (branch != null)
				{
					str = branch.ToUpper();
				}
				else
				{
					str = null;
				}
			}
			else
			{
				str = null;
			}
			this.gameExtensionBranch = str;
		}

		internal void Add(CompilablePlugin plugin)
		{
			if (!this.queuedPlugins.Add(plugin))
			{
				return;
			}
			plugin.Loader.PluginLoadingStarted(plugin);
			plugin.CompilerErrors = null;
			plugin.OnCompilationStarted();
			foreach (Plugin plugin1 in 
				from pl in Interface.Oxide.RootPluginManager.GetPlugins()
				where pl is CSharpPlugin
				select pl)
			{
				CompilablePlugin compilablePlugin = CSharpPluginLoader.GetCompilablePlugin(plugin.Directory, plugin1.Name);
				if (!compilablePlugin.Requires.Contains(plugin.Name))
				{
					continue;
				}
				this.AddDependency(compilablePlugin);
			}
		}

		private void AddDependency(CompilablePlugin plugin)
		{
			if (plugin.IsLoading || this.plugins.Contains(plugin) || this.queuedPlugins.Contains(plugin))
			{
				return;
			}
			CompiledAssembly compiledAssembly = plugin.CompiledAssembly;
			if (compiledAssembly == null || compiledAssembly.IsOutdated())
			{
				this.Add(plugin);
			}
			else
			{
				this.referencedPlugins.Add(plugin.Name);
				if (!this.references.ContainsKey(compiledAssembly.Name))
				{
					this.references[compiledAssembly.Name] = new CompilerFile(compiledAssembly.Name, compiledAssembly.RawAssembly);
					return;
				}
			}
		}

		private void AddReference(CompilablePlugin plugin, string assemblyName)
		{
			Assembly assembly;
			if (!File.Exists(Path.Combine(Interface.Oxide.ExtensionDirectory, string.Concat(assemblyName, ".dll"))))
			{
				if (assemblyName.StartsWith("Oxide."))
				{
					plugin.References.Add(assemblyName);
					return;
				}
				Interface.Oxide.LogError(string.Concat(new string[] { "Assembly referenced by ", plugin.Name, " plugin does not exist: ", assemblyName, ".dll" }), Array.Empty<object>());
				plugin.CompilerErrors = string.Concat("Referenced assembly does not exist: ", assemblyName);
				this.RemovePlugin(plugin);
				return;
			}
			try
			{
				assembly = Assembly.Load(assemblyName);
			}
			catch (FileNotFoundException fileNotFoundException)
			{
				Interface.Oxide.LogError(string.Concat(new string[] { "Assembly referenced by ", plugin.Name, " plugin is invalid: ", assemblyName, ".dll" }), Array.Empty<object>());
				plugin.CompilerErrors = string.Concat("Referenced assembly is invalid: ", assemblyName);
				this.RemovePlugin(plugin);
				return;
			}
			this.AddReference(plugin, assembly.GetName());
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < (int)referencedAssemblies.Length; i++)
			{
				AssemblyName assemblyName1 = referencedAssemblies[i];
				if (!assemblyName1.Name.StartsWith("Newtonsoft.Json") && !assemblyName1.Name.StartsWith("Rust.Workshop"))
				{
					if (File.Exists(Path.Combine(Interface.Oxide.ExtensionDirectory, string.Concat(assemblyName1.Name, ".dll"))))
					{
						this.AddReference(plugin, assemblyName1);
					}
					else
					{
						Interface.Oxide.LogWarning(string.Concat(new string[] { "Reference ", assemblyName1.Name, ".dll from ", assembly.GetName().Name, ".dll not found" }), Array.Empty<object>());
					}
				}
			}
		}

		private void AddReference(CompilablePlugin plugin, AssemblyName reference)
		{
			string compilerFile = string.Concat(reference.Name, ".dll");
			if (!this.references.ContainsKey(compilerFile))
			{
				this.references[compilerFile] = new CompilerFile(Interface.Oxide.ExtensionDirectory, compilerFile);
			}
			plugin.References.Add(reference.Name);
		}

		private void CacheModifiedScripts()
		{
			CompilablePlugin[] array = this.plugins.Where<CompilablePlugin>((CompilablePlugin pl) => {
				if (pl.ScriptLines == null || pl.HasBeenModified())
				{
					return true;
				}
				return pl.LastCachedScriptAt != pl.LastModifiedAt;
			}).ToArray<CompilablePlugin>();
			if ((int)array.Length < 1)
			{
				return;
			}
			CompilablePlugin[] compilablePluginArray = array;
			for (int i = 0; i < (int)compilablePluginArray.Length; i++)
			{
				this.CacheScriptLines(compilablePluginArray[i]);
			}
			Thread.Sleep(100);
			this.CacheModifiedScripts();
		}

		private bool CacheScriptLines(CompilablePlugin plugin)
		{
			bool flag;
			bool flag1 = false;
			while (true)
			{
				try
				{
					if (File.Exists(plugin.ScriptPath))
					{
						plugin.CheckLastModificationTime();
						if (plugin.LastCachedScriptAt != plugin.LastModifiedAt)
						{
							using (StreamReader streamReader = File.OpenText(plugin.ScriptPath))
							{
								List<string> strs = new List<string>();
								while (!streamReader.EndOfStream)
								{
									strs.Add(streamReader.ReadLine());
								}
								if (!string.IsNullOrEmpty(this.gameExtensionName))
								{
									strs.Insert(0, string.Concat("#define ", this.gameExtensionName));
								}
								if (!string.IsNullOrEmpty(this.gameExtensionName))
								{
									strs.Insert(0, string.Concat("#define ", this.gameExtensionName));
									if (!string.IsNullOrEmpty(this.gameExtensionBranch) && this.gameExtensionBranch != "public")
									{
										strs.Insert(0, string.Concat("#define ", this.gameExtensionName, this.gameExtensionBranch));
									}
								}
								plugin.ScriptLines = strs.ToArray();
								plugin.ScriptEncoding = streamReader.CurrentEncoding;
							}
							plugin.LastCachedScriptAt = plugin.LastModifiedAt;
							if (this.plugins.Remove(plugin))
							{
								this.queuedPlugins.Add(plugin);
							}
						}
						flag = true;
						break;
					}
					else
					{
						Interface.Oxide.LogWarning("Script no longer exists: {0}", new object[] { plugin.Name });
						plugin.CompilerErrors = "Plugin file was deleted";
						this.RemovePlugin(plugin);
						flag = false;
						break;
					}
				}
				catch (IOException oException)
				{
					if (!flag1)
					{
						flag1 = true;
						Interface.Oxide.LogWarning("Waiting for another application to stop using script: {0}", new object[] { plugin.Name });
					}
					Thread.Sleep(50);
				}
			}
			return flag;
		}

		internal void Completed(byte[] rawAssembly = null)
		{
			this.endedAt = Interface.Oxide.Now;
			if (this.plugins.Count > 0 && rawAssembly != null)
			{
				this.compiledAssembly = new CompiledAssembly(this.name, this.plugins.ToArray<CompilablePlugin>(), rawAssembly, this.duration);
			}
			Interface.Oxide.NextTick(() => this.callback(this));
		}

		internal bool IncludesRequiredPlugin(string name)
		{
			if (this.referencedPlugins.Contains(name))
			{
				return true;
			}
			CompilablePlugin compilablePlugin = this.plugins.SingleOrDefault<CompilablePlugin>((CompilablePlugin pl) => pl.Name == name);
			if (compilablePlugin == null)
			{
				return false;
			}
			return compilablePlugin.CompilerErrors == null;
		}

		internal void Prepare(Action callback)
		{
			ThreadPool.QueueUserWorkItem((object _) => {
				CompilablePlugin compilablePlugin;
				try
				{
					this.referencedPlugins.Clear();
					this.references.Clear();
					foreach (string pluginReference in CSharpPluginLoader.PluginReferences)
					{
						if (File.Exists(Path.Combine(Interface.Oxide.ExtensionDirectory, string.Concat(pluginReference, ".dll"))))
						{
							this.references[string.Concat(pluginReference, ".dll")] = new CompilerFile(Interface.Oxide.ExtensionDirectory, string.Concat(pluginReference, ".dll"));
						}
						if (!File.Exists(Path.Combine(Interface.Oxide.ExtensionDirectory, string.Concat(pluginReference, ".exe"))))
						{
							continue;
						}
						this.references[string.Concat(pluginReference, ".exe")] = new CompilerFile(Interface.Oxide.ExtensionDirectory, string.Concat(pluginReference, ".exe"));
					}
					while (this.queuedPlugins.TryDequeue(out compilablePlugin))
					{
						if (Compilation.Current == null)
						{
							Compilation.Current = this;
						}
						if (!this.CacheScriptLines(compilablePlugin) || (int)compilablePlugin.ScriptLines.Length < 1)
						{
							compilablePlugin.References.Clear();
							compilablePlugin.IncludePaths.Clear();
							compilablePlugin.Requires.Clear();
							Interface.Oxide.LogWarning(string.Concat("Plugin script is empty: ", compilablePlugin.Name), Array.Empty<object>());
							this.RemovePlugin(compilablePlugin);
						}
						else if (this.plugins.Add(compilablePlugin))
						{
							this.PreparseScript(compilablePlugin);
							this.ResolveReferences(compilablePlugin);
						}
						this.CacheModifiedScripts();
						if (this.queuedPlugins.Count != 0 || Compilation.Current != this)
						{
							continue;
						}
						Compilation.Current = null;
					}
					callback();
				}
				catch (Exception exception)
				{
					Interface.Oxide.LogException("Exception while resolving plugin references", exception);
				}
			});
		}

		private void PreparseScript(CompilablePlugin plugin)
		{
			plugin.References.Clear();
			plugin.IncludePaths.Clear();
			plugin.Requires.Clear();
			if (plugin.ScriptLines.Any<string>((string line) => {
				if (line.Contains("uMod"))
				{
					return true;
				}
				return line.Contains("Universal");
			}))
			{
				plugin.ScriptLines = (
					from s in plugin.ScriptLines
					select s.Replace("uMod", "Oxide.Core").Replace("using Oxide.Core;", "using Oxide.Core; using Oxide.Core.Plugins;").Replace("namespace Oxide.Core.Plugins", "namespace Oxide.Plugins").Replace("Libraries.Universal", "Libraries.Covalence").Replace("UniversalPlugin", "CovalencePlugin").Replace("Oxide.Core.Version", "OxideMod.Version")).ToArray<string>();
			}
			bool flag = false;
			for (int i = 0; i < (int)plugin.ScriptLines.Length; i++)
			{
				string str = plugin.ScriptLines[i].Trim();
				if (str.Length >= 1)
				{
					if (!flag)
					{
						Match match = Regex.Match(str, "^//\\s*Requires:\\s*(\\S+?)(\\.cs)?\\s*$", RegexOptions.IgnoreCase);
						if (!match.Success)
						{
							match = Regex.Match(str, "^//\\s*Reference:\\s*(\\S+)\\s*$", RegexOptions.IgnoreCase);
							if (!match.Success)
							{
								match = Regex.Match(str, "^\\s*using\\s+(Oxide\\.(?:Core|Ext|Game)\\.(?:[^\\.]+))[^;]*;.*$", RegexOptions.IgnoreCase);
								if (!match.Success)
								{
									match = Regex.Match(str, "^\\s*namespace Oxide\\.Plugins\\s*(\\{\\s*)?$", RegexOptions.IgnoreCase);
									if (match.Success)
									{
										flag = true;
									}
								}
								else
								{
									string value = match.Groups[1].Value;
									string str1 = Regex.Replace(value, "Oxide\\.[\\w]+\\.([\\w]+)", "Oxide.$1");
									if (string.IsNullOrEmpty(str1) || !File.Exists(Path.Combine(Interface.get_Oxide().get_ExtensionDirectory(), string.Concat(str1, ".dll"))))
									{
										this.AddReference(plugin, value);
									}
									else
									{
										this.AddReference(plugin, str1);
									}
								}
							}
							else
							{
								string value1 = match.Groups[1].Value;
								if (value1.StartsWith("Oxide.") || value1.StartsWith("Newtonsoft.Json") || value1.StartsWith("protobuf-net") || value1.StartsWith("Rust."))
								{
									Interface.get_Oxide().LogWarning("Ignored unnecessary '// Reference: {0}' in plugin '{1}'", new object[] { value1, plugin.Name });
								}
								else
								{
									this.AddReference(plugin, value1);
									Interface.get_Oxide().LogInfo("Added '// Reference: {0}' in plugin '{1}'", new object[] { value1, plugin.Name });
								}
							}
						}
						else
						{
							string value2 = match.Groups[1].Value;
							plugin.Requires.Add(value2);
							if (!File.Exists(Path.Combine(plugin.Directory, string.Concat(value2, ".cs"))))
							{
								Interface.get_Oxide().LogError(string.Concat(plugin.Name, " plugin requires missing dependency: ", value2), Array.Empty<object>());
								plugin.CompilerErrors = string.Concat("Missing dependency: ", value2);
								this.RemovePlugin(plugin);
								return;
							}
							this.AddDependency(CSharpPluginLoader.GetCompilablePlugin(plugin.Directory, value2));
						}
					}
					else
					{
						Match match1 = Regex.Match(str, "^\\s*\\{?\\s*$", RegexOptions.IgnoreCase);
						if (!match1.Success)
						{
							match1 = Regex.Match(str, "^\\s*\\[", RegexOptions.IgnoreCase);
							if (!match1.Success)
							{
								match1 = Regex.Match(str, "^\\s*(?:public|private|protected|internal)?\\s*class\\s+(\\S+)\\s+\\:\\s+\\S+Plugin\\s*$", RegexOptions.IgnoreCase);
								if (!match1.Success)
								{
									break;
								}
								string str2 = match1.Groups[1].Value;
								if (str2 == plugin.Name)
								{
									break;
								}
								Interface.get_Oxide().LogError(string.Concat(new string[] { "Plugin filename ", plugin.ScriptName, ".cs must match the main class ", str2, " (should be ", str2, ".cs)" }), Array.Empty<object>());
								plugin.CompilerErrors = string.Concat(new string[] { "Plugin filename ", plugin.ScriptName, ".cs must match the main class ", str2, " (should be ", str2, ".cs)" });
								this.RemovePlugin(plugin);
								return;
							}
						}
					}
				}
			}
		}

		private void RemovePlugin(CompilablePlugin plugin)
		{
			Func<CompilablePlugin, bool> func = null;
			if (plugin.LastCompiledAt == new DateTime())
			{
				return;
			}
			this.queuedPlugins.Remove(plugin);
			this.plugins.Remove(plugin);
			plugin.OnCompilationFailed();
			HashSet<CompilablePlugin> compilablePlugins = this.plugins;
			Func<CompilablePlugin, bool> func1 = func;
			if (func1 == null)
			{
				Func<CompilablePlugin, bool> isCompilationNeeded = (CompilablePlugin pl) => {
					if (pl.IsCompilationNeeded)
					{
						return false;
					}
					return plugin.Requires.Contains(pl.Name);
				};
				Func<CompilablePlugin, bool> func2 = isCompilationNeeded;
				func = isCompilationNeeded;
				func1 = func2;
			}
			CompilablePlugin[] array = compilablePlugins.Where<CompilablePlugin>(func1).ToArray<CompilablePlugin>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				CompilablePlugin compilablePlugin = array[i];
				if (!this.plugins.Any<CompilablePlugin>((CompilablePlugin pl) => pl.Requires.Contains(compilablePlugin.Name)))
				{
					this.RemovePlugin(compilablePlugin);
				}
			}
		}

		private void ResolveReferences(CompilablePlugin plugin)
		{
			foreach (string reference in plugin.References)
			{
				Match match = Regex.Match(reference, "^(Oxide\\.(?:Ext|Game)\\.(.+))$", RegexOptions.IgnoreCase);
				if (!match.Success)
				{
					continue;
				}
				string value = match.Groups[1].Value;
				string str = match.Groups[2].Value;
				if (this.extensionNames.Contains<string>(str))
				{
					continue;
				}
				if (Directory.Exists(this.includePath))
				{
					string str1 = Path.Combine(this.includePath, string.Concat("Ext.", str, ".cs"));
					if (File.Exists(str1))
					{
						plugin.IncludePaths.Add(str1);
						continue;
					}
				}
				string str2 = string.Concat(new string[] { value, " is referenced by ", plugin.Name, " plugin but is not loaded! An appropriate include file needs to be saved to plugins\\include\\Ext.", str, ".cs if this extension is not required." });
				Interface.Oxide.LogError(str2, Array.Empty<object>());
				plugin.CompilerErrors = str2;
				this.RemovePlugin(plugin);
			}
		}

		internal void Started()
		{
			this.startedAt = Interface.Oxide.Now;
			this.name = string.Concat((this.plugins.Count < 2 ? this.plugins.First<CompilablePlugin>().Name : "plugins_"), Math.Round((double)(Interface.Oxide.Now * 1E+07f)), ".dll");
		}
	}
}