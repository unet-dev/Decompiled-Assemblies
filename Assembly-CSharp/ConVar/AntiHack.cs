using System;

namespace ConVar
{
	[Factory("antihack")]
	public class AntiHack : ConsoleSystem
	{
		[Help("report violations to the anti cheat backend")]
		[ServerVar]
		public static bool reporting;

		[Help("are admins allowed to use their admin cheat")]
		[ServerVar]
		public static bool admincheat;

		[Help("use antihack to verify object placement by players")]
		[ServerVar]
		public static bool objectplacement;

		[Help("use antihack to verify model state sent by players")]
		[ServerVar]
		public static bool modelstate;

		[Help("whether or not to force the position on the client")]
		[ServerVar]
		public static bool forceposition;

		[Help("0 == users, 1 == admins, 2 == developers")]
		[ServerVar]
		public static int userlevel;

		[Help("0 == no enforcement, 1 == kick, 2 == ban (DISABLED)")]
		[ServerVar]
		public static int enforcementlevel;

		[Help("max allowed client desync, lower value = more false positives")]
		[ServerVar]
		public static float maxdesync;

		[Help("max allowed client tick interval delta time, lower value = more false positives")]
		[ServerVar]
		public static float maxdeltatime;

		[Help("the rate at which violation values go back down")]
		[ServerVar]
		public static float relaxationrate;

		[Help("the time before violation values go back down")]
		[ServerVar]
		public static float relaxationpause;

		[Help("violation value above this results in enforcement")]
		[ServerVar]
		public static float maxviolation;

		[Help("0 == disabled, 1 == ray, 2 == sphere, 3 == curve")]
		[ServerVar]
		public static int noclip_protection;

		[Help("whether or not to reject movement when noclip is detected")]
		[ServerVar]
		public static bool noclip_reject;

		[Help("violation penalty to hand out when noclip is detected")]
		[ServerVar]
		public static float noclip_penalty;

		[Help("collider margin when checking for noclipping")]
		[ServerVar]
		public static float noclip_margin;

		[Help("collider backtracking when checking for noclipping")]
		[ServerVar]
		public static float noclip_backtracking;

		[Help("movement curve step size, lower value = less false positives")]
		[ServerVar]
		public static float noclip_stepsize;

		[Help("movement curve max steps, lower value = more false positives")]
		[ServerVar]
		public static int noclip_maxsteps;

		[Help("0 == disabled, 1 == simple, 2 == advanced")]
		[ServerVar]
		public static int speedhack_protection;

		[Help("whether or not to reject movement when speedhack is detected")]
		[ServerVar]
		public static bool speedhack_reject;

		[Help("violation penalty to hand out when speedhack is detected")]
		[ServerVar]
		public static float speedhack_penalty;

		[Help("speed threshold to assume speedhacking, lower value = more false positives")]
		[ServerVar]
		public static float speedhack_forgiveness;

		[Help("speed forgiveness when moving down slopes, lower value = more false positives")]
		[ServerVar]
		public static float speedhack_slopespeed;

		[Help("0 == disabled, 1 == client, 2 == capsule, 3 == curve")]
		[ServerVar]
		public static int flyhack_protection;

		[Help("whether or not to reject movement when flyhack is detected")]
		[ServerVar]
		public static bool flyhack_reject;

		[Help("violation penalty to hand out when flyhack is detected")]
		[ServerVar]
		public static float flyhack_penalty;

		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		[ServerVar]
		public static float flyhack_forgiveness_vertical;

		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		[ServerVar]
		public static float flyhack_forgiveness_horizontal;

		[Help("collider downwards extrusion when checking for flyhacking")]
		[ServerVar]
		public static float flyhack_extrusion;

		[Help("collider margin when checking for flyhacking")]
		[ServerVar]
		public static float flyhack_margin;

		[Help("movement curve step size, lower value = less false positives")]
		[ServerVar]
		public static float flyhack_stepsize;

		[Help("movement curve max steps, lower value = more false positives")]
		[ServerVar]
		public static int flyhack_maxsteps;

		[Help("0 == disabled, 1 == speed, 2 == speed + entity, 3 == speed + entity + LOS, 4 == speed + entity + LOS + trajectory, 5 == speed + entity + LOS + trajectory + update")]
		[ServerVar]
		public static int projectile_protection;

		[Help("violation penalty to hand out when projectile hack is detected")]
		[ServerVar]
		public static float projectile_penalty;

		[Help("projectile speed forgiveness in percent, lower value = more false positives")]
		[ServerVar]
		public static float projectile_forgiveness;

		[Help("projectile server frames to include in delay, lower value = more false positives")]
		[ServerVar]
		public static float projectile_serverframes;

		[Help("projectile client frames to include in delay, lower value = more false positives")]
		[ServerVar]
		public static float projectile_clientframes;

		[Help("projectile trajectory forgiveness, lower value = more false positives")]
		[ServerVar]
		public static float projectile_trajectory_vertical;

		[Help("projectile trajectory forgiveness, lower value = more false positives")]
		[ServerVar]
		public static float projectile_trajectory_horizontal;

		[Help("0 == disabled, 1 == initiator, 2 == initiator + target, 3 == initiator + target + LOS")]
		[ServerVar]
		public static int melee_protection;

		[Help("violation penalty to hand out when melee hack is detected")]
		[ServerVar]
		public static float melee_penalty;

		[Help("melee distance forgiveness in percent, lower value = more false positives")]
		[ServerVar]
		public static float melee_forgiveness;

		[Help("melee server frames to include in delay, lower value = more false positives")]
		[ServerVar]
		public static float melee_serverframes;

		[Help("melee client frames to include in delay, lower value = more false positives")]
		[ServerVar]
		public static float melee_clientframes;

		[Help("0 == disabled, 1 == distance, 2 == distance + LOS")]
		[ServerVar]
		public static int eye_protection;

		[Help("violation penalty to hand out when eye hack is detected")]
		[ServerVar]
		public static float eye_penalty;

		[Help("eye speed forgiveness in percent, lower value = more false positives")]
		[ServerVar]
		public static float eye_forgiveness;

		[Help("eye server frames to include in delay, lower value = more false positives")]
		[ServerVar]
		public static float eye_serverframes;

		[Help("eye client frames to include in delay, lower value = more false positives")]
		[ServerVar]
		public static float eye_clientframes;

		[Help("0 == silent, 1 == print max violation, 2 == print nonzero violation, 3 == print any violation")]
		[ServerVar]
		public static int debuglevel;

		static AntiHack()
		{
			AntiHack.reporting = true;
			AntiHack.admincheat = true;
			AntiHack.objectplacement = true;
			AntiHack.modelstate = true;
			AntiHack.forceposition = true;
			AntiHack.userlevel = 2;
			AntiHack.enforcementlevel = 1;
			AntiHack.maxdesync = 1f;
			AntiHack.maxdeltatime = 1f;
			AntiHack.relaxationrate = 0.1f;
			AntiHack.relaxationpause = 10f;
			AntiHack.maxviolation = 100f;
			AntiHack.noclip_protection = 3;
			AntiHack.noclip_reject = true;
			AntiHack.noclip_penalty = 0f;
			AntiHack.noclip_margin = 0.09f;
			AntiHack.noclip_backtracking = 0.01f;
			AntiHack.noclip_stepsize = 0.1f;
			AntiHack.noclip_maxsteps = 15;
			AntiHack.speedhack_protection = 2;
			AntiHack.speedhack_reject = true;
			AntiHack.speedhack_penalty = 0f;
			AntiHack.speedhack_forgiveness = 2f;
			AntiHack.speedhack_slopespeed = 10f;
			AntiHack.flyhack_protection = 3;
			AntiHack.flyhack_reject = false;
			AntiHack.flyhack_penalty = 100f;
			AntiHack.flyhack_forgiveness_vertical = 1.5f;
			AntiHack.flyhack_forgiveness_horizontal = 1.5f;
			AntiHack.flyhack_extrusion = 2f;
			AntiHack.flyhack_margin = 0.05f;
			AntiHack.flyhack_stepsize = 0.1f;
			AntiHack.flyhack_maxsteps = 15;
			AntiHack.projectile_protection = 5;
			AntiHack.projectile_penalty = 0f;
			AntiHack.projectile_forgiveness = 0.5f;
			AntiHack.projectile_serverframes = 2f;
			AntiHack.projectile_clientframes = 2f;
			AntiHack.projectile_trajectory_vertical = 1f;
			AntiHack.projectile_trajectory_horizontal = 1f;
			AntiHack.melee_protection = 3;
			AntiHack.melee_penalty = 0f;
			AntiHack.melee_forgiveness = 0.5f;
			AntiHack.melee_serverframes = 2f;
			AntiHack.melee_clientframes = 2f;
			AntiHack.eye_protection = 2;
			AntiHack.eye_penalty = 0f;
			AntiHack.eye_forgiveness = 0.5f;
			AntiHack.eye_serverframes = 2f;
			AntiHack.eye_clientframes = 2f;
			AntiHack.debuglevel = 1;
		}

		public AntiHack()
		{
		}
	}
}