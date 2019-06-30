using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust;
using Oxide.Game.Rust.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Game.Rust.Libraries.Covalence
{
	public class RustCommandSystem : ICommandSystem
	{
		private readonly RustCovalenceProvider rustCovalence = RustCovalenceProvider.Instance;

		private readonly Command cmdlib = Interface.Oxide.GetLibrary<Command>(null);

		private readonly RustConsolePlayer consolePlayer;

		private readonly CommandHandler commandHandler;

		internal IDictionary<string, RustCommandSystem.RegisteredCommand> registeredCommands;

		public RustCommandSystem()
		{
			this.registeredCommands = new Dictionary<string, RustCommandSystem.RegisteredCommand>();
			CommandCallback commandCallback = new CommandCallback(this.CommandCallback);
			IDictionary<string, RustCommandSystem.RegisteredCommand> strs = this.registeredCommands;
			this.commandHandler = new CommandHandler(commandCallback, new Func<string, bool>(strs.ContainsKey));
			this.consolePlayer = new RustConsolePlayer();
		}

		private bool CanOverrideCommand(string command)
		{
			RustCommandSystem.RegisteredCommand registeredCommand;
			Command.ChatCommand chatCommand;
			Command.ConsoleCommand consoleCommand;
			string[] strArray = command.Split(new Char[] { '.' });
			string str = String.Concat(((int)strArray.Length >= 2 ? strArray[0].Trim() : "global"), ".", ((int)strArray.Length >= 2 ? String.Join(".", strArray.Skip<string>(1).ToArray<string>()) : strArray[0].Trim()));
			if (this.registeredCommands.TryGetValue(command, out registeredCommand) && registeredCommand.Source.IsCorePlugin)
			{
				return false;
			}
			if (this.cmdlib.chatCommands.TryGetValue(command, out chatCommand) && chatCommand.Plugin.IsCorePlugin)
			{
				return false;
			}
			if (this.cmdlib.consoleCommands.TryGetValue(str, out consoleCommand) && consoleCommand.Callback.Plugin.IsCorePlugin)
			{
				return false;
			}
			if (RustCore.RestrictedCommands.Contains<string>(command))
			{
				return false;
			}
			return !RustCore.RestrictedCommands.Contains<string>(str);
		}

		private bool CommandCallback(IPlayer caller, string cmd, string[] args)
		{
			RustCommandSystem.RegisteredCommand registeredCommand;
			if (!this.registeredCommands.TryGetValue(cmd, out registeredCommand))
			{
				return false;
			}
			return registeredCommand.Callback(caller, cmd, args);
		}

		private static string[] ExtractArgs(ConsoleSystem.Arg arg)
		{
			if (arg == null)
			{
				return new String[0];
			}
			List<string> strs = new List<string>();
			int num = 0;
			while (true)
			{
				int num1 = num + 1;
				num = num1;
				if (!arg.HasArgs(num1))
				{
					break;
				}
				strs.Add(arg.GetString(num - 1, ""));
			}
			return strs.ToArray();
		}

		public bool HandleChatMessage(IPlayer player, string message)
		{
			return this.commandHandler.HandleChatMessage(player, message);
		}

		public void RegisterCommand(string command, Plugin plugin, CommandCallback callback)
		{
			RustCommandSystem.RegisteredCommand registeredCommand;
			Command.ChatCommand chatCommand;
			Command.ConsoleCommand consoleCommand;
			ConsoleSystem.Command command1;
			object name;
			object obj;
			object name1;
			object obj1;
			object name2;
			object obj2;
			object name3;
			string str = command;
			str = str.ToLowerInvariant().Trim();
			string[] strArray = str.Split(new Char[] { '.' });
			string str1 = ((int)strArray.Length >= 2 ? strArray[0].Trim() : "global");
			string rustCommand = ((int)strArray.Length >= 2 ? String.Join(".", strArray.Skip<string>(1).ToArray<string>()) : strArray[0].Trim());
			string rustCommand1 = String.Concat(str1, ".", rustCommand);
			if (str1 == "global")
			{
				str = rustCommand;
			}
			RustCommandSystem.RegisteredCommand originalCallback = new RustCommandSystem.RegisteredCommand(plugin, str, callback);
			if (!this.CanOverrideCommand(str))
			{
				throw new CommandAlreadyExistsException(str);
			}
			if (this.registeredCommands.TryGetValue(str, out registeredCommand))
			{
				if (registeredCommand.OriginalCallback != null)
				{
					originalCallback.OriginalCallback = registeredCommand.OriginalCallback;
				}
				Plugin source = registeredCommand.Source;
				if (source != null)
				{
					obj2 = source.Name;
				}
				else
				{
					obj2 = null;
				}
				if (obj2 == null)
				{
					obj2 = "an unknown plugin";
				}
				string str2 = obj2;
				if (plugin != null)
				{
					name3 = plugin.Name;
				}
				else
				{
					name3 = null;
				}
				if (name3 == null)
				{
					name3 = "An unknown plugin";
				}
				string str3 = name3;
				string str4 = String.Concat(new String[] { str3, " has replaced the '", str, "' command previously registered by ", str2 });
				Interface.Oxide.LogWarning(str4, Array.Empty<object>());
				ConsoleSystem.Index.Server.Dict.Remove(rustCommand1);
				if (str1 == "global")
				{
					ConsoleSystem.Index.Server.GlobalDict.Remove(rustCommand);
				}
				ConsoleSystem.Index.All = ConsoleSystem.Index.Server.Dict.Values.ToArray<ConsoleSystem.Command>();
			}
			if (this.cmdlib.chatCommands.TryGetValue(str, out chatCommand))
			{
				Plugin plugin1 = chatCommand.Plugin;
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
				string str5 = obj1;
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
				string str6 = name2;
				string str7 = String.Concat(new String[] { str6, " has replaced the '", str, "' chat command previously registered by ", str5 });
				Interface.Oxide.LogWarning(str7, Array.Empty<object>());
				this.cmdlib.chatCommands.Remove(str);
			}
			if (this.cmdlib.consoleCommands.TryGetValue(rustCommand1, out consoleCommand))
			{
				if (consoleCommand.OriginalCallback != null)
				{
					originalCallback.OriginalCallback = consoleCommand.OriginalCallback;
				}
				Plugin plugin2 = consoleCommand.Callback.Plugin;
				if (plugin2 != null)
				{
					obj = plugin2.Name;
				}
				else
				{
					obj = null;
				}
				if (obj == null)
				{
					obj = "an unknown plugin";
				}
				string str8 = obj;
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
				string str9 = name1;
				string str10 = String.Concat(new String[] { str9, " has replaced the '", rustCommand1, "' console command previously registered by ", str8 });
				Interface.Oxide.LogWarning(str10, Array.Empty<object>());
				ConsoleSystem.Index.Server.Dict.Remove(consoleCommand.RustCommand.FullName);
				if (str1 == "global")
				{
					ConsoleSystem.Index.Server.GlobalDict.Remove(consoleCommand.RustCommand.Name);
				}
				ConsoleSystem.Index.All = ConsoleSystem.Index.Server.Dict.Values.ToArray<ConsoleSystem.Command>();
				this.cmdlib.consoleCommands.Remove(consoleCommand.RustCommand.FullName);
			}
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
					string str11 = name;
					Interface.Oxide.LogError(String.Concat(str11, " tried to register the ", rustCommand1, " console variable as a command!"), Array.Empty<object>());
					return;
				}
				originalCallback.OriginalCallback = command1.Call;
				originalCallback.OriginalRustCommand = command1;
			}
			originalCallback.RustCommand = new ConsoleSystem.Command()
			{
				Name = rustCommand,
				Parent = str1,
				FullName = str,
				ServerUser = true,
				ServerAdmin = true,
				Client = true,
				ClientInfo = false,
				Variable = false,
				Call = (ConsoleSystem.Arg arg) => {
					if (arg == null)
					{
						return;
					}
					BasePlayer basePlayer = arg.Player();
					if (arg.Connection != null && basePlayer != null)
					{
						RustPlayer player = basePlayer.IPlayer as RustPlayer;
						if (player != null)
						{
							player.LastCommand = CommandType.Console;
							callback(player, str, RustCommandSystem.ExtractArgs(arg));
							return;
						}
					}
					else if (arg.Connection == null)
					{
						this.consolePlayer.LastCommand = CommandType.Console;
						callback(this.consolePlayer, str, RustCommandSystem.ExtractArgs(arg));
					}
				}
			};
			ConsoleSystem.Index.Server.Dict[rustCommand1] = originalCallback.RustCommand;
			if (str1 == "global")
			{
				ConsoleSystem.Index.Server.GlobalDict[rustCommand] = originalCallback.RustCommand;
			}
			ConsoleSystem.Index.All = ConsoleSystem.Index.Server.Dict.Values.ToArray<ConsoleSystem.Command>();
			this.registeredCommands[str] = originalCallback;
		}

		public void UnregisterCommand(string command, Plugin plugin)
		{
			RustCommandSystem.RegisteredCommand registeredCommand;
			string str;
			if (!this.registeredCommands.TryGetValue(command, out registeredCommand))
			{
				return;
			}
			if (plugin != registeredCommand.Source)
			{
				return;
			}
			string[] strArray = command.Split(new Char[] { '.' });
			str = ((int)strArray.Length >= 2 ? strArray[0].Trim() : "global");
			string originalCallback = ((int)strArray.Length >= 2 ? String.Join(".", strArray.Skip<string>(1).ToArray<string>()) : strArray[0].Trim());
			string originalRustCommand = String.Concat(str, ".", originalCallback);
			this.registeredCommands.Remove(command);
			if (registeredCommand.OriginalCallback == null)
			{
				ConsoleSystem.Index.Server.Dict.Remove(originalRustCommand);
				if (originalRustCommand.StartsWith("global."))
				{
					ConsoleSystem.Index.Server.GlobalDict.Remove(originalCallback);
				}
			}
			else
			{
				ConsoleSystem.Index.Server.Dict[originalRustCommand].Call = registeredCommand.OriginalCallback;
				if (originalRustCommand.StartsWith("global."))
				{
					ConsoleSystem.Index.Server.GlobalDict[originalCallback].Call = registeredCommand.OriginalCallback;
				}
				if (registeredCommand.OriginalRustCommand != null)
				{
					ConsoleSystem.Index.Server.Dict[originalRustCommand] = registeredCommand.OriginalRustCommand;
					if (originalRustCommand.StartsWith("global."))
					{
						ConsoleSystem.Index.Server.GlobalDict[originalCallback] = registeredCommand.OriginalRustCommand;
					}
				}
			}
			ConsoleSystem.Index.All = ConsoleSystem.Index.Server.Dict.Values.ToArray<ConsoleSystem.Command>();
		}

		internal class RegisteredCommand
		{
			public readonly Plugin Source;

			public readonly string Command;

			public readonly CommandCallback Callback;

			public ConsoleSystem.Command RustCommand;

			public ConsoleSystem.Command OriginalRustCommand;

			public Action<ConsoleSystem.Arg> OriginalCallback;

			public RegisteredCommand(Plugin source, string command, CommandCallback callback)
			{
				this.Source = source;
				this.Command = command;
				this.Callback = callback;
			}
		}
	}
}