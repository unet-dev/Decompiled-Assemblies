using Apex.AI;
using Apex.LoadBalancing;
using System;
using UnityEngine;

namespace ConVar
{
	[Factory("ai")]
	public class AI : ConsoleSystem
	{
		[ServerVar]
		public static bool think;

		[ServerVar]
		public static bool ignoreplayers;

		[ServerVar]
		public static bool move;

		[ServerVar]
		public static float sensetime;

		[ServerVar]
		public static float frametime;

		[ServerVar]
		public static int ocean_patrol_path_iterations;

		[ServerVar(Help="If npc_enable is set to false then npcs won't spawn. (default: true)")]
		public static bool npc_enable;

		[ServerVar(Help="npc_max_population_military_tunnels defines the size of the npc population at military tunnels. (default: 3)")]
		public static int npc_max_population_military_tunnels;

		[ServerVar(Help="npc_spawn_per_tick_max_military_tunnels defines how many can maximum spawn at once at military tunnels. (default: 1)")]
		public static int npc_spawn_per_tick_max_military_tunnels;

		[ServerVar(Help="npc_spawn_per_tick_min_military_tunnels defineshow many will minimum spawn at once at military tunnels. (default: 1)")]
		public static int npc_spawn_per_tick_min_military_tunnels;

		[ServerVar(Help="npc_respawn_delay_max_military_tunnels defines the maximum delay between spawn ticks at military tunnels. (default: 1920)")]
		public static float npc_respawn_delay_max_military_tunnels;

		[ServerVar(Help="npc_respawn_delay_min_military_tunnels defines the minimum delay between spawn ticks at military tunnels. (default: 480)")]
		public static float npc_respawn_delay_min_military_tunnels;

		[ServerVar(Help="npc_valid_aim_cone defines how close their aim needs to be on target in order to fire. (default: 0.8)")]
		public static float npc_valid_aim_cone;

		[ServerVar(Help="npc_valid_mounted_aim_cone defines how close their aim needs to be on target in order to fire while mounted. (default: 0.92)")]
		public static float npc_valid_mounted_aim_cone;

		[ServerVar(Help="npc_cover_compromised_cooldown defines how long a cover point is marked as compromised before it's cleared again for selection. (default: 10)")]
		public static float npc_cover_compromised_cooldown;

		[ServerVar(Help="If npc_cover_use_path_distance is set to true then npcs will look at the distance between the cover point and their target using the path between the two, rather than the straight-line distance.")]
		public static bool npc_cover_use_path_distance;

		[ServerVar(Help="npc_cover_path_vs_straight_dist_max_diff defines what the maximum difference between straight-line distance and path distance can be when evaluating cover points. (default: 2)")]
		public static float npc_cover_path_vs_straight_dist_max_diff;

		[ServerVar(Help="npc_door_trigger_size defines the size of the trigger box on doors that opens the door as npcs walk close to it (default: 1.5)")]
		public static float npc_door_trigger_size;

		[ServerVar(Help="npc_patrol_point_cooldown defines the cooldown time on a patrol point until it's available again (default: 5)")]
		public static float npc_patrol_point_cooldown;

		[ServerVar(Help="npc_speed_walk define the speed of an npc when in the walk state, and should be a number between 0 and 1. (Default: 0.18)")]
		public static float npc_speed_walk;

		[ServerVar(Help="npc_speed_walk define the speed of an npc when in the run state, and should be a number between 0 and 1. (Default: 0.4)")]
		public static float npc_speed_run;

		[ServerVar(Help="npc_speed_walk define the speed of an npc when in the sprint state, and should be a number between 0 and 1. (Default: 1.0)")]
		public static float npc_speed_sprint;

		[ServerVar(Help="npc_speed_walk define the speed of an npc when in the crouched walk state, and should be a number between 0 and 1. (Default: 0.1)")]
		public static float npc_speed_crouch_walk;

		[ServerVar(Help="npc_speed_crouch_run define the speed of an npc when in the crouched run state, and should be a number between 0 and 1. (Default: 0.25)")]
		public static float npc_speed_crouch_run;

		[ServerVar(Help="npc_alertness_drain_rate define the rate at which we drain the alertness level of an NPC when there are no enemies in sight. (Default: 0.01)")]
		public static float npc_alertness_drain_rate;

		[ServerVar(Help="npc_alertness_zero_detection_mod define the threshold of visibility required to detect an enemy when alertness is zero. (Default: 0.5)")]
		public static float npc_alertness_zero_detection_mod;

		[ServerVar(Help="npc_junkpile_a_spawn_chance define the chance for scientists to spawn at junkpile a. (Default: 0.1)")]
		public static float npc_junkpile_a_spawn_chance;

		[ServerVar(Help="npc_junkpile_g_spawn_chance define the chance for scientists to spawn at junkpile g. (Default: 0.1)")]
		public static float npc_junkpile_g_spawn_chance;

		[ServerVar(Help="npc_junkpile_dist_aggro_gate define at what range (or closer) a junkpile scientist will get aggressive. (Default: 8)")]
		public static float npc_junkpile_dist_aggro_gate;

		[ServerVar(Help="npc_max_junkpile_count define how many npcs can spawn into the world at junkpiles at the same time (does not include monuments) (Default: 30)")]
		public static int npc_max_junkpile_count;

		[ServerVar(Help="If npc_families_no_hurt is true, npcs of the same family won't be able to hurt each other. (default: true)")]
		public static bool npc_families_no_hurt;

		[ServerVar(Help="If npc_ignore_chairs is true, npcs won't care about seeking out and sitting in chairs. (default: true)")]
		public static bool npc_ignore_chairs;

		[ServerVar(Help="The rate at which we tick the sensory system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 5)")]
		public static float npc_sensory_system_tick_rate_multiplier;

		[ServerVar(Help="The rate at which we gather information about available cover points. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 20)")]
		public static float npc_cover_info_tick_rate_multiplier;

		[ServerVar(Help="The rate at which we tick the reasoning system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 1)")]
		public static float npc_reasoning_system_tick_rate_multiplier;

		[ServerVar(Help="If animal_ignore_food is true, animals will not sense food sources or interact with them (server optimization). (default: true)")]
		public static bool animal_ignore_food;

		[ServerVar(Help="The modifier by which a silencer reduce the noise that a gun makes when shot. (Default: 0.15)")]
		public static float npc_gun_noise_silencer_modifier;

		[ServerVar(Help="If nav_carve_use_building_optimization is true, we attempt to reduce the amount of navmesh carves for a building. (default: false)")]
		public static bool nav_carve_use_building_optimization;

		[ServerVar(Help="The minimum number of building blocks a building needs to consist of for this optimization to be applied. (default: 25)")]
		public static int nav_carve_min_building_blocks_to_apply_optimization;

		[ServerVar(Help="The minimum size we allow a carving volume to be. (default: 2)")]
		public static float nav_carve_min_base_size;

		[ServerVar(Help="The size multiplier applied to the size of the carve volume. The smaller the value, the tighter the skirt around foundation edges, but too small and animals can attack through walls. (default: 4)")]
		public static float nav_carve_size_multiplier;

		[ServerVar(Help="The height of the carve volume. (default: 2)")]
		public static float nav_carve_height;

		[ServerVar(Help="If npc_only_hurt_active_target_in_safezone is true, npcs won't any player other than their actively targeted player when in a safe zone. (default: true)")]
		public static bool npc_only_hurt_active_target_in_safezone;

		[ServerVar(Help="If npc_use_new_aim_system is true, npcs will miss on purpose on occasion, where the old system would randomize aim cone. (default: true)")]
		public static bool npc_use_new_aim_system;

		[ServerVar(Help="If npc_use_thrown_weapons is true, npcs will throw grenades, etc. This is an experimental feature. (default: true)")]
		public static bool npc_use_thrown_weapons;

		[ServerVar(Help="This is multiplied with the max roam range stat of an NPC to determine how far from its spawn point the NPC is allowed to roam. (default: 3)")]
		public static float npc_max_roam_multiplier;

		[ServerVar(Help="This is multiplied with the current alertness (0-10) to decide how long it will take for the NPC to deliberately miss again. (default: 0.33)")]
		public static float npc_alertness_to_aim_modifier;

		[ServerVar(Help="The time it takes for the NPC to deliberately miss to the time the NPC tries to hit its target. (default: 1.5)")]
		public static float npc_deliberate_miss_to_hit_alignment_time;

		[ServerVar(Help="The offset with which the NPC will maximum miss the target. (default: 1.25)")]
		public static float npc_deliberate_miss_offset_multiplier;

		[ServerVar(Help="The percentage away from a maximum miss the randomizer is allowed to travel when shooting to deliberately hit the target (we don't want perfect hits with every shot). (default: 0.85f)")]
		public static float npc_deliberate_hit_randomizer;

		[ServerVar(Help="Baseline damage modifier for the new HTN Player NPCs to nerf their damage compared to the old NPCs. (default: 1.15f)")]
		public static float npc_htn_player_base_damage_modifier;

		[ServerVar(Help="Spawn NPCs on the Cargo Ship. (default: true)")]
		public static bool npc_spawn_on_cargo_ship;

		[ServerVar(Help="npc_htn_player_frustration_threshold defines where the frustration threshold for NPCs go, where they have the opportunity to change to a more aggressive tactic. (default: 3)")]
		public static int npc_htn_player_frustration_threshold;

		[ServerVar]
		public static float tickrate;

		static AI()
		{
			AI.think = true;
			AI.ignoreplayers = false;
			AI.move = true;
			AI.sensetime = 1f;
			AI.frametime = 5f;
			AI.ocean_patrol_path_iterations = 100000;
			AI.npc_enable = true;
			AI.npc_max_population_military_tunnels = 3;
			AI.npc_spawn_per_tick_max_military_tunnels = 1;
			AI.npc_spawn_per_tick_min_military_tunnels = 1;
			AI.npc_respawn_delay_max_military_tunnels = 1920f;
			AI.npc_respawn_delay_min_military_tunnels = 480f;
			AI.npc_valid_aim_cone = 0.8f;
			AI.npc_valid_mounted_aim_cone = 0.92f;
			AI.npc_cover_compromised_cooldown = 10f;
			AI.npc_cover_use_path_distance = true;
			AI.npc_cover_path_vs_straight_dist_max_diff = 2f;
			AI.npc_door_trigger_size = 1.5f;
			AI.npc_patrol_point_cooldown = 5f;
			AI.npc_speed_walk = 0.18f;
			AI.npc_speed_run = 0.4f;
			AI.npc_speed_sprint = 1f;
			AI.npc_speed_crouch_walk = 0.1f;
			AI.npc_speed_crouch_run = 0.25f;
			AI.npc_alertness_drain_rate = 0.01f;
			AI.npc_alertness_zero_detection_mod = 0.5f;
			AI.npc_junkpile_a_spawn_chance = 0.1f;
			AI.npc_junkpile_g_spawn_chance = 0.1f;
			AI.npc_junkpile_dist_aggro_gate = 8f;
			AI.npc_max_junkpile_count = 30;
			AI.npc_families_no_hurt = true;
			AI.npc_ignore_chairs = true;
			AI.npc_sensory_system_tick_rate_multiplier = 5f;
			AI.npc_cover_info_tick_rate_multiplier = 20f;
			AI.npc_reasoning_system_tick_rate_multiplier = 1f;
			AI.animal_ignore_food = true;
			AI.npc_gun_noise_silencer_modifier = 0.15f;
			AI.nav_carve_use_building_optimization = false;
			AI.nav_carve_min_building_blocks_to_apply_optimization = 25;
			AI.nav_carve_min_base_size = 2f;
			AI.nav_carve_size_multiplier = 4f;
			AI.nav_carve_height = 2f;
			AI.npc_only_hurt_active_target_in_safezone = true;
			AI.npc_use_new_aim_system = true;
			AI.npc_use_thrown_weapons = true;
			AI.npc_max_roam_multiplier = 3f;
			AI.npc_alertness_to_aim_modifier = 0.5f;
			AI.npc_deliberate_miss_to_hit_alignment_time = 1.5f;
			AI.npc_deliberate_miss_offset_multiplier = 1.25f;
			AI.npc_deliberate_hit_randomizer = 0.85f;
			AI.npc_htn_player_base_damage_modifier = 1.15f;
			AI.npc_spawn_on_cargo_ship = true;
			AI.npc_htn_player_frustration_threshold = 3;
			AI.tickrate = 5f;
		}

		public AI()
		{
		}

		private static void AddLBTableEntry(ref TextTable table, string name, LoadBalancedQueue lb)
		{
			if (lb == null)
			{
				return;
			}
			float single = 0f;
			if (lb.updatedItemsCount > 0)
			{
				single = Mathf.Clamp(lb.updatesOverdueByTotal / (float)lb.updatedItemsCount - 0.02f, 0f, Single.MaxValue);
			}
			string[] str = new string[] { name, null, null };
			str[1] = lb.itemCount.ToString();
			str[2] = single.ToString("N2");
			table.AddRow(str);
		}

		[ServerVar]
		public static void aiDebug_LoadBalanceOverdueReportServer(ConsoleSystem.Arg args)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("count");
			textTable.AddColumn("overdue");
			AI.AddLBTableEntry(ref textTable, "Default", LoadBalancer.defaultBalancer as LoadBalancedQueue);
			AI.AddLBTableEntry(ref textTable, "Ai Manager", AiManagerLoadBalancer.aiManagerLoadBalancer as LoadBalancedQueue);
			AI.AddLBTableEntry(ref textTable, "Ai Behaviour", AILoadBalancer.aiLoadBalancer as LoadBalancedQueue);
			AI.AddLBTableEntry(ref textTable, "Npc Senses", NPCSensesLoadBalancer.NpcSensesLoadBalancer as LoadBalancedQueue);
			AI.AddLBTableEntry(ref textTable, "Animal Senses", AnimalSensesLoadBalancer.animalSensesLoadBalancer as LoadBalancedQueue);
			args.ReplyWith(textTable.ToString());
		}

		[ServerVar]
		public static void aiDebug_toggle(ConsoleSystem.Arg args)
		{
			BaseNpc.AiStatistics.FamilyEnum family;
			float single;
			bool hasPath;
			int num = args.GetInt(0, 0);
			if (num == 0)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find((uint)num) as BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			NPCPlayerApex nPCPlayerApex = baseEntity as NPCPlayerApex;
			if (nPCPlayerApex != null)
			{
				TextTable textTable = new TextTable();
				textTable.AddColumn("type");
				textTable.AddColumn("state");
				textTable.AddColumn("posSync");
				textTable.AddColumn("health");
				textTable.AddColumn("stuckTime");
				textTable.AddColumn("hasPath");
				textTable.AddColumn("hasEnemyTarget");
				textTable.AddColumn("isMounted");
				TextTable textTable1 = textTable;
				string[] str = new string[8];
				family = nPCPlayerApex.Family;
				str[0] = family.ToString();
				str[1] = (nPCPlayerApex.IsDormant ? "dormant" : "awake");
				str[2] = nPCPlayerApex.syncPosition.ToString();
				single = nPCPlayerApex.health;
				str[3] = single.ToString("N2");
				str[4] = nPCPlayerApex.stuckDuration.ToString("N2");
				hasPath = nPCPlayerApex.HasPath;
				str[5] = hasPath.ToString();
				hasPath = nPCPlayerApex.AttackTarget != null;
				str[6] = hasPath.ToString();
				str[7] = (nPCPlayerApex.isMounted ? "true" : "false");
				textTable1.AddRow(str);
				args.ReplyWith(textTable.ToString());
				return;
			}
			BaseAnimalNPC baseAnimalNPC = baseEntity as BaseAnimalNPC;
			if (baseAnimalNPC != null)
			{
				TextTable textTable2 = new TextTable();
				textTable2.AddColumn("type");
				textTable2.AddColumn("state");
				textTable2.AddColumn("posSync");
				textTable2.AddColumn("health");
				textTable2.AddColumn("stuckTime");
				textTable2.AddColumn("hasPath");
				textTable2.AddColumn("hasEnemyTarget");
				textTable2.AddColumn("hasFoodTarget");
				TextTable textTable3 = textTable2;
				string[] strArrays = new string[] { baseAnimalNPC.Stats.Family.ToString(), null, null, null, null, null, null, null };
				strArrays[1] = (baseAnimalNPC.IsDormant ? "dormant" : "awake");
				strArrays[2] = baseAnimalNPC.syncPosition.ToString();
				single = baseAnimalNPC.health;
				strArrays[3] = single.ToString("N2");
				strArrays[4] = baseAnimalNPC.stuckDuration.ToString("N2");
				hasPath = baseAnimalNPC.HasPath;
				strArrays[5] = hasPath.ToString();
				hasPath = baseAnimalNPC.AttackTarget != null;
				strArrays[6] = hasPath.ToString();
				hasPath = baseAnimalNPC.FoodTarget != null;
				strArrays[7] = hasPath.ToString();
				textTable3.AddRow(strArrays);
				args.ReplyWith(textTable2.ToString());
				return;
			}
			HTNPlayer hTNPlayer = baseEntity as HTNPlayer;
			if (hTNPlayer == null)
			{
				return;
			}
			TextTable textTable4 = new TextTable();
			textTable4.AddColumn("type");
			textTable4.AddColumn("state");
			textTable4.AddColumn("posSync");
			textTable4.AddColumn("health");
			textTable4.AddColumn("hasEnemyTarget");
			textTable4.AddColumn("isMounted");
			TextTable textTable5 = textTable4;
			string[] str1 = new string[6];
			family = hTNPlayer.Family;
			str1[0] = family.ToString();
			str1[1] = (hTNPlayer.IsDormant ? "dormant" : "awake");
			str1[2] = hTNPlayer.syncPosition.ToString();
			single = hTNPlayer.health;
			str1[3] = single.ToString("N2");
			hasPath = hTNPlayer.MainTarget != null;
			str1[4] = hasPath.ToString();
			str1[5] = (hTNPlayer.isMounted ? "true" : "false");
			textTable5.AddRow(str1);
			args.ReplyWith(textTable4.ToString());
		}

		[ServerVar(Help="Set the update interval for the behaviour ai of animals and npcs. (Default: 0.25)")]
		public static void aiLoadBalancerUpdateInterval(ConsoleSystem.Arg args)
		{
			LoadBalancedQueue num = AILoadBalancer.aiLoadBalancer as LoadBalancedQueue;
			if (num != null)
			{
				num.defaultUpdateInterval = args.GetFloat(0, num.defaultUpdateInterval);
			}
		}

		[ServerVar(Help="Set the update interval for the agency of dormant and active animals and npcs. (Default: 2.0)")]
		public static void aiManagerLoadBalancerUpdateInterval(ConsoleSystem.Arg args)
		{
			LoadBalancedQueue num = AiManagerLoadBalancer.aiManagerLoadBalancer as LoadBalancedQueue;
			if (num != null)
			{
				num.defaultUpdateInterval = args.GetFloat(0, num.defaultUpdateInterval);
			}
		}

		[ServerVar(Help="Set the update interval for animal senses that updates the knowledge gathering of animals. (Default: 0.2)")]
		public static void AnimalSenseLoadBalancerUpdateInterval(ConsoleSystem.Arg args)
		{
			LoadBalancedQueue num = AnimalSensesLoadBalancer.animalSensesLoadBalancer as LoadBalancedQueue;
			if (num != null)
			{
				num.defaultUpdateInterval = args.GetFloat(0, num.defaultUpdateInterval);
			}
		}

		[ServerVar(Help="Set the update interval for the default load balancer, currently used for cover point generation. (Default: 2.5)")]
		public static void defaultLoadBalancerUpdateInterval(ConsoleSystem.Arg args)
		{
			LoadBalancedQueue num = LoadBalancer.defaultBalancer as LoadBalancedQueue;
			if (num != null)
			{
				num.defaultUpdateInterval = args.GetFloat(0, num.defaultUpdateInterval);
			}
		}

		[ServerVar(Help="Set the update interval for npc senses that updates the knowledge gathering of npcs. (Default: 0.2)")]
		public static void NpcSenseLoadBalancerUpdateInterval(ConsoleSystem.Arg args)
		{
			LoadBalancedQueue npcSensesLoadBalancer = NPCSensesLoadBalancer.NpcSensesLoadBalancer as LoadBalancedQueue;
			if (npcSensesLoadBalancer != null)
			{
				npcSensesLoadBalancer.defaultUpdateInterval = args.GetFloat(0, npcSensesLoadBalancer.defaultUpdateInterval);
			}
		}

		[ServerVar]
		public static void selectNPCLookatServer(ConsoleSystem.Arg args)
		{
		}

		public static float TickDelta()
		{
			return 1f / (float)AI.tickrate;
		}
	}
}