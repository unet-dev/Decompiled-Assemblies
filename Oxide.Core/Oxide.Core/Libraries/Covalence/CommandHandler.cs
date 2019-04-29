using System;
using System.Collections.Generic;
using System.Text;

namespace Oxide.Core.Libraries.Covalence
{
	public sealed class CommandHandler
	{
		private CommandCallback callback;

		private Func<string, bool> commandFilter;

		public CommandHandler(CommandCallback callback, Func<string, bool> commandFilter)
		{
			this.callback = callback;
			this.commandFilter = commandFilter;
		}

		public bool HandleChatMessage(IPlayer player, string message)
		{
			string str;
			string[] strArrays;
			if (message.Length == 0)
			{
				return false;
			}
			if (message[0] != '/')
			{
				return false;
			}
			message = message.Substring(1);
			this.ParseCommand(message, out str, out strArrays);
			player.LastCommand = CommandType.Chat;
			if (str == null)
			{
				return false;
			}
			return this.HandleCommand(player, str, strArrays);
		}

		private bool HandleCommand(IPlayer player, string command, string[] args)
		{
			if (this.commandFilter != null && !this.commandFilter(command) || this.callback == null)
			{
				return false;
			}
			return this.callback(player, command, args);
		}

		public bool HandleConsoleMessage(IPlayer player, string message)
		{
			string str;
			string[] strArrays;
			if (message.StartsWith("global."))
			{
				message = message.Substring(7);
			}
			this.ParseCommand(message, out str, out strArrays);
			player.LastCommand = CommandType.Console;
			if (str == null)
			{
				return false;
			}
			return this.HandleCommand(player, str, strArrays);
		}

		private void ParseCommand(string argstr, out string cmd, out string[] args)
		{
			List<string> strs = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			for (int i = 0; i < argstr.Length; i++)
			{
				char chr = argstr[i];
				if (chr == '\"')
				{
					if (!flag)
					{
						flag = true;
					}
					else
					{
						string str = stringBuilder.ToString().Trim();
						if (!string.IsNullOrEmpty(str))
						{
							strs.Add(str);
						}
						stringBuilder = new StringBuilder();
						flag = false;
					}
				}
				else if (!char.IsWhiteSpace(chr) || flag)
				{
					stringBuilder.Append(chr);
				}
				else
				{
					string str1 = stringBuilder.ToString().Trim();
					if (!string.IsNullOrEmpty(str1))
					{
						strs.Add(str1);
					}
					stringBuilder = new StringBuilder();
				}
			}
			if (stringBuilder.Length > 0)
			{
				string str2 = stringBuilder.ToString().Trim();
				if (!string.IsNullOrEmpty(str2))
				{
					strs.Add(str2);
				}
			}
			if (strs.Count == 0)
			{
				cmd = null;
				args = null;
				return;
			}
			cmd = strs[0].ToLowerInvariant();
			strs.RemoveAt(0);
			args = strs.ToArray();
		}
	}
}