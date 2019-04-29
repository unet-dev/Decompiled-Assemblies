using Facepunch.Extend;
using System;
using System.Collections.Generic;

namespace Facepunch
{
	public static class CommandLine
	{
		private static bool initialized;

		private static string commandline;

		private static Dictionary<string, string> switches;

		public static string Full
		{
			get
			{
				CommandLine.Initalize();
				return CommandLine.commandline;
			}
		}

		static CommandLine()
		{
			CommandLine.initialized = false;
			CommandLine.commandline = "";
			CommandLine.switches = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		public static void Force(string val)
		{
			CommandLine.commandline = val;
			CommandLine.initialized = false;
		}

		public static string GetSwitch(string strName, string strDefault)
		{
			CommandLine.Initalize();
			string str = "";
			if (!CommandLine.switches.TryGetValue(strName, out str))
			{
				return strDefault;
			}
			return str;
		}

		public static Dictionary<string, string> GetSwitches()
		{
			CommandLine.Initalize();
			return CommandLine.switches;
		}

		public static int GetSwitchInt(string strName, int iDefault)
		{
			CommandLine.Initalize();
			string str = "";
			if (!CommandLine.switches.TryGetValue(strName, out str))
			{
				return iDefault;
			}
			int num = iDefault;
			if (!int.TryParse(str, out num))
			{
				return iDefault;
			}
			return num;
		}

		public static bool HasSwitch(string strName)
		{
			CommandLine.Initalize();
			return CommandLine.switches.ContainsKey(strName);
		}

		private static void Initalize()
		{
			string[] commandLineArgs;
			int i;
			if (CommandLine.initialized)
			{
				return;
			}
			CommandLine.initialized = true;
			if (CommandLine.commandline == "")
			{
				commandLineArgs = Environment.GetCommandLineArgs();
				for (i = 0; i < (int)commandLineArgs.Length; i++)
				{
					string str = commandLineArgs[i];
					CommandLine.commandline = string.Concat(CommandLine.commandline, "\"", str, "\" ");
				}
			}
			if (CommandLine.commandline == "")
			{
				return;
			}
			string str1 = "";
			commandLineArgs = CommandLine.commandline.SplitQuotesStrings();
			for (i = 0; i < (int)commandLineArgs.Length; i++)
			{
				string str2 = commandLineArgs[i];
				if (str2.Length != 0)
				{
					if (str2[0] == '-' || str2[0] == '+')
					{
						if (str1 != "" && !CommandLine.switches.ContainsKey(str1))
						{
							CommandLine.switches.Add(str1, "");
						}
						str1 = str2;
					}
					else if (str1 != "")
					{
						if (!CommandLine.switches.ContainsKey(str1))
						{
							CommandLine.switches.Add(str1, str2);
						}
						str1 = "";
					}
				}
			}
			if (str1 != "" && !CommandLine.switches.ContainsKey(str1))
			{
				CommandLine.switches.Add(str1, "");
			}
		}
	}
}