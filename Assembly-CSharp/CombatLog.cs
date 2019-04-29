using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatLog
{
	private const string selfname = "you";

	private const string noname = "N/A";

	private BasePlayer player;

	private Queue<CombatLog.Event> storage;

	private static Dictionary<ulong, Queue<CombatLog.Event>> players;

	static CombatLog()
	{
		CombatLog.players = new Dictionary<ulong, Queue<CombatLog.Event>>();
	}

	public CombatLog(BasePlayer player)
	{
		this.player = player;
	}

	public string Get(int count)
	{
		if (this.storage == null)
		{
			return string.Empty;
		}
		if (this.storage.Count == 0)
		{
			return "Combat log empty.";
		}
		TextTable textTable = new TextTable();
		textTable.AddColumn("time");
		textTable.AddColumn("attacker");
		textTable.AddColumn("id");
		textTable.AddColumn("target");
		textTable.AddColumn("id");
		textTable.AddColumn("weapon");
		textTable.AddColumn("ammo");
		textTable.AddColumn("area");
		textTable.AddColumn("distance");
		textTable.AddColumn("old_hp");
		textTable.AddColumn("new_hp");
		textTable.AddColumn("info");
		int num = this.storage.Count - count;
		int num1 = ConVar.Server.combatlogdelay;
		int num2 = 0;
		foreach (CombatLog.Event @event in this.storage)
		{
			if (num <= 0)
			{
				float single = UnityEngine.Time.realtimeSinceStartup - @event.time;
				if (single < (float)num1)
				{
					num2++;
				}
				else
				{
					string str = single.ToString("0.0s");
					string str1 = @event.attacker;
					string str2 = @event.attacker_id.ToString();
					string str3 = @event.target;
					string str4 = @event.target_id.ToString();
					string str5 = @event.weapon;
					string str6 = @event.ammo;
					string lower = HitAreaUtil.Format(@event.area).ToLower();
					string str7 = @event.distance.ToString("0.0m");
					string str8 = @event.health_old.ToString("0.0");
					string str9 = @event.health_new.ToString("0.0");
					string str10 = @event.info;
					textTable.AddRow(new string[] { str, str1, str2, str3, str4, str5, str6, lower, str7, str8, str9, str10 });
				}
			}
			else
			{
				num--;
			}
		}
		string str11 = textTable.ToString();
		if (num2 > 0)
		{
			object[] objArray = new object[] { str11, "+ ", num2, " ", null };
			objArray[4] = (num2 > 1 ? "events" : "event");
			str11 = string.Concat(objArray);
			object[] objArray1 = new object[] { str11, " in the last ", num1, " ", null };
			objArray1[4] = (num1 > 1 ? "seconds" : "second");
			str11 = string.Concat(objArray1);
		}
		return str11;
	}

	public static Queue<CombatLog.Event> Get(ulong id)
	{
		Queue<CombatLog.Event> events;
		if (CombatLog.players.TryGetValue(id, out events))
		{
			return events;
		}
		events = new Queue<CombatLog.Event>();
		CombatLog.players.Add(id, events);
		return events;
	}

	public void Init()
	{
		this.storage = CombatLog.Get(this.player.userID);
	}

	public void Log(AttackEntity weapon, string description = null)
	{
		this.Log(weapon, null, description);
	}

	public void Log(AttackEntity weapon, Projectile projectile, string description = null)
	{
		uint d;
		CombatLog.Event @event = new CombatLog.Event()
		{
			time = UnityEngine.Time.realtimeSinceStartup
		};
		if (!this.player || this.player.net == null)
		{
			d = 0;
		}
		else
		{
			d = this.player.net.ID;
		}
		@event.attacker_id = d;
		@event.target_id = 0;
		@event.attacker = "you";
		@event.target = "N/A";
		@event.weapon = (weapon ? weapon.name : "N/A");
		@event.ammo = (projectile ? projectile.name : "N/A");
		@event.bone = "N/A";
		@event.area = (HitArea)0;
		@event.distance = 0f;
		@event.health_old = 0f;
		@event.health_new = 0f;
		@event.info = (description != null ? description : string.Empty);
		this.Log(@event);
	}

	public void Log(HitInfo info, string description = null)
	{
		float single = (info.HitEntity ? info.HitEntity.Health() : 0f);
		this.Log(info, single, single, description);
	}

	public void Log(HitInfo info, float health_old, float health_new, string description = null)
	{
		uint d;
		uint num;
		string str;
		string str1;
		CombatLog.Event healthOld = new CombatLog.Event()
		{
			time = UnityEngine.Time.realtimeSinceStartup
		};
		if (!info.Initiator || info.Initiator.net == null)
		{
			d = 0;
		}
		else
		{
			d = info.Initiator.net.ID;
		}
		healthOld.attacker_id = d;
		if (!info.HitEntity || info.HitEntity.net == null)
		{
			num = 0;
		}
		else
		{
			num = info.HitEntity.net.ID;
		}
		healthOld.target_id = num;
		if (this.player == info.Initiator)
		{
			str = "you";
		}
		else
		{
			str = (info.Initiator ? info.Initiator.ShortPrefabName : "N/A");
		}
		healthOld.attacker = str;
		if (this.player == info.HitEntity)
		{
			str1 = "you";
		}
		else
		{
			str1 = (info.HitEntity ? info.HitEntity.ShortPrefabName : "N/A");
		}
		healthOld.target = str1;
		healthOld.weapon = (info.WeaponPrefab ? info.WeaponPrefab.name : "N/A");
		healthOld.ammo = (info.ProjectilePrefab ? info.ProjectilePrefab.name : "N/A");
		healthOld.bone = info.boneName;
		healthOld.area = info.boneArea;
		healthOld.distance = (info.IsProjectile() ? info.ProjectileDistance : Vector3.Distance(info.PointStart, info.HitPositionWorld));
		healthOld.health_old = health_old;
		healthOld.health_new = health_new;
		healthOld.info = (description != null ? description : string.Empty);
		this.Log(healthOld);
	}

	public void Log(CombatLog.Event val)
	{
		if (this.storage == null)
		{
			return;
		}
		this.storage.Enqueue(val);
		int num = Mathf.Max(0, ConVar.Server.combatlogsize);
		while (this.storage.Count > num)
		{
			this.storage.Dequeue();
		}
	}

	public void Save()
	{
	}

	public struct Event
	{
		public float time;

		public uint attacker_id;

		public uint target_id;

		public string attacker;

		public string target;

		public string weapon;

		public string ammo;

		public string bone;

		public HitArea area;

		public float distance;

		public float health_old;

		public float health_new;

		public string info;
	}
}