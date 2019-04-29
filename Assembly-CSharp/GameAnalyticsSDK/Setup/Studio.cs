using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GameAnalyticsSDK.Setup
{
	public class Studio
	{
		public List<Game> Games
		{
			get;
			private set;
		}

		public string ID
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public Studio(string name, string id, List<Game> games)
		{
			this.Name = name;
			this.ID = id;
			this.Games = games;
		}

		public static string[] GetGameNames(int index, List<Studio> studios)
		{
			if (studios == null || studios[index].Games == null)
			{
				return new string[] { "-" };
			}
			string[] strArrays = new string[studios[index].Games.Count + 1];
			strArrays[0] = "-";
			string str = "";
			for (int i = 0; i < studios[index].Games.Count; i++)
			{
				strArrays[i + 1] = string.Concat(studios[index].Games[i].Name, str);
				str = string.Concat(str, " ");
			}
			return strArrays;
		}

		public static string[] GetStudioNames(List<Studio> studios, bool addFirstEmpty = true)
		{
			if (studios == null)
			{
				return new string[] { "-" };
			}
			if (!addFirstEmpty)
			{
				string[] strArrays = new string[studios.Count];
				string str = "";
				for (int i = 0; i < studios.Count; i++)
				{
					strArrays[i] = string.Concat(studios[i].Name, str);
					str = string.Concat(str, " ");
				}
				return strArrays;
			}
			string[] strArrays1 = new string[studios.Count + 1];
			strArrays1[0] = "-";
			string str1 = "";
			for (int j = 0; j < studios.Count; j++)
			{
				strArrays1[j + 1] = string.Concat(studios[j].Name, str1);
				str1 = string.Concat(str1, " ");
			}
			return strArrays1;
		}
	}
}