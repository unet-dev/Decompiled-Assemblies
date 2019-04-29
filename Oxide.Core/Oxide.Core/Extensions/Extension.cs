using Oxide.Core;
using System;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Extensions
{
	public abstract class Extension
	{
		public abstract string Author
		{
			get;
		}

		public virtual string Branch { get; } = "public";

		public virtual string[] DefaultReferences { get; protected set; } = new string[0];

		public string Filename
		{
			get;
			set;
		}

		public virtual bool IsCoreExtension
		{
			get;
		}

		public virtual bool IsGameExtension
		{
			get;
		}

		public ExtensionManager Manager
		{
			get;
		}

		public abstract string Name
		{
			get;
		}

		public virtual bool SupportsReloading
		{
			get;
		}

		public abstract VersionNumber Version
		{
			get;
		}

		public virtual string[] WhitelistAssemblies { get; protected set; } = new string[0];

		public virtual string[] WhitelistNamespaces { get; protected set; } = new string[0];

		public Extension(ExtensionManager manager)
		{
			this.Manager = manager;
		}

		public virtual void Load()
		{
		}

		public virtual void LoadPluginWatchers(string pluginDirectory)
		{
		}

		public virtual void OnModLoad()
		{
		}

		public virtual void OnShutdown()
		{
		}

		public virtual void Unload()
		{
		}
	}
}