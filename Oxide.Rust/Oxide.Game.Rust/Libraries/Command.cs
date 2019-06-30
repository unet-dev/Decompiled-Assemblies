using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Game.Rust;
using Oxide.Game.Rust.Libraries.Covalence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Libraries
{
	public class Command : Library
	{
		internal readonly Dictionary<string, Command.ConsoleCommand> consoleCommands;

		internal readonly Dictionary<string, Command.ChatCommand> chatCommands;

		private readonly Dictionary<Plugin, Event.Callback<Plugin, PluginManager>> pluginRemovedFromManager;

		public Command()
		{
			this.consoleCommands = new Dictionary<string, Command.ConsoleCommand>();
			this.chatCommands = new Dictionary<string, Command.ChatCommand>();
			this.pluginRemovedFromManager = new Dictionary<Plugin, Event.Callback<Plugin, PluginManager>>();
		}

		[LibraryFunction("AddChatCommand")]
		public void AddChatCommand(string name, Plugin plugin, string callback)
		{
			this.AddChatCommand(name, plugin, (BasePlayer player, string command, string[] args) => plugin.CallHook(callback, new Object[] { player, command, args }));
		}

		public void AddChatCommand(string command, Plugin plugin, Action<BasePlayer, string, string[]> callback)
		{
			Command.ChatCommand chatCommand;
			RustCommandSystem.RegisteredCommand registeredCommand;
			object name;
			object obj;
			object name1;
			object obj1;
			object name2;
			string lowerInvariant = command.ToLowerInvariant();
			if (!this.CanOverrideCommand(command, "chat"))
			{
				if (plugin != null)
				{
					name2 = plugin.Name;
				}
				else
				{
					name2 = null;
				}
				if (name2 == null)
				{
					name2 = "An unknown plugin";
				}
				string str = name2;
				Interface.Oxide.LogError("{0} tried to register command '{1}', this command already exists and cannot be overridden!", new Object[] { str, lowerInvariant });
				return;
			}
			if (this.chatCommands.TryGetValue(lowerInvariant, out chatCommand))
			{
				Plugin plugin1 = chatCommand.Plugin;
				if (plugin1 != null)
				{
					name1 = plugin1.Name;
				}
				else
				{
					name1 = null;
				}
				if (name1 == null)
				{
					name1 = "an unknown plugin";
				}
				string str1 = name1;
				if (plugin != null)
				{
					obj1 = plugin.Name;
				}
				else
				{
					obj1 = null;
				}
				if (obj1 == null)
				{
					obj1 = "An unknown plugin";
				}
				string str2 = obj1;
				string str3 = String.Concat(new String[] { str2, " has replaced the '", lowerInvariant, "' chat command previously registered by ", str1 });
				Interface.Oxide.LogWarning(str3, Array.Empty<object>());
			}
			if (RustCore.Covalence.CommandSystem.registeredCommands.TryGetValue(lowerInvariant, out registeredCommand))
			{
				Plugin source = registeredCommand.Source;
				if (source != null)
				{
					name = source.Name;
				}
				else
				{
					name = null;
				}
				if (name == null)
				{
					name = "an unknown plugin";
				}
				string str4 = name;
				if (plugin != null)
				{
					obj = plugin.Name;
				}
				else
				{
					obj = null;
				}
				if (obj == null)
				{
					obj = "An unknown plugin";
				}
				string str5 = obj;
				string str6 = String.Concat(new String[] { str5, " has replaced the '", lowerInvariant, "' command previously registered by ", str4 });
				Interface.Oxide.LogWarning(str6, Array.Empty<object>());
				RustCore.Covalence.CommandSystem.UnregisterCommand(lowerInvariant, registeredCommand.Source);
			}
			chatCommand = new Command.ChatCommand(lowerInvariant, plugin, callback);
			this.chatCommands[lowerInvariant] = chatCommand;
			if (plugin != null && !this.pluginRemovedFromManager.ContainsKey(plugin))
			{
				this.pluginRemovedFromManager[plugin] = plugin.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.plugin_OnRemovedFromManager));
			}
		}

		[LibraryFunction("AddConsoleCommand")]
		public void AddConsoleCommand(string command, Plugin plugin, string callback)
		{
			this.AddConsoleCommand(command, plugin, (ConsoleSystem.Arg arg) => (object)plugin.CallHook(callback, new Object[] { arg }) != (object)null);
		}

		public void AddConsoleCommand(string command, Plugin plugin, Func<ConsoleSystem.Arg, bool> callback)
		{
			Command.ConsoleCommand consoleCommand;
			RustCommandSystem.RegisteredCommand registeredCommand;
			ConsoleSystem.Command command1;
			object name;
			object obj;
			object name1;
			object obj1;
			object name2;
			object obj2;
			if (plugin != null && !this.pluginRemovedFromManager.ContainsKey(plugin))
			{
				this.pluginRemovedFromManager[plugin] = plugin.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.plugin_OnRemovedFromManager));
			}
			string[] strArray = command.Split(new Char[] { '.' });
			string str = ((int)strArray.Length >= 2 ? strArray[0].Trim() : "global");
			string rustCommand = ((int)strArray.Length >= 2 ? String.Join(".", strArray.Skip<string>(1).ToArray<string>()) : strArray[0].Trim());
			string rustCommand1 = String.Concat(str, ".", rustCommand);
			Command.ConsoleCommand originalCallback = new Command.ConsoleCommand(rustCommand1);
			if (!this.CanOverrideCommand((str == "global" ? rustCommand : rustCommand1), "console"))
			{
				if (plugin != null)
				{
					obj2 = plugin.Name;
				}
				else
				{
					obj2 = null;
				}
				if (obj2 == null)
				{
					obj2 = "An unknown plugin";
				}
				string str1 = obj2;
				Interface.Oxide.LogError("{0} tried to register command '{1}', this command already exists and cannot be overridden!", new Object[] { str1, rustCommand1 });
				return;
			}
			if (this.consoleCommands.TryGetValue(rustCommand1, out consoleCommand))
			{
				if (consoleCommand.OriginalCallback != null)
				{
					originalCallback.OriginalCallback = consoleCommand.OriginalCallback;
				}
				Plugin plugin1 = consoleCommand.Callback.Plugin;
				if (plugin1 != null)
				{
					obj1 = plugin1.Name;
				}
				else
				{
					obj1 = null;
				}
				if (obj1 == null)
				{
					obj1 = "an unknown plugin";
				}
				string str2 = obj1;
				if (plugin != null)
				{
					name2 = plugin.Name;
				}
				else
				{
					name2 = null;
				}
				if (name2 == null)
				{
					name2 = "An unknown plugin";
				}
				string str3 = name2;
				string str4 = String.Concat(new String[] { str3, " has replaced the '", command, "' console command previously registered by ", str2 });
				Interface.Oxide.LogWarning(str4, Array.Empty<object>());
				ConsoleSystem.Index.Server.Dict.Remove(consoleCommand.RustCommand.FullName);
				if (str == "global")
				{
					ConsoleSystem.Index.Server.GlobalDict.Remove(consoleCommand.RustCommand.Name);
				}
				ConsoleSystem.Index.All = ConsoleSystem.Index.Server.Dict.Values.ToArray<ConsoleSystem.Command>();
			}
			if (RustCore.Covalence.CommandSystem.registeredCommands.TryGetValue((str == "global" ? rustCommand : rustCommand1), out registeredCommand))
			{
				if (registeredCommand.OriginalCallback != null)
				{
					originalCallback.OriginalCallback = registeredCommand.OriginalCallback;
				}
				Plugin source = registeredCommand.Source;
				if (source != null)
				{
					obj = source.Name;
				}
				else
				{
					obj = null;
				}
				if (obj == null)
				{
					obj = "an unknown plugin";
				}
				string str5 = obj;
				if (plugin != null)
				{
					name1 = plugin.Name;
				}
				else
				{
					name1 = null;
				}
				if (name1 == null)
				{
					name1 = "An unknown plugin";
				}
				string str6 = name1;
				string str7 = String.Concat(new String[] { str6, " has replaced the '", rustCommand1, "' command previously registered by ", str5 });
				Interface.Oxide.LogWarning(str7, Array.Empty<object>());
				RustCore.Covalence.CommandSystem.UnregisterCommand((str == "global" ? rustCommand : rustCommand1), registeredCommand.Source);
			}
			originalCallback.AddCallback(plugin, callback);
			if (ConsoleSystem.Index.Server.Dict.TryGetValue(rustCommand1, out command1))
			{
				if (command1.Variable)
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
					string str8 = name;
					Interface.Oxide.LogError(String.Concat(str8, " tried to register the ", rustCommand, " console variable as a command!"), Array.Empty<object>());
					return;
				}
				originalCallback.OriginalCallback = command1.Call;
			}
			ConsoleSystem.Index.Server.Dict[rustCommand1] = originalCallback.RustCommand;
			if (str == "global")
			{
				ConsoleSystem.Index.Server.GlobalDict[rustCommand] = originalCallback.RustCommand;
			}
			ConsoleSystem.Index.All = ConsoleSystem.Index.Server.Dict.Values.ToArray<ConsoleSystem.Command>();
			this.consoleCommands[rustCommand1] = originalCallback;
		}

		private bool CanOverrideCommand(string command, string type)
		{
			RustCommandSystem.RegisteredCommand registeredCommand;
			Command.ChatCommand chatCommand;
			Command.ConsoleCommand consoleCommand;
			string[] strArray = command.Split(new Char[] { '.' });
			string str = ((int)strArray.Length >= 2 ? strArray[0].Trim() : "global");
			string str1 = ((int)strArray.Length >= 2 ? String.Join(".", strArray.Skip<string>(1).ToArray<string>()) : strArray[0].Trim());
			string str2 = String.Concat(str, ".", str1);
			if (RustCore.Covalence.CommandSystem.registeredCommands.TryGetValue(command, out registeredCommand) && registeredCommand.Source.IsCorePlugin)
			{
				return false;
			}
			if (type == "chat")
			{
				if (this.chatCommands.TryGetValue(command, out chatCommand) && chatCommand.Plugin.IsCorePlugin)
				{
					return false;
				}
			}
			else if (type == "console")
			{
				if (this.consoleCommands.TryGetValue((str == "global" ? str1 : str2), out consoleCommand) && consoleCommand.Callback.Plugin.IsCorePlugin)
				{
					return false;
				}
			}
			if (RustCore.RestrictedCommands.Contains<string>(command))
			{
				return false;
			}
			return !RustCore.RestrictedCommands.Contains<string>(str2);
		}

		internal bool HandleChatCommand(BasePlayer sender, string name, string[] args)
		{
			Command.ChatCommand chatCommand;
			if (!this.chatCommands.TryGetValue(name.ToLowerInvariant(), out chatCommand))
			{
				return false;
			}
			chatCommand.HandleCommand(sender, name, args);
			return true;
		}

		private void plugin_OnRemovedFromManager(Plugin sender, PluginManager manager)
		{
			Event.Callback<Plugin, PluginManager> callback;
			int i;
			Func<Command.ChatCommand, bool> func = null;
			Command.ConsoleCommand[] array = (
				from c in this.consoleCommands.Values
				where c.Callback.Plugin == sender
				select c).ToArray<Command.ConsoleCommand>();
			for (i = 0; i < (int)array.Length; i++)
			{
				this.RemoveConsoleCommand(array[i]);
			}
			Dictionary<!0, !1>.ValueCollection values = this.chatCommands.Values;
			Func<Command.ChatCommand, bool> func1 = func;
			if (func1 == null)
			{
				Func<Command.ChatCommand, bool> plugin = (Command.ChatCommand c) => c.Plugin == sender;
				Func<Command.ChatCommand, bool> func2 = plugin;
				func = plugin;
				func1 = func2;
			}
			Command.ChatCommand[] chatCommandArray = values.Where<Command.ChatCommand>(func1).ToArray<Command.ChatCommand>();
			for (i = 0; i < (int)chatCommandArray.Length; i++)
			{
				this.RemoveChatCommand(chatCommandArray[i]);
			}
			if (this.pluginRemovedFromManager.TryGetValue(sender, out callback))
			{
				callback.Remove();
				this.pluginRemovedFromManager.Remove(sender);
			}
		}

		[LibraryFunction("RemoveChatCommand")]
		public void RemoveChatCommand(string command, Plugin plugin)
		{
			Command.ChatCommand chatCommand = (
				from x in this.chatCommands.Values
				where x.Plugin == plugin
				select x).FirstOrDefault<Command.ChatCommand>((Command.ChatCommand x) => x.Name == command);
			if (chatCommand != null)
			{
				this.RemoveChatCommand(chatCommand);
			}
		}

		private void RemoveChatCommand(Command.ChatCommand command)
		{
			if (this.chatCommands.ContainsKey(command.Name))
			{
				this.chatCommands.Remove(command.Name);
			}
		}

		[LibraryFunction("RemoveConsoleCommand")]
		public void RemoveConsoleCommand(string command, Plugin plugin)
		{
			Command.ConsoleCommand consoleCommand = (
				from x in this.consoleCommands.Values
				where x.Callback.Plugin == plugin
				select x).FirstOrDefault<Command.ConsoleCommand>((Command.ConsoleCommand x) => x.Name == command);
			if (consoleCommand != null)
			{
				this.RemoveConsoleCommand(consoleCommand);
			}
		}

		private void RemoveConsoleCommand(Command.ConsoleCommand command)
		{
			if (this.consoleCommands.ContainsKey(command.Name))
			{
				this.consoleCommands.Remove(command.Name);
				if (command.OriginalCallback == null)
				{
					ConsoleSystem.Index.Server.Dict.Remove(command.RustCommand.FullName);
					if (command.Name.StartsWith("global."))
					{
						ConsoleSystem.Index.Server.GlobalDict.Remove(command.RustCommand.Name);
					}
					ConsoleSystem.Index.All = ConsoleSystem.Index.Server.Dict.Values.ToArray<ConsoleSystem.Command>();
				}
				else
				{
					ConsoleSystem.Index.Server.Dict[command.RustCommand.FullName].Call = command.OriginalCallback;
					if (command.RustCommand.FullName.StartsWith("global."))
					{
						ConsoleSystem.Index.Server.GlobalDict[command.RustCommand.Name].Call = command.OriginalCallback;
						return;
					}
				}
			}
		}

		internal class ChatCommand
		{
			public readonly string Name;

			public readonly Plugin Plugin;

			private readonly Action<BasePlayer, string, string[]> _callback;

			public ChatCommand(string name, Plugin plugin, Action<BasePlayer, string, string[]> callback)
			{
				this.Name = name;
				this.Plugin = plugin;
				this._callback = callback;
			}

			public void HandleCommand(BasePlayer sender, string name, string[] args)
			{
				Plugin plugin = this.Plugin;
				if (plugin != null)
				{
					plugin.TrackStart();
				}
				else
				{
				}
				Action<BasePlayer, string, string[]> action = this._callback;
				if (action != null)
				{
					action(sender, name, args);
				}
				else
				{
				}
				Plugin plugin1 = this.Plugin;
				if (plugin1 == null)
				{
					return;
				}
				plugin1.TrackEnd();
			}
		}

		internal class ConsoleCommand
		{
			public readonly string Name;

			public Command.PluginCallback Callback;

			public readonly ConsoleSystem.Command RustCommand;

			public Action<ConsoleSystem.Arg> OriginalCallback;

			internal readonly Permission permission;

			public ConsoleCommand(string name)
			{
				this.Name = name;
				string[] strArray = this.Name.Split(new Char[] { '.' });
				this.RustCommand = new ConsoleSystem.Command()
				{
					Name = strArray[1],
					Parent = strArray[0],
					FullName = name,
					ServerUser = true,
					ServerAdmin = true,
					Client = true,
					ClientInfo = false,
					Variable = false,
					Call = new Action<ConsoleSystem.Arg>(this.HandleCommand)
				};
			}

			public void AddCallback(Plugin plugin, string name)
			{
				this.Callback = new Command.PluginCallback(plugin, name);
			}

			public void AddCallback(Plugin plugin, Func<ConsoleSystem.Arg, bool> callback)
			{
				this.Callback = new Command.PluginCallback(plugin, callback);
			}

			public void HandleCommand(ConsoleSystem.Arg arg)
			{
				Plugin plugin = this.Callback.Plugin;
				if (plugin != null)
				{
					plugin.TrackStart();
				}
				else
				{
				}
				this.Callback.Call(arg);
				Plugin plugin1 = this.Callback.Plugin;
				if (plugin1 == null)
				{
					return;
				}
				plugin1.TrackEnd();
			}
		}

		internal struct PluginCallback
		{
			public readonly Plugin Plugin;

			public readonly string Name;

			public Func<ConsoleSystem.Arg, bool> Call;

			public PluginCallback(Plugin plugin, string name)
			{
				this.Plugin = plugin;
				this.Name = name;
				this.Call = null;
			}

			public PluginCallback(Plugin plugin, Func<ConsoleSystem.Arg, bool> callback)
			{
				this.Plugin = plugin;
				this.Call = callback;
				this.Name = null;
			}
		}
	}
}