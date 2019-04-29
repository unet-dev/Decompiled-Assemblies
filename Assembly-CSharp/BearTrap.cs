using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BearTrap : BaseTrap
{
	protected Animator animator;

	private GameObject hurtTarget;

	public BearTrap()
	{
	}

	public override void Arm()
	{
		base.Arm();
		this.RadialResetCorpses(120f);
	}

	public bool Armed()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player) || this.Armed())
		{
			return false;
		}
		return player.CanBuild();
	}

	public void DelayedFire()
	{
		if (this.hurtTarget)
		{
			BaseEntity baseEntity = this.hurtTarget.ToBaseEntity();
			if (baseEntity != null)
			{
				HitInfo hitInfo = new HitInfo(this, baseEntity, DamageType.Bite, 50f, base.transform.position);
				hitInfo.damageTypes.Add(DamageType.Stab, 30f);
				baseEntity.OnAttacked(hitInfo);
			}
			this.hurtTarget = null;
		}
		this.RadialResetCorpses(1800f);
		this.Fire();
		this.Hurt(25f);
	}

	public void Fire()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void InitShared()
	{
		this.animator = base.GetComponent<Animator>();
		base.InitShared();
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!base.isServer && this.animator.isInitialized)
		{
			this.animator.SetBool("armed", this.Armed());
		}
	}

	public override void ObjectEntered(GameObject obj)
	{
		if (!this.Armed())
		{
			return;
		}
		if (Interface.CallHook("OnTrapTrigger", this, obj) != null)
		{
			return;
		}
		this.hurtTarget = obj;
		base.Invoke(new Action(this.DelayedFire), 0.05f);
	}

	public override void OnAttacked(HitInfo info)
	{
		float single = info.damageTypes.Total();
		if (info.damageTypes.IsMeleeType() && single > 20f || single > 30f)
		{
			this.Fire();
		}
		base.OnAttacked(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("BearTrap.OnRpcMessage", 0.1f))
		{
			if (rpc != 547827602 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Arm "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Arm", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Arm", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Arm(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Arm");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void RadialResetCorpses(float duration)
	{
		List<BaseCorpse> list = Facepunch.Pool.GetList<BaseCorpse>();
		Vis.Entities<BaseCorpse>(base.transform.position, 5f, list, 512, QueryTriggerInteraction.Collide);
		foreach (BaseCorpse baseCorpse in list)
		{
			baseCorpse.ResetRemovalTime(duration);
		}
		Facepunch.Pool.FreeList<BaseCorpse>(ref list);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_Arm(BaseEntity.RPCMessage rpc)
	{
		if (this.Armed())
		{
			return;
		}
		if (Interface.CallHook("OnTrapArm", this, rpc.player) != null)
		{
			return;
		}
		this.Arm();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.Arm();
	}
}