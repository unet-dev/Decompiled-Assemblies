using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Logging;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Oxide.Core.Libraries.Covalence
{
	public class Covalence : Library
	{
		private ICommandSystem cmdSystem;

		private ICovalenceProvider provider;

		private readonly Logger logger;

		[LibraryProperty("ClientAppId")]
		public uint ClientAppId
		{
			get
			{
				ICovalenceProvider covalenceProvider = this.provider;
				if (covalenceProvider != null)
				{
					return covalenceProvider.ClientAppId;
				}
				return (uint)0;
			}
		}

		[LibraryProperty("Game")]
		public string Game
		{
			get
			{
				object gameName;
				ICovalenceProvider covalenceProvider = this.provider;
				if (covalenceProvider != null)
				{
					gameName = covalenceProvider.GameName;
				}
				else
				{
					gameName = null;
				}
				if (gameName == null)
				{
					gameName = string.Empty;
				}
				return gameName;
			}
		}

		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		[LibraryProperty("Players")]
		public IPlayerManager Players
		{
			get;
			private set;
		}

		[LibraryProperty("Server")]
		public IServer Server
		{
			get;
			private set;
		}

		[LibraryProperty("ServerAppId")]
		public uint ServerAppId
		{
			get
			{
				ICovalenceProvider covalenceProvider = this.provider;
				if (covalenceProvider != null)
				{
					return covalenceProvider.ServerAppId;
				}
				return (uint)0;
			}
		}

		public Covalence()
		{
			this.logger = Interface.Oxide.RootLogger;
		}

		public string FormatText(string text)
		{
			return this.provider.FormatText(text);
		}

		internal void Initialize()
		{
			Type item;
			object obj;
			TypeFilter typeFilter2 = null;
			Type type1 = typeof(ICovalenceProvider);
			IEnumerable<Type> types = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < (int)assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				Type[] typeArray = null;
				try
				{
					typeArray = assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException reflectionTypeLoadException)
				{
					typeArray = reflectionTypeLoadException.Types;
				}
				catch (TypeLoadException typeLoadException1)
				{
					TypeLoadException typeLoadException = typeLoadException1;
					this.logger.Write(LogType.Warning, "Covalence: Type {0} could not be loaded from assembly '{1}': {2}", new object[] { typeLoadException.TypeName, assembly.FullName, typeLoadException });
				}
				if (typeArray != null)
				{
					if (types != null)
					{
						obj = types.Concat<Type>(typeArray);
					}
					else
					{
						obj = null;
					}
					if (obj == null)
					{
						obj = typeArray;
					}
					types = (IEnumerable<Type>)obj;
				}
			}
			if (types == null)
			{
				this.logger.Write(LogType.Warning, "Covalence not available yet for this game", Array.Empty<object>());
				return;
			}
			List<Type> types1 = new List<Type>(types.Where<Type>((Type t) => {
				if (!(t != null) || !t.IsClass || t.IsAbstract)
				{
					return false;
				}
				Type type = t;
				TypeFilter u003cu003e9_1 = typeFilter2;
				if (u003cu003e9_1 == null)
				{
					TypeFilter typeFilter = (Type m, object o) => m == type1;
					TypeFilter typeFilter1 = typeFilter;
					typeFilter2 = typeFilter;
					u003cu003e9_1 = typeFilter1;
				}
				return (int)type.FindInterfaces(u003cu003e9_1, null).Length == 1;
			}));
			if (types1.Count == 0)
			{
				this.logger.Write(LogType.Warning, "Covalence not available yet for this game", Array.Empty<object>());
				return;
			}
			if (types1.Count <= 1)
			{
				item = types1[0];
			}
			else
			{
				item = types1[0];
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 1; j < types1.Count; j++)
				{
					if (j > 1)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(types1[j].FullName);
				}
				this.logger.Write(LogType.Warning, "Multiple Covalence providers found! Using {0}. (Also found {1})", new object[] { item, stringBuilder });
			}
			try
			{
				this.provider = (ICovalenceProvider)Activator.CreateInstance(item);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.logger.Write(LogType.Warning, "Got exception when instantiating Covalence provider, Covalence will not be functional for this session.", Array.Empty<object>());
				this.logger.Write(LogType.Warning, "{0}", new object[] { exception });
				return;
			}
			this.Server = this.provider.CreateServer();
			this.Players = this.provider.CreatePlayerManager();
			this.cmdSystem = this.provider.CreateCommandSystemProvider();
			this.logger.Write(LogType.Info, "Using Covalence provider for game '{0}'", new object[] { this.provider.GameName });
		}

		public void RegisterCommand(string command, Plugin plugin, CommandCallback callback)
		{
			object name;
			if (this.cmdSystem == null)
			{
				return;
			}
			try
			{
				this.cmdSystem.RegisterCommand(command, plugin, callback);
			}
			catch (CommandAlreadyExistsException commandAlreadyExistsException)
			{
				if (plugin != null)
				{
					name = plugin.Name;
				}
				else
				{
					name = null;
				}
				if (name == null)
				{
					name = "An unknown plugin";
				}
				string str = (string)name;
				this.logger.Write(LogType.Error, "{0} tried to register command '{1}', this command already exists and cannot be overridden!", new object[] { str, command });
			}
		}

		public void UnregisterCommand(string command, Plugin plugin)
		{
			ICommandSystem commandSystem = this.cmdSystem;
			if (commandSystem == null)
			{
				return;
			}
			commandSystem.UnregisterCommand(command, plugin);
		}
	}
}