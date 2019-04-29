using Facepunch.Extend;
using System;
using System.Collections.Generic;

namespace Facepunch.Utility
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
				Facepunch.Utility.CommandLine.Initalize();
				return Facepunch.Utility.CommandLine.commandline;
			}
		}

		static CommandLine()
		{
			Facepunch.Utility.CommandLine.initialized = false;
			Facepunch.Utility.CommandLine.commandline = "";
			Facepunch.Utility.CommandLine.switches = new Dictionary<string, string>();
		}

		public static void Force(string val)
		{
			Facepunch.Utility.CommandLine.commandline = val;
			Facepunch.Utility.CommandLine.initialized = false;
		}

		public static string GetSwitch(string strName, string strDefault)
		{
			Facepunch.Utility.CommandLine.Initalize();
			string str = "";
			if (!Facepunch.Utility.CommandLine.switches.TryGetValue(strName, out str))
			{
				return strDefault;
			}
			return str;
		}

		public static Dictionary<string, string> GetSwitches()
		{
			Facepunch.Utility.CommandLine.Initalize();
			return Facepunch.Utility.CommandLine.switches;
		}

		public static int GetSwitchInt(string strName, int iDefault)
		{
			Facepunch.Utility.CommandLine.Initalize();
			string str = "";
			if (!Facepunch.Utility.CommandLine.switches.TryGetValue(strName, out str))
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
			Facepunch.Utility.CommandLine.Initalize();
			return Facepunch.Utility.CommandLine.switches.ContainsKey(strName);
		}

		private static void Initalize()
		{
			string[] commandLineArgs;
			int i;
			if (Facepunch.Utility.CommandLine.initialized)
			{
				return;
			}
			Facepunch.Utility.CommandLine.initialized = true;
			if (Facepunch.Utility.CommandLine.commandline == "")
			{
				commandLineArgs = Environment.GetCommandLineArgs();
				for (i = 0; i < (int)commandLineArgs.Length; i++)
				{
					string str = commandLineArgs[i];
					Facepunch.Utility.CommandLine.commandline = string.Concat(Facepunch.Utility.CommandLine.commandline, "\"", str, "\" ");
				}
			}
			if (Facepunch.Utility.CommandLine.commandline == "")
			{
				return;
			}
			string str1 = "";
			commandLineArgs = Facepunch.Utility.CommandLine.commandline.SplitQuotesStrings();
			for (i = 0; i < (int)commandLineArgs.Length; i++)
			{
				string str2 = commandLineArgs[i];
				if (str2.Length != 0)
				{
					if (str2[0] == '-' || str2[0] == '+')
					{
						if (str1 != "" && !Facepunch.Utility.CommandLine.switches.ContainsKey(str1))
						{
							Facepunch.Utility.CommandLine.switches.Add(str1, "");
						}
						str1 = str2;
					}
					else if (str1 != "")
					{
						if (!Facepunch.Utility.CommandLine.switches.ContainsKey(str1))
						{
							Facepunch.Utility.CommandLine.switches.Add(str1, str2);
						}
						str1 = "";
					}
				}
			}
			if (str1 != "" && !Facepunch.Utility.CommandLine.switches.ContainsKey(str1))
			{
				Facepunch.Utility.CommandLine.switches.Add(str1, "");
			}
		}
	}
}