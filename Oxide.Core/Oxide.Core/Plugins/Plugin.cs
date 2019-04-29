using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Oxide.Core.Plugins
{
	public abstract class Plugin
	{
		private string name;

		private bool isCorePlugin;

		public PluginManagerEvent OnAddedToManager = new PluginManagerEvent();

		public PluginManagerEvent OnRemovedFromManager = new PluginManagerEvent();

		private Stopwatch trackStopwatch = new Stopwatch();

		private Stopwatch stopwatch = new Stopwatch();

		private float averageAt;

		private double sum;

		private int preHookGcCount;

		protected int nestcount;

		private IDictionary<string, Plugin.CommandInfo> commandInfos;

		private Permission permission = Interface.Oxide.GetLibrary<Permission>(null);

		public string Author
		{
			get;
			protected set;
		}

		public DynamicConfigFile Config
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			protected set;
		}

		public string Filename
		{
			get;
			protected set;
		}

		public bool HasConfig
		{
			get;
			protected set;
		}

		public bool HasMessages
		{
			get;
			protected set;
		}

		public bool IsCorePlugin
		{
			get
			{
				return this.isCorePlugin;
			}
			set
			{
				if (!Interface.Oxide.HasLoadedCorePlugins)
				{
					this.isCorePlugin = value;
				}
			}
		}

		public bool IsLoaded
		{
			get;
			internal set;
		}

		public PluginLoader Loader
		{
			get;
			set;
		}

		public PluginManager Manager
		{
			get;
			private set;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (string.IsNullOrEmpty(this.Name) || this.name == this.GetType().Name)
				{
					this.name = value;
				}
			}
		}

		public virtual object Object
		{
			get
			{
				return this;
			}
		}

		public int ResourceId
		{
			get;
			protected set;
		}

		public string Title
		{
			get;
			protected set;
		}

		public double TotalHookTime
		{
			get;
			internal set;
		}

		public VersionNumber Version
		{
			get;
			protected set;
		}

		protected Plugin()
		{
			this.Name = this.GetType().Name;
			this.Title = this.Name.Humanize();
			this.Author = "Unnamed";
			this.Version = new VersionNumber(1, 0, 0);
			this.commandInfos = new Dictionary<string, Plugin.CommandInfo>();
		}

		public void AddCovalenceCommand(string command, string callback, string perm = null)
		{
			// 
			// Current member / type: System.Void Oxide.Core.Plugins.Plugin::AddCovalenceCommand(System.String,System.String,System.String)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.Core.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void AddCovalenceCommand(System.String,System.String,System.String)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\BinaryExpression.cs:line 214
			//    at Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.set_Left(Expression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\BinaryExpression.cs:line 241
			//    at ..(BinaryExpression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:line 74
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 97
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:line 44
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 91
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public void AddCovalenceCommand(string[] commands, string callback, string perm)
		{
			// 
			// Current member / type: System.Void Oxide.Core.Plugins.Plugin::AddCovalenceCommand(System.String[],System.String,System.String)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.Core.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void AddCovalenceCommand(System.String[],System.String,System.String)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\BinaryExpression.cs:line 214
			//    at Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.set_Left(Expression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\BinaryExpression.cs:line 241
			//    at ..(BinaryExpression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:line 74
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 97
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:line 44
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 91
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public void AddCovalenceCommand(string[] commands, string callback, string[] perms = null)
		{
			this.AddCovalenceCommand(commands, perms, (IPlayer caller, string command, string[] args) => {
				this.CallHook(callback, new object[] { caller, command, args });
				return true;
			});
			Covalence library = Interface.Oxide.GetLibrary<Covalence>(null);
			string[] strArrays = commands;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				library.RegisterCommand(str, this, new CommandCallback(this.CovalenceCommandCallback));
			}
		}

		protected void AddCovalenceCommand(string[] commands, string[] perms, CommandCallback callback)
		{
			int i;
			string[] strArrays = commands;
			for (i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (!this.commandInfos.ContainsKey(str.ToLowerInvariant()))
				{
					this.commandInfos.Add(str.ToLowerInvariant(), new Plugin.CommandInfo(commands, perms, callback));
				}
				else
				{
					Interface.Oxide.LogWarning("Covalence command alias already exists: {0}", new object[] { str });
				}
			}
			if (perms == null)
			{
				return;
			}
			strArrays = perms;
			for (i = 0; i < (int)strArrays.Length; i++)
			{
				string str1 = strArrays[i];
				if (!this.permission.PermissionExists(str1, null))
				{
					this.permission.RegisterPermission(str1, this);
				}
			}
		}

		public object Call(string hook, params object[] args)
		{
			return this.CallHook(hook, args);
		}

		public T Call<T>(string hook, params object[] args)
		{
			return (T)Convert.ChangeType(this.CallHook(hook, args), typeof(T));
		}

		public object CallHook(string hook, params object[] args)
		{
			object obj;
			float now = 0f;
			if (!this.IsCorePlugin && this.nestcount == 0)
			{
				this.preHookGcCount = GC.CollectionCount(0);
				now = Interface.Oxide.Now;
				this.stopwatch.Start();
				if (this.averageAt < 1f)
				{
					this.averageAt = now;
				}
			}
			this.TrackStart();
			this.nestcount++;
			try
			{
				try
				{
					obj = this.OnCallHook(hook, args);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					Interface.Oxide.LogException(string.Format("Failed to call hook '{0}' on plugin '{1} v{2}'", hook, this.Name, this.Version), exception);
					obj = null;
				}
			}
			finally
			{
				this.nestcount--;
				this.TrackEnd();
				if (now > 0f)
				{
					this.stopwatch.Stop();
					double totalSeconds = this.stopwatch.Elapsed.TotalSeconds;
					if (totalSeconds > 0.1)
					{
						string str = (this.preHookGcCount == GC.CollectionCount(0) ? string.Empty : " [GARBAGE COLLECT]");
						Interface.Oxide.LogWarning(string.Format("Calling '{0}' on '{1} v{2}' took {3:0}ms{4}", new object[] { hook, this.Name, this.Version, totalSeconds * 1000, str }), Array.Empty<object>());
					}
					this.stopwatch.Reset();
					double num = this.sum + totalSeconds;
					double num1 = (double)now + totalSeconds;
					if (num1 - (double)this.averageAt <= 10)
					{
						this.sum = num;
					}
					else
					{
						num = num / (num1 - (double)this.averageAt);
						if (num > 0.1)
						{
							string str1 = (this.preHookGcCount == GC.CollectionCount(0) ? string.Empty : " [GARBAGE COLLECT]");
							Interface.Oxide.LogWarning(string.Format("Calling '{0}' on '{1} v{2}' took average {3:0}ms{4}", new object[] { hook, this.Name, this.Version, this.sum * 1000, str1 }), Array.Empty<object>());
						}
						this.sum = 0;
						this.averageAt = 0f;
					}
				}
			}
			return obj;
		}

		private bool CovalenceCommandCallback(IPlayer caller, string cmd, string[] args)
		{
			Plugin.CommandInfo commandInfo;
			if (!this.commandInfos.TryGetValue(cmd, out commandInfo))
			{
				return false;
			}
			if (caller == null)
			{
				Interface.Oxide.LogWarning("Plugin.CovalenceCommandCallback received null as the caller (bad game Covalence bindings?)", Array.Empty<object>());
				return false;
			}
			if (commandInfo.PermissionsRequired != null)
			{
				string[] permissionsRequired = commandInfo.PermissionsRequired;
				for (int i = 0; i < (int)permissionsRequired.Length; i++)
				{
					if (!caller.HasPermission(permissionsRequired[i]) && !caller.IsServer && (!caller.IsAdmin || !this.IsCorePlugin))
					{
						caller.Message(string.Concat("You don't have permission to use the command '", cmd, "'!"));
						return true;
					}
				}
			}
			commandInfo.Callback(caller, cmd, args);
			return true;
		}

		public virtual void HandleAddedToManager(PluginManager manager)
		{
			this.Manager = manager;
			if (this.HasConfig)
			{
				this.LoadConfig();
			}
			if (this.HasMessages)
			{
				this.LoadDefaultMessages();
			}
			PluginManagerEvent onAddedToManager = this.OnAddedToManager;
			if (onAddedToManager != null)
			{
				onAddedToManager.Invoke(this, manager);
			}
			else
			{
			}
			this.RegisterWithCovalence();
		}

		public virtual void HandleRemovedFromManager(PluginManager manager)
		{
			this.UnregisterWithCovalence();
			if (this.Manager == manager)
			{
				this.Manager = null;
			}
			PluginManagerEvent onRemovedFromManager = this.OnRemovedFromManager;
			if (onRemovedFromManager == null)
			{
				return;
			}
			onRemovedFromManager.Invoke(this, manager);
		}

		public virtual void Load()
		{
		}

		protected virtual void LoadConfig()
		{
			this.Config = new DynamicConfigFile(Path.Combine(this.Manager.ConfigPath, string.Concat(this.Name, ".json")));
			if (!this.Config.Exists(null))
			{
				this.LoadDefaultConfig();
				this.SaveConfig();
			}
			try
			{
				this.Config.Load(null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.RaiseError(string.Concat("Failed to load config file (is the config file corrupt?) (", exception.Message, ")"));
			}
		}

		protected virtual void LoadDefaultConfig()
		{
			this.CallHook("LoadDefaultConfig", null);
		}

		protected virtual void LoadDefaultMessages()
		{
			this.CallHook("LoadDefaultMessages", null);
		}

		protected abstract object OnCallHook(string hook, object[] args);

		public static implicit operator Boolean(Plugin plugin)
		{
			return plugin != null;
		}

		public static bool operator !(Plugin plugin)
		{
			return !plugin;
		}

		public void RaiseError(string message)
		{
			PluginError pluginError = this.OnError;
			if (pluginError == null)
			{
				return;
			}
			pluginError(this, message);
		}

		private void RegisterWithCovalence()
		{
			Covalence library = Interface.Oxide.GetLibrary<Covalence>(null);
			foreach (KeyValuePair<string, Plugin.CommandInfo> commandInfo in this.commandInfos)
			{
				library.RegisterCommand(commandInfo.Key, this, new CommandCallback(this.CovalenceCommandCallback));
			}
		}

		protected virtual void SaveConfig()
		{
			if (this.Config == null)
			{
				return;
			}
			try
			{
				this.Config.Save(null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.RaiseError(string.Concat("Failed to save config file (does the config have illegal objects in it?) (", exception.Message, ")"));
			}
		}

		protected void Subscribe(string hook)
		{
			this.Manager.SubscribeToHook(hook, this);
		}

		public void TrackEnd()
		{
			if (this.IsCorePlugin || this.nestcount > 0)
			{
				return;
			}
			Stopwatch stopwatch = this.trackStopwatch;
			if (!stopwatch.IsRunning)
			{
				return;
			}
			stopwatch.Stop();
			this.TotalHookTime = this.TotalHookTime + stopwatch.Elapsed.TotalSeconds;
			stopwatch.Reset();
		}

		public void TrackStart()
		{
			if (this.IsCorePlugin || this.nestcount > 0)
			{
				return;
			}
			Stopwatch stopwatch = this.trackStopwatch;
			if (stopwatch.IsRunning)
			{
				return;
			}
			stopwatch.Start();
		}

		private void UnregisterWithCovalence()
		{
			Covalence library = Interface.Oxide.GetLibrary<Covalence>(null);
			foreach (KeyValuePair<string, Plugin.CommandInfo> commandInfo in this.commandInfos)
			{
				library.UnregisterCommand(commandInfo.Key, this);
			}
		}

		protected void Unsubscribe(string hook)
		{
			this.Manager.UnsubscribeToHook(hook, this);
		}

		public event PluginError OnError;

		private class CommandInfo
		{
			public readonly string[] Names;

			public readonly string[] PermissionsRequired;

			public readonly CommandCallback Callback;

			public CommandInfo(string[] names, string[] perms, CommandCallback callback)
			{
				this.Names = names;
				this.PermissionsRequired = perms;
				this.Callback = callback;
			}
		}
	}
}