using Oxide.Core;
using Oxide.Core.Plugins;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	public class Plugins : Library
	{
		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		public Oxide.Core.Plugins.PluginManager PluginManager
		{
			get;
			private set;
		}

		public Plugins(Oxide.Core.Plugins.PluginManager pluginmanager)
		{
			this.PluginManager = pluginmanager;
		}

		[LibraryFunction("CallHook")]
		public object CallHook(string hookname, params object[] args)
		{
			return Interface.Call(hookname, args);
		}

		[LibraryFunction("Exists")]
		public bool Exists(string name)
		{
			return this.PluginManager.GetPlugin(name) != null;
		}

		[LibraryFunction("Find")]
		public Plugin Find(string name)
		{
			return this.PluginManager.GetPlugin(name);
		}

		[LibraryFunction("GetAll")]
		public Plugin[] GetAll()
		{
			return this.PluginManager.GetPlugins().ToArray<Plugin>();
		}
	}
}