using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Oxide.Core
{
	public sealed class CommandLine
	{
		private readonly Dictionary<string, string> variables = new Dictionary<string, string>();

		public CommandLine(string[] commandline)
		{
			int i;
			string empty = string.Empty;
			string str = string.Empty;
			string[] strArrays = commandline;
			for (i = 0; i < (int)strArrays.Length; i++)
			{
				string str1 = strArrays[i];
				empty = string.Concat(empty, "\"", str1.Trim(new char[] { '/', '\\' }), "\"");
			}
			strArrays = CommandLine.Split(empty);
			for (i = 0; i < (int)strArrays.Length; i++)
			{
				string str2 = strArrays[i];
				if (str2.Length > 0)
				{
					string str3 = str2;
					if (str2[0] == '-' || str2[0] == '+')
					{
						if (str != string.Empty && !this.variables.ContainsKey(str))
						{
							this.variables.Add(str, string.Empty);
						}
						str = str3.Substring(1);
					}
					else if (str != string.Empty)
					{
						if (!this.variables.ContainsKey(str))
						{
							if (str.Contains("dir"))
							{
								str3 = str3.Replace('/', '\\');
							}
							this.variables.Add(str, str3);
						}
						str = string.Empty;
					}
				}
			}
			if (str != string.Empty && !this.variables.ContainsKey(str))
			{
				this.variables.Add(str, string.Empty);
			}
		}

		public void GetArgument(string var, out string varname, out string format)
		{
			string variable = this.GetVariable(var);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder1 = new StringBuilder();
			int num = 0;
			string str = variable;
			for (int i = 0; i < str.Length; i++)
			{
				char chr = str[i];
				if (chr == '{')
				{
					num++;
				}
				else if (chr == '}')
				{
					num--;
					if (num == 0)
					{
						stringBuilder1.Append("{0}");
					}
				}
				else if (num != 0)
				{
					stringBuilder.Append(chr);
				}
				else
				{
					stringBuilder1.Append(chr);
				}
			}
			varname = stringBuilder.ToString();
			format = stringBuilder1.ToString();
		}

		public string GetVariable(string name)
		{
			string value;
			try
			{
				KeyValuePair<string, string> keyValuePair = this.variables.Single<KeyValuePair<string, string>>((KeyValuePair<string, string> v) => v.Key == name);
				value = keyValuePair.Value;
			}
			catch (Exception exception)
			{
				value = null;
			}
			return value;
		}

		public bool HasVariable(string name)
		{
			return this.variables.Any<KeyValuePair<string, string>>((KeyValuePair<string, string> v) => v.Key == name);
		}

		public static string[] Split(string input)
		{
			input = input.Replace("\\\"", "&quot;");
			MatchCollection matchCollections = (new Regex("\"([^\"]+)\"|'([^']+)'|\\S+")).Matches(input);
			string[] strArrays = new string[matchCollections.Count];
			for (int i = 0; i < matchCollections.Count; i++)
			{
				char[] chrArray = new char[] { ' ', '\"' };
				strArrays[i] = matchCollections[i].Groups[0].Value.Trim(chrArray);
				strArrays[i] = strArrays[i].Replace("&quot;", "\"");
			}
			return strArrays;
		}
	}
}