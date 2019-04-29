using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class FlameThrower : AttackEntity
{
	[Header("Flame Thrower")]
	public int maxAmmo = 100;

	public int ammo = 100;

	public ItemDefinition fuelType;

	public float timeSinceLastAttack;

	[FormerlySerializedAs("nextAttackTime")]
	public float nextReadyTime;

	public float flameRange = 10f;

	public float flameRadius = 2.5f;

	public ParticleSystem[] flameEffects;

	public FlameJet jet;

	public GameObjectRef fireballPrefab;

	public List<DamageTypeEntry> damagePerSec;

	public SoundDefinition flameStart3P;

	public SoundDefinition flameLoop3P;

	public SoundDefinition flameStop3P;

	public SoundDefinition pilotLoopSoundDef;

	private float tickRate = 0.25f;

	private float lastFlameTick;

	public float fuelPerSec;

	private float ammoRemainder;

	public float reloadDuration = 3.5f;

	private float lastReloadTime = -10f;

	private float nextFlameTime;

	public FlameThrower()
	{
	}

	public override float AmmoFraction()
	{
		return (float)this.ammo / (float)this.maxAmmo;
	}

	public override bool CanReload()
	{
		return this.ammo < this.maxAmmo;
	}

	private void ClearBusy()
	{
		this.nextReadyTime = UnityEngine.Time.realtimeSinceStartup - 1f;
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		this.ServerCommand(item, "unload_ammo", crafter);
	}

	[IsActiveItem]
	[RPC_Server]
	public void DoReload(BaseEntity.RPCMessage msg)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		Item item = null;
		while (this.ammo < this.maxAmmo)
		{
			Item ammo = this.GetAmmo();
			item = ammo;
			if (ammo == null || item.amount <= 0)
			{
				break;
			}
			int num = Mathf.Min(this.maxAmmo - this.ammo, item.amount);
			this.ammo += num;
			item.UseItem(num);
		}
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	public void FlameTick()
	{
		RaycastHit raycastHit;
		float single = UnityEngine.Time.realtimeSinceStartup - this.lastFlameTick;
		this.lastFlameTick = UnityEngine.Time.realtimeSinceStartup;
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		this.ReduceAmmo(single);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		Ray ray = ownerPlayer.eyes.BodyRay();
		Vector3 vector3 = ray.origin;
		bool flag = UnityEngine.Physics.SphereCast(ray, 0.3f, out raycastHit, this.flameRange, 1218652417);
		if (!flag)
		{
			raycastHit.point = vector3 + (ray.direction * this.flameRange);
		}
		float single1 = (ownerPlayer.IsNpc ? this.npcDamageScale : 1f);
		float item = this.damagePerSec[0].amount;
		this.damagePerSec[0].amount = item * single * single1;
		DamageUtil.RadiusDamage(ownerPlayer, base.LookupPrefab(), raycastHit.point - (ray.direction * 0.1f), this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2246913, true);
		this.damagePerSec[0].amount = item;
		if (flag && UnityEngine.Time.realtimeSinceStartup >= this.nextFlameTime && raycastHit.distance > 1.1f)
		{
			this.nextFlameTime = UnityEngine.Time.realtimeSinceStartup + 0.45f;
			Vector3 vector31 = raycastHit.point;
			GameManager gameManager = GameManager.server;
			string str = this.fireballPrefab.resourcePath;
			Vector3 vector32 = vector31 - (ray.direction * 0.25f);
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector32, quaternion, true);
			if (baseEntity)
			{
				baseEntity.creatorEntity = ownerPlayer;
				baseEntity.Spawn();
			}
		}
		if (this.ammo == 0)
		{
			this.SetFlameState(false);
		}
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem != null)
		{
			ownerItem.LoseCondition(single);
		}
	}

	public Item GetAmmo()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(this.fuelType.shortname) ?? ownerPlayer.inventory.containerBelt.FindItemsByItemName(this.fuelType.shortname);
		return item;
	}

	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	public bool IsFlameOn()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	public bool IsPilotOn()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	private bool IsWeaponBusy()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.nextReadyTime;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	public override void OnHeldChanged()
	{
		this.SetFlameState(false);
		base.OnHeldChanged();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("FlameThrower.OnRpcMessage", 0.1f))
		{
			if (rpc == -913613379 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoReload "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoReload", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoReload", this, player))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoReload(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoReload");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == -545396361 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetFiring "));
				}
				using (timeWarning1 = TimeWarning.New("SetFiring", 0.1f))
				{
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetFiring(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in SetFiring");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc != 1057268396 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - TogglePilotLight "));
				}
				using (timeWarning1 = TimeWarning.New("TogglePilotLight", 0.1f))
				{
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TogglePilotLight(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in TogglePilotLight");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void PilotLightToggle_Shared()
	{
		base.SetFlag(BaseEntity.Flags.On, !base.HasFlag(BaseEntity.Flags.On), false, true);
		if (base.isServer)
		{
			base.SendNetworkUpdateImmediate(false);
		}
	}

	public void ReduceAmmo(float firingTime)
	{
		this.ammoRemainder = this.ammoRemainder + this.fuelPerSec * firingTime;
		if (this.ammoRemainder >= 1f)
		{
			int num = Mathf.FloorToInt(this.ammoRemainder);
			this.ammoRemainder -= (float)num;
			if (this.ammoRemainder >= 1f)
			{
				num++;
				this.ammoRemainder -= 1f;
			}
			this.ammo -= num;
			if (this.ammo <= 0)
			{
				this.ammo = 0;
			}
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = new ProtoBuf.BaseProjectile()
		{
			primaryMagazine = Facepunch.Pool.Get<Magazine>()
		};
		info.msg.baseProjectile.primaryMagazine.contents = this.ammo;
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item == null)
		{
			return;
		}
		if (command == "unload_ammo")
		{
			int num = this.ammo;
			if (num > 0)
			{
				this.ammo = 0;
				base.SendNetworkUpdateImmediate(false);
				Item item1 = ItemManager.Create(this.fuelType, num, (ulong)0);
				if (!item1.MoveToContainer(player.inventory.containerMain, -1, true))
				{
					item1.Drop(player.eyes.position, player.eyes.BodyForward() * 2f, new Quaternion());
				}
			}
		}
	}

	public override bool ServerIsReloading()
	{
		return UnityEngine.Time.time < this.lastReloadTime + this.reloadDuration;
	}

	public override void ServerReload()
	{
		if (this.ServerIsReloading())
		{
			return;
		}
		this.lastReloadTime = UnityEngine.Time.time;
		base.StartAttackCooldown(this.reloadDuration);
		base.GetOwnerPlayer().SignalBroadcast(BaseEntity.Signal.Reload, null);
		this.ammo = this.maxAmmo;
	}

	public override void ServerUse()
	{
		if (base.IsOnFire())
		{
			return;
		}
		this.SetFlameState(true);
		base.Invoke(new Action(this.StopFlameState), 0.2f);
		base.ServerUse();
	}

	private void SetBusyFor(float dur)
	{
		this.nextReadyTime = UnityEngine.Time.realtimeSinceStartup + dur;
	}

	[RPC_Server]
	public void SetFiring(BaseEntity.RPCMessage msg)
	{
		this.SetFlameState(msg.read.Bit());
	}

	public void SetFlameState(bool wantsOn)
	{
		if (wantsOn)
		{
			this.ammo--;
			if (this.ammo < 0)
			{
				this.ammo = 0;
			}
		}
		if (wantsOn && this.ammo <= 0)
		{
			wantsOn = false;
		}
		base.SetFlag(BaseEntity.Flags.OnFire, wantsOn, false, true);
		if (!this.IsFlameOn())
		{
			base.CancelInvoke(new Action(this.FlameTick));
			return;
		}
		this.nextFlameTime = UnityEngine.Time.realtimeSinceStartup + 1f;
		this.lastFlameTick = UnityEngine.Time.realtimeSinceStartup;
		base.InvokeRepeating(new Action(this.FlameTick), this.tickRate, this.tickRate);
	}

	public void StopFlameState()
	{
		this.SetFlameState(false);
	}

	[RPC_Server]
	public void TogglePilotLight(BaseEntity.RPCMessage msg)
	{
		this.PilotLightToggle_Shared();
	}

	public override void TopUpAmmo()
	{
		this.ammo = this.maxAmmo;
	}
}