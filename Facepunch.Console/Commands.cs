using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Commands : ConsoleSystem
{
	public Commands()
	{
	}

	[ClientVar(AllowRunFromServer=true)]
	[ServerVar]
	public static void Echo(string fullString)
	{
		Debug.Log(fullString);
	}

	[ClientVar]
	[ServerVar]
	public static void Find(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			return;
		}
		string str = arg.GetString(0, "");
		string str1 = "";
		string str2 = "";
		foreach (ConsoleSystem.Command command in ConsoleSystem.Index.All.Where<ConsoleSystem.Command>((ConsoleSystem.Command x) => {
			if (x.Description.Contains(str) || x.FullName.Contains(str))
			{
				return true;
			}
			return x.Arguments.Contains(str);
		}))
		{
			if (!arg.CanSeeInFind(command))
			{
				continue;
			}
			if (!command.Variable || command.GetOveride == null)
			{
				string str3 = string.Format("{0}( {1} )", command.FullName, command.Arguments);
				str2 = string.Concat(str2, string.Format(" {0} {1}\n", str3, command.Description));
			}
			else
			{
				str1 = string.Concat(str1, string.Format(" {0} {1} ({2})\n", command.FullName.PadRight(24), command.Description, command.String));
			}
		}
		arg.ReplyWith(string.Concat("Variables:\n", str1, "\nCommands:\n", str2));
	}
}