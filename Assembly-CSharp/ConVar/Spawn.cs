using System;

namespace ConVar
{
	[Factory("spawn")]
	public class Spawn : ConsoleSystem
	{
		[ServerVar]
		public static float min_rate;

		[ServerVar]
		public static float max_rate;

		[ServerVar]
		public static float min_density;

		[ServerVar]
		public static float max_density;

		[ServerVar]
		public static float player_base;

		[ServerVar]
		public static float player_scale;

		[ServerVar]
		public static bool respawn_populations;

		[ServerVar]
		public static bool respawn_groups;

		[ServerVar]
		public static bool respawn_individuals;

		[ServerVar]
		public static float tick_populations;

		[ServerVar]
		public static float tick_individuals;

		static Spawn()
		{
			Spawn.min_rate = 0.5f;
			Spawn.max_rate = 1f;
			Spawn.min_density = 0.5f;
			Spawn.max_density = 1f;
			Spawn.player_base = 100f;
			Spawn.player_scale = 2f;
			Spawn.respawn_populations = true;
			Spawn.respawn_groups = true;
			Spawn.respawn_individuals = true;
			Spawn.tick_populations = 60f;
			Spawn.tick_individuals = 300f;
		}

		public Spawn()
		{
		}

		[ServerVar]
		public static void fill_groups(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillGroups();
			}
		}

		[ServerVar]
		public static void fill_individuals(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillIndividuals();
			}
		}

		[ServerVar]
		public static void fill_populations(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillPopulations();
			}
		}

		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<SpawnHandler>.Instance)
			{
				args.ReplyWith("No spawn handler found.");
				return;
			}
			args.ReplyWith(SingletonComponent<SpawnHandler>.Instance.GetReport(false));
		}

		[ServerVar]
		public static void scalars(ConsoleSystem.Arg args)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("Type");
			textTable.AddColumn("Value");
			string[] str = new string[] { "Player Fraction", null };
			float single = SpawnHandler.PlayerFraction();
			str[1] = single.ToString();
			textTable.AddRow(str);
			string[] strArrays = new string[] { "Player Excess", null };
			single = SpawnHandler.PlayerExcess();
			strArrays[1] = single.ToString();
			textTable.AddRow(strArrays);
			string[] str1 = new string[] { "Population Rate", null };
			single = SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate);
			str1[1] = single.ToString();
			textTable.AddRow(str1);
			string[] strArrays1 = new string[] { "Population Density", null };
			single = SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density);
			strArrays1[1] = single.ToString();
			textTable.AddRow(strArrays1);
			string[] str2 = new string[] { "Group Rate", null };
			single = SpawnHandler.PlayerScale(Spawn.player_scale);
			str2[1] = single.ToString();
			textTable.AddRow(str2);
			args.ReplyWith(textTable.ToString());
		}
	}
}