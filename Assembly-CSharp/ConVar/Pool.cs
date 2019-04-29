using Facepunch;
using Facepunch.Extend;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ConVar
{
	[Factory("pool")]
	public class Pool : ConsoleSystem
	{
		[ClientVar]
		[ServerVar]
		public static int mode;

		[ClientVar]
		[ServerVar]
		public static bool enabled;

		[ClientVar]
		[ServerVar]
		public static bool debug;

		static Pool()
		{
			ConVar.Pool.mode = 2;
			ConVar.Pool.enabled = true;
			ConVar.Pool.debug = false;
		}

		public Pool()
		{
		}

		[ClientVar]
		[ServerVar]
		public static void clear_assets(ConsoleSystem.Arg arg)
		{
			AssetPool.Clear();
		}

		[ClientVar]
		[ServerVar]
		public static void clear_memory(ConsoleSystem.Arg arg)
		{
			Facepunch.Pool.Clear();
		}

		[ClientVar]
		[ServerVar]
		public static void clear_prefabs(ConsoleSystem.Arg arg)
		{
			GameManager.server.pool.Clear();
		}

		[ClientVar]
		[ServerVar]
		public static void export_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection prefabPoolCollection = GameManager.server.pool;
			if (prefabPoolCollection.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string str = arg.GetString(0, string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in prefabPoolCollection.storage)
			{
				string str1 = keyValuePair.Key.ToString();
				string str2 = StringPool.Get(keyValuePair.Key);
				string str3 = keyValuePair.Value.Count.ToString();
				if (!string.IsNullOrEmpty(str) && !str2.Contains(str, CompareOptions.IgnoreCase))
				{
					continue;
				}
				stringBuilder.AppendLine(string.Format("{0},{1},{2}", str1, Path.GetFileNameWithoutExtension(str2), str3));
			}
			File.WriteAllText("prefabs.csv", stringBuilder.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void print_assets(ConsoleSystem.Arg arg)
		{
			if (AssetPool.storage.Count == 0)
			{
				arg.ReplyWith("Asset pool is empty.");
				return;
			}
			string str = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("allocated");
			textTable.AddColumn("available");
			foreach (KeyValuePair<Type, AssetPool.Pool> keyValuePair in AssetPool.storage)
			{
				string str1 = keyValuePair.Key.ToString();
				string str2 = keyValuePair.Value.allocated.ToString();
				string str3 = keyValuePair.Value.available.ToString();
				if (!string.IsNullOrEmpty(str) && !str1.Contains(str, CompareOptions.IgnoreCase))
				{
					continue;
				}
				textTable.AddRow(new string[] { str1, str2, str3 });
			}
			arg.ReplyWith(textTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void print_memory(ConsoleSystem.Arg arg)
		{
			if (Facepunch.Pool.directory.Count == 0)
			{
				arg.ReplyWith("Memory pool is empty.");
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("pooled");
			textTable.AddColumn("active");
			textTable.AddColumn("hits");
			textTable.AddColumn("misses");
			textTable.AddColumn("spills");
			foreach (KeyValuePair<Type, Facepunch.Pool.ICollection> keyValuePair in 
				from x in Facepunch.Pool.directory
				orderby x.Value.ItemsCreated descending
				select x)
			{
				string str = keyValuePair.Key.ToString().Replace("System.Collections.Generic.", "");
				Facepunch.Pool.ICollection value = keyValuePair.Value;
				textTable.AddRow(new string[] { str, value.ItemsInStack.FormatNumberShort(), value.ItemsInUse.FormatNumberShort(), value.ItemsTaken.FormatNumberShort(), value.ItemsCreated.FormatNumberShort(), value.ItemsSpilled.FormatNumberShort() });
			}
			arg.ReplyWith(textTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void print_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection prefabPoolCollection = GameManager.server.pool;
			if (prefabPoolCollection.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string str = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("count");
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in prefabPoolCollection.storage)
			{
				string str1 = keyValuePair.Key.ToString();
				string str2 = StringPool.Get(keyValuePair.Key);
				string str3 = keyValuePair.Value.Count.ToString();
				if (!string.IsNullOrEmpty(str) && !str2.Contains(str, CompareOptions.IgnoreCase))
				{
					continue;
				}
				textTable.AddRow(new string[] { str1, Path.GetFileNameWithoutExtension(str2), str3 });
			}
			arg.ReplyWith(textTable.ToString());
		}
	}
}