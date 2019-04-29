using ConVar;
using EasyAntiCheat.Server.Scout;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class AntiHack
{
	private const int movement_mask = 429990145;

	private const int grounded_mask = 1503731969;

	private const int vehicle_mask = 8192;

	private const int player_mask = 131072;

	private static Collider[] buffer;

	private static Dictionary<ulong, int> kicks;

	private static Dictionary<ulong, int> bans;

	static AntiHack()
	{
		AntiHack.buffer = new Collider[4];
		AntiHack.kicks = new Dictionary<ulong, int>();
		AntiHack.bans = new Dictionary<ulong, int>();
	}

	private static void AddRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (!records.ContainsKey(ply.userID))
		{
			records.Add(ply.userID, 1);
			return;
		}
		Dictionary<ulong, int> item = records;
		ulong num = ply.userID;
		item[num] = item[num] + 1;
	}

	public static void AddViolation(BasePlayer ply, AntiHackType type, float amount)
	{
		if (Interface.CallHook("OnPlayerViolation", ply, type, amount) != null)
		{
			return;
		}
		ply.lastViolationType = type;
		ply.lastViolationTime = UnityEngine.Time.realtimeSinceStartup;
		ply.violationLevel += amount;
		if (ConVar.AntiHack.debuglevel >= 2 && amount > 0f || ConVar.AntiHack.debuglevel >= 3)
		{
			AntiHack.LogToConsole(ply, type, string.Concat(new object[] { "Added violation of ", amount, " in frame ", UnityEngine.Time.frameCount, " (now has ", ply.violationLevel, ")" }));
		}
		AntiHack.EnforceViolations(ply);
	}

	public static void Ban(BasePlayer ply, string reason)
	{
		if (EACServer.eacScout != null)
		{
			EACServer.eacScout.SendKickReport(ply.userID.ToString(), reason, KickReasonCategory.KickReasonCheating);
		}
		AntiHack.AddRecord(ply, AntiHack.bans);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "ban", new object[] { ply.userID, reason });
	}

	public static void EnforceViolations(BasePlayer ply)
	{
		if (ConVar.AntiHack.enforcementlevel <= 0)
		{
			return;
		}
		if (ply.violationLevel > ConVar.AntiHack.maxviolation)
		{
			if (ConVar.AntiHack.debuglevel >= 1)
			{
				AntiHack.LogToConsole(ply, ply.lastViolationType, string.Concat("Enforcing (violation of ", ply.violationLevel, ")"));
			}
			string str = string.Concat(ply.lastViolationType, " Violation Level ", ply.violationLevel);
			if (ConVar.AntiHack.enforcementlevel > 1)
			{
				AntiHack.Kick(ply, str);
				return;
			}
			AntiHack.Kick(ply, str);
		}
	}

	public static void FadeViolations(BasePlayer ply, float deltaTime)
	{
		if (UnityEngine.Time.realtimeSinceStartup - ply.lastViolationTime > ConVar.AntiHack.relaxationpause)
		{
			ply.violationLevel = Mathf.Max(0f, ply.violationLevel - ConVar.AntiHack.relaxationrate * deltaTime);
		}
	}

	public static int GetBanRecord(BasePlayer ply)
	{
		return AntiHack.GetRecord(ply, AntiHack.bans);
	}

	public static int GetKickRecord(BasePlayer ply)
	{
		return AntiHack.GetRecord(ply, AntiHack.kicks);
	}

	private static int GetRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (!records.ContainsKey(ply.userID))
		{
			return 0;
		}
		return records[ply.userID];
	}

	public static bool IsFlying(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		ply.flyhackPauseTime = Mathf.Max(0f, ply.flyhackPauseTime - deltaTime);
		if (ply.flyhackPauseTime > 0f)
		{
			return false;
		}
		if (ConVar.AntiHack.flyhack_protection <= 0)
		{
			return false;
		}
		ticks.Reset();
		if (!ticks.HasNext())
		{
			return false;
		}
		bool flag = ply.transform.parent == null;
		Matrix4x4 matrix4x4 = (flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix);
		Vector3 vector3 = (flag ? ticks.StartPoint : matrix4x4.MultiplyPoint3x4(ticks.StartPoint));
		Vector3 vector31 = (flag ? ticks.EndPoint : matrix4x4.MultiplyPoint3x4(ticks.EndPoint));
		if (ConVar.AntiHack.flyhack_protection >= 3)
		{
			float single = Mathf.Max(ConVar.AntiHack.flyhack_stepsize, 0.1f);
			int num = Mathf.Max(ConVar.AntiHack.flyhack_maxsteps, 1);
			single = Mathf.Max(ticks.Length / (float)num, single);
			while (ticks.MoveNext(single))
			{
				vector31 = (flag ? ticks.CurrentPoint : matrix4x4.MultiplyPoint3x4(ticks.CurrentPoint));
				if (AntiHack.TestFlying(ply, vector3, vector31, true))
				{
					return true;
				}
				vector3 = vector31;
			}
		}
		else if (ConVar.AntiHack.flyhack_protection >= 2)
		{
			if (AntiHack.TestFlying(ply, vector3, vector31, true))
			{
				return true;
			}
		}
		else if (AntiHack.TestFlying(ply, vector3, vector31, false))
		{
			return true;
		}
		return false;
	}

	public static bool IsNoClipping(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		if (ConVar.AntiHack.noclip_protection <= 0)
		{
			return false;
		}
		ticks.Reset();
		if (!ticks.HasNext())
		{
			return false;
		}
		bool flag = ply.transform.parent == null;
		Matrix4x4 matrix4x4 = (flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix);
		Vector3 vector3 = (flag ? ticks.StartPoint : matrix4x4.MultiplyPoint3x4(ticks.StartPoint));
		Vector3 vector31 = (flag ? ticks.EndPoint : matrix4x4.MultiplyPoint3x4(ticks.EndPoint));
		if (ConVar.AntiHack.noclip_protection >= 3)
		{
			float single = Mathf.Max(ConVar.AntiHack.noclip_stepsize, 0.1f);
			int num = Mathf.Max(ConVar.AntiHack.noclip_maxsteps, 1);
			single = Mathf.Max(ticks.Length / (float)num, single);
			while (ticks.MoveNext(single))
			{
				vector31 = (flag ? ticks.CurrentPoint : matrix4x4.MultiplyPoint3x4(ticks.CurrentPoint));
				if (AntiHack.TestNoClipping(ply, vector3, vector31, true, deltaTime))
				{
					return true;
				}
				vector3 = vector31;
			}
		}
		else if (ConVar.AntiHack.noclip_protection >= 2)
		{
			if (AntiHack.TestNoClipping(ply, vector3, vector31, true, deltaTime))
			{
				return true;
			}
		}
		else if (AntiHack.TestNoClipping(ply, vector3, vector31, false, deltaTime))
		{
			return true;
		}
		return false;
	}

	public static bool IsSpeeding(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		ply.speedhackPauseTime = Mathf.Max(0f, ply.speedhackPauseTime - deltaTime);
		if (ply.speedhackPauseTime > 0f)
		{
			return false;
		}
		if (ConVar.AntiHack.speedhack_protection <= 0)
		{
			return false;
		}
		bool flag = ply.transform.parent == null;
		Matrix4x4 matrix4x4 = (flag ? Matrix4x4.identity : ply.transform.parent.localToWorldMatrix);
		Vector3 vector3 = (flag ? ticks.StartPoint : matrix4x4.MultiplyPoint3x4(ticks.StartPoint));
		Vector3 vector31 = (flag ? ticks.EndPoint : matrix4x4.MultiplyPoint3x4(ticks.EndPoint)) - vector3;
		float single = vector31.Magnitude2D();
		if (single > 0f)
		{
			Vector3 vector32 = (TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetNormal(vector3) : Vector3.up);
			float single1 = Mathf.Max(0f, Vector3.Dot(vector32.XZ3D(), vector31.XZ3D())) * ConVar.AntiHack.speedhack_slopespeed * deltaTime;
			single = Mathf.Max(0f, single - single1);
		}
		float single2 = 1f;
		float single3 = 0f;
		if (ConVar.AntiHack.speedhack_protection >= 2)
		{
			single2 = (ply.IsRunning() ? 1f : 0f);
			single3 = (ply.IsDucked() || ply.IsSwimming() ? 1f : 0f);
		}
		float single4 = Mathf.Max(ConVar.AntiHack.speedhack_forgiveness, 0.1f);
		float single5 = 2f * single4;
		ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance + single - deltaTime * ply.GetSpeed(single2, single3), -single5, single5);
		if (ply.speedhackDistance > single4)
		{
			return true;
		}
		return false;
	}

	public static void Kick(BasePlayer ply, string reason)
	{
		if (EACServer.eacScout != null)
		{
			EACServer.eacScout.SendKickReport(ply.userID.ToString(), reason, KickReasonCategory.KickReasonOther);
		}
		AntiHack.AddRecord(ply, AntiHack.kicks);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "kick", new object[] { ply.userID, reason });
	}

	public static void Log(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.debuglevel > 1)
		{
			AntiHack.LogToConsole(ply, type, message);
		}
		AntiHack.LogToEAC(ply, type, message);
	}

	private static void LogToConsole(BasePlayer ply, AntiHackType type, string message)
	{
		Debug.LogWarning(string.Concat(new object[] { ply, " ", type, ": ", message }));
	}

	private static void LogToEAC(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.reporting && EACServer.eacScout != null)
		{
			EACServer.eacScout.SendInvalidPlayerStateReport(ply.UserIDString, InvalidPlayerStateReportCategory.PlayerReportExploiting, string.Concat(type, ": ", message));
		}
	}

	public static void NoteAdminHack(BasePlayer ply)
	{
		AntiHack.Ban(ply, "Cheat Detected!");
	}

	public static void ResetTimer(BasePlayer ply)
	{
		ply.lastViolationTime = UnityEngine.Time.realtimeSinceStartup;
	}

	public static bool TestFlying(BasePlayer ply, Vector3 oldPos, Vector3 newPos, bool verifyGrounded)
	{
		ply.isInAir = false;
		ply.isOnPlayer = false;
		if (!verifyGrounded)
		{
			ply.isInAir = (ply.OnLadder() || ply.IsSwimming() ? false : !ply.IsOnGround());
		}
		else
		{
			float flyhackExtrusion = ConVar.AntiHack.flyhack_extrusion;
			Vector3 vector3 = (oldPos + newPos) * 0.5f;
			if (!ply.OnLadder() && !WaterLevel.Test(vector3 - new Vector3(0f, flyhackExtrusion, 0f)) && (int)(EnvironmentManager.Get(vector3) & EnvironmentType.Elevator) == 0)
			{
				float flyhackMargin = ConVar.AntiHack.flyhack_margin;
				float radius = ply.GetRadius();
				float height = ply.GetHeight(false);
				Vector3 vector31 = vector3 + new Vector3(0f, radius - flyhackExtrusion, 0f);
				Vector3 vector32 = vector3 + new Vector3(0f, height - radius, 0f);
				float single = radius - flyhackMargin;
				ply.isInAir = !UnityEngine.Physics.CheckCapsule(vector31, vector32, single, 1503731969, QueryTriggerInteraction.Ignore);
				if (ply.isInAir)
				{
					int num = UnityEngine.Physics.OverlapCapsuleNonAlloc(vector31, vector32, single, AntiHack.buffer, 131072, QueryTriggerInteraction.Ignore);
					int num1 = 0;
					while (num1 < num)
					{
						BasePlayer baseEntity = AntiHack.buffer[num1].gameObject.ToBaseEntity() as BasePlayer;
						if (baseEntity == null || baseEntity == ply || baseEntity.isInAir || baseEntity.isOnPlayer || baseEntity.TriggeredAntiHack(1f, Single.PositiveInfinity) || baseEntity.IsSleeping())
						{
							num1++;
						}
						else
						{
							ply.isOnPlayer = true;
							ply.isInAir = false;
							break;
						}
					}
					for (int i = 0; i < (int)AntiHack.buffer.Length; i++)
					{
						AntiHack.buffer[i] = null;
					}
				}
			}
		}
		if (!ply.isInAir)
		{
			ply.flyhackDistanceVertical = 0f;
			ply.flyhackDistanceHorizontal = 0f;
		}
		else
		{
			bool flag = false;
			Vector3 vector33 = newPos - oldPos;
			float single1 = Mathf.Abs(vector33.y);
			float single2 = vector33.Magnitude2D();
			if (vector33.y >= 0f)
			{
				ply.flyhackDistanceVertical += vector33.y;
				flag = true;
			}
			if (single1 < single2)
			{
				ply.flyhackDistanceHorizontal += single2;
				flag = true;
			}
			if (flag)
			{
				float jumpHeight = ply.GetJumpHeight() + ConVar.AntiHack.flyhack_forgiveness_vertical;
				if (ply.flyhackDistanceVertical > jumpHeight)
				{
					return true;
				}
				float flyhackForgivenessHorizontal = 5f + ConVar.AntiHack.flyhack_forgiveness_horizontal;
				if (ply.flyhackDistanceHorizontal > flyhackForgivenessHorizontal)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool TestNoClipping(BasePlayer ply, Vector3 oldPos, Vector3 newPos, bool sphereCast, float deltaTime = 0f)
	{
		ply.vehiclePauseTime = Mathf.Max(0f, ply.vehiclePauseTime - deltaTime);
		int num = 429990145;
		if (ply.vehiclePauseTime > 0f)
		{
			num &= -8193;
		}
		float noclipBacktracking = ConVar.AntiHack.noclip_backtracking;
		float noclipMargin = ConVar.AntiHack.noclip_margin;
		float radius = ply.GetRadius();
		float height = ply.GetHeight(true);
		Vector3 vector3 = newPos - oldPos;
		Vector3 vector31 = vector3.normalized;
		float single = radius - noclipMargin;
		Vector3 vector32 = (oldPos + new Vector3(0f, height - radius, 0f)) - (vector31 * noclipBacktracking);
		vector3 = (newPos + new Vector3(0f, height - radius, 0f)) - vector32;
		float single1 = vector3.magnitude;
		RaycastHit raycastHit = new RaycastHit();
		bool flag = UnityEngine.Physics.Raycast(new Ray(vector32, vector31), out raycastHit, single1 + single, num, QueryTriggerInteraction.Ignore);
		if (!flag & sphereCast)
		{
			flag = UnityEngine.Physics.SphereCast(new Ray(vector32, vector31), single, out raycastHit, single1, num, QueryTriggerInteraction.Ignore);
		}
		if (!flag)
		{
			return false;
		}
		return GamePhysics.Verify(raycastHit);
	}

	public static bool ValidateMove(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("AntiHack.ValidateMove", 0.1f))
		{
			if (ply.IsFlying)
			{
				ply.lastAdminCheatTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (ply.IsAdmin)
			{
				if (ConVar.AntiHack.userlevel < 1)
				{
					flag = true;
					return flag;
				}
				else if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat(1f))
				{
					flag = true;
					return flag;
				}
			}
			if (ply.IsDeveloper)
			{
				if (ConVar.AntiHack.userlevel < 2)
				{
					flag = true;
					return flag;
				}
				else if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat(1f))
				{
					flag = true;
					return flag;
				}
			}
			if (ply.IsSleeping())
			{
				flag = true;
			}
			else if (ply.IsWounded())
			{
				flag = true;
			}
			else if (ply.IsSpectating())
			{
				flag = true;
			}
			else if (!ply.IsDead())
			{
				bool flag1 = deltaTime > ConVar.AntiHack.maxdeltatime;
				using (TimeWarning timeWarning1 = TimeWarning.New("IsNoClipping", 0.1f))
				{
					if (AntiHack.IsNoClipping(ply, ticks, deltaTime))
					{
						if (!flag1)
						{
							AntiHack.AddViolation(ply, AntiHackType.NoClip, ConVar.AntiHack.noclip_penalty * ticks.Length);
							if (ConVar.AntiHack.noclip_reject)
							{
								flag = false;
								return flag;
							}
						}
						else
						{
							flag = false;
							return flag;
						}
					}
				}
				using (timeWarning1 = TimeWarning.New("IsSpeeding", 0.1f))
				{
					if (AntiHack.IsSpeeding(ply, ticks, deltaTime))
					{
						if (!flag1)
						{
							AntiHack.AddViolation(ply, AntiHackType.SpeedHack, ConVar.AntiHack.speedhack_penalty * ticks.Length);
							if (ConVar.AntiHack.speedhack_reject)
							{
								flag = false;
								return flag;
							}
						}
						else
						{
							flag = false;
							return flag;
						}
					}
				}
				using (timeWarning1 = TimeWarning.New("IsFlying", 0.1f))
				{
					if (AntiHack.IsFlying(ply, ticks, deltaTime))
					{
						if (!flag1)
						{
							AntiHack.AddViolation(ply, AntiHackType.FlyHack, ConVar.AntiHack.flyhack_penalty * ticks.Length);
							if (ConVar.AntiHack.flyhack_reject)
							{
								flag = false;
								return flag;
							}
						}
						else
						{
							flag = false;
							return flag;
						}
					}
				}
				flag = true;
			}
			else
			{
				flag = true;
			}
		}
		return flag;
	}
}