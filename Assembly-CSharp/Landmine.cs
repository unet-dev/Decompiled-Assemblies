using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Landmine : BaseTrap
{
	public GameObjectRef explosionEffect;

	public GameObjectRef triggeredEffect;

	public float minExplosionRadius;

	public float explosionRadius;

	public bool blocked;

	private ulong triggerPlayerID;

	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	public Landmine()
	{
	}

	public override void Arm()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public bool Armed()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	public virtual void Explode()
	{
		base.health = Single.PositiveInfinity;
		Effect.server.Run(this.explosionEffect.resourcePath, base.PivotPoint(), base.transform.up, null, true);
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 2230528, true);
		if (base.IsDestroyed)
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && info.msg.landmine != null)
		{
			this.triggerPlayerID = info.msg.landmine.triggeredID;
		}
	}

	public override void ObjectEntered(GameObject obj)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.Armed())
		{
			Landmine landmine = this;
			base.CancelInvoke(new Action(landmine.Arm));
			this.blocked = true;
			return;
		}
		if (Interface.CallHook("OnTrapTrigger", this, obj) != null)
		{
			return;
		}
		this.Trigger(obj.ToBaseEntity() as BasePlayer);
	}

	public override void OnEmpty()
	{
		if (this.blocked)
		{
			this.Arm();
			this.blocked = false;
			return;
		}
		if (this.Triggered())
		{
			base.Invoke(new Action(this.TryExplode), 0.05f);
		}
	}

	private void OnGroundMissing()
	{
		this.Explode();
	}

	public override void OnKilled(HitInfo info)
	{
		Landmine landmine = this;
		base.Invoke(new Action(landmine.Explode), UnityEngine.Random.Range(0.1f, 0.3f));
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Landmine.OnRpcMessage", 0.1f))
		{
			if (rpc != 1552281787 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Disarm "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Disarm", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Disarm(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Disarm");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[RPC_Server]
	private void RPC_Disarm(BaseEntity.RPCMessage rpc)
	{
		if ((ulong)rpc.player.net.ID == this.triggerPlayerID)
		{
			return;
		}
		if (!this.Armed())
		{
			return;
		}
		if (Interface.CallHook("OnTrapDisarm", this, rpc.player) != null)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (UnityEngine.Random.Range(0, 100) < 15)
		{
			base.Invoke(new Action(this.TryExplode), 0.05f);
			return;
		}
		rpc.player.GiveItem(ItemManager.CreateByName("trap.landmine", 1, (ulong)0), BaseEntity.GiveItemReason.PickedUp);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.landmine = Facepunch.Pool.Get<ProtoBuf.Landmine>();
			info.msg.landmine.triggeredID = this.triggerPlayerID;
		}
	}

	public override void ServerInit()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		Landmine landmine = this;
		base.Invoke(new Action(landmine.Arm), 1.5f);
		base.ServerInit();
	}

	public void Trigger(BasePlayer ply = null)
	{
		if (ply)
		{
			this.triggerPlayerID = ply.userID;
		}
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public bool Triggered()
	{
		return base.HasFlag(BaseEntity.Flags.Open);
	}

	private void TryExplode()
	{
		if (this.Armed())
		{
			this.Explode();
		}
	}
}