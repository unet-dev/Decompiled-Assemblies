using Facepunch;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlameTurret : StorageContainer
{
	public Transform upper;

	public Vector3 aimDir;

	public float arc = 45f;

	public float triggeredDuration = 5f;

	public float flameRange = 7f;

	public float flameRadius = 4f;

	public float fuelPerSec = 1f;

	public Transform eyeTransform;

	public List<DamageTypeEntry> damagePerSec;

	public GameObjectRef triggeredEffect;

	public GameObjectRef fireballPrefab;

	public GameObjectRef explosionEffect;

	public TargetTrigger trigger;

	private float nextFireballTime;

	private int turnDir = 1;

	private float lastServerThink;

	private float triggeredTime;

	private float triggerCheckRate = 2f;

	private float nextTriggerCheckTime;

	private float pendingFuel;

	public FlameTurret()
	{
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player))
		{
			return false;
		}
		return !this.IsTriggered();
	}

	public bool CheckTrigger()
	{
		if (Time.realtimeSinceStartup < this.nextTriggerCheckTime)
		{
			return false;
		}
		this.nextTriggerCheckTime = Time.realtimeSinceStartup + 1f / this.triggerCheckRate;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> baseEntities = this.trigger.entityContents;
		bool flag = false;
		if (baseEntities != null)
		{
			foreach (BaseEntity baseEntity in baseEntities)
			{
				BasePlayer component = baseEntity.GetComponent<BasePlayer>();
				if (component.IsSleeping() || !component.IsAlive() || component.IsBuildingAuthed() || component.transform.position.y > this.GetEyePosition().y + 0.5f)
				{
					continue;
				}
				list.Clear();
				Vector3 vector3 = component.eyes.position;
				Vector3 eyePosition = this.GetEyePosition() - component.eyes.position;
				GamePhysics.TraceAll(new Ray(vector3, eyePosition.normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal);
				int num = 0;
				while (num < list.Count)
				{
					BaseEntity entity = list[num].GetEntity();
					if (!(entity != null) || !(entity == this) && !entity.EqualNetID(this))
					{
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
						num++;
					}
					else
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				Pool.FreeList<RaycastHit>(ref list);
				return flag;
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	public void DoFlame(float delta)
	{
		RaycastHit raycastHit;
		if (!this.UseFuel(delta))
		{
			return;
		}
		Ray ray = new Ray(this.GetEyePosition(), base.transform.TransformDirection(Quaternion.Euler(this.aimDir) * Vector3.forward));
		Vector3 vector3 = ray.origin;
		bool flag = Physics.SphereCast(ray, 0.4f, out raycastHit, this.flameRange, 1218652417);
		if (!flag)
		{
			raycastHit.point = vector3 + (ray.direction * this.flameRange);
		}
		float item = this.damagePerSec[0].amount;
		this.damagePerSec[0].amount = item * delta;
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), raycastHit.point - (ray.direction * 0.1f), this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2230272, true);
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), base.transform.position + new Vector3(0f, 1.25f, 0f), 0.25f, 0.25f, this.damagePerSec, 133120, false);
		this.damagePerSec[0].amount = item;
		if (Time.realtimeSinceStartup >= this.nextFireballTime)
		{
			this.nextFireballTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(1f, 2f);
			Vector3 vector31 = (UnityEngine.Random.Range(0, 10) <= 7 & flag ? raycastHit.point : ray.origin + ((ray.direction * (flag ? raycastHit.distance : this.flameRange)) * UnityEngine.Random.Range(0.4f, 1f)));
			GameManager gameManager = GameManager.server;
			string str = this.fireballPrefab.resourcePath;
			Vector3 vector32 = vector31 - (ray.direction * 0.25f);
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector32, quaternion, true);
			if (baseEntity)
			{
				baseEntity.creatorEntity = this;
				baseEntity.Spawn();
			}
		}
	}

	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	public int GetFuelAmount()
	{
		Item slot = this.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	public float GetSpinSpeed()
	{
		return (float)((this.IsTriggered() ? 180 : 45));
	}

	public bool HasFuel()
	{
		return this.GetFuelAmount() > 0;
	}

	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	public void MovementUpdate(float delta)
	{
		this.aimDir = this.aimDir + (new Vector3(0f, delta * this.GetSpinSpeed(), 0f) * (float)this.turnDir);
		if (this.aimDir.y >= this.arc || this.aimDir.y <= -this.arc)
		{
			this.turnDir *= -1;
			this.aimDir.y = Mathf.Clamp(this.aimDir.y, -this.arc, this.arc);
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		if (info.damageTypes.IsMeleeType())
		{
			this.SetTriggered(true);
		}
		base.OnAttacked(info);
	}

	public override void OnKilled(HitInfo info)
	{
		float fuelAmount = (float)this.GetFuelAmount() / 500f;
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), this.GetEyePosition(), 2f, 6f, this.damagePerSec, 133120, true);
		Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		int num = Mathf.CeilToInt(Mathf.Clamp(fuelAmount * 8f, 1f, 8f));
		for (int i = 0; i < num; i++)
		{
			BaseEntity vector3 = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, base.transform.position, base.transform.rotation, true);
			if (vector3)
			{
				Vector3 vector31 = UnityEngine.Random.onUnitSphere;
				vector3.transform.position = (base.transform.position + new Vector3(0f, 1.5f, 0f)) + (vector31 * UnityEngine.Random.Range(-1f, 1f));
				vector3.Spawn();
				vector3.SetVelocity(vector31 * (float)UnityEngine.Random.Range(3, 10));
			}
		}
		base.OnKilled(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("FlameTurret.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void SendAimDir()
	{
		base.ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", this.aimDir);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.SendAimDir), 0f, 0.1f);
	}

	public void ServerThink()
	{
		float single = Time.realtimeSinceStartup - this.lastServerThink;
		if (single < 0.1f)
		{
			return;
		}
		bool flag = this.IsTriggered();
		this.lastServerThink = Time.realtimeSinceStartup;
		this.MovementUpdate(single);
		if (this.IsTriggered() && (Time.realtimeSinceStartup - this.triggeredTime > this.triggeredDuration || !this.HasFuel()))
		{
			this.SetTriggered(false);
		}
		if (!this.IsTriggered() && this.HasFuel() && this.CheckTrigger())
		{
			this.SetTriggered(true);
			Effect.server.Run(this.triggeredEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (flag != this.IsTriggered())
		{
			base.SendNetworkUpdateImmediate(false);
		}
		if (this.IsTriggered())
		{
			this.DoFlame(single);
		}
	}

	public void SetTriggered(bool triggered)
	{
		if (triggered && this.HasFuel())
		{
			this.triggeredTime = Time.realtimeSinceStartup;
		}
		base.SetFlag(BaseEntity.Flags.Reserved4, (!triggered ? false : this.HasFuel()), false, true);
	}

	public void Update()
	{
		if (base.isServer)
		{
			this.ServerThink();
		}
	}

	public bool UseFuel(float seconds)
	{
		Item slot = this.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel = this.pendingFuel + seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			this.pendingFuel -= (float)num;
		}
		return true;
	}
}