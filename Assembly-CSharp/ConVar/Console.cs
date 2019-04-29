using Facepunch;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ConVar
{
	[Factory("console")]
	public class Console : ConsoleSystem
	{
		public Console()
		{
		}

		[Help("Search the console for a particular string")]
		[ServerVar]
		public static IEnumerable<Output.Entry> search(ConsoleSystem.Arg arg)
		{
			string str = arg.GetString(0, null);
			if (str == null)
			{
				return Enumerable.Empty<Output.Entry>();
			}
			return Output.HistoryOutput.Where<Output.Entry>((Output.Entry x) => {
				if (x.Message.Length >= 4096)
				{
					return false;
				}
				return x.Message.Contains(str, CompareOptions.IgnoreCase);
			});
		}

		[Help("Return the last x lines of the console. Default is 200")]
		[ServerVar]
		public static IEnumerable<Output.Entry> tail(ConsoleSystem.Arg arg)
		{
			int num = arg.GetInt(0, 200);
			int count = Output.HistoryOutput.Count - num;
			if (count < 0)
			{
				count = 0;
			}
			return Output.HistoryOutput.Skip<Output.Entry>(count);
		}
	}
}