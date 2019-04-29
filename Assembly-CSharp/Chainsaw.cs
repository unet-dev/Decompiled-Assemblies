using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Chainsaw : BaseMelee
{
	[Header("Chainsaw")]
	public float fuelPerSec = 1f;

	public int maxAmmo = 100;

	public int ammo = 100;

	public ItemDefinition fuelType;

	public float reloadDuration = 2.5f;

	[Header("Sounds")]
	public SoundPlayer idleLoop;

	public SoundPlayer attackLoopAir;

	public SoundPlayer revUp;

	public SoundPlayer revDown;

	public SoundPlayer offSound;

	public float engineStartChance = 0.33f;

	private float ammoRemainder;

	public float attackFadeInTime = 0.1f;

	public float attackFadeInDelay = 0.1f;

	public float attackFadeOutTime = 0.1f;

	public float idleFadeInTimeFromOff = 0.1f;

	public float idleFadeInTimeFromAttack = 0.3f;

	public float idleFadeInDelay = 0.1f;

	public float idleFadeOutTime = 0.1f;

	public Renderer chainRenderer;

	private MaterialPropertyBlock block;

	private Vector2 saveST;

	public Chainsaw()
	{
	}

	public void AttackTick()
	{
		this.ReduceAmmo(this.fuelPerSec);
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		this.ServerCommand(item, "unload_ammo", crafter);
	}

	private void DelayedStopAttack()
	{
		this.SetAttackStatus(false);
	}

	public void DisableHitEffects()
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	public override void DoAttackShared(HitInfo info)
	{
		base.DoAttackShared(info);
		if (base.isServer)
		{
			this.EnableHitEffect(info.HitMaterial);
		}
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

	public void EnableHitEffect(uint hitMaterial)
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		if (hitMaterial == StringPool.Get("Flesh"))
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
		}
		else if (hitMaterial != StringPool.Get("Wood"))
		{
			base.SetFlag(BaseEntity.Flags.Reserved6, true, false, true);
		}
		else
		{
			base.SetFlag(BaseEntity.Flags.Reserved7, true, false, true);
		}
		base.SendNetworkUpdateImmediate(false);
		base.CancelInvoke(new Action(this.DisableHitEffects));
		base.Invoke(new Action(this.DisableHitEffects), 0.5f);
	}

	public bool EngineOn()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	public void EngineTick()
	{
		this.ReduceAmmo(0.05f);
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

	public bool IsAttacking()
	{
		return base.HasFlag(BaseEntity.Flags.Busy);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("Chainsaw.OnRpcMessage", 0.1f))
		{
			if (rpc == -913613379 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
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
			else if (rpc == 706698034 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Server_SetAttacking "));
				}
				using (timeWarning1 = TimeWarning.New("Server_SetAttacking", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("Server_SetAttacking", this, player))
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
							this.Server_SetAttacking(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in Server_SetAttacking");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == -413172429 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Server_StartEngine "));
				}
				using (timeWarning1 = TimeWarning.New("Server_StartEngine", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("Server_StartEngine", this, player))
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
							this.Server_StartEngine(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in Server_StartEngine");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc != 841093980 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Server_StopEngine "));
				}
				using (timeWarning1 = TimeWarning.New("Server_StopEngine", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("Server_StopEngine", this, player))
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
							this.Server_StopEngine(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in Server_StopEngine");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void ReduceAmmo(float firingTime)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsNpc)
		{
			return;
		}
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
		if ((float)this.ammo <= 0f)
		{
			this.SetEngineStatus(false);
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
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

	[IsActiveItem]
	[RPC_Server]
	public void Server_SetAttacking(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (this.IsAttacking() == flag)
		{
			return;
		}
		if (!this.EngineOn())
		{
			return;
		}
		this.SetAttackStatus(flag);
		base.SendNetworkUpdateImmediate(false);
	}

	[IsActiveItem]
	[RPC_Server]
	public void Server_StartEngine(BaseEntity.RPCMessage msg)
	{
		if (this.ammo <= 0 || this.EngineOn())
		{
			return;
		}
		this.ReduceAmmo(0.25f);
		if (UnityEngine.Random.Range(0f, 1f) <= this.engineStartChance)
		{
			this.SetEngineStatus(true);
			base.SendNetworkUpdateImmediate(false);
		}
	}

	[IsActiveItem]
	[RPC_Server]
	public void Server_StopEngine(BaseEntity.RPCMessage msg)
	{
		this.SetEngineStatus(false);
		base.SendNetworkUpdateImmediate(false);
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
					item1.Drop(player.GetDropPosition(), player.GetDropVelocity(), new Quaternion());
				}
			}
		}
	}

	public void ServerNPCStart()
	{
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsNpc)
		{
			this.DoReload(new BaseEntity.RPCMessage());
			this.SetEngineStatus(true);
			base.SendNetworkUpdateImmediate(false);
		}
	}

	public override void ServerUse()
	{
		base.ServerUse();
		this.SetAttackStatus(true);
		base.Invoke(new Action(this.DelayedStopAttack), this.attackSpacing - 0.5f);
	}

	public void SetAttackStatus(bool status)
	{
		if (!this.EngineOn())
		{
			status = false;
		}
		base.SetFlag(BaseEntity.Flags.Busy, status, false, true);
		base.CancelInvoke(new Action(this.AttackTick));
		if (status)
		{
			base.InvokeRepeating(new Action(this.AttackTick), 0f, 1f);
		}
	}

	public void SetEngineStatus(bool status)
	{
		base.SetFlag(BaseEntity.Flags.On, status, false, true);
		if (!status)
		{
			this.SetAttackStatus(false);
		}
		base.CancelInvoke(new Action(this.EngineTick));
		if (status)
		{
			base.InvokeRepeating(new Action(this.EngineTick), 0f, 1f);
		}
	}

	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.SetEngineStatus(false);
		}
		base.SetHeld(bHeld);
	}

	protected override bool VerifyClientAttack(BasePlayer player)
	{
		if (!this.EngineOn())
		{
			return false;
		}
		return base.VerifyClientAttack(player);
	}
}