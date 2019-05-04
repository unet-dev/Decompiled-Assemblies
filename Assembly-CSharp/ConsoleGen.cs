using ConVar;
using Facepunch;
using Facepunch.Extend;
using Rust.Ai;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConsoleGen
{
	public static ConsoleSystem.Command[] All;

	static ConsoleGen()
	{
		ConsoleSystem.Command[] command = new ConsoleSystem.Command[526];
		command[0] = new ConsoleSystem.Command()
		{
			Name = "framebudgetms",
			Parent = "aithinkmanager",
			FullName = "aithinkmanager.framebudgetms",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => AIThinkManager.framebudgetms.ToString(),
			SetOveride = (string str) => AIThinkManager.framebudgetms = str.ToFloat(0f)
		};
		command[1] = new ConsoleSystem.Command()
		{
			Name = "generate_paths",
			Parent = "baseboat",
			FullName = "baseboat.generate_paths",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => BaseBoat.generate_paths.ToString(),
			SetOveride = (string str) => BaseBoat.generate_paths = str.ToBool()
		};
		command[2] = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "bear",
			FullName = "bear.population",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Bear.Population.ToString(),
			SetOveride = (string str) => Bear.Population = str.ToFloat(0f)
		};
		command[3] = new ConsoleSystem.Command()
		{
			Name = "spinfrequencyseconds",
			Parent = "bigwheelgame",
			FullName = "bigwheelgame.spinfrequencyseconds",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => BigWheelGame.spinFrequencySeconds.ToString(),
			SetOveride = (string str) => BigWheelGame.spinFrequencySeconds = str.ToFloat(0f)
		};
		command[4] = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "boar",
			FullName = "boar.population",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Boar.Population.ToString(),
			SetOveride = (string str) => Boar.Population = str.ToFloat(0f)
		};
		command[5] = new ConsoleSystem.Command()
		{
			Name = "egress_duration_minutes",
			Parent = "cargoship",
			FullName = "cargoship.egress_duration_minutes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => CargoShip.egress_duration_minutes.ToString(),
			SetOveride = (string str) => CargoShip.egress_duration_minutes = str.ToFloat(0f)
		};
		command[6] = new ConsoleSystem.Command()
		{
			Name = "event_duration_minutes",
			Parent = "cargoship",
			FullName = "cargoship.event_duration_minutes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => CargoShip.event_duration_minutes.ToString(),
			SetOveride = (string str) => CargoShip.event_duration_minutes = str.ToFloat(0f)
		};
		command[7] = new ConsoleSystem.Command()
		{
			Name = "event_enabled",
			Parent = "cargoship",
			FullName = "cargoship.event_enabled",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => CargoShip.event_enabled.ToString(),
			SetOveride = (string str) => CargoShip.event_enabled = str.ToBool()
		};
		command[8] = new ConsoleSystem.Command()
		{
			Name = "loot_round_spacing_minutes",
			Parent = "cargoship",
			FullName = "cargoship.loot_round_spacing_minutes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => CargoShip.loot_round_spacing_minutes.ToString(),
			SetOveride = (string str) => CargoShip.loot_round_spacing_minutes = str.ToFloat(0f)
		};
		command[9] = new ConsoleSystem.Command()
		{
			Name = "loot_rounds",
			Parent = "cargoship",
			FullName = "cargoship.loot_rounds",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => CargoShip.loot_rounds.ToString(),
			SetOveride = (string str) => CargoShip.loot_rounds = str.ToInt(0)
		};
		command[10] = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "chicken",
			FullName = "chicken.population",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Chicken.Population.ToString(),
			SetOveride = (string str) => Chicken.Population = str.ToFloat(0f)
		};
		command[11] = new ConsoleSystem.Command()
		{
			Name = "clothloddist",
			Parent = "clothlod",
			FullName = "clothlod.clothloddist",
			ServerAdmin = true,
			Description = "distance cloth will simulate until",
			Variable = true,
			GetOveride = () => ClothLOD.clothLODDist.ToString(),
			SetOveride = (string str) => ClothLOD.clothLODDist = str.ToFloat(0f)
		};
		command[12] = new ConsoleSystem.Command()
		{
			Name = "echo",
			Parent = "commands",
			FullName = "commands.echo",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Commands.Echo(arg.FullString)
		};
		command[13] = new ConsoleSystem.Command()
		{
			Name = "find",
			Parent = "commands",
			FullName = "commands.find",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Commands.Find(arg)
		};
		command[14] = new ConsoleSystem.Command()
		{
			Name = "ban",
			Parent = "global",
			FullName = "global.ban",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.ban(arg)
		};
		command[15] = new ConsoleSystem.Command()
		{
			Name = "banid",
			Parent = "global",
			FullName = "global.banid",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.banid(arg)
		};
		command[16] = new ConsoleSystem.Command()
		{
			Name = "banlist",
			Parent = "global",
			FullName = "global.banlist",
			ServerAdmin = true,
			Description = "List of banned users (sourceds compat)",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.banlist(arg)
		};
		command[17] = new ConsoleSystem.Command()
		{
			Name = "banlistex",
			Parent = "global",
			FullName = "global.banlistex",
			ServerAdmin = true,
			Description = "List of banned users - shows reasons and usernames",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.banlistex(arg)
		};
		command[18] = new ConsoleSystem.Command()
		{
			Name = "bans",
			Parent = "global",
			FullName = "global.bans",
			ServerAdmin = true,
			Description = "List of banned users",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Admin.Bans())
		};
		command[19] = new ConsoleSystem.Command()
		{
			Name = "buildinfo",
			Parent = "global",
			FullName = "global.buildinfo",
			ServerAdmin = true,
			Description = "Get information about this build",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Admin.BuildInfo())
		};
		command[20] = new ConsoleSystem.Command()
		{
			Name = "clientperf",
			Parent = "global",
			FullName = "global.clientperf",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.clientperf(arg)
		};
		command[21] = new ConsoleSystem.Command()
		{
			Name = "entid",
			Parent = "global",
			FullName = "global.entid",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.entid(arg)
		};
		command[22] = new ConsoleSystem.Command()
		{
			Name = "kick",
			Parent = "global",
			FullName = "global.kick",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.kick(arg)
		};
		command[23] = new ConsoleSystem.Command()
		{
			Name = "kickall",
			Parent = "global",
			FullName = "global.kickall",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.kickall(arg)
		};
		command[24] = new ConsoleSystem.Command()
		{
			Name = "listid",
			Parent = "global",
			FullName = "global.listid",
			ServerAdmin = true,
			Description = "List of banned users, by ID (sourceds compat)",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.listid(arg)
		};
		command[25] = new ConsoleSystem.Command()
		{
			Name = "moderatorid",
			Parent = "global",
			FullName = "global.moderatorid",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.moderatorid(arg)
		};
		command[26] = new ConsoleSystem.Command()
		{
			Name = "mutechat",
			Parent = "global",
			FullName = "global.mutechat",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.mutechat(arg)
		};
		command[27] = new ConsoleSystem.Command()
		{
			Name = "mutevoice",
			Parent = "global",
			FullName = "global.mutevoice",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.mutevoice(arg)
		};
		command[28] = new ConsoleSystem.Command()
		{
			Name = "ownerid",
			Parent = "global",
			FullName = "global.ownerid",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.ownerid(arg)
		};
		command[29] = new ConsoleSystem.Command()
		{
			Name = "playerlist",
			Parent = "global",
			FullName = "global.playerlist",
			ServerAdmin = true,
			Description = "Get a list of players",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Admin.playerlist())
		};
		command[30] = new ConsoleSystem.Command()
		{
			Name = "players",
			Parent = "global",
			FullName = "global.players",
			ServerAdmin = true,
			Description = "Print out currently connected clients etc",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.players(arg)
		};
		command[31] = new ConsoleSystem.Command()
		{
			Name = "removemoderator",
			Parent = "global",
			FullName = "global.removemoderator",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.removemoderator(arg)
		};
		command[32] = new ConsoleSystem.Command()
		{
			Name = "removeowner",
			Parent = "global",
			FullName = "global.removeowner",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.removeowner(arg)
		};
		command[33] = new ConsoleSystem.Command()
		{
			Name = "say",
			Parent = "global",
			FullName = "global.say",
			ServerAdmin = true,
			Description = "Sends a message in chat",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.say(arg)
		};
		command[34] = new ConsoleSystem.Command()
		{
			Name = "serverinfo",
			Parent = "global",
			FullName = "global.serverinfo",
			ServerAdmin = true,
			Description = "Get a list of information about the server",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Admin.ServerInfo())
		};
		command[35] = new ConsoleSystem.Command()
		{
			Name = "skipqueue",
			Parent = "global",
			FullName = "global.skipqueue",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.skipqueue(arg)
		};
		command[36] = new ConsoleSystem.Command()
		{
			Name = "stats",
			Parent = "global",
			FullName = "global.stats",
			ServerAdmin = true,
			Description = "Print out stats of currently connected clients",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.stats(arg)
		};
		command[37] = new ConsoleSystem.Command()
		{
			Name = "status",
			Parent = "global",
			FullName = "global.status",
			ServerAdmin = true,
			Description = "Print out currently connected clients",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.status(arg)
		};
		command[38] = new ConsoleSystem.Command()
		{
			Name = "unban",
			Parent = "global",
			FullName = "global.unban",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.unban(arg)
		};
		command[39] = new ConsoleSystem.Command()
		{
			Name = "unmutechat",
			Parent = "global",
			FullName = "global.unmutechat",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.unmutechat(arg)
		};
		command[40] = new ConsoleSystem.Command()
		{
			Name = "unmutevoice",
			Parent = "global",
			FullName = "global.unmutevoice",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.unmutevoice(arg)
		};
		command[41] = new ConsoleSystem.Command()
		{
			Name = "users",
			Parent = "global",
			FullName = "global.users",
			ServerAdmin = true,
			Description = "Show user info for players on server.",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Admin.users(arg)
		};
		command[42] = new ConsoleSystem.Command()
		{
			Name = "aidebug_loadbalanceoverduereportserver",
			Parent = "ai",
			FullName = "ai.aidebug_loadbalanceoverduereportserver",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => AI.aiDebug_LoadBalanceOverdueReportServer(arg)
		};
		command[43] = new ConsoleSystem.Command()
		{
			Name = "aidebug_toggle",
			Parent = "ai",
			FullName = "ai.aidebug_toggle",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => AI.aiDebug_toggle(arg)
		};
		command[44] = new ConsoleSystem.Command()
		{
			Name = "ailoadbalancerupdateinterval",
			Parent = "ai",
			FullName = "ai.ailoadbalancerupdateinterval",
			ServerAdmin = true,
			Description = "Set the update interval for the behaviour ai of animals and npcs. (Default: 0.25)",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => AI.aiLoadBalancerUpdateInterval(arg)
		};
		command[45] = new ConsoleSystem.Command()
		{
			Name = "aimanagerloadbalancerupdateinterval",
			Parent = "ai",
			FullName = "ai.aimanagerloadbalancerupdateinterval",
			ServerAdmin = true,
			Description = "Set the update interval for the agency of dormant and active animals and npcs. (Default: 2.0)",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => AI.aiManagerLoadBalancerUpdateInterval(arg)
		};
		command[46] = new ConsoleSystem.Command()
		{
			Name = "animal_ignore_food",
			Parent = "ai",
			FullName = "ai.animal_ignore_food",
			ServerAdmin = true,
			Description = "If animal_ignore_food is true, animals will not sense food sources or interact with them (server optimization). (default: true)",
			Variable = true,
			GetOveride = () => AI.animal_ignore_food.ToString(),
			SetOveride = (string str) => AI.animal_ignore_food = str.ToBool()
		};
		command[47] = new ConsoleSystem.Command()
		{
			Name = "animalsenseloadbalancerupdateinterval",
			Parent = "ai",
			FullName = "ai.animalsenseloadbalancerupdateinterval",
			ServerAdmin = true,
			Description = "Set the update interval for animal senses that updates the knowledge gathering of animals. (Default: 0.2)",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => AI.AnimalSenseLoadBalancerUpdateInterval(arg)
		};
		command[48] = new ConsoleSystem.Command()
		{
			Name = "defaultloadbalancerupdateinterval",
			Parent = "ai",
			FullName = "ai.defaultloadbalancerupdateinterval",
			ServerAdmin = true,
			Description = "Set the update interval for the default load balancer, currently used for cover point generation. (Default: 2.5)",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => AI.defaultLoadBalancerUpdateInterval(arg)
		};
		command[49] = new ConsoleSystem.Command()
		{
			Name = "frametime",
			Parent = "ai",
			FullName = "ai.frametime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => AI.frametime.ToString(),
			SetOveride = (string str) => AI.frametime = str.ToFloat(0f)
		};
		command[50] = new ConsoleSystem.Command()
		{
			Name = "ignoreplayers",
			Parent = "ai",
			FullName = "ai.ignoreplayers",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => AI.ignoreplayers.ToString(),
			SetOveride = (string str) => AI.ignoreplayers = str.ToBool()
		};
		command[51] = new ConsoleSystem.Command()
		{
			Name = "move",
			Parent = "ai",
			FullName = "ai.move",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => AI.move.ToString(),
			SetOveride = (string str) => AI.move = str.ToBool()
		};
		command[52] = new ConsoleSystem.Command()
		{
			Name = "nav_carve_height",
			Parent = "ai",
			FullName = "ai.nav_carve_height",
			ServerAdmin = true,
			Description = "The height of the carve volume. (default: 2)",
			Variable = true,
			GetOveride = () => AI.nav_carve_height.ToString(),
			SetOveride = (string str) => AI.nav_carve_height = str.ToFloat(0f)
		};
		command[53] = new ConsoleSystem.Command()
		{
			Name = "nav_carve_min_base_size",
			Parent = "ai",
			FullName = "ai.nav_carve_min_base_size",
			ServerAdmin = true,
			Description = "The minimum size we allow a carving volume to be. (default: 2)",
			Variable = true,
			GetOveride = () => AI.nav_carve_min_base_size.ToString(),
			SetOveride = (string str) => AI.nav_carve_min_base_size = str.ToFloat(0f)
		};
		command[54] = new ConsoleSystem.Command()
		{
			Name = "nav_carve_min_building_blocks_to_apply_optimization",
			Parent = "ai",
			FullName = "ai.nav_carve_min_building_blocks_to_apply_optimization",
			ServerAdmin = true,
			Description = "The minimum number of building blocks a building needs to consist of for this optimization to be applied. (default: 25)",
			Variable = true,
			GetOveride = () => AI.nav_carve_min_building_blocks_to_apply_optimization.ToString(),
			SetOveride = (string str) => AI.nav_carve_min_building_blocks_to_apply_optimization = str.ToInt(0)
		};
		command[55] = new ConsoleSystem.Command()
		{
			Name = "nav_carve_size_multiplier",
			Parent = "ai",
			FullName = "ai.nav_carve_size_multiplier",
			ServerAdmin = true,
			Description = "The size multiplier applied to the size of the carve volume. The smaller the value, the tighter the skirt around foundation edges, but too small and animals can attack through walls. (default: 4)",
			Variable = true,
			GetOveride = () => AI.nav_carve_size_multiplier.ToString(),
			SetOveride = (string str) => AI.nav_carve_size_multiplier = str.ToFloat(0f)
		};
		command[56] = new ConsoleSystem.Command()
		{
			Name = "nav_carve_use_building_optimization",
			Parent = "ai",
			FullName = "ai.nav_carve_use_building_optimization",
			ServerAdmin = true,
			Description = "If nav_carve_use_building_optimization is true, we attempt to reduce the amount of navmesh carves for a building. (default: false)",
			Variable = true,
			GetOveride = () => AI.nav_carve_use_building_optimization.ToString(),
			SetOveride = (string str) => AI.nav_carve_use_building_optimization = str.ToBool()
		};
		command[57] = new ConsoleSystem.Command()
		{
			Name = "npc_alertness_drain_rate",
			Parent = "ai",
			FullName = "ai.npc_alertness_drain_rate",
			ServerAdmin = true,
			Description = "npc_alertness_drain_rate define the rate at which we drain the alertness level of an NPC when there are no enemies in sight. (Default: 0.01)",
			Variable = true,
			GetOveride = () => AI.npc_alertness_drain_rate.ToString(),
			SetOveride = (string str) => AI.npc_alertness_drain_rate = str.ToFloat(0f)
		};
		command[58] = new ConsoleSystem.Command()
		{
			Name = "npc_alertness_to_aim_modifier",
			Parent = "ai",
			FullName = "ai.npc_alertness_to_aim_modifier",
			ServerAdmin = true,
			Description = "This is multiplied with the current alertness (0-10) to decide how long it will take for the NPC to deliberately miss again. (default: 0.33)",
			Variable = true,
			GetOveride = () => AI.npc_alertness_to_aim_modifier.ToString(),
			SetOveride = (string str) => AI.npc_alertness_to_aim_modifier = str.ToFloat(0f)
		};
		command[59] = new ConsoleSystem.Command()
		{
			Name = "npc_alertness_zero_detection_mod",
			Parent = "ai",
			FullName = "ai.npc_alertness_zero_detection_mod",
			ServerAdmin = true,
			Description = "npc_alertness_zero_detection_mod define the threshold of visibility required to detect an enemy when alertness is zero. (Default: 0.5)",
			Variable = true,
			GetOveride = () => AI.npc_alertness_zero_detection_mod.ToString(),
			SetOveride = (string str) => AI.npc_alertness_zero_detection_mod = str.ToFloat(0f)
		};
		command[60] = new ConsoleSystem.Command()
		{
			Name = "npc_cover_compromised_cooldown",
			Parent = "ai",
			FullName = "ai.npc_cover_compromised_cooldown",
			ServerAdmin = true,
			Description = "npc_cover_compromised_cooldown defines how long a cover point is marked as compromised before it's cleared again for selection. (default: 10)",
			Variable = true,
			GetOveride = () => AI.npc_cover_compromised_cooldown.ToString(),
			SetOveride = (string str) => AI.npc_cover_compromised_cooldown = str.ToFloat(0f)
		};
		command[61] = new ConsoleSystem.Command()
		{
			Name = "npc_cover_info_tick_rate_multiplier",
			Parent = "ai",
			FullName = "ai.npc_cover_info_tick_rate_multiplier",
			ServerAdmin = true,
			Description = "The rate at which we gather information about available cover points. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 20)",
			Variable = true,
			GetOveride = () => AI.npc_cover_info_tick_rate_multiplier.ToString(),
			SetOveride = (string str) => AI.npc_cover_info_tick_rate_multiplier = str.ToFloat(0f)
		};
		command[62] = new ConsoleSystem.Command()
		{
			Name = "npc_cover_path_vs_straight_dist_max_diff",
			Parent = "ai",
			FullName = "ai.npc_cover_path_vs_straight_dist_max_diff",
			ServerAdmin = true,
			Description = "npc_cover_path_vs_straight_dist_max_diff defines what the maximum difference between straight-line distance and path distance can be when evaluating cover points. (default: 2)",
			Variable = true,
			GetOveride = () => AI.npc_cover_path_vs_straight_dist_max_diff.ToString(),
			SetOveride = (string str) => AI.npc_cover_path_vs_straight_dist_max_diff = str.ToFloat(0f)
		};
		command[63] = new ConsoleSystem.Command()
		{
			Name = "npc_cover_use_path_distance",
			Parent = "ai",
			FullName = "ai.npc_cover_use_path_distance",
			ServerAdmin = true,
			Description = "If npc_cover_use_path_distance is set to true then npcs will look at the distance between the cover point and their target using the path between the two, rather than the straight-line distance.",
			Variable = true,
			GetOveride = () => AI.npc_cover_use_path_distance.ToString(),
			SetOveride = (string str) => AI.npc_cover_use_path_distance = str.ToBool()
		};
		command[64] = new ConsoleSystem.Command()
		{
			Name = "npc_deliberate_hit_randomizer",
			Parent = "ai",
			FullName = "ai.npc_deliberate_hit_randomizer",
			ServerAdmin = true,
			Description = "The percentage away from a maximum miss the randomizer is allowed to travel when shooting to deliberately hit the target (we don't want perfect hits with every shot). (default: 0.85f)",
			Variable = true,
			GetOveride = () => AI.npc_deliberate_hit_randomizer.ToString(),
			SetOveride = (string str) => AI.npc_deliberate_hit_randomizer = str.ToFloat(0f)
		};
		command[65] = new ConsoleSystem.Command()
		{
			Name = "npc_deliberate_miss_offset_multiplier",
			Parent = "ai",
			FullName = "ai.npc_deliberate_miss_offset_multiplier",
			ServerAdmin = true,
			Description = "The offset with which the NPC will maximum miss the target. (default: 1.25)",
			Variable = true,
			GetOveride = () => AI.npc_deliberate_miss_offset_multiplier.ToString(),
			SetOveride = (string str) => AI.npc_deliberate_miss_offset_multiplier = str.ToFloat(0f)
		};
		command[66] = new ConsoleSystem.Command()
		{
			Name = "npc_deliberate_miss_to_hit_alignment_time",
			Parent = "ai",
			FullName = "ai.npc_deliberate_miss_to_hit_alignment_time",
			ServerAdmin = true,
			Description = "The time it takes for the NPC to deliberately miss to the time the NPC tries to hit its target. (default: 1.5)",
			Variable = true,
			GetOveride = () => AI.npc_deliberate_miss_to_hit_alignment_time.ToString(),
			SetOveride = (string str) => AI.npc_deliberate_miss_to_hit_alignment_time = str.ToFloat(0f)
		};
		command[67] = new ConsoleSystem.Command()
		{
			Name = "npc_door_trigger_size",
			Parent = "ai",
			FullName = "ai.npc_door_trigger_size",
			ServerAdmin = true,
			Description = "npc_door_trigger_size defines the size of the trigger box on doors that opens the door as npcs walk close to it (default: 1.5)",
			Variable = true,
			GetOveride = () => AI.npc_door_trigger_size.ToString(),
			SetOveride = (string str) => AI.npc_door_trigger_size = str.ToFloat(0f)
		};
		command[68] = new ConsoleSystem.Command()
		{
			Name = "npc_enable",
			Parent = "ai",
			FullName = "ai.npc_enable",
			ServerAdmin = true,
			Description = "If npc_enable is set to false then npcs won't spawn. (default: true)",
			Variable = true,
			GetOveride = () => AI.npc_enable.ToString(),
			SetOveride = (string str) => AI.npc_enable = str.ToBool()
		};
		command[69] = new ConsoleSystem.Command()
		{
			Name = "npc_families_no_hurt",
			Parent = "ai",
			FullName = "ai.npc_families_no_hurt",
			ServerAdmin = true,
			Description = "If npc_families_no_hurt is true, npcs of the same family won't be able to hurt each other. (default: true)",
			Variable = true,
			GetOveride = () => AI.npc_families_no_hurt.ToString(),
			SetOveride = (string str) => AI.npc_families_no_hurt = str.ToBool()
		};
		command[70] = new ConsoleSystem.Command()
		{
			Name = "npc_gun_noise_silencer_modifier",
			Parent = "ai",
			FullName = "ai.npc_gun_noise_silencer_modifier",
			ServerAdmin = true,
			Description = "The modifier by which a silencer reduce the noise that a gun makes when shot. (Default: 0.15)",
			Variable = true,
			GetOveride = () => AI.npc_gun_noise_silencer_modifier.ToString(),
			SetOveride = (string str) => AI.npc_gun_noise_silencer_modifier = str.ToFloat(0f)
		};
		command[71] = new ConsoleSystem.Command()
		{
			Name = "npc_htn_player_base_damage_modifier",
			Parent = "ai",
			FullName = "ai.npc_htn_player_base_damage_modifier",
			ServerAdmin = true,
			Description = "Baseline damage modifier for the new HTN Player NPCs to nerf their damage compared to the old NPCs. (default: 1.15f)",
			Variable = true,
			GetOveride = () => AI.npc_htn_player_base_damage_modifier.ToString(),
			SetOveride = (string str) => AI.npc_htn_player_base_damage_modifier = str.ToFloat(0f)
		};
		command[72] = new ConsoleSystem.Command()
		{
			Name = "npc_htn_player_frustration_threshold",
			Parent = "ai",
			FullName = "ai.npc_htn_player_frustration_threshold",
			ServerAdmin = true,
			Description = "npc_htn_player_frustration_threshold defines where the frustration threshold for NPCs go, where they have the opportunity to change to a more aggressive tactic. (default: 3)",
			Variable = true,
			GetOveride = () => AI.npc_htn_player_frustration_threshold.ToString(),
			SetOveride = (string str) => AI.npc_htn_player_frustration_threshold = str.ToInt(0)
		};
		command[73] = new ConsoleSystem.Command()
		{
			Name = "npc_ignore_chairs",
			Parent = "ai",
			FullName = "ai.npc_ignore_chairs",
			ServerAdmin = true,
			Description = "If npc_ignore_chairs is true, npcs won't care about seeking out and sitting in chairs. (default: true)",
			Variable = true,
			GetOveride = () => AI.npc_ignore_chairs.ToString(),
			SetOveride = (string str) => AI.npc_ignore_chairs = str.ToBool()
		};
		command[74] = new ConsoleSystem.Command()
		{
			Name = "npc_junkpile_a_spawn_chance",
			Parent = "ai",
			FullName = "ai.npc_junkpile_a_spawn_chance",
			ServerAdmin = true,
			Description = "npc_junkpile_a_spawn_chance define the chance for scientists to spawn at junkpile a. (Default: 0.1)",
			Variable = true,
			GetOveride = () => AI.npc_junkpile_a_spawn_chance.ToString(),
			SetOveride = (string str) => AI.npc_junkpile_a_spawn_chance = str.ToFloat(0f)
		};
		command[75] = new ConsoleSystem.Command()
		{
			Name = "npc_junkpile_dist_aggro_gate",
			Parent = "ai",
			FullName = "ai.npc_junkpile_dist_aggro_gate",
			ServerAdmin = true,
			Description = "npc_junkpile_dist_aggro_gate define at what range (or closer) a junkpile scientist will get aggressive. (Default: 8)",
			Variable = true,
			GetOveride = () => AI.npc_junkpile_dist_aggro_gate.ToString(),
			SetOveride = (string str) => AI.npc_junkpile_dist_aggro_gate = str.ToFloat(0f)
		};
		command[76] = new ConsoleSystem.Command()
		{
			Name = "npc_junkpile_g_spawn_chance",
			Parent = "ai",
			FullName = "ai.npc_junkpile_g_spawn_chance",
			ServerAdmin = true,
			Description = "npc_junkpile_g_spawn_chance define the chance for scientists to spawn at junkpile g. (Default: 0.1)",
			Variable = true,
			GetOveride = () => AI.npc_junkpile_g_spawn_chance.ToString(),
			SetOveride = (string str) => AI.npc_junkpile_g_spawn_chance = str.ToFloat(0f)
		};
		command[77] = new ConsoleSystem.Command()
		{
			Name = "npc_max_junkpile_count",
			Parent = "ai",
			FullName = "ai.npc_max_junkpile_count",
			ServerAdmin = true,
			Description = "npc_max_junkpile_count define how many npcs can spawn into the world at junkpiles at the same time (does not include monuments) (Default: 30)",
			Variable = true,
			GetOveride = () => AI.npc_max_junkpile_count.ToString(),
			SetOveride = (string str) => AI.npc_max_junkpile_count = str.ToInt(0)
		};
		command[78] = new ConsoleSystem.Command()
		{
			Name = "npc_max_population_military_tunnels",
			Parent = "ai",
			FullName = "ai.npc_max_population_military_tunnels",
			ServerAdmin = true,
			Description = "npc_max_population_military_tunnels defines the size of the npc population at military tunnels. (default: 3)",
			Variable = true,
			GetOveride = () => AI.npc_max_population_military_tunnels.ToString(),
			SetOveride = (string str) => AI.npc_max_population_military_tunnels = str.ToInt(0)
		};
		command[79] = new ConsoleSystem.Command()
		{
			Name = "npc_max_roam_multiplier",
			Parent = "ai",
			FullName = "ai.npc_max_roam_multiplier",
			ServerAdmin = true,
			Description = "This is multiplied with the max roam range stat of an NPC to determine how far from its spawn point the NPC is allowed to roam. (default: 3)",
			Variable = true,
			GetOveride = () => AI.npc_max_roam_multiplier.ToString(),
			SetOveride = (string str) => AI.npc_max_roam_multiplier = str.ToFloat(0f)
		};
		command[80] = new ConsoleSystem.Command()
		{
			Name = "npc_only_hurt_active_target_in_safezone",
			Parent = "ai",
			FullName = "ai.npc_only_hurt_active_target_in_safezone",
			ServerAdmin = true,
			Description = "If npc_only_hurt_active_target_in_safezone is true, npcs won't any player other than their actively targeted player when in a safe zone. (default: true)",
			Variable = true,
			GetOveride = () => AI.npc_only_hurt_active_target_in_safezone.ToString(),
			SetOveride = (string str) => AI.npc_only_hurt_active_target_in_safezone = str.ToBool()
		};
		command[81] = new ConsoleSystem.Command()
		{
			Name = "npc_patrol_point_cooldown",
			Parent = "ai",
			FullName = "ai.npc_patrol_point_cooldown",
			ServerAdmin = true,
			Description = "npc_patrol_point_cooldown defines the cooldown time on a patrol point until it's available again (default: 5)",
			Variable = true,
			GetOveride = () => AI.npc_patrol_point_cooldown.ToString(),
			SetOveride = (string str) => AI.npc_patrol_point_cooldown = str.ToFloat(0f)
		};
		command[82] = new ConsoleSystem.Command()
		{
			Name = "npc_reasoning_system_tick_rate_multiplier",
			Parent = "ai",
			FullName = "ai.npc_reasoning_system_tick_rate_multiplier",
			ServerAdmin = true,
			Description = "The rate at which we tick the reasoning system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 1)",
			Variable = true,
			GetOveride = () => AI.npc_reasoning_system_tick_rate_multiplier.ToString(),
			SetOveride = (string str) => AI.npc_reasoning_system_tick_rate_multiplier = str.ToFloat(0f)
		};
		command[83] = new ConsoleSystem.Command()
		{
			Name = "npc_respawn_delay_max_military_tunnels",
			Parent = "ai",
			FullName = "ai.npc_respawn_delay_max_military_tunnels",
			ServerAdmin = true,
			Description = "npc_respawn_delay_max_military_tunnels defines the maximum delay between spawn ticks at military tunnels. (default: 1920)",
			Variable = true,
			GetOveride = () => AI.npc_respawn_delay_max_military_tunnels.ToString(),
			SetOveride = (string str) => AI.npc_respawn_delay_max_military_tunnels = str.ToFloat(0f)
		};
		command[84] = new ConsoleSystem.Command()
		{
			Name = "npc_respawn_delay_min_military_tunnels",
			Parent = "ai",
			FullName = "ai.npc_respawn_delay_min_military_tunnels",
			ServerAdmin = true,
			Description = "npc_respawn_delay_min_military_tunnels defines the minimum delay between spawn ticks at military tunnels. (default: 480)",
			Variable = true,
			GetOveride = () => AI.npc_respawn_delay_min_military_tunnels.ToString(),
			SetOveride = (string str) => AI.npc_respawn_delay_min_military_tunnels = str.ToFloat(0f)
		};
		command[85] = new ConsoleSystem.Command()
		{
			Name = "npc_sensory_system_tick_rate_multiplier",
			Parent = "ai",
			FullName = "ai.npc_sensory_system_tick_rate_multiplier",
			ServerAdmin = true,
			Description = "The rate at which we tick the sensory system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 5)",
			Variable = true,
			GetOveride = () => AI.npc_sensory_system_tick_rate_multiplier.ToString(),
			SetOveride = (string str) => AI.npc_sensory_system_tick_rate_multiplier = str.ToFloat(0f)
		};
		command[86] = new ConsoleSystem.Command()
		{
			Name = "npc_spawn_on_cargo_ship",
			Parent = "ai",
			FullName = "ai.npc_spawn_on_cargo_ship",
			ServerAdmin = true,
			Description = "Spawn NPCs on the Cargo Ship. (default: true)",
			Variable = true,
			GetOveride = () => AI.npc_spawn_on_cargo_ship.ToString(),
			SetOveride = (string str) => AI.npc_spawn_on_cargo_ship = str.ToBool()
		};
		command[87] = new ConsoleSystem.Command()
		{
			Name = "npc_spawn_per_tick_max_military_tunnels",
			Parent = "ai",
			FullName = "ai.npc_spawn_per_tick_max_military_tunnels",
			ServerAdmin = true,
			Description = "npc_spawn_per_tick_max_military_tunnels defines how many can maximum spawn at once at military tunnels. (default: 1)",
			Variable = true,
			GetOveride = () => AI.npc_spawn_per_tick_max_military_tunnels.ToString(),
			SetOveride = (string str) => AI.npc_spawn_per_tick_max_military_tunnels = str.ToInt(0)
		};
		command[88] = new ConsoleSystem.Command()
		{
			Name = "npc_spawn_per_tick_min_military_tunnels",
			Parent = "ai",
			FullName = "ai.npc_spawn_per_tick_min_military_tunnels",
			ServerAdmin = true,
			Description = "npc_spawn_per_tick_min_military_tunnels defineshow many will minimum spawn at once at military tunnels. (default: 1)",
			Variable = true,
			GetOveride = () => AI.npc_spawn_per_tick_min_military_tunnels.ToString(),
			SetOveride = (string str) => AI.npc_spawn_per_tick_min_military_tunnels = str.ToInt(0)
		};
		command[89] = new ConsoleSystem.Command()
		{
			Name = "npc_speed_crouch_run",
			Parent = "ai",
			FullName = "ai.npc_speed_crouch_run",
			ServerAdmin = true,
			Description = "npc_speed_crouch_run define the speed of an npc when in the crouched run state, and should be a number between 0 and 1. (Default: 0.25)",
			Variable = true,
			GetOveride = () => AI.npc_speed_crouch_run.ToString(),
			SetOveride = (string str) => AI.npc_speed_crouch_run = str.ToFloat(0f)
		};
		command[90] = new ConsoleSystem.Command()
		{
			Name = "npc_speed_crouch_walk",
			Parent = "ai",
			FullName = "ai.npc_speed_crouch_walk",
			ServerAdmin = true,
			Description = "npc_speed_walk define the speed of an npc when in the crouched walk state, and should be a number between 0 and 1. (Default: 0.1)",
			Variable = true,
			GetOveride = () => AI.npc_speed_crouch_walk.ToString(),
			SetOveride = (string str) => AI.npc_speed_crouch_walk = str.ToFloat(0f)
		};
		command[91] = new ConsoleSystem.Command()
		{
			Name = "npc_speed_run",
			Parent = "ai",
			FullName = "ai.npc_speed_run",
			ServerAdmin = true,
			Description = "npc_speed_walk define the speed of an npc when in the run state, and should be a number between 0 and 1. (Default: 0.4)",
			Variable = true,
			GetOveride = () => AI.npc_speed_run.ToString(),
			SetOveride = (string str) => AI.npc_speed_run = str.ToFloat(0f)
		};
		command[92] = new ConsoleSystem.Command()
		{
			Name = "npc_speed_sprint",
			Parent = "ai",
			FullName = "ai.npc_speed_sprint",
			ServerAdmin = true,
			Description = "npc_speed_walk define the speed of an npc when in the sprint state, and should be a number between 0 and 1. (Default: 1.0)",
			Variable = true,
			GetOveride = () => AI.npc_speed_sprint.ToString(),
			SetOveride = (string str) => AI.npc_speed_sprint = str.ToFloat(0f)
		};
		command[93] = new ConsoleSystem.Command()
		{
			Name = "npc_speed_walk",
			Parent = "ai",
			FullName = "ai.npc_speed_walk",
			ServerAdmin = true,
			Description = "npc_speed_walk define the speed of an npc when in the walk state, and should be a number between 0 and 1. (Default: 0.18)",
			Variable = true,
			GetOveride = () => AI.npc_speed_walk.ToString(),
			SetOveride = (string str) => AI.npc_speed_walk = str.ToFloat(0f)
		};
		command[94] = new ConsoleSystem.Command()
		{
			Name = "npc_use_new_aim_system",
			Parent = "ai",
			FullName = "ai.npc_use_new_aim_system",
			ServerAdmin = true,
			Description = "If npc_use_new_aim_system is true, npcs will miss on purpose on occasion, where the old system would randomize aim cone. (default: true)",
			Variable = true,
			GetOveride = () => AI.npc_use_new_aim_system.ToString(),
			SetOveride = (string str) => AI.npc_use_new_aim_system = str.ToBool()
		};
		command[95] = new ConsoleSystem.Command()
		{
			Name = "npc_use_thrown_weapons",
			Parent = "ai",
			FullName = "ai.npc_use_thrown_weapons",
			ServerAdmin = true,
			Description = "If npc_use_thrown_weapons is true, npcs will throw grenades, etc. This is an experimental feature. (default: true)",
			Variable = true,
			GetOveride = () => AI.npc_use_thrown_weapons.ToString(),
			SetOveride = (string str) => AI.npc_use_thrown_weapons = str.ToBool()
		};
		command[96] = new ConsoleSystem.Command()
		{
			Name = "npc_valid_aim_cone",
			Parent = "ai",
			FullName = "ai.npc_valid_aim_cone",
			ServerAdmin = true,
			Description = "npc_valid_aim_cone defines how close their aim needs to be on target in order to fire. (default: 0.8)",
			Variable = true,
			GetOveride = () => AI.npc_valid_aim_cone.ToString(),
			SetOveride = (string str) => AI.npc_valid_aim_cone = str.ToFloat(0f)
		};
		command[97] = new ConsoleSystem.Command()
		{
			Name = "npc_valid_mounted_aim_cone",
			Parent = "ai",
			FullName = "ai.npc_valid_mounted_aim_cone",
			ServerAdmin = true,
			Description = "npc_valid_mounted_aim_cone defines how close their aim needs to be on target in order to fire while mounted. (default: 0.92)",
			Variable = true,
			GetOveride = () => AI.npc_valid_mounted_aim_cone.ToString(),
			SetOveride = (string str) => AI.npc_valid_mounted_aim_cone = str.ToFloat(0f)
		};
		command[98] = new ConsoleSystem.Command()
		{
			Name = "npcsenseloadbalancerupdateinterval",
			Parent = "ai",
			FullName = "ai.npcsenseloadbalancerupdateinterval",
			ServerAdmin = true,
			Description = "Set the update interval for npc senses that updates the knowledge gathering of npcs. (Default: 0.2)",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => AI.NpcSenseLoadBalancerUpdateInterval(arg)
		};
		command[99] = new ConsoleSystem.Command()
		{
			Name = "ocean_patrol_path_iterations",
			Parent = "ai",
			FullName = "ai.ocean_patrol_path_iterations",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => AI.ocean_patrol_path_iterations.ToString(),
			SetOveride = (string str) => AI.ocean_patrol_path_iterations = str.ToInt(0)
		};
		command[100] = new ConsoleSystem.Command()
		{
			Name = "selectnpclookatserver",
			Parent = "ai",
			FullName = "ai.selectnpclookatserver",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => AI.selectNPCLookatServer(arg)
		};
		command[101] = new ConsoleSystem.Command()
		{
			Name = "sensetime",
			Parent = "ai",
			FullName = "ai.sensetime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => AI.sensetime.ToString(),
			SetOveride = (string str) => AI.sensetime = str.ToFloat(0f)
		};
		command[102] = new ConsoleSystem.Command()
		{
			Name = "think",
			Parent = "ai",
			FullName = "ai.think",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => AI.think.ToString(),
			SetOveride = (string str) => AI.think = str.ToBool()
		};
		command[103] = new ConsoleSystem.Command()
		{
			Name = "tickrate",
			Parent = "ai",
			FullName = "ai.tickrate",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => AI.tickrate.ToString(),
			SetOveride = (string str) => AI.tickrate = str.ToFloat(0f)
		};
		command[104] = new ConsoleSystem.Command()
		{
			Name = "admincheat",
			Parent = "antihack",
			FullName = "antihack.admincheat",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.admincheat.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.admincheat = str.ToBool()
		};
		command[105] = new ConsoleSystem.Command()
		{
			Name = "debuglevel",
			Parent = "antihack",
			FullName = "antihack.debuglevel",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.debuglevel.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.debuglevel = str.ToInt(0)
		};
		command[106] = new ConsoleSystem.Command()
		{
			Name = "enforcementlevel",
			Parent = "antihack",
			FullName = "antihack.enforcementlevel",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.enforcementlevel.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.enforcementlevel = str.ToInt(0)
		};
		command[107] = new ConsoleSystem.Command()
		{
			Name = "eye_clientframes",
			Parent = "antihack",
			FullName = "antihack.eye_clientframes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.eye_clientframes.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.eye_clientframes = str.ToFloat(0f)
		};
		command[108] = new ConsoleSystem.Command()
		{
			Name = "eye_forgiveness",
			Parent = "antihack",
			FullName = "antihack.eye_forgiveness",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.eye_forgiveness.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.eye_forgiveness = str.ToFloat(0f)
		};
		command[109] = new ConsoleSystem.Command()
		{
			Name = "eye_penalty",
			Parent = "antihack",
			FullName = "antihack.eye_penalty",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.eye_penalty.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.eye_penalty = str.ToFloat(0f)
		};
		command[110] = new ConsoleSystem.Command()
		{
			Name = "eye_protection",
			Parent = "antihack",
			FullName = "antihack.eye_protection",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.eye_protection.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.eye_protection = str.ToInt(0)
		};
		command[111] = new ConsoleSystem.Command()
		{
			Name = "eye_serverframes",
			Parent = "antihack",
			FullName = "antihack.eye_serverframes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.eye_serverframes.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.eye_serverframes = str.ToFloat(0f)
		};
		command[112] = new ConsoleSystem.Command()
		{
			Name = "flyhack_extrusion",
			Parent = "antihack",
			FullName = "antihack.flyhack_extrusion",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_extrusion.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_extrusion = str.ToFloat(0f)
		};
		command[113] = new ConsoleSystem.Command()
		{
			Name = "flyhack_forgiveness_horizontal",
			Parent = "antihack",
			FullName = "antihack.flyhack_forgiveness_horizontal",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_horizontal.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_forgiveness_horizontal = str.ToFloat(0f)
		};
		command[114] = new ConsoleSystem.Command()
		{
			Name = "flyhack_forgiveness_vertical",
			Parent = "antihack",
			FullName = "antihack.flyhack_forgiveness_vertical",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_vertical.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_forgiveness_vertical = str.ToFloat(0f)
		};
		command[115] = new ConsoleSystem.Command()
		{
			Name = "flyhack_margin",
			Parent = "antihack",
			FullName = "antihack.flyhack_margin",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_margin.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_margin = str.ToFloat(0f)
		};
		command[116] = new ConsoleSystem.Command()
		{
			Name = "flyhack_maxsteps",
			Parent = "antihack",
			FullName = "antihack.flyhack_maxsteps",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_maxsteps.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_maxsteps = str.ToInt(0)
		};
		command[117] = new ConsoleSystem.Command()
		{
			Name = "flyhack_penalty",
			Parent = "antihack",
			FullName = "antihack.flyhack_penalty",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_penalty.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_penalty = str.ToFloat(0f)
		};
		command[118] = new ConsoleSystem.Command()
		{
			Name = "flyhack_protection",
			Parent = "antihack",
			FullName = "antihack.flyhack_protection",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_protection.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_protection = str.ToInt(0)
		};
		command[119] = new ConsoleSystem.Command()
		{
			Name = "flyhack_reject",
			Parent = "antihack",
			FullName = "antihack.flyhack_reject",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_reject.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_reject = str.ToBool()
		};
		command[120] = new ConsoleSystem.Command()
		{
			Name = "flyhack_stepsize",
			Parent = "antihack",
			FullName = "antihack.flyhack_stepsize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.flyhack_stepsize.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.flyhack_stepsize = str.ToFloat(0f)
		};
		command[121] = new ConsoleSystem.Command()
		{
			Name = "forceposition",
			Parent = "antihack",
			FullName = "antihack.forceposition",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.forceposition.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.forceposition = str.ToBool()
		};
		command[122] = new ConsoleSystem.Command()
		{
			Name = "maxdeltatime",
			Parent = "antihack",
			FullName = "antihack.maxdeltatime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.maxdeltatime.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.maxdeltatime = str.ToFloat(0f)
		};
		command[123] = new ConsoleSystem.Command()
		{
			Name = "maxdesync",
			Parent = "antihack",
			FullName = "antihack.maxdesync",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.maxdesync.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.maxdesync = str.ToFloat(0f)
		};
		command[124] = new ConsoleSystem.Command()
		{
			Name = "maxviolation",
			Parent = "antihack",
			FullName = "antihack.maxviolation",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.maxviolation.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.maxviolation = str.ToFloat(0f)
		};
		command[125] = new ConsoleSystem.Command()
		{
			Name = "melee_clientframes",
			Parent = "antihack",
			FullName = "antihack.melee_clientframes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.melee_clientframes.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.melee_clientframes = str.ToFloat(0f)
		};
		command[126] = new ConsoleSystem.Command()
		{
			Name = "melee_forgiveness",
			Parent = "antihack",
			FullName = "antihack.melee_forgiveness",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.melee_forgiveness.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.melee_forgiveness = str.ToFloat(0f)
		};
		command[127] = new ConsoleSystem.Command()
		{
			Name = "melee_penalty",
			Parent = "antihack",
			FullName = "antihack.melee_penalty",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.melee_penalty.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.melee_penalty = str.ToFloat(0f)
		};
		command[128] = new ConsoleSystem.Command()
		{
			Name = "melee_protection",
			Parent = "antihack",
			FullName = "antihack.melee_protection",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.melee_protection.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.melee_protection = str.ToInt(0)
		};
		command[129] = new ConsoleSystem.Command()
		{
			Name = "melee_serverframes",
			Parent = "antihack",
			FullName = "antihack.melee_serverframes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.melee_serverframes.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.melee_serverframes = str.ToFloat(0f)
		};
		command[130] = new ConsoleSystem.Command()
		{
			Name = "modelstate",
			Parent = "antihack",
			FullName = "antihack.modelstate",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.modelstate.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.modelstate = str.ToBool()
		};
		command[131] = new ConsoleSystem.Command()
		{
			Name = "noclip_backtracking",
			Parent = "antihack",
			FullName = "antihack.noclip_backtracking",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.noclip_backtracking.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.noclip_backtracking = str.ToFloat(0f)
		};
		command[132] = new ConsoleSystem.Command()
		{
			Name = "noclip_margin",
			Parent = "antihack",
			FullName = "antihack.noclip_margin",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.noclip_margin.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.noclip_margin = str.ToFloat(0f)
		};
		command[133] = new ConsoleSystem.Command()
		{
			Name = "noclip_maxsteps",
			Parent = "antihack",
			FullName = "antihack.noclip_maxsteps",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.noclip_maxsteps.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.noclip_maxsteps = str.ToInt(0)
		};
		command[134] = new ConsoleSystem.Command()
		{
			Name = "noclip_penalty",
			Parent = "antihack",
			FullName = "antihack.noclip_penalty",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.noclip_penalty.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.noclip_penalty = str.ToFloat(0f)
		};
		command[135] = new ConsoleSystem.Command()
		{
			Name = "noclip_protection",
			Parent = "antihack",
			FullName = "antihack.noclip_protection",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.noclip_protection.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.noclip_protection = str.ToInt(0)
		};
		command[136] = new ConsoleSystem.Command()
		{
			Name = "noclip_reject",
			Parent = "antihack",
			FullName = "antihack.noclip_reject",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.noclip_reject.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.noclip_reject = str.ToBool()
		};
		command[137] = new ConsoleSystem.Command()
		{
			Name = "noclip_stepsize",
			Parent = "antihack",
			FullName = "antihack.noclip_stepsize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.noclip_stepsize.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.noclip_stepsize = str.ToFloat(0f)
		};
		command[138] = new ConsoleSystem.Command()
		{
			Name = "objectplacement",
			Parent = "antihack",
			FullName = "antihack.objectplacement",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.objectplacement.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.objectplacement = str.ToBool()
		};
		command[139] = new ConsoleSystem.Command()
		{
			Name = "projectile_clientframes",
			Parent = "antihack",
			FullName = "antihack.projectile_clientframes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.projectile_clientframes.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.projectile_clientframes = str.ToFloat(0f)
		};
		command[140] = new ConsoleSystem.Command()
		{
			Name = "projectile_forgiveness",
			Parent = "antihack",
			FullName = "antihack.projectile_forgiveness",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.projectile_forgiveness.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.projectile_forgiveness = str.ToFloat(0f)
		};
		command[141] = new ConsoleSystem.Command()
		{
			Name = "projectile_penalty",
			Parent = "antihack",
			FullName = "antihack.projectile_penalty",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.projectile_penalty.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.projectile_penalty = str.ToFloat(0f)
		};
		command[142] = new ConsoleSystem.Command()
		{
			Name = "projectile_protection",
			Parent = "antihack",
			FullName = "antihack.projectile_protection",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.projectile_protection.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.projectile_protection = str.ToInt(0)
		};
		command[143] = new ConsoleSystem.Command()
		{
			Name = "projectile_serverframes",
			Parent = "antihack",
			FullName = "antihack.projectile_serverframes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.projectile_serverframes.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.projectile_serverframes = str.ToFloat(0f)
		};
		command[144] = new ConsoleSystem.Command()
		{
			Name = "projectile_trajectory_horizontal",
			Parent = "antihack",
			FullName = "antihack.projectile_trajectory_horizontal",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.projectile_trajectory_horizontal.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.projectile_trajectory_horizontal = str.ToFloat(0f)
		};
		command[145] = new ConsoleSystem.Command()
		{
			Name = "projectile_trajectory_vertical",
			Parent = "antihack",
			FullName = "antihack.projectile_trajectory_vertical",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.projectile_trajectory_vertical.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.projectile_trajectory_vertical = str.ToFloat(0f)
		};
		command[146] = new ConsoleSystem.Command()
		{
			Name = "relaxationpause",
			Parent = "antihack",
			FullName = "antihack.relaxationpause",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.relaxationpause.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.relaxationpause = str.ToFloat(0f)
		};
		command[147] = new ConsoleSystem.Command()
		{
			Name = "relaxationrate",
			Parent = "antihack",
			FullName = "antihack.relaxationrate",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.relaxationrate.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.relaxationrate = str.ToFloat(0f)
		};
		command[148] = new ConsoleSystem.Command()
		{
			Name = "reporting",
			Parent = "antihack",
			FullName = "antihack.reporting",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.reporting.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.reporting = str.ToBool()
		};
		command[149] = new ConsoleSystem.Command()
		{
			Name = "speedhack_forgiveness",
			Parent = "antihack",
			FullName = "antihack.speedhack_forgiveness",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.speedhack_forgiveness.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.speedhack_forgiveness = str.ToFloat(0f)
		};
		command[150] = new ConsoleSystem.Command()
		{
			Name = "speedhack_penalty",
			Parent = "antihack",
			FullName = "antihack.speedhack_penalty",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.speedhack_penalty.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.speedhack_penalty = str.ToFloat(0f)
		};
		command[151] = new ConsoleSystem.Command()
		{
			Name = "speedhack_protection",
			Parent = "antihack",
			FullName = "antihack.speedhack_protection",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.speedhack_protection.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.speedhack_protection = str.ToInt(0)
		};
		command[152] = new ConsoleSystem.Command()
		{
			Name = "speedhack_reject",
			Parent = "antihack",
			FullName = "antihack.speedhack_reject",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.speedhack_reject.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.speedhack_reject = str.ToBool()
		};
		command[153] = new ConsoleSystem.Command()
		{
			Name = "speedhack_slopespeed",
			Parent = "antihack",
			FullName = "antihack.speedhack_slopespeed",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.speedhack_slopespeed.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.speedhack_slopespeed = str.ToFloat(0f)
		};
		command[154] = new ConsoleSystem.Command()
		{
			Name = "userlevel",
			Parent = "antihack",
			FullName = "antihack.userlevel",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.AntiHack.userlevel.ToString(),
			SetOveride = (string str) => ConVar.AntiHack.userlevel = str.ToInt(0)
		};
		command[155] = new ConsoleSystem.Command()
		{
			Name = "collider_capacity",
			Parent = "batching",
			FullName = "batching.collider_capacity",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Batching.collider_capacity.ToString(),
			SetOveride = (string str) => Batching.collider_capacity = str.ToInt(0)
		};
		command[156] = new ConsoleSystem.Command()
		{
			Name = "collider_submeshes",
			Parent = "batching",
			FullName = "batching.collider_submeshes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Batching.collider_submeshes.ToString(),
			SetOveride = (string str) => Batching.collider_submeshes = str.ToInt(0)
		};
		command[157] = new ConsoleSystem.Command()
		{
			Name = "collider_threading",
			Parent = "batching",
			FullName = "batching.collider_threading",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Batching.collider_threading.ToString(),
			SetOveride = (string str) => Batching.collider_threading = str.ToBool()
		};
		command[158] = new ConsoleSystem.Command()
		{
			Name = "collider_vertices",
			Parent = "batching",
			FullName = "batching.collider_vertices",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Batching.collider_vertices.ToString(),
			SetOveride = (string str) => Batching.collider_vertices = str.ToInt(0)
		};
		command[159] = new ConsoleSystem.Command()
		{
			Name = "colliders",
			Parent = "batching",
			FullName = "batching.colliders",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Batching.colliders.ToString(),
			SetOveride = (string str) => Batching.colliders = str.ToBool()
		};
		command[160] = new ConsoleSystem.Command()
		{
			Name = "print_colliders",
			Parent = "batching",
			FullName = "batching.print_colliders",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Batching.print_colliders(arg)
		};
		command[161] = new ConsoleSystem.Command()
		{
			Name = "refresh_colliders",
			Parent = "batching",
			FullName = "batching.refresh_colliders",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Batching.refresh_colliders(arg)
		};
		command[162] = new ConsoleSystem.Command()
		{
			Name = "verbose",
			Parent = "batching",
			FullName = "batching.verbose",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Batching.verbose.ToString(),
			SetOveride = (string str) => Batching.verbose = str.ToInt(0)
		};
		command[163] = new ConsoleSystem.Command()
		{
			Name = "enabled",
			Parent = "bradley",
			FullName = "bradley.enabled",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Bradley.enabled.ToString(),
			SetOveride = (string str) => Bradley.enabled = str.ToBool()
		};
		command[164] = new ConsoleSystem.Command()
		{
			Name = "quickrespawn",
			Parent = "bradley",
			FullName = "bradley.quickrespawn",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Bradley.quickrespawn(arg)
		};
		command[165] = new ConsoleSystem.Command()
		{
			Name = "respawndelayminutes",
			Parent = "bradley",
			FullName = "bradley.respawndelayminutes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Bradley.respawnDelayMinutes.ToString(),
			SetOveride = (string str) => Bradley.respawnDelayMinutes = str.ToFloat(0f)
		};
		command[166] = new ConsoleSystem.Command()
		{
			Name = "respawndelayvariance",
			Parent = "bradley",
			FullName = "bradley.respawndelayvariance",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Bradley.respawnDelayVariance.ToString(),
			SetOveride = (string str) => Bradley.respawnDelayVariance = str.ToFloat(0f)
		};
		command[167] = new ConsoleSystem.Command()
		{
			Name = "enabled",
			Parent = "chat",
			FullName = "chat.enabled",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Chat.enabled.ToString(),
			SetOveride = (string str) => Chat.enabled = str.ToBool()
		};
		command[168] = new ConsoleSystem.Command()
		{
			Name = "say",
			Parent = "chat",
			FullName = "chat.say",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Chat.say(arg)
		};
		command[169] = new ConsoleSystem.Command()
		{
			Name = "search",
			Parent = "chat",
			FullName = "chat.search",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Chat.search(arg))
		};
		command[170] = new ConsoleSystem.Command()
		{
			Name = "serverlog",
			Parent = "chat",
			FullName = "chat.serverlog",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Chat.serverlog.ToString(),
			SetOveride = (string str) => Chat.serverlog = str.ToBool()
		};
		command[171] = new ConsoleSystem.Command()
		{
			Name = "tail",
			Parent = "chat",
			FullName = "chat.tail",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Chat.tail(arg))
		};
		command[172] = new ConsoleSystem.Command()
		{
			Name = "search",
			Parent = "console",
			FullName = "console.search",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(ConVar.Console.search(arg))
		};
		command[173] = new ConsoleSystem.Command()
		{
			Name = "tail",
			Parent = "console",
			FullName = "console.tail",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(ConVar.Console.tail(arg))
		};
		command[174] = new ConsoleSystem.Command()
		{
			Name = "frameminutes",
			Parent = "construct",
			FullName = "construct.frameminutes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Construct.frameminutes.ToString(),
			SetOveride = (string str) => Construct.frameminutes = str.ToFloat(0f)
		};
		command[175] = new ConsoleSystem.Command()
		{
			Name = "add",
			Parent = "craft",
			FullName = "craft.add",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Craft.@add(arg)
		};
		command[176] = new ConsoleSystem.Command()
		{
			Name = "cancel",
			Parent = "craft",
			FullName = "craft.cancel",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Craft.cancel(arg)
		};
		command[177] = new ConsoleSystem.Command()
		{
			Name = "canceltask",
			Parent = "craft",
			FullName = "craft.canceltask",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Craft.canceltask(arg)
		};
		command[178] = new ConsoleSystem.Command()
		{
			Name = "instant",
			Parent = "craft",
			FullName = "craft.instant",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Craft.instant.ToString(),
			SetOveride = (string str) => Craft.instant = str.ToBool()
		};
		command[179] = new ConsoleSystem.Command()
		{
			Name = "export",
			Parent = "data",
			FullName = "data.export",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Data.export(arg)
		};
		command[180] = new ConsoleSystem.Command()
		{
			Name = "breakheld",
			Parent = "debug",
			FullName = "debug.breakheld",
			ServerAdmin = true,
			Description = "Break the current held object",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.breakheld(arg)
		};
		command[181] = new ConsoleSystem.Command()
		{
			Name = "breakitem",
			Parent = "debug",
			FullName = "debug.breakitem",
			ServerAdmin = true,
			Description = "Break all the items in your inventory whose name match the passed string",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.breakitem(arg)
		};
		command[182] = new ConsoleSystem.Command()
		{
			Name = "callbacks",
			Parent = "debug",
			FullName = "debug.callbacks",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Debugging.callbacks.ToString(),
			SetOveride = (string str) => Debugging.callbacks = str.ToBool()
		};
		command[183] = new ConsoleSystem.Command()
		{
			Name = "checktriggers",
			Parent = "debug",
			FullName = "debug.checktriggers",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Debugging.checktriggers.ToString(),
			SetOveride = (string str) => Debugging.checktriggers = str.ToBool()
		};
		command[184] = new ConsoleSystem.Command()
		{
			Name = "disablecondition",
			Parent = "debug",
			FullName = "debug.disablecondition",
			ServerAdmin = true,
			Description = "Do not damage any items",
			Variable = true,
			GetOveride = () => Debugging.disablecondition.ToString(),
			SetOveride = (string str) => Debugging.disablecondition = str.ToBool()
		};
		command[185] = new ConsoleSystem.Command()
		{
			Name = "drink",
			Parent = "debug",
			FullName = "debug.drink",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.drink(arg)
		};
		command[186] = new ConsoleSystem.Command()
		{
			Name = "eat",
			Parent = "debug",
			FullName = "debug.eat",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.eat(arg)
		};
		command[187] = new ConsoleSystem.Command()
		{
			Name = "flushgroup",
			Parent = "debug",
			FullName = "debug.flushgroup",
			ServerAdmin = true,
			Description = "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.flushgroup(arg)
		};
		command[188] = new ConsoleSystem.Command()
		{
			Name = "hurt",
			Parent = "debug",
			FullName = "debug.hurt",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.hurt(arg)
		};
		command[189] = new ConsoleSystem.Command()
		{
			Name = "log",
			Parent = "debug",
			FullName = "debug.log",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Debugging.log.ToString(),
			SetOveride = (string str) => Debugging.log = str.ToBool()
		};
		command[190] = new ConsoleSystem.Command()
		{
			Name = "puzzlereset",
			Parent = "debug",
			FullName = "debug.puzzlereset",
			ServerAdmin = true,
			Description = "reset all puzzles",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.puzzlereset(arg)
		};
		command[191] = new ConsoleSystem.Command()
		{
			Name = "renderinfo",
			Parent = "debug",
			FullName = "debug.renderinfo",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.renderinfo(arg)
		};
		command[192] = new ConsoleSystem.Command()
		{
			Name = "stall",
			Parent = "debug",
			FullName = "debug.stall",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Debugging.stall(arg)
		};
		command[193] = new ConsoleSystem.Command()
		{
			Name = "bracket_0_blockcount",
			Parent = "decay",
			FullName = "decay.bracket_0_blockcount",
			ServerAdmin = true,
			Description = "Between 0 and this value are considered bracket 0 and will cost bracket_0_costfraction per upkeep period to maintain",
			Variable = true,
			GetOveride = () => ConVar.Decay.bracket_0_blockcount.ToString(),
			SetOveride = (string str) => ConVar.Decay.bracket_0_blockcount = str.ToInt(0)
		};
		command[194] = new ConsoleSystem.Command()
		{
			Name = "bracket_0_costfraction",
			Parent = "decay",
			FullName = "decay.bracket_0_costfraction",
			ServerAdmin = true,
			Description = "blocks within bracket 0 will cost this fraction per upkeep period to maintain",
			Variable = true,
			GetOveride = () => ConVar.Decay.bracket_0_costfraction.ToString(),
			SetOveride = (string str) => ConVar.Decay.bracket_0_costfraction = str.ToFloat(0f)
		};
		command[195] = new ConsoleSystem.Command()
		{
			Name = "bracket_1_blockcount",
			Parent = "decay",
			FullName = "decay.bracket_1_blockcount",
			ServerAdmin = true,
			Description = "Between bracket_0_blockcount and this value are considered bracket 1 and will cost bracket_1_costfraction per upkeep period to maintain",
			Variable = true,
			GetOveride = () => ConVar.Decay.bracket_1_blockcount.ToString(),
			SetOveride = (string str) => ConVar.Decay.bracket_1_blockcount = str.ToInt(0)
		};
		command[196] = new ConsoleSystem.Command()
		{
			Name = "bracket_1_costfraction",
			Parent = "decay",
			FullName = "decay.bracket_1_costfraction",
			ServerAdmin = true,
			Description = "blocks within bracket 1 will cost this fraction per upkeep period to maintain",
			Variable = true,
			GetOveride = () => ConVar.Decay.bracket_1_costfraction.ToString(),
			SetOveride = (string str) => ConVar.Decay.bracket_1_costfraction = str.ToFloat(0f)
		};
		command[197] = new ConsoleSystem.Command()
		{
			Name = "bracket_2_blockcount",
			Parent = "decay",
			FullName = "decay.bracket_2_blockcount",
			ServerAdmin = true,
			Description = "Between bracket_1_blockcount and this value are considered bracket 2 and will cost bracket_2_costfraction per upkeep period to maintain",
			Variable = true,
			GetOveride = () => ConVar.Decay.bracket_2_blockcount.ToString(),
			SetOveride = (string str) => ConVar.Decay.bracket_2_blockcount = str.ToInt(0)
		};
		command[198] = new ConsoleSystem.Command()
		{
			Name = "bracket_2_costfraction",
			Parent = "decay",
			FullName = "decay.bracket_2_costfraction",
			ServerAdmin = true,
			Description = "blocks within bracket 2 will cost this fraction per upkeep period to maintain",
			Variable = true,
			GetOveride = () => ConVar.Decay.bracket_2_costfraction.ToString(),
			SetOveride = (string str) => ConVar.Decay.bracket_2_costfraction = str.ToFloat(0f)
		};
		command[199] = new ConsoleSystem.Command()
		{
			Name = "bracket_3_blockcount",
			Parent = "decay",
			FullName = "decay.bracket_3_blockcount",
			ServerAdmin = true,
			Description = "Between bracket_2_blockcount and this value (and beyond) are considered bracket 3 and will cost bracket_3_costfraction per upkeep period to maintain",
			Variable = true,
			GetOveride = () => ConVar.Decay.bracket_3_blockcount.ToString(),
			SetOveride = (string str) => ConVar.Decay.bracket_3_blockcount = str.ToInt(0)
		};
		command[200] = new ConsoleSystem.Command()
		{
			Name = "bracket_3_costfraction",
			Parent = "decay",
			FullName = "decay.bracket_3_costfraction",
			ServerAdmin = true,
			Description = "blocks within bracket 3 will cost this fraction per upkeep period to maintain",
			Variable = true,
			GetOveride = () => ConVar.Decay.bracket_3_costfraction.ToString(),
			SetOveride = (string str) => ConVar.Decay.bracket_3_costfraction = str.ToFloat(0f)
		};
		command[201] = new ConsoleSystem.Command()
		{
			Name = "debug",
			Parent = "decay",
			FullName = "decay.debug",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Decay.debug.ToString(),
			SetOveride = (string str) => ConVar.Decay.debug = str.ToBool()
		};
		command[202] = new ConsoleSystem.Command()
		{
			Name = "delay_metal",
			Parent = "decay",
			FullName = "decay.delay_metal",
			ServerAdmin = true,
			Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.delay_metal.ToString(),
			SetOveride = (string str) => ConVar.Decay.delay_metal = str.ToFloat(0f)
		};
		command[203] = new ConsoleSystem.Command()
		{
			Name = "delay_override",
			Parent = "decay",
			FullName = "decay.delay_override",
			ServerAdmin = true,
			Description = "When set to a value above 0 everything will decay with this delay",
			Variable = true,
			GetOveride = () => ConVar.Decay.delay_override.ToString(),
			SetOveride = (string str) => ConVar.Decay.delay_override = str.ToFloat(0f)
		};
		command[204] = new ConsoleSystem.Command()
		{
			Name = "delay_stone",
			Parent = "decay",
			FullName = "decay.delay_stone",
			ServerAdmin = true,
			Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.delay_stone.ToString(),
			SetOveride = (string str) => ConVar.Decay.delay_stone = str.ToFloat(0f)
		};
		command[205] = new ConsoleSystem.Command()
		{
			Name = "delay_toptier",
			Parent = "decay",
			FullName = "decay.delay_toptier",
			ServerAdmin = true,
			Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.delay_toptier.ToString(),
			SetOveride = (string str) => ConVar.Decay.delay_toptier = str.ToFloat(0f)
		};
		command[206] = new ConsoleSystem.Command()
		{
			Name = "delay_twig",
			Parent = "decay",
			FullName = "decay.delay_twig",
			ServerAdmin = true,
			Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.delay_twig.ToString(),
			SetOveride = (string str) => ConVar.Decay.delay_twig = str.ToFloat(0f)
		};
		command[207] = new ConsoleSystem.Command()
		{
			Name = "delay_wood",
			Parent = "decay",
			FullName = "decay.delay_wood",
			ServerAdmin = true,
			Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.delay_wood.ToString(),
			SetOveride = (string str) => ConVar.Decay.delay_wood = str.ToFloat(0f)
		};
		command[208] = new ConsoleSystem.Command()
		{
			Name = "duration_metal",
			Parent = "decay",
			FullName = "decay.duration_metal",
			ServerAdmin = true,
			Description = "How long should this building grade take to decay when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.duration_metal.ToString(),
			SetOveride = (string str) => ConVar.Decay.duration_metal = str.ToFloat(0f)
		};
		command[209] = new ConsoleSystem.Command()
		{
			Name = "duration_override",
			Parent = "decay",
			FullName = "decay.duration_override",
			ServerAdmin = true,
			Description = "When set to a value above 0 everything will decay with this duration",
			Variable = true,
			GetOveride = () => ConVar.Decay.duration_override.ToString(),
			SetOveride = (string str) => ConVar.Decay.duration_override = str.ToFloat(0f)
		};
		command[210] = new ConsoleSystem.Command()
		{
			Name = "duration_stone",
			Parent = "decay",
			FullName = "decay.duration_stone",
			ServerAdmin = true,
			Description = "How long should this building grade take to decay when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.duration_stone.ToString(),
			SetOveride = (string str) => ConVar.Decay.duration_stone = str.ToFloat(0f)
		};
		command[211] = new ConsoleSystem.Command()
		{
			Name = "duration_toptier",
			Parent = "decay",
			FullName = "decay.duration_toptier",
			ServerAdmin = true,
			Description = "How long should this building grade take to decay when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.duration_toptier.ToString(),
			SetOveride = (string str) => ConVar.Decay.duration_toptier = str.ToFloat(0f)
		};
		command[212] = new ConsoleSystem.Command()
		{
			Name = "duration_twig",
			Parent = "decay",
			FullName = "decay.duration_twig",
			ServerAdmin = true,
			Description = "How long should this building grade take to decay when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.duration_twig.ToString(),
			SetOveride = (string str) => ConVar.Decay.duration_twig = str.ToFloat(0f)
		};
		command[213] = new ConsoleSystem.Command()
		{
			Name = "duration_wood",
			Parent = "decay",
			FullName = "decay.duration_wood",
			ServerAdmin = true,
			Description = "How long should this building grade take to decay when not protected by upkeep, in hours",
			Variable = true,
			GetOveride = () => ConVar.Decay.duration_wood.ToString(),
			SetOveride = (string str) => ConVar.Decay.duration_wood = str.ToFloat(0f)
		};
		command[214] = new ConsoleSystem.Command()
		{
			Name = "outside_test_range",
			Parent = "decay",
			FullName = "decay.outside_test_range",
			ServerAdmin = true,
			Description = "Maximum distance to test to see if a structure is outside, higher values are slower but accurate for huge buildings",
			Variable = true,
			GetOveride = () => ConVar.Decay.outside_test_range.ToString(),
			SetOveride = (string str) => ConVar.Decay.outside_test_range = str.ToFloat(0f)
		};
		command[215] = new ConsoleSystem.Command()
		{
			Name = "scale",
			Parent = "decay",
			FullName = "decay.scale",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Decay.scale.ToString(),
			SetOveride = (string str) => ConVar.Decay.scale = str.ToFloat(0f)
		};
		command[216] = new ConsoleSystem.Command()
		{
			Name = "tick",
			Parent = "decay",
			FullName = "decay.tick",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Decay.tick.ToString(),
			SetOveride = (string str) => ConVar.Decay.tick = str.ToFloat(0f)
		};
		command[217] = new ConsoleSystem.Command()
		{
			Name = "upkeep",
			Parent = "decay",
			FullName = "decay.upkeep",
			ServerAdmin = true,
			Description = "Is upkeep enabled",
			Variable = true,
			GetOveride = () => ConVar.Decay.upkeep.ToString(),
			SetOveride = (string str) => ConVar.Decay.upkeep = str.ToBool()
		};
		command[218] = new ConsoleSystem.Command()
		{
			Name = "upkeep_grief_protection",
			Parent = "decay",
			FullName = "decay.upkeep_grief_protection",
			ServerAdmin = true,
			Description = "How many minutes can the upkeep cost last after the cupboard was destroyed? default : 1440 (24 hours)",
			Variable = true,
			GetOveride = () => ConVar.Decay.upkeep_grief_protection.ToString(),
			SetOveride = (string str) => ConVar.Decay.upkeep_grief_protection = str.ToFloat(0f)
		};
		command[219] = new ConsoleSystem.Command()
		{
			Name = "upkeep_heal_scale",
			Parent = "decay",
			FullName = "decay.upkeep_heal_scale",
			ServerAdmin = true,
			Description = "Scale at which objects heal when upkeep conditions are met, default of 1 is same rate at which they decay",
			Variable = true,
			GetOveride = () => ConVar.Decay.upkeep_heal_scale.ToString(),
			SetOveride = (string str) => ConVar.Decay.upkeep_heal_scale = str.ToFloat(0f)
		};
		command[220] = new ConsoleSystem.Command()
		{
			Name = "upkeep_inside_decay_scale",
			Parent = "decay",
			FullName = "decay.upkeep_inside_decay_scale",
			ServerAdmin = true,
			Description = "Scale at which objects decay when they are inside, default of 0.1",
			Variable = true,
			GetOveride = () => ConVar.Decay.upkeep_inside_decay_scale.ToString(),
			SetOveride = (string str) => ConVar.Decay.upkeep_inside_decay_scale = str.ToFloat(0f)
		};
		command[221] = new ConsoleSystem.Command()
		{
			Name = "upkeep_period_minutes",
			Parent = "decay",
			FullName = "decay.upkeep_period_minutes",
			ServerAdmin = true,
			Description = "How many minutes does the upkeep cost last? default : 1440 (24 hours)",
			Variable = true,
			GetOveride = () => ConVar.Decay.upkeep_period_minutes.ToString(),
			SetOveride = (string str) => ConVar.Decay.upkeep_period_minutes = str.ToFloat(0f)
		};
		command[222] = new ConsoleSystem.Command()
		{
			Name = "debug_toggle",
			Parent = "entity",
			FullName = "entity.debug_toggle",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.debug_toggle(arg)
		};
		command[223] = new ConsoleSystem.Command()
		{
			Name = "deleteby",
			Parent = "entity",
			FullName = "entity.deleteby",
			ServerAdmin = true,
			Description = "Destroy all entities created by this user",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Entity.DeleteBy(arg.GetULong(0, (ulong)0)))
		};
		command[224] = new ConsoleSystem.Command()
		{
			Name = "find_entity",
			Parent = "entity",
			FullName = "entity.find_entity",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.find_entity(arg)
		};
		command[225] = new ConsoleSystem.Command()
		{
			Name = "find_group",
			Parent = "entity",
			FullName = "entity.find_group",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.find_group(arg)
		};
		command[226] = new ConsoleSystem.Command()
		{
			Name = "find_id",
			Parent = "entity",
			FullName = "entity.find_id",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.find_id(arg)
		};
		command[227] = new ConsoleSystem.Command()
		{
			Name = "find_parent",
			Parent = "entity",
			FullName = "entity.find_parent",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.find_parent(arg)
		};
		command[228] = new ConsoleSystem.Command()
		{
			Name = "find_radius",
			Parent = "entity",
			FullName = "entity.find_radius",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.find_radius(arg)
		};
		command[229] = new ConsoleSystem.Command()
		{
			Name = "find_self",
			Parent = "entity",
			FullName = "entity.find_self",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.find_self(arg)
		};
		command[230] = new ConsoleSystem.Command()
		{
			Name = "find_status",
			Parent = "entity",
			FullName = "entity.find_status",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.find_status(arg)
		};
		command[231] = new ConsoleSystem.Command()
		{
			Name = "nudge",
			Parent = "entity",
			FullName = "entity.nudge",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.nudge(arg.GetInt(0, 0))
		};
		command[232] = new ConsoleSystem.Command()
		{
			Name = "spawnlootfrom",
			Parent = "entity",
			FullName = "entity.spawnlootfrom",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Entity.spawnlootfrom(arg)
		};
		command[233] = new ConsoleSystem.Command()
		{
			Name = "spawn",
			Parent = "entity",
			FullName = "entity.spawn",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Entity.svspawn(arg.GetString(0, ""), arg.GetVector3(1, Vector3.zero)))
		};
		command[234] = new ConsoleSystem.Command()
		{
			Name = "spawnitem",
			Parent = "entity",
			FullName = "entity.spawnitem",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Entity.svspawnitem(arg.GetString(0, ""), arg.GetVector3(1, Vector3.zero)))
		};
		command[235] = new ConsoleSystem.Command()
		{
			Name = "addtime",
			Parent = "env",
			FullName = "env.addtime",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Env.addtime(arg)
		};
		command[236] = new ConsoleSystem.Command()
		{
			Name = "day",
			Parent = "env",
			FullName = "env.day",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Env.day.ToString(),
			SetOveride = (string str) => Env.day = str.ToInt(0)
		};
		command[237] = new ConsoleSystem.Command()
		{
			Name = "month",
			Parent = "env",
			FullName = "env.month",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Env.month.ToString(),
			SetOveride = (string str) => Env.month = str.ToInt(0)
		};
		command[238] = new ConsoleSystem.Command()
		{
			Name = "progresstime",
			Parent = "env",
			FullName = "env.progresstime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Env.progresstime.ToString(),
			SetOveride = (string str) => Env.progresstime = str.ToBool()
		};
		command[239] = new ConsoleSystem.Command()
		{
			Name = "time",
			Parent = "env",
			FullName = "env.time",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Env.time.ToString(),
			SetOveride = (string str) => Env.time = str.ToFloat(0f)
		};
		command[240] = new ConsoleSystem.Command()
		{
			Name = "year",
			Parent = "env",
			FullName = "env.year",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Env.year.ToString(),
			SetOveride = (string str) => Env.year = str.ToInt(0)
		};
		command[241] = new ConsoleSystem.Command()
		{
			Name = "limit",
			Parent = "fps",
			FullName = "fps.limit",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => FPS.limit.ToString(),
			SetOveride = (string str) => FPS.limit = str.ToInt(0)
		};
		command[242] = new ConsoleSystem.Command()
		{
			Name = "collect",
			Parent = "gc",
			FullName = "gc.collect",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.GC.collect()
		};
		command[243] = new ConsoleSystem.Command()
		{
			Name = "unload",
			Parent = "gc",
			FullName = "gc.unload",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.GC.unload()
		};
		command[244] = new ConsoleSystem.Command()
		{
			Name = "breakitem",
			Parent = "global",
			FullName = "global.breakitem",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.breakitem(arg)
		};
		command[245] = new ConsoleSystem.Command()
		{
			Name = "colliders",
			Parent = "global",
			FullName = "global.colliders",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.colliders(arg)
		};
		command[246] = new ConsoleSystem.Command()
		{
			Name = "developer",
			Parent = "global",
			FullName = "global.developer",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Global.developer.ToString(),
			SetOveride = (string str) => Global.developer = str.ToInt(0)
		};
		command[247] = new ConsoleSystem.Command()
		{
			Name = "error",
			Parent = "global",
			FullName = "global.error",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.error(arg)
		};
		command[248] = new ConsoleSystem.Command()
		{
			Name = "free",
			Parent = "global",
			FullName = "global.free",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.free(arg)
		};
		command[249] = new ConsoleSystem.Command()
		{
			Name = "injure",
			Parent = "global",
			FullName = "global.injure",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.injure(arg)
		};
		command[250] = new ConsoleSystem.Command()
		{
			Name = "kill",
			Parent = "global",
			FullName = "global.kill",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.kill(arg)
		};
		command[251] = new ConsoleSystem.Command()
		{
			Name = "maxthreads",
			Parent = "global",
			FullName = "global.maxthreads",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Global.maxthreads.ToString(),
			SetOveride = (string str) => Global.maxthreads = str.ToInt(0)
		};
		command[252] = new ConsoleSystem.Command()
		{
			Name = "objects",
			Parent = "global",
			FullName = "global.objects",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.objects(arg)
		};
		command[253] = new ConsoleSystem.Command()
		{
			Name = "perf",
			Parent = "global",
			FullName = "global.perf",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Global.perf.ToString(),
			SetOveride = (string str) => Global.perf = str.ToInt(0)
		};
		command[254] = new ConsoleSystem.Command()
		{
			Name = "queue",
			Parent = "global",
			FullName = "global.queue",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.queue(arg)
		};
		command[255] = new ConsoleSystem.Command()
		{
			Name = "quit",
			Parent = "global",
			FullName = "global.quit",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.quit(arg)
		};
		command[256] = new ConsoleSystem.Command()
		{
			Name = "report",
			Parent = "global",
			FullName = "global.report",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.report(arg)
		};
		command[257] = new ConsoleSystem.Command()
		{
			Name = "respawn",
			Parent = "global",
			FullName = "global.respawn",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.respawn(arg)
		};
		command[258] = new ConsoleSystem.Command()
		{
			Name = "respawn_sleepingbag",
			Parent = "global",
			FullName = "global.respawn_sleepingbag",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.respawn_sleepingbag(arg)
		};
		command[259] = new ConsoleSystem.Command()
		{
			Name = "respawn_sleepingbag_remove",
			Parent = "global",
			FullName = "global.respawn_sleepingbag_remove",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.respawn_sleepingbag_remove(arg)
		};
		command[260] = new ConsoleSystem.Command()
		{
			Name = "restart",
			Parent = "global",
			FullName = "global.restart",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.restart(arg)
		};
		command[261] = new ConsoleSystem.Command()
		{
			Name = "setinfo",
			Parent = "global",
			FullName = "global.setinfo",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.setinfo(arg)
		};
		command[262] = new ConsoleSystem.Command()
		{
			Name = "sleep",
			Parent = "global",
			FullName = "global.sleep",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.sleep(arg)
		};
		command[263] = new ConsoleSystem.Command()
		{
			Name = "spectate",
			Parent = "global",
			FullName = "global.spectate",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.spectate(arg)
		};
		command[264] = new ConsoleSystem.Command()
		{
			Name = "status_sv",
			Parent = "global",
			FullName = "global.status_sv",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.status_sv(arg)
		};
		command[265] = new ConsoleSystem.Command()
		{
			Name = "subscriptions",
			Parent = "global",
			FullName = "global.subscriptions",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.subscriptions(arg)
		};
		command[266] = new ConsoleSystem.Command()
		{
			Name = "sysinfo",
			Parent = "global",
			FullName = "global.sysinfo",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.sysinfo(arg)
		};
		command[267] = new ConsoleSystem.Command()
		{
			Name = "sysuid",
			Parent = "global",
			FullName = "global.sysuid",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.sysuid(arg)
		};
		command[268] = new ConsoleSystem.Command()
		{
			Name = "teleport",
			Parent = "global",
			FullName = "global.teleport",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.teleport(arg)
		};
		command[269] = new ConsoleSystem.Command()
		{
			Name = "teleport2me",
			Parent = "global",
			FullName = "global.teleport2me",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.teleport2me(arg)
		};
		command[270] = new ConsoleSystem.Command()
		{
			Name = "teleportany",
			Parent = "global",
			FullName = "global.teleportany",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.teleportany(arg)
		};
		command[271] = new ConsoleSystem.Command()
		{
			Name = "teleportpos",
			Parent = "global",
			FullName = "global.teleportpos",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.teleportpos(arg)
		};
		command[272] = new ConsoleSystem.Command()
		{
			Name = "textures",
			Parent = "global",
			FullName = "global.textures",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.textures(arg)
		};
		command[273] = new ConsoleSystem.Command()
		{
			Name = "timewarning",
			Parent = "global",
			FullName = "global.timewarning",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Global.timewarning.ToString(),
			SetOveride = (string str) => Global.timewarning = str.ToBool()
		};
		command[274] = new ConsoleSystem.Command()
		{
			Name = "version",
			Parent = "global",
			FullName = "global.version",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Global.version(arg)
		};
		command[275] = new ConsoleSystem.Command()
		{
			Name = "enabled",
			Parent = "halloween",
			FullName = "halloween.enabled",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Halloween.enabled.ToString(),
			SetOveride = (string str) => Halloween.enabled = str.ToBool()
		};
		command[276] = new ConsoleSystem.Command()
		{
			Name = "murdererpopulation",
			Parent = "halloween",
			FullName = "halloween.murdererpopulation",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Halloween.murdererpopulation.ToString(),
			SetOveride = (string str) => Halloween.murdererpopulation = str.ToFloat(0f)
		};
		command[277] = new ConsoleSystem.Command()
		{
			Name = "scarecrow_beancan_vs_player_dmg_modifier",
			Parent = "halloween",
			FullName = "halloween.scarecrow_beancan_vs_player_dmg_modifier",
			ServerAdmin = true,
			Description = "Modified damage from beancan explosion vs players (Default: 0.1).",
			Variable = true,
			GetOveride = () => Halloween.scarecrow_beancan_vs_player_dmg_modifier.ToString(),
			SetOveride = (string str) => Halloween.scarecrow_beancan_vs_player_dmg_modifier = str.ToFloat(0f)
		};
		command[278] = new ConsoleSystem.Command()
		{
			Name = "scarecrow_body_dmg_modifier",
			Parent = "halloween",
			FullName = "halloween.scarecrow_body_dmg_modifier",
			ServerAdmin = true,
			Description = "Modifier to how much damage scarecrows take to the body. (Default: 0.25)",
			Variable = true,
			GetOveride = () => Halloween.scarecrow_body_dmg_modifier.ToString(),
			SetOveride = (string str) => Halloween.scarecrow_body_dmg_modifier = str.ToFloat(0f)
		};
		command[279] = new ConsoleSystem.Command()
		{
			Name = "scarecrow_chase_stopping_distance",
			Parent = "halloween",
			FullName = "halloween.scarecrow_chase_stopping_distance",
			ServerAdmin = true,
			Description = "Stopping distance for destinations set while chasing a target (Default: 0.5)",
			Variable = true,
			GetOveride = () => Halloween.scarecrow_chase_stopping_distance.ToString(),
			SetOveride = (string str) => Halloween.scarecrow_chase_stopping_distance = str.ToFloat(0f)
		};
		command[280] = new ConsoleSystem.Command()
		{
			Name = "scarecrow_throw_beancan_global_delay",
			Parent = "halloween",
			FullName = "halloween.scarecrow_throw_beancan_global_delay",
			ServerAdmin = true,
			Description = "The delay globally on a server between each time a scarecrow throws a beancan (Default: 8 seconds).",
			Variable = true,
			GetOveride = () => Halloween.scarecrow_throw_beancan_global_delay.ToString(),
			SetOveride = (string str) => Halloween.scarecrow_throw_beancan_global_delay = str.ToFloat(0f)
		};
		command[281] = new ConsoleSystem.Command()
		{
			Name = "scarecrowpopulation",
			Parent = "halloween",
			FullName = "halloween.scarecrowpopulation",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Halloween.scarecrowpopulation.ToString(),
			SetOveride = (string str) => Halloween.scarecrowpopulation = str.ToFloat(0f)
		};
		command[282] = new ConsoleSystem.Command()
		{
			Name = "scarecrows_throw_beancans",
			Parent = "halloween",
			FullName = "halloween.scarecrows_throw_beancans",
			ServerAdmin = true,
			Description = "Scarecrows can throw beancans (Default: true).",
			Variable = true,
			GetOveride = () => Halloween.scarecrows_throw_beancans.ToString(),
			SetOveride = (string str) => Halloween.scarecrows_throw_beancans = str.ToBool()
		};
		command[283] = new ConsoleSystem.Command()
		{
			Name = "cd",
			Parent = "hierarchy",
			FullName = "hierarchy.cd",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Hierarchy.cd(arg)
		};
		command[284] = new ConsoleSystem.Command()
		{
			Name = "del",
			Parent = "hierarchy",
			FullName = "hierarchy.del",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Hierarchy.del(arg)
		};
		command[285] = new ConsoleSystem.Command()
		{
			Name = "ls",
			Parent = "hierarchy",
			FullName = "hierarchy.ls",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Hierarchy.ls(arg)
		};
		command[286] = new ConsoleSystem.Command()
		{
			Name = "endloot",
			Parent = "inventory",
			FullName = "inventory.endloot",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.endloot(arg)
		};
		command[287] = new ConsoleSystem.Command()
		{
			Name = "give",
			Parent = "inventory",
			FullName = "inventory.give",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.give(arg)
		};
		command[288] = new ConsoleSystem.Command()
		{
			Name = "giveall",
			Parent = "inventory",
			FullName = "inventory.giveall",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.giveall(arg)
		};
		command[289] = new ConsoleSystem.Command()
		{
			Name = "givearm",
			Parent = "inventory",
			FullName = "inventory.givearm",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.givearm(arg)
		};
		command[290] = new ConsoleSystem.Command()
		{
			Name = "giveid",
			Parent = "inventory",
			FullName = "inventory.giveid",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.giveid(arg)
		};
		command[291] = new ConsoleSystem.Command()
		{
			Name = "giveto",
			Parent = "inventory",
			FullName = "inventory.giveto",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.giveto(arg)
		};
		command[292] = new ConsoleSystem.Command()
		{
			Name = "lighttoggle",
			Parent = "inventory",
			FullName = "inventory.lighttoggle",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.lighttoggle(arg)
		};
		command[293] = new ConsoleSystem.Command()
		{
			Name = "resetbp",
			Parent = "inventory",
			FullName = "inventory.resetbp",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.resetbp(arg)
		};
		command[294] = new ConsoleSystem.Command()
		{
			Name = "unlockall",
			Parent = "inventory",
			FullName = "inventory.unlockall",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Inventory.unlockall(arg)
		};
		command[295] = new ConsoleSystem.Command()
		{
			Name = "printmanifest",
			Parent = "manifest",
			FullName = "manifest.printmanifest",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(ConVar.Manifest.PrintManifest())
		};
		command[296] = new ConsoleSystem.Command()
		{
			Name = "printmanifestraw",
			Parent = "manifest",
			FullName = "manifest.printmanifestraw",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(ConVar.Manifest.PrintManifestRaw())
		};
		command[297] = new ConsoleSystem.Command()
		{
			Name = "visdebug",
			Parent = "net",
			FullName = "net.visdebug",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Net.visdebug.ToString(),
			SetOveride = (string str) => Net.visdebug = str.ToBool()
		};
		command[298] = new ConsoleSystem.Command()
		{
			Name = "bulletaccuracy",
			Parent = "heli",
			FullName = "heli.bulletaccuracy",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => PatrolHelicopter.bulletAccuracy.ToString(),
			SetOveride = (string str) => PatrolHelicopter.bulletAccuracy = str.ToFloat(0f)
		};
		command[299] = new ConsoleSystem.Command()
		{
			Name = "bulletdamagescale",
			Parent = "heli",
			FullName = "heli.bulletdamagescale",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => PatrolHelicopter.bulletDamageScale.ToString(),
			SetOveride = (string str) => PatrolHelicopter.bulletDamageScale = str.ToFloat(0f)
		};
		command[300] = new ConsoleSystem.Command()
		{
			Name = "call",
			Parent = "heli",
			FullName = "heli.call",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => PatrolHelicopter.call(arg)
		};
		command[301] = new ConsoleSystem.Command()
		{
			Name = "calltome",
			Parent = "heli",
			FullName = "heli.calltome",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => PatrolHelicopter.calltome(arg)
		};
		command[302] = new ConsoleSystem.Command()
		{
			Name = "drop",
			Parent = "heli",
			FullName = "heli.drop",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => PatrolHelicopter.drop(arg)
		};
		command[303] = new ConsoleSystem.Command()
		{
			Name = "guns",
			Parent = "heli",
			FullName = "heli.guns",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => PatrolHelicopter.guns.ToString(),
			SetOveride = (string str) => PatrolHelicopter.guns = str.ToInt(0)
		};
		command[304] = new ConsoleSystem.Command()
		{
			Name = "lifetimeminutes",
			Parent = "heli",
			FullName = "heli.lifetimeminutes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => PatrolHelicopter.lifetimeMinutes.ToString(),
			SetOveride = (string str) => PatrolHelicopter.lifetimeMinutes = str.ToFloat(0f)
		};
		command[305] = new ConsoleSystem.Command()
		{
			Name = "strafe",
			Parent = "heli",
			FullName = "heli.strafe",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => PatrolHelicopter.strafe(arg)
		};
		command[306] = new ConsoleSystem.Command()
		{
			Name = "testpuzzle",
			Parent = "heli",
			FullName = "heli.testpuzzle",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => PatrolHelicopter.testpuzzle(arg)
		};
		command[307] = new ConsoleSystem.Command()
		{
			Name = "bouncethreshold",
			Parent = "physics",
			FullName = "physics.bouncethreshold",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Physics.bouncethreshold.ToString(),
			SetOveride = (string str) => ConVar.Physics.bouncethreshold = str.ToFloat(0f)
		};
		command[308] = new ConsoleSystem.Command()
		{
			Name = "droppedmode",
			Parent = "physics",
			FullName = "physics.droppedmode",
			ServerAdmin = true,
			Description = "The collision detection mode that dropped items and corpses should use",
			Variable = true,
			GetOveride = () => ConVar.Physics.droppedmode.ToString(),
			SetOveride = (string str) => ConVar.Physics.droppedmode = str.ToInt(0)
		};
		command[309] = new ConsoleSystem.Command()
		{
			Name = "gravity",
			Parent = "physics",
			FullName = "physics.gravity",
			ServerAdmin = true,
			Description = "Gravity multiplier",
			Variable = true,
			GetOveride = () => ConVar.Physics.gravity.ToString(),
			SetOveride = (string str) => ConVar.Physics.gravity = str.ToFloat(0f)
		};
		command[310] = new ConsoleSystem.Command()
		{
			Name = "minsteps",
			Parent = "physics",
			FullName = "physics.minsteps",
			ServerAdmin = true,
			Description = "The slowest physics steps will operate",
			Variable = true,
			GetOveride = () => ConVar.Physics.minsteps.ToString(),
			SetOveride = (string str) => ConVar.Physics.minsteps = str.ToFloat(0f)
		};
		command[311] = new ConsoleSystem.Command()
		{
			Name = "sendeffects",
			Parent = "physics",
			FullName = "physics.sendeffects",
			ServerAdmin = true,
			Description = "Send effects to clients when physics objects collide",
			Variable = true,
			GetOveride = () => ConVar.Physics.sendeffects.ToString(),
			SetOveride = (string str) => ConVar.Physics.sendeffects = str.ToBool()
		};
		command[312] = new ConsoleSystem.Command()
		{
			Name = "sleepthreshold",
			Parent = "physics",
			FullName = "physics.sleepthreshold",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Physics.sleepthreshold.ToString(),
			SetOveride = (string str) => ConVar.Physics.sleepthreshold = str.ToFloat(0f)
		};
		command[313] = new ConsoleSystem.Command()
		{
			Name = "solveriterationcount",
			Parent = "physics",
			FullName = "physics.solveriterationcount",
			ServerAdmin = true,
			Description = "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive",
			Variable = true,
			GetOveride = () => ConVar.Physics.solveriterationcount.ToString(),
			SetOveride = (string str) => ConVar.Physics.solveriterationcount = str.ToInt(0)
		};
		command[314] = new ConsoleSystem.Command()
		{
			Name = "steps",
			Parent = "physics",
			FullName = "physics.steps",
			ServerAdmin = true,
			Description = "The amount of physics steps per second",
			Variable = true,
			GetOveride = () => ConVar.Physics.steps.ToString(),
			SetOveride = (string str) => ConVar.Physics.steps = str.ToFloat(0f)
		};
		command[315] = new ConsoleSystem.Command()
		{
			Name = "clear_assets",
			Parent = "pool",
			FullName = "pool.clear_assets",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Pool.clear_assets(arg)
		};
		command[316] = new ConsoleSystem.Command()
		{
			Name = "clear_memory",
			Parent = "pool",
			FullName = "pool.clear_memory",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Pool.clear_memory(arg)
		};
		command[317] = new ConsoleSystem.Command()
		{
			Name = "clear_prefabs",
			Parent = "pool",
			FullName = "pool.clear_prefabs",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Pool.clear_prefabs(arg)
		};
		command[318] = new ConsoleSystem.Command()
		{
			Name = "debug",
			Parent = "pool",
			FullName = "pool.debug",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Pool.debug.ToString(),
			SetOveride = (string str) => ConVar.Pool.debug = str.ToBool()
		};
		command[319] = new ConsoleSystem.Command()
		{
			Name = "enabled",
			Parent = "pool",
			FullName = "pool.enabled",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Pool.enabled.ToString(),
			SetOveride = (string str) => ConVar.Pool.enabled = str.ToBool()
		};
		command[320] = new ConsoleSystem.Command()
		{
			Name = "export_prefabs",
			Parent = "pool",
			FullName = "pool.export_prefabs",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Pool.export_prefabs(arg)
		};
		command[321] = new ConsoleSystem.Command()
		{
			Name = "mode",
			Parent = "pool",
			FullName = "pool.mode",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Pool.mode.ToString(),
			SetOveride = (string str) => ConVar.Pool.mode = str.ToInt(0)
		};
		command[322] = new ConsoleSystem.Command()
		{
			Name = "print_assets",
			Parent = "pool",
			FullName = "pool.print_assets",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Pool.print_assets(arg)
		};
		command[323] = new ConsoleSystem.Command()
		{
			Name = "print_memory",
			Parent = "pool",
			FullName = "pool.print_memory",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Pool.print_memory(arg)
		};
		command[324] = new ConsoleSystem.Command()
		{
			Name = "print_prefabs",
			Parent = "pool",
			FullName = "pool.print_prefabs",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Pool.print_prefabs(arg)
		};
		command[325] = new ConsoleSystem.Command()
		{
			Name = "start",
			Parent = "profile",
			FullName = "profile.start",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Profile.start(arg)
		};
		command[326] = new ConsoleSystem.Command()
		{
			Name = "stop",
			Parent = "profile",
			FullName = "profile.stop",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => ConVar.Profile.stop(arg)
		};
		command[327] = new ConsoleSystem.Command()
		{
			Name = "hostileduration",
			Parent = "sentry",
			FullName = "sentry.hostileduration",
			ServerAdmin = true,
			Description = "how long until something is considered hostile after it attacked",
			Variable = true,
			GetOveride = () => Sentry.hostileduration.ToString(),
			SetOveride = (string str) => Sentry.hostileduration = str.ToFloat(0f)
		};
		command[328] = new ConsoleSystem.Command()
		{
			Name = "targetall",
			Parent = "sentry",
			FullName = "sentry.targetall",
			ServerAdmin = true,
			Description = "target everyone regardless of authorization",
			Variable = true,
			GetOveride = () => Sentry.targetall.ToString(),
			SetOveride = (string str) => Sentry.targetall = str.ToBool()
		};
		command[329] = new ConsoleSystem.Command()
		{
			Name = "arrowarmor",
			Parent = "server",
			FullName = "server.arrowarmor",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.arrowarmor.ToString(),
			SetOveride = (string str) => Server.arrowarmor = str.ToFloat(0f)
		};
		command[330] = new ConsoleSystem.Command()
		{
			Name = "arrowdamage",
			Parent = "server",
			FullName = "server.arrowdamage",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.arrowdamage.ToString(),
			SetOveride = (string str) => Server.arrowdamage = str.ToFloat(0f)
		};
		command[331] = new ConsoleSystem.Command()
		{
			Name = "authtimeout",
			Parent = "server",
			FullName = "server.authtimeout",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.authtimeout.ToString(),
			SetOveride = (string str) => Server.authtimeout = str.ToInt(0)
		};
		command[332] = new ConsoleSystem.Command()
		{
			Name = "backup",
			Parent = "server",
			FullName = "server.backup",
			ServerAdmin = true,
			Description = "Backup server folder",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.backup()
		};
		command[333] = new ConsoleSystem.Command()
		{
			Name = "bleedingarmor",
			Parent = "server",
			FullName = "server.bleedingarmor",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.bleedingarmor.ToString(),
			SetOveride = (string str) => Server.bleedingarmor = str.ToFloat(0f)
		};
		command[334] = new ConsoleSystem.Command()
		{
			Name = "bleedingdamage",
			Parent = "server",
			FullName = "server.bleedingdamage",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.bleedingdamage.ToString(),
			SetOveride = (string str) => Server.bleedingdamage = str.ToFloat(0f)
		};
		command[335] = new ConsoleSystem.Command()
		{
			Name = "branch",
			Parent = "server",
			FullName = "server.branch",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.branch.ToString(),
			SetOveride = (string str) => Server.branch = str
		};
		command[336] = new ConsoleSystem.Command()
		{
			Name = "bulletarmor",
			Parent = "server",
			FullName = "server.bulletarmor",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.bulletarmor.ToString(),
			SetOveride = (string str) => Server.bulletarmor = str.ToFloat(0f)
		};
		command[337] = new ConsoleSystem.Command()
		{
			Name = "bulletdamage",
			Parent = "server",
			FullName = "server.bulletdamage",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.bulletdamage.ToString(),
			SetOveride = (string str) => Server.bulletdamage = str.ToFloat(0f)
		};
		command[338] = new ConsoleSystem.Command()
		{
			Name = "cheatreport",
			Parent = "server",
			FullName = "server.cheatreport",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.cheatreport(arg)
		};
		command[339] = new ConsoleSystem.Command()
		{
			Name = "combatlog",
			Parent = "server",
			FullName = "server.combatlog",
			ServerUser = true,
			Description = "Get the player combat log",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Server.combatlog(arg))
		};
		command[340] = new ConsoleSystem.Command()
		{
			Name = "combatlogdelay",
			Parent = "server",
			FullName = "server.combatlogdelay",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.combatlogdelay.ToString(),
			SetOveride = (string str) => Server.combatlogdelay = str.ToInt(0)
		};
		command[341] = new ConsoleSystem.Command()
		{
			Name = "combatlogsize",
			Parent = "server",
			FullName = "server.combatlogsize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.combatlogsize.ToString(),
			SetOveride = (string str) => Server.combatlogsize = str.ToInt(0)
		};
		command[342] = new ConsoleSystem.Command()
		{
			Name = "compression",
			Parent = "server",
			FullName = "server.compression",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.compression.ToString(),
			SetOveride = (string str) => Server.compression = str.ToBool()
		};
		command[343] = new ConsoleSystem.Command()
		{
			Name = "corpsedespawn",
			Parent = "server",
			FullName = "server.corpsedespawn",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.corpsedespawn.ToString(),
			SetOveride = (string str) => Server.corpsedespawn = str.ToFloat(0f)
		};
		command[344] = new ConsoleSystem.Command()
		{
			Name = "corpses",
			Parent = "server",
			FullName = "server.corpses",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.corpses.ToString(),
			SetOveride = (string str) => Server.corpses = str.ToBool()
		};
		command[345] = new ConsoleSystem.Command()
		{
			Name = "cycletime",
			Parent = "server",
			FullName = "server.cycletime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.cycletime.ToString(),
			SetOveride = (string str) => Server.cycletime = str.ToFloat(0f)
		};
		command[346] = new ConsoleSystem.Command()
		{
			Name = "debrisdespawn",
			Parent = "server",
			FullName = "server.debrisdespawn",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.debrisdespawn.ToString(),
			SetOveride = (string str) => Server.debrisdespawn = str.ToFloat(0f)
		};
		command[347] = new ConsoleSystem.Command()
		{
			Name = "description",
			Parent = "server",
			FullName = "server.description",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.description.ToString(),
			SetOveride = (string str) => Server.description = str
		};
		command[348] = new ConsoleSystem.Command()
		{
			Name = "dropitems",
			Parent = "server",
			FullName = "server.dropitems",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.dropitems.ToString(),
			SetOveride = (string str) => Server.dropitems = str.ToBool()
		};
		command[349] = new ConsoleSystem.Command()
		{
			Name = "encryption",
			Parent = "server",
			FullName = "server.encryption",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.encryption.ToString(),
			SetOveride = (string str) => Server.encryption = str.ToInt(0)
		};
		command[350] = new ConsoleSystem.Command()
		{
			Name = "entitybatchsize",
			Parent = "server",
			FullName = "server.entitybatchsize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.entitybatchsize.ToString(),
			SetOveride = (string str) => Server.entitybatchsize = str.ToInt(0)
		};
		command[351] = new ConsoleSystem.Command()
		{
			Name = "entitybatchtime",
			Parent = "server",
			FullName = "server.entitybatchtime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.entitybatchtime.ToString(),
			SetOveride = (string str) => Server.entitybatchtime = str.ToFloat(0f)
		};
		command[352] = new ConsoleSystem.Command()
		{
			Name = "entityrate",
			Parent = "server",
			FullName = "server.entityrate",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.entityrate.ToString(),
			SetOveride = (string str) => Server.entityrate = str.ToInt(0)
		};
		command[353] = new ConsoleSystem.Command()
		{
			Name = "events",
			Parent = "server",
			FullName = "server.events",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.events.ToString(),
			SetOveride = (string str) => Server.events = str.ToBool()
		};
		command[354] = new ConsoleSystem.Command()
		{
			Name = "fps",
			Parent = "server",
			FullName = "server.fps",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.fps(arg)
		};
		command[355] = new ConsoleSystem.Command()
		{
			Name = "globalchat",
			Parent = "server",
			FullName = "server.globalchat",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.globalchat.ToString(),
			SetOveride = (string str) => Server.globalchat = str.ToBool()
		};
		command[356] = new ConsoleSystem.Command()
		{
			Name = "headerimage",
			Parent = "server",
			FullName = "server.headerimage",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.headerimage.ToString(),
			SetOveride = (string str) => Server.headerimage = str
		};
		command[357] = new ConsoleSystem.Command()
		{
			Name = "hostname",
			Parent = "server",
			FullName = "server.hostname",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.hostname.ToString(),
			SetOveride = (string str) => Server.hostname = str
		};
		command[358] = new ConsoleSystem.Command()
		{
			Name = "identity",
			Parent = "server",
			FullName = "server.identity",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.identity.ToString(),
			SetOveride = (string str) => Server.identity = str
		};
		command[359] = new ConsoleSystem.Command()
		{
			Name = "idlekick",
			Parent = "server",
			FullName = "server.idlekick",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.idlekick.ToString(),
			SetOveride = (string str) => Server.idlekick = str.ToInt(0)
		};
		command[360] = new ConsoleSystem.Command()
		{
			Name = "idlekickadmins",
			Parent = "server",
			FullName = "server.idlekickadmins",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.idlekickadmins.ToString(),
			SetOveride = (string str) => Server.idlekickadmins = str.ToInt(0)
		};
		command[361] = new ConsoleSystem.Command()
		{
			Name = "idlekickmode",
			Parent = "server",
			FullName = "server.idlekickmode",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.idlekickmode.ToString(),
			SetOveride = (string str) => Server.idlekickmode = str.ToInt(0)
		};
		command[362] = new ConsoleSystem.Command()
		{
			Name = "ip",
			Parent = "server",
			FullName = "server.ip",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.ip.ToString(),
			SetOveride = (string str) => Server.ip = str
		};
		command[363] = new ConsoleSystem.Command()
		{
			Name = "ipqueriespermin",
			Parent = "server",
			FullName = "server.ipqueriespermin",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.ipQueriesPerMin.ToString(),
			SetOveride = (string str) => Server.ipQueriesPerMin = str.ToInt(0)
		};
		command[364] = new ConsoleSystem.Command()
		{
			Name = "itemdespawn",
			Parent = "server",
			FullName = "server.itemdespawn",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.itemdespawn.ToString(),
			SetOveride = (string str) => Server.itemdespawn = str.ToFloat(0f)
		};
		command[365] = new ConsoleSystem.Command()
		{
			Name = "level",
			Parent = "server",
			FullName = "server.level",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.level.ToString(),
			SetOveride = (string str) => Server.level = str
		};
		command[366] = new ConsoleSystem.Command()
		{
			Name = "levelurl",
			Parent = "server",
			FullName = "server.levelurl",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.levelurl.ToString(),
			SetOveride = (string str) => Server.levelurl = str
		};
		command[367] = new ConsoleSystem.Command()
		{
			Name = "maxcommandpacketsize",
			Parent = "server",
			FullName = "server.maxcommandpacketsize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxcommandpacketsize.ToString(),
			SetOveride = (string str) => Server.maxcommandpacketsize = str.ToInt(0)
		};
		command[368] = new ConsoleSystem.Command()
		{
			Name = "maxcommandspersecond",
			Parent = "server",
			FullName = "server.maxcommandspersecond",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxcommandspersecond.ToString(),
			SetOveride = (string str) => Server.maxcommandspersecond = str.ToInt(0)
		};
		command[369] = new ConsoleSystem.Command()
		{
			Name = "maxpacketsize",
			Parent = "server",
			FullName = "server.maxpacketsize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxpacketsize.ToString(),
			SetOveride = (string str) => Server.maxpacketsize = str.ToInt(0)
		};
		command[370] = new ConsoleSystem.Command()
		{
			Name = "maxpacketspersecond",
			Parent = "server",
			FullName = "server.maxpacketspersecond",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxpacketspersecond.ToString(),
			SetOveride = (string str) => Server.maxpacketspersecond = str.ToInt(0)
		};
		command[371] = new ConsoleSystem.Command()
		{
			Name = "maxplayers",
			Parent = "server",
			FullName = "server.maxplayers",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxplayers.ToString(),
			SetOveride = (string str) => Server.maxplayers = str.ToInt(0)
		};
		command[372] = new ConsoleSystem.Command()
		{
			Name = "maxreceivetime",
			Parent = "server",
			FullName = "server.maxreceivetime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxreceivetime.ToString(),
			SetOveride = (string str) => Server.maxreceivetime = str.ToFloat(0f)
		};
		command[373] = new ConsoleSystem.Command()
		{
			Name = "maxrpcspersecond",
			Parent = "server",
			FullName = "server.maxrpcspersecond",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxrpcspersecond.ToString(),
			SetOveride = (string str) => Server.maxrpcspersecond = str.ToInt(0)
		};
		command[374] = new ConsoleSystem.Command()
		{
			Name = "maxtickspersecond",
			Parent = "server",
			FullName = "server.maxtickspersecond",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxtickspersecond.ToString(),
			SetOveride = (string str) => Server.maxtickspersecond = str.ToInt(0)
		};
		command[375] = new ConsoleSystem.Command()
		{
			Name = "maxunack",
			Parent = "server",
			FullName = "server.maxunack",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.maxunack.ToString(),
			SetOveride = (string str) => Server.maxunack = str.ToInt(0)
		};
		command[376] = new ConsoleSystem.Command()
		{
			Name = "meleearmor",
			Parent = "server",
			FullName = "server.meleearmor",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.meleearmor.ToString(),
			SetOveride = (string str) => Server.meleearmor = str.ToFloat(0f)
		};
		ConsoleSystem.Command command1 = new ConsoleSystem.Command()
		{
			Name = "meleedamage",
			Parent = "server",
			FullName = "server.meleedamage",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.meleedamage.ToString(),
			SetOveride = (string str) => Server.meleedamage = str.ToFloat(0f)
		};
		command[377] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "metabolismtick",
			Parent = "server",
			FullName = "server.metabolismtick",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.metabolismtick.ToString(),
			SetOveride = (string str) => Server.metabolismtick = str.ToFloat(0f)
		};
		command[378] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "netcache",
			Parent = "server",
			FullName = "server.netcache",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.netcache.ToString(),
			SetOveride = (string str) => Server.netcache = str.ToBool()
		};
		command[379] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "netcachesize",
			Parent = "server",
			FullName = "server.netcachesize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.netcachesize.ToString(),
			SetOveride = (string str) => Server.netcachesize = str.ToInt(0)
		};
		command[380] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "netlog",
			Parent = "server",
			FullName = "server.netlog",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.netlog.ToString(),
			SetOveride = (string str) => Server.netlog = str.ToBool()
		};
		command[381] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "official",
			Parent = "server",
			FullName = "server.official",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.official.ToString(),
			SetOveride = (string str) => Server.official = str.ToBool()
		};
		command[382] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "plantlightdetection",
			Parent = "server",
			FullName = "server.plantlightdetection",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.plantlightdetection.ToString(),
			SetOveride = (string str) => Server.plantlightdetection = str.ToBool()
		};
		command[383] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "planttick",
			Parent = "server",
			FullName = "server.planttick",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.planttick.ToString(),
			SetOveride = (string str) => Server.planttick = str.ToFloat(0f)
		};
		command[384] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "planttickscale",
			Parent = "server",
			FullName = "server.planttickscale",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.planttickscale.ToString(),
			SetOveride = (string str) => Server.planttickscale = str.ToFloat(0f)
		};
		command[385] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "playerserverfall",
			Parent = "server",
			FullName = "server.playerserverfall",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.playerserverfall.ToString(),
			SetOveride = (string str) => Server.playerserverfall = str.ToBool()
		};
		command[386] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "playertimeout",
			Parent = "server",
			FullName = "server.playertimeout",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.playertimeout.ToString(),
			SetOveride = (string str) => Server.playertimeout = str.ToInt(0)
		};
		command[387] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "port",
			Parent = "server",
			FullName = "server.port",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.port.ToString(),
			SetOveride = (string str) => Server.port = str.ToInt(0)
		};
		command[388] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "printeyes",
			Parent = "server",
			FullName = "server.printeyes",
			ServerAdmin = true,
			Description = "Print the current player eyes.",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Server.printeyes(arg))
		};
		command[389] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "printpos",
			Parent = "server",
			FullName = "server.printpos",
			ServerAdmin = true,
			Description = "Print the current player position.",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Server.printpos(arg))
		};
		command[390] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "printrot",
			Parent = "server",
			FullName = "server.printrot",
			ServerAdmin = true,
			Description = "Print the current player rotation.",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Server.printrot(arg))
		};
		command[391] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "pve",
			Parent = "server",
			FullName = "server.pve",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.pve.ToString(),
			SetOveride = (string str) => Server.pve = str.ToBool()
		};
		command[392] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "queriespersecond",
			Parent = "server",
			FullName = "server.queriespersecond",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.queriesPerSecond.ToString(),
			SetOveride = (string str) => Server.queriesPerSecond = str.ToInt(0)
		};
		command[393] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "queryport",
			Parent = "server",
			FullName = "server.queryport",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.queryport.ToString(),
			SetOveride = (string str) => Server.queryport = str.ToInt(0)
		};
		command[394] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "radiation",
			Parent = "server",
			FullName = "server.radiation",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.radiation.ToString(),
			SetOveride = (string str) => Server.radiation = str.ToBool()
		};
		command[395] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "readcfg",
			Parent = "server",
			FullName = "server.readcfg",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => arg.ReplyWithObject(Server.readcfg(arg))
		};
		command[396] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "respawnresetrange",
			Parent = "server",
			FullName = "server.respawnresetrange",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.respawnresetrange.ToString(),
			SetOveride = (string str) => Server.respawnresetrange = str.ToFloat(0f)
		};
		command[397] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "salt",
			Parent = "server",
			FullName = "server.salt",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.salt.ToString(),
			SetOveride = (string str) => Server.salt = str.ToInt(0)
		};
		command[398] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "save",
			Parent = "server",
			FullName = "server.save",
			ServerAdmin = true,
			Description = "Force save the current game",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.save(arg)
		};
		command[399] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "savecachesize",
			Parent = "server",
			FullName = "server.savecachesize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.savecachesize.ToString(),
			SetOveride = (string str) => Server.savecachesize = str.ToInt(0)
		};
		command[400] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "saveinterval",
			Parent = "server",
			FullName = "server.saveinterval",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.saveinterval.ToString(),
			SetOveride = (string str) => Server.saveinterval = str.ToInt(0)
		};
		command[401] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "schematime",
			Parent = "server",
			FullName = "server.schematime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.schematime.ToString(),
			SetOveride = (string str) => Server.schematime = str.ToFloat(0f)
		};
		command[402] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "secure",
			Parent = "server",
			FullName = "server.secure",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.secure.ToString(),
			SetOveride = (string str) => Server.secure = str.ToBool()
		};
		command[403] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "seed",
			Parent = "server",
			FullName = "server.seed",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.seed.ToString(),
			SetOveride = (string str) => Server.seed = str.ToInt(0)
		};
		command[404] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "sendnetworkupdate",
			Parent = "server",
			FullName = "server.sendnetworkupdate",
			ServerAdmin = true,
			Description = "Send network update for all players",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.sendnetworkupdate(arg)
		};
		command[405] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "setshowholstereditems",
			Parent = "server",
			FullName = "server.setshowholstereditems",
			ServerAdmin = true,
			Description = "Show holstered items on player bodies",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.setshowholstereditems(arg)
		};
		command[406] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "showholstereditems",
			Parent = "server",
			FullName = "server.showholstereditems",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.showHolsteredItems.ToString(),
			SetOveride = (string str) => Server.showHolsteredItems = str.ToBool()
		};
		command[407] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "snapshot",
			Parent = "server",
			FullName = "server.snapshot",
			ServerUser = true,
			Description = "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.snapshot(arg)
		};
		command[408] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "stability",
			Parent = "server",
			FullName = "server.stability",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.stability.ToString(),
			SetOveride = (string str) => Server.stability = str.ToBool()
		};
		command[409] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "start",
			Parent = "server",
			FullName = "server.start",
			ServerAdmin = true,
			Description = "Starts a server",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.start(arg)
		};
		command[410] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "stats",
			Parent = "server",
			FullName = "server.stats",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.stats.ToString(),
			SetOveride = (string str) => Server.stats = str.ToBool()
		};
		command[411] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "stop",
			Parent = "server",
			FullName = "server.stop",
			ServerAdmin = true,
			Description = "Stops a server",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.stop(arg)
		};
		command[412] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "tickrate",
			Parent = "server",
			FullName = "server.tickrate",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.tickrate.ToString(),
			SetOveride = (string str) => Server.tickrate = str.ToInt(0)
		};
		command[413] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "updatebatch",
			Parent = "server",
			FullName = "server.updatebatch",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.updatebatch.ToString(),
			SetOveride = (string str) => Server.updatebatch = str.ToInt(0)
		};
		command[414] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "updatebatchspawn",
			Parent = "server",
			FullName = "server.updatebatchspawn",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.updatebatchspawn.ToString(),
			SetOveride = (string str) => Server.updatebatchspawn = str.ToInt(0)
		};
		command[415] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "url",
			Parent = "server",
			FullName = "server.url",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.url.ToString(),
			SetOveride = (string str) => Server.url = str
		};
		command[416] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "worldsize",
			Parent = "server",
			FullName = "server.worldsize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Server.worldsize.ToString(),
			SetOveride = (string str) => Server.worldsize = str.ToInt(0)
		};
		command[417] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "woundingenabled",
			Parent = "server",
			FullName = "server.woundingenabled",
			ServerAdmin = true,
			Saved = true,
			Variable = true,
			GetOveride = () => Server.woundingenabled.ToString(),
			SetOveride = (string str) => Server.woundingenabled = str.ToBool()
		};
		command[418] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "writecfg",
			Parent = "server",
			FullName = "server.writecfg",
			ServerAdmin = true,
			Description = "Writes config files",
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Server.writecfg(arg)
		};
		command[419] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "fill_groups",
			Parent = "spawn",
			FullName = "spawn.fill_groups",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Spawn.fill_groups(arg)
		};
		command[420] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "fill_individuals",
			Parent = "spawn",
			FullName = "spawn.fill_individuals",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Spawn.fill_individuals(arg)
		};
		command[421] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "fill_populations",
			Parent = "spawn",
			FullName = "spawn.fill_populations",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Spawn.fill_populations(arg)
		};
		command[422] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "max_density",
			Parent = "spawn",
			FullName = "spawn.max_density",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.max_density.ToString(),
			SetOveride = (string str) => Spawn.max_density = str.ToFloat(0f)
		};
		command[423] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "max_rate",
			Parent = "spawn",
			FullName = "spawn.max_rate",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.max_rate.ToString(),
			SetOveride = (string str) => Spawn.max_rate = str.ToFloat(0f)
		};
		command[424] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "min_density",
			Parent = "spawn",
			FullName = "spawn.min_density",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.min_density.ToString(),
			SetOveride = (string str) => Spawn.min_density = str.ToFloat(0f)
		};
		command[425] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "min_rate",
			Parent = "spawn",
			FullName = "spawn.min_rate",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.min_rate.ToString(),
			SetOveride = (string str) => Spawn.min_rate = str.ToFloat(0f)
		};
		command[426] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "player_base",
			Parent = "spawn",
			FullName = "spawn.player_base",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.player_base.ToString(),
			SetOveride = (string str) => Spawn.player_base = str.ToFloat(0f)
		};
		command[427] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "player_scale",
			Parent = "spawn",
			FullName = "spawn.player_scale",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.player_scale.ToString(),
			SetOveride = (string str) => Spawn.player_scale = str.ToFloat(0f)
		};
		command[428] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "report",
			Parent = "spawn",
			FullName = "spawn.report",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Spawn.report(arg)
		};
		command[429] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "respawn_groups",
			Parent = "spawn",
			FullName = "spawn.respawn_groups",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.respawn_groups.ToString(),
			SetOveride = (string str) => Spawn.respawn_groups = str.ToBool()
		};
		command[430] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "respawn_individuals",
			Parent = "spawn",
			FullName = "spawn.respawn_individuals",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.respawn_individuals.ToString(),
			SetOveride = (string str) => Spawn.respawn_individuals = str.ToBool()
		};
		command[431] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "respawn_populations",
			Parent = "spawn",
			FullName = "spawn.respawn_populations",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.respawn_populations.ToString(),
			SetOveride = (string str) => Spawn.respawn_populations = str.ToBool()
		};
		command[432] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "scalars",
			Parent = "spawn",
			FullName = "spawn.scalars",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Spawn.scalars(arg)
		};
		command[433] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "tick_individuals",
			Parent = "spawn",
			FullName = "spawn.tick_individuals",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.tick_individuals.ToString(),
			SetOveride = (string str) => Spawn.tick_individuals = str.ToFloat(0f)
		};
		command[434] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "tick_populations",
			Parent = "spawn",
			FullName = "spawn.tick_populations",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Spawn.tick_populations.ToString(),
			SetOveride = (string str) => Spawn.tick_populations = str.ToFloat(0f)
		};
		command[435] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "accuracy",
			Parent = "stability",
			FullName = "stability.accuracy",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Stability.accuracy.ToString(),
			SetOveride = (string str) => Stability.accuracy = str.ToFloat(0f)
		};
		command[436] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "collapse",
			Parent = "stability",
			FullName = "stability.collapse",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Stability.collapse.ToString(),
			SetOveride = (string str) => Stability.collapse = str.ToFloat(0f)
		};
		command[437] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "refresh_stability",
			Parent = "stability",
			FullName = "stability.refresh_stability",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Stability.refresh_stability(arg)
		};
		command[438] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "stabilityqueue",
			Parent = "stability",
			FullName = "stability.stabilityqueue",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Stability.stabilityqueue.ToString(),
			SetOveride = (string str) => Stability.stabilityqueue = str.ToFloat(0f)
		};
		command[439] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "strikes",
			Parent = "stability",
			FullName = "stability.strikes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Stability.strikes.ToString(),
			SetOveride = (string str) => Stability.strikes = str.ToInt(0)
		};
		command[440] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "surroundingsqueue",
			Parent = "stability",
			FullName = "stability.surroundingsqueue",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Stability.surroundingsqueue.ToString(),
			SetOveride = (string str) => Stability.surroundingsqueue = str.ToFloat(0f)
		};
		command[441] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "verbose",
			Parent = "stability",
			FullName = "stability.verbose",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => Stability.verbose.ToString(),
			SetOveride = (string str) => Stability.verbose = str.ToInt(0)
		};
		command[442] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "call",
			Parent = "supply",
			FullName = "supply.call",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Supply.call(arg)
		};
		command[443] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "drop",
			Parent = "supply",
			FullName = "supply.drop",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Supply.drop(arg)
		};
		command[444] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "fixeddelta",
			Parent = "time",
			FullName = "time.fixeddelta",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Time.fixeddelta.ToString(),
			SetOveride = (string str) => ConVar.Time.fixeddelta = str.ToFloat(0f)
		};
		command[445] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "maxdelta",
			Parent = "time",
			FullName = "time.maxdelta",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Time.maxdelta.ToString(),
			SetOveride = (string str) => ConVar.Time.maxdelta = str.ToFloat(0f)
		};
		command[446] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "pausewhileloading",
			Parent = "time",
			FullName = "time.pausewhileloading",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Time.pausewhileloading.ToString(),
			SetOveride = (string str) => ConVar.Time.pausewhileloading = str.ToBool()
		};
		command[447] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "timescale",
			Parent = "time",
			FullName = "time.timescale",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Time.timescale.ToString(),
			SetOveride = (string str) => ConVar.Time.timescale = str.ToFloat(0f)
		};
		command[448] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "boat_corpse_seconds",
			Parent = "vehicle",
			FullName = "vehicle.boat_corpse_seconds",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => vehicle.boat_corpse_seconds.ToString(),
			SetOveride = (string str) => vehicle.boat_corpse_seconds = str.ToFloat(0f)
		};
		command[449] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "swapseats",
			Parent = "vehicle",
			FullName = "vehicle.swapseats",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => vehicle.swapseats(arg)
		};
		command[450] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "attack",
			Parent = "vis",
			FullName = "vis.attack",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Vis.attack.ToString(),
			SetOveride = (string str) => ConVar.Vis.attack = str.ToBool()
		};
		command[451] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "damage",
			Parent = "vis",
			FullName = "vis.damage",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Vis.damage.ToString(),
			SetOveride = (string str) => ConVar.Vis.damage = str.ToBool()
		};
		command[452] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "hitboxes",
			Parent = "vis",
			FullName = "vis.hitboxes",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Vis.hitboxes.ToString(),
			SetOveride = (string str) => ConVar.Vis.hitboxes = str.ToBool()
		};
		command[453] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "lineofsight",
			Parent = "vis",
			FullName = "vis.lineofsight",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Vis.lineofsight.ToString(),
			SetOveride = (string str) => ConVar.Vis.lineofsight = str.ToBool()
		};
		command[454] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "protection",
			Parent = "vis",
			FullName = "vis.protection",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Vis.protection.ToString(),
			SetOveride = (string str) => ConVar.Vis.protection = str.ToBool()
		};
		command[455] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "sense",
			Parent = "vis",
			FullName = "vis.sense",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Vis.sense.ToString(),
			SetOveride = (string str) => ConVar.Vis.sense = str.ToBool()
		};
		command[456] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "triggers",
			Parent = "vis",
			FullName = "vis.triggers",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Vis.triggers.ToString(),
			SetOveride = (string str) => ConVar.Vis.triggers = str.ToBool()
		};
		command[457] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "weakspots",
			Parent = "vis",
			FullName = "vis.weakspots",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.Vis.weakspots.ToString(),
			SetOveride = (string str) => ConVar.Vis.weakspots = str.ToBool()
		};
		command[458] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "clouds",
			Parent = "weather",
			FullName = "weather.clouds",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Weather.clouds(arg)
		};
		command[459] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "fog",
			Parent = "weather",
			FullName = "weather.fog",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Weather.fog(arg)
		};
		command[460] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "rain",
			Parent = "weather",
			FullName = "weather.rain",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Weather.rain(arg)
		};
		command[461] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "wind",
			Parent = "weather",
			FullName = "weather.wind",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Weather.wind(arg)
		};
		command[462] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "print_approved_skins",
			Parent = "workshop",
			FullName = "workshop.print_approved_skins",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => Workshop.print_approved_skins(arg)
		};
		command[463] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "cache",
			Parent = "world",
			FullName = "world.cache",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => ConVar.World.cache.ToString(),
			SetOveride = (string str) => ConVar.World.cache = str.ToBool()
		};
		command[464] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "enabled",
			Parent = "xmas",
			FullName = "xmas.enabled",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => XMas.enabled.ToString(),
			SetOveride = (string str) => XMas.enabled = str.ToBool()
		};
		command[465] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "giftsperplayer",
			Parent = "xmas",
			FullName = "xmas.giftsperplayer",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => XMas.giftsPerPlayer.ToString(),
			SetOveride = (string str) => XMas.giftsPerPlayer = str.ToInt(0)
		};
		command[466] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "refill",
			Parent = "xmas",
			FullName = "xmas.refill",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => XMas.refill(arg)
		};
		command[467] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "spawnattempts",
			Parent = "xmas",
			FullName = "xmas.spawnattempts",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => XMas.spawnAttempts.ToString(),
			SetOveride = (string str) => XMas.spawnAttempts = str.ToInt(0)
		};
		command[468] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "spawnrange",
			Parent = "xmas",
			FullName = "xmas.spawnrange",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => XMas.spawnRange.ToString(),
			SetOveride = (string str) => XMas.spawnRange = str.ToFloat(0f)
		};
		command[469] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "endtest",
			Parent = "cui",
			FullName = "cui.endtest",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => cui.endtest(arg)
		};
		command[470] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "test",
			Parent = "cui",
			FullName = "cui.test",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => cui.test(arg)
		};
		command[471] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "dump",
			Parent = "global",
			FullName = "global.dump",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => DiagnosticsConSys.dump(arg)
		};
		command[472] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "ip",
			Parent = "rcon",
			FullName = "rcon.ip",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => RCon.Ip.ToString(),
			SetOveride = (string str) => RCon.Ip = str
		};
		command[473] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "port",
			Parent = "rcon",
			FullName = "rcon.port",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => RCon.Port.ToString(),
			SetOveride = (string str) => RCon.Port = str.ToInt(0)
		};
		command[474] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "print",
			Parent = "rcon",
			FullName = "rcon.print",
			ServerAdmin = true,
			Description = "If true, rcon commands etc will be printed in the console",
			Variable = true,
			GetOveride = () => RCon.Print.ToString(),
			SetOveride = (string str) => RCon.Print = str.ToBool()
		};
		command[475] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "web",
			Parent = "rcon",
			FullName = "rcon.web",
			ServerAdmin = true,
			Description = "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.",
			Variable = true,
			GetOveride = () => RCon.Web.ToString(),
			SetOveride = (string str) => RCon.Web = str.ToBool()
		};
		command[476] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "decayseconds",
			Parent = "hackablelockedcrate",
			FullName = "hackablelockedcrate.decayseconds",
			ServerAdmin = true,
			Description = "How many seconds until the crate is destroyed without any hack attempts",
			Variable = true,
			GetOveride = () => HackableLockedCrate.decaySeconds.ToString(),
			SetOveride = (string str) => HackableLockedCrate.decaySeconds = str.ToFloat(0f)
		};
		command[477] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "requiredhackseconds",
			Parent = "hackablelockedcrate",
			FullName = "hackablelockedcrate.requiredhackseconds",
			ServerAdmin = true,
			Description = "How many seconds for the crate to unlock",
			Variable = true,
			GetOveride = () => HackableLockedCrate.requiredHackSeconds.ToString(),
			SetOveride = (string str) => HackableLockedCrate.requiredHackSeconds = str.ToFloat(0f)
		};
		command[478] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "horse",
			FullName = "horse.population",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Horse.Population.ToString(),
			SetOveride = (string str) => Horse.Population = str.ToFloat(0f)
		};
		command[479] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "outsidedecayminutes",
			Parent = "hotairballoon",
			FullName = "hotairballoon.outsidedecayminutes",
			ServerAdmin = true,
			Description = "How long before a HAB is killed while outside",
			Variable = true,
			GetOveride = () => HotAirBalloon.outsidedecayminutes.ToString(),
			SetOveride = (string str) => HotAirBalloon.outsidedecayminutes = str.ToFloat(0f)
		};
		command[480] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "hotairballoon",
			FullName = "hotairballoon.population",
			ServerAdmin = true,
			Description = "Population active on the server",
			Variable = true,
			GetOveride = () => HotAirBalloon.population.ToString(),
			SetOveride = (string str) => HotAirBalloon.population = str.ToFloat(0f)
		};
		command[481] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "serviceceiling",
			Parent = "hotairballoon",
			FullName = "hotairballoon.serviceceiling",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => HotAirBalloon.serviceCeiling.ToString(),
			SetOveride = (string str) => HotAirBalloon.serviceCeiling = str.ToFloat(0f)
		};
		command[482] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "backtracking",
			Parent = "ioentity",
			FullName = "ioentity.backtracking",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => IOEntity.backtracking.ToString(),
			SetOveride = (string str) => IOEntity.backtracking = str.ToInt(0)
		};
		command[483] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "framebudgetms",
			Parent = "ioentity",
			FullName = "ioentity.framebudgetms",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => IOEntity.framebudgetms.ToString(),
			SetOveride = (string str) => IOEntity.framebudgetms = str.ToFloat(0f)
		};
		command[484] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "responsetime",
			Parent = "ioentity",
			FullName = "ioentity.responsetime",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => IOEntity.responsetime.ToString(),
			SetOveride = (string str) => IOEntity.responsetime = str.ToFloat(0f)
		};
		command[485] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "outsidedecayminutes",
			Parent = "minicopter",
			FullName = "minicopter.outsidedecayminutes",
			ServerAdmin = true,
			Description = "How long before a minicopter is killed while outside",
			Variable = true,
			GetOveride = () => MiniCopter.outsidedecayminutes.ToString(),
			SetOveride = (string str) => MiniCopter.outsidedecayminutes = str.ToFloat(0f)
		};
		command[486] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "minicopter",
			FullName = "minicopter.population",
			ServerAdmin = true,
			Description = "Population active on the server",
			Variable = true,
			GetOveride = () => MiniCopter.population.ToString(),
			SetOveride = (string str) => MiniCopter.population = str.ToFloat(0f)
		};
		command[487] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "outsidedecayminutes",
			Parent = "motorrowboat",
			FullName = "motorrowboat.outsidedecayminutes",
			ServerAdmin = true,
			Description = "How long before a boat is killed while outside",
			Variable = true,
			GetOveride = () => MotorRowboat.outsidedecayminutes.ToString(),
			SetOveride = (string str) => MotorRowboat.outsidedecayminutes = str.ToFloat(0f)
		};
		command[488] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "motorrowboat",
			FullName = "motorrowboat.population",
			ServerAdmin = true,
			Description = "Population active on the server",
			Variable = true,
			GetOveride = () => MotorRowboat.population.ToString(),
			SetOveride = (string str) => MotorRowboat.population = str.ToFloat(0f)
		};
		command[489] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "update",
			Parent = "note",
			FullName = "note.update",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => note.update(arg)
		};
		command[490] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "sleeperhostiledelay",
			Parent = "npcautoturret",
			FullName = "npcautoturret.sleeperhostiledelay",
			ServerAdmin = true,
			Description = "How many seconds until a sleeping player is considered hostile",
			Variable = true,
			GetOveride = () => NPCAutoTurret.sleeperhostiledelay.ToString(),
			SetOveride = (string str) => NPCAutoTurret.sleeperhostiledelay = str.ToFloat(0f)
		};
		command[491] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "forcebirthday",
			Parent = "playerinventory",
			FullName = "playerinventory.forcebirthday",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => PlayerInventory.forceBirthday.ToString(),
			SetOveride = (string str) => PlayerInventory.forceBirthday = str.ToBool()
		};
		command[492] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "acceptinvite",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.acceptinvite",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.acceptinvite(arg)
		};
		command[493] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "addtoteam",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.addtoteam",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.addtoteam(arg)
		};
		command[494] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "fakeinvite",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.fakeinvite",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.fakeinvite(arg)
		};
		command[495] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "kickmember",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.kickmember",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.kickmember(arg)
		};
		command[496] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "leaveteam",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.leaveteam",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.leaveteam(arg)
		};
		command[497] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "maxteamsize",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.maxteamsize",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => RelationshipManager.maxTeamSize.ToString(),
			SetOveride = (string str) => RelationshipManager.maxTeamSize = str.ToInt(0)
		};
		command[498] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "promote",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.promote",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.promote(arg)
		};
		command[499] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "rejectinvite",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.rejectinvite",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.rejectinvite(arg)
		};
		command[500] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "sendinvite",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.sendinvite",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.sendinvite(arg)
		};
		command[501] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "sleeptoggle",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.sleeptoggle",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.sleeptoggle(arg)
		};
		command[502] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "trycreateteam",
			Parent = "relationshipmanager",
			FullName = "relationshipmanager.trycreateteam",
			ServerUser = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => RelationshipManager.trycreateteam(arg)
		};
		command[503] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "rhibpopulation",
			Parent = "rhib",
			FullName = "rhib.rhibpopulation",
			ServerAdmin = true,
			Description = "Population active on the server",
			Variable = true,
			GetOveride = () => RHIB.rhibpopulation.ToString(),
			SetOveride = (string str) => RHIB.rhibpopulation = str.ToFloat(0f)
		};
		command[504] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "ai_dormant",
			Parent = "aimanager",
			FullName = "aimanager.ai_dormant",
			ServerAdmin = true,
			Description = "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.",
			Variable = true,
			GetOveride = () => AiManager.ai_dormant.ToString(),
			SetOveride = (string str) => AiManager.ai_dormant = str.ToBool()
		};
		command[505] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "ai_dormant_max_wakeup_per_tick",
			Parent = "aimanager",
			FullName = "aimanager.ai_dormant_max_wakeup_per_tick",
			ServerAdmin = true,
			Description = "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)",
			Variable = true,
			GetOveride = () => AiManager.ai_dormant_max_wakeup_per_tick.ToString(),
			SetOveride = (string str) => AiManager.ai_dormant_max_wakeup_per_tick = str.ToInt(0)
		};
		command[506] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "ai_htn_animal_tick_budget",
			Parent = "aimanager",
			FullName = "aimanager.ai_htn_animal_tick_budget",
			ServerAdmin = true,
			Description = "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)",
			Variable = true,
			GetOveride = () => AiManager.ai_htn_animal_tick_budget.ToString(),
			SetOveride = (string str) => AiManager.ai_htn_animal_tick_budget = str.ToFloat(0f)
		};
		command[507] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "ai_htn_player_junkpile_tick_budget",
			Parent = "aimanager",
			FullName = "aimanager.ai_htn_player_junkpile_tick_budget",
			ServerAdmin = true,
			Description = "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)",
			Variable = true,
			GetOveride = () => AiManager.ai_htn_player_junkpile_tick_budget.ToString(),
			SetOveride = (string str) => AiManager.ai_htn_player_junkpile_tick_budget = str.ToFloat(0f)
		};
		command[508] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "ai_htn_player_tick_budget",
			Parent = "aimanager",
			FullName = "aimanager.ai_htn_player_tick_budget",
			ServerAdmin = true,
			Description = "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)",
			Variable = true,
			GetOveride = () => AiManager.ai_htn_player_tick_budget.ToString(),
			SetOveride = (string str) => AiManager.ai_htn_player_tick_budget = str.ToFloat(0f)
		};
		command[509] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "ai_htn_use_agency_tick",
			Parent = "aimanager",
			FullName = "aimanager.ai_htn_use_agency_tick",
			ServerAdmin = true,
			Description = "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)",
			Variable = true,
			GetOveride = () => AiManager.ai_htn_use_agency_tick.ToString(),
			SetOveride = (string str) => AiManager.ai_htn_use_agency_tick = str.ToBool()
		};
		command[510] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "ai_to_player_distance_wakeup_range",
			Parent = "aimanager",
			FullName = "aimanager.ai_to_player_distance_wakeup_range",
			ServerAdmin = true,
			Description = "If an agent is beyond this distance to a player, it's flagged for becoming dormant.",
			Variable = true,
			GetOveride = () => AiManager.ai_to_player_distance_wakeup_range.ToString(),
			SetOveride = (string str) => AiManager.ai_to_player_distance_wakeup_range = str.ToFloat(0f)
		};
		command[511] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "nav_disable",
			Parent = "aimanager",
			FullName = "aimanager.nav_disable",
			ServerAdmin = true,
			Description = "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move",
			Variable = true,
			GetOveride = () => AiManager.nav_disable.ToString(),
			SetOveride = (string str) => AiManager.nav_disable = str.ToBool()
		};
		command[512] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "nav_obstacles_carve_state",
			Parent = "aimanager",
			FullName = "aimanager.nav_obstacles_carve_state",
			ServerAdmin = true,
			Description = "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.",
			Variable = true,
			GetOveride = () => AiManager.nav_obstacles_carve_state.ToString(),
			SetOveride = (string str) => AiManager.nav_obstacles_carve_state = str.ToInt(0)
		};
		command[513] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "nav_wait",
			Parent = "aimanager",
			FullName = "aimanager.nav_wait",
			ServerAdmin = true,
			Description = "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.",
			Variable = true,
			GetOveride = () => AiManager.nav_wait.ToString(),
			SetOveride = (string str) => AiManager.nav_wait = str.ToBool()
		};
		command[514] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "pathfindingiterationsperframe",
			Parent = "aimanager",
			FullName = "aimanager.pathfindingiterationsperframe",
			ServerAdmin = true,
			Description = "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.",
			Variable = true,
			GetOveride = () => AiManager.pathfindingIterationsPerFrame.ToString(),
			SetOveride = (string str) => AiManager.pathfindingIterationsPerFrame = str.ToInt(0)
		};
		command[515] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "cover_point_sample_step_height",
			Parent = "coverpointvolume",
			FullName = "coverpointvolume.cover_point_sample_step_height",
			ServerAdmin = true,
			Description = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)",
			Variable = true,
			GetOveride = () => CoverPointVolume.cover_point_sample_step_height.ToString(),
			SetOveride = (string str) => CoverPointVolume.cover_point_sample_step_height = str.ToFloat(0f)
		};
		command[516] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "cover_point_sample_step_size",
			Parent = "coverpointvolume",
			FullName = "coverpointvolume.cover_point_sample_step_size",
			ServerAdmin = true,
			Description = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)",
			Variable = true,
			GetOveride = () => CoverPointVolume.cover_point_sample_step_size.ToString(),
			SetOveride = (string str) => CoverPointVolume.cover_point_sample_step_size = str.ToFloat(0f)
		};
		command[517] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "alltarget",
			Parent = "samsite",
			FullName = "samsite.alltarget",
			ServerAdmin = true,
			Description = "targetmode, 1 = all air vehicles, 0 = only hot air ballons",
			Variable = true,
			GetOveride = () => SamSite.alltarget.ToString(),
			SetOveride = (string str) => SamSite.alltarget = str.ToBool()
		};
		command[518] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "staticrepairseconds",
			Parent = "samsite",
			FullName = "samsite.staticrepairseconds",
			ServerAdmin = true,
			Description = "how long until static sam sites auto repair",
			Variable = true,
			GetOveride = () => SamSite.staticrepairseconds.ToString(),
			SetOveride = (string str) => SamSite.staticrepairseconds = str.ToFloat(0f)
		};
		command[519] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "altitudeaboveterrain",
			Parent = "santasleigh",
			FullName = "santasleigh.altitudeaboveterrain",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => SantaSleigh.altitudeAboveTerrain.ToString(),
			SetOveride = (string str) => SantaSleigh.altitudeAboveTerrain = str.ToFloat(0f)
		};
		command[520] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "desiredaltitude",
			Parent = "santasleigh",
			FullName = "santasleigh.desiredaltitude",
			ServerAdmin = true,
			Variable = true,
			GetOveride = () => SantaSleigh.desiredAltitude.ToString(),
			SetOveride = (string str) => SantaSleigh.desiredAltitude = str.ToFloat(0f)
		};
		command[521] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "drop",
			Parent = "santasleigh",
			FullName = "santasleigh.drop",
			ServerAdmin = true,
			Variable = false,
			Call = (ConsoleSystem.Arg arg) => SantaSleigh.drop(arg)
		};
		command[522] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "stag",
			FullName = "stag.population",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Stag.Population.ToString(),
			SetOveride = (string str) => Stag.Population = str.ToFloat(0f)
		};
		command[523] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "wolf",
			FullName = "wolf.population",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Wolf.Population.ToString(),
			SetOveride = (string str) => Wolf.Population = str.ToFloat(0f)
		};
		command[524] = command1;
		command1 = new ConsoleSystem.Command()
		{
			Name = "population",
			Parent = "zombie",
			FullName = "zombie.population",
			ServerAdmin = true,
			Description = "Population active on the server, per square km",
			Variable = true,
			GetOveride = () => Zombie.Population.ToString(),
			SetOveride = (string str) => Zombie.Population = str.ToFloat(0f)
		};
		command[525] = command1;
		ConsoleGen.All = command;
	}

	public ConsoleGen()
	{
	}
}