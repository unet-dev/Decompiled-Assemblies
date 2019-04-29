using Oxide.Core;
using Oxide.Core.Extensions;
using Oxide.Core.Plugins;
using Oxide.Core.Plugins.Watchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins
{
	public class CSharpExtension : Extension
	{
		internal static System.Reflection.Assembly Assembly;

		internal static System.Reflection.AssemblyName AssemblyName;

		internal static VersionNumber AssemblyVersion;

		internal static string AssemblyAuthors;

		private CSharpPluginLoader loader;

		public override string Author
		{
			get
			{
				return CSharpExtension.AssemblyAuthors;
			}
		}

		public override bool IsCoreExtension
		{
			get
			{
				return true;
			}
		}

		public override string Name
		{
			get
			{
				return "CSharp";
			}
		}

		public override VersionNumber Version
		{
			get
			{
				return CSharpExtension.AssemblyVersion;
			}
		}

		public FSWatcher Watcher
		{
			get;
			private set;
		}

		static CSharpExtension()
		{
			CSharpExtension.Assembly = System.Reflection.Assembly.GetExecutingAssembly();
			CSharpExtension.AssemblyName = CSharpExtension.Assembly.GetName();
			CSharpExtension.AssemblyVersion = new VersionNumber(CSharpExtension.AssemblyName.Version.Major, CSharpExtension.AssemblyName.Version.Minor, CSharpExtension.AssemblyName.Version.Build);
			CSharpExtension.AssemblyAuthors = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(CSharpExtension.Assembly, typeof(AssemblyCompanyAttribute), false)).Company;
		}

		public CSharpExtension(ExtensionManager manager) : base(manager)
		{
			string str = Path.Combine(Interface.Oxide.RootDirectory, "CSharpCompiler");
			string str1 = Path.Combine(Interface.Oxide.RootDirectory, "Compiler");
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				str = string.Concat(str, ".exe");
				if (File.Exists(str))
				{
					str1 = string.Concat(str1, ".exe");
					if (!File.Exists(str1))
					{
						File.Move(str, str1);
					}
					Cleanup.Add(str);
				}
				return;
			}
			Cleanup.Add(Path.Combine(Interface.Oxide.ExtensionDirectory, "Mono.Posix.dll.config"));
			str = string.Concat(str, ".x86");
			if (File.Exists(str))
			{
				str1 = string.Concat(str1, ".x86");
				if (!File.Exists(str1))
				{
					File.Move(str, str1);
				}
				Cleanup.Add(str);
			}
			str = string.Concat(str, "_x64");
			if (File.Exists(str))
			{
				str1 = string.Concat(str1, "_x64");
				if (!File.Exists(str1))
				{
					File.Move(str, str1);
				}
				Cleanup.Add(str);
			}
			string extensionDirectory = Interface.Oxide.ExtensionDirectory;
			string str2 = Path.Combine(extensionDirectory, "Oxide.References.dll.config");
			if (File.Exists(str2))
			{
				if (!(new string[] { "target=\"x64", "target=\"./x64" }).Any<string>(new Func<string, bool>(File.ReadAllText(str2).Contains)))
				{
					return;
				}
			}
			File.WriteAllText(str2, string.Concat(new string[] { "<configuration>\n<dllmap dll=\"MonoPosixHelper\" target=\"", extensionDirectory, "/x86/libMonoPosixHelper.so\" os=\"!windows,osx\" wordsize=\"32\" />\n<dllmap dll=\"MonoPosixHelper\" target=\"", extensionDirectory, "/x64/libMonoPosixHelper.so\" os=\"!windows,osx\" wordsize=\"64\" />\n</configuration>" }));
		}

		public override void Load()
		{
			this.loader = new CSharpPluginLoader(this);
			base.Manager.RegisterPluginLoader(this.loader);
			Interface.Oxide.OnFrame(new Action<float>(this.OnFrame));
		}

		public override void LoadPluginWatchers(string pluginDirectory)
		{
			this.Watcher = new FSWatcher(pluginDirectory, "*.cs");
			base.Manager.RegisterPluginChangeWatcher(this.Watcher);
		}

		private void OnFrame(float delta)
		{
			object[] objArray = new object[] { delta };
			foreach (KeyValuePair<string, Plugin> loadedPlugin in this.loader.LoadedPlugins)
			{
				CSharpPlugin value = loadedPlugin.Value as CSharpPlugin;
				if (value == null || !value.HookedOnFrame)
				{
					continue;
				}
				value.CallHook("OnFrame", objArray);
			}
		}

		public override void OnModLoad()
		{
			this.loader.OnModLoaded();
		}

		public override void OnShutdown()
		{
			base.OnShutdown();
			this.loader.OnShutdown();
		}
	}
}