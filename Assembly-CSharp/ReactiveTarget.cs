using ConVar;
using Network;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ReactiveTarget : DecayEntity
{
	public Animator myAnimator;

	public GameObjectRef bullseyeEffect;

	public GameObjectRef knockdownEffect;

	private float lastToggleTime = Single.NegativeInfinity;

	private float knockdownHealth = 100f;

	public ReactiveTarget()
	{
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player))
		{
			return false;
		}
		return this.CanToggle();
	}

	public bool CanToggle()
	{
		return UnityEngine.Time.realtimeSinceStartup > this.lastToggleTime + 1f;
	}

	public bool IsKnockedDown()
	{
		return !base.HasFlag(BaseEntity.Flags.On);
	}

	public override void OnAttacked(HitInfo info)
	{
		this.OnHitShared(info);
		base.OnAttacked(info);
	}

	public void OnHitShared(HitInfo info)
	{
		if (this.IsKnockedDown())
		{
			return;
		}
		bool hitBone = info.HitBone == StringPool.Get("target_collider");
		bool flag = info.HitBone == StringPool.Get("target_collider_bullseye");
		if (!hitBone && !flag)
		{
			return;
		}
		if (base.isServer)
		{
			float single = info.damageTypes.Total();
			if (flag)
			{
				single *= 2f;
				Effect.server.Run(this.bullseyeEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero, null, false);
			}
			this.knockdownHealth -= single;
			if (this.knockdownHealth > 0f)
			{
				base.ClientRPC<uint>(null, "HitEffect", info.Initiator.net.ID);
			}
			else
			{
				Effect.server.Run(this.knockdownEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero, null, false);
				base.SetFlag(BaseEntity.Flags.On, false, false, true);
				this.QueueReset();
				base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
			this.Hurt(1f, DamageType.Suicide, info.Initiator, false);
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		BaseEntity.RPCMessage rPCMessage;
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("ReactiveTarget.OnRpcMessage", 0.1f))
		{
			if (rpc == 1798082523 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Lower "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Lower", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Lower(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Lower");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -2125489919 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Reset "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_Reset", 0.1f))
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
							this.RPC_Reset(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_Reset");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void QueueReset()
	{
		base.Invoke(new Action(this.ResetTarget), 6f);
	}

	public void ResetTarget()
	{
		if (base.HasFlag(BaseEntity.Flags.On) || !this.CanToggle())
		{
			return;
		}
		base.CancelInvoke(new Action(this.ResetTarget));
		this.lastToggleTime = UnityEngine.Time.realtimeSinceStartup;
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.knockdownHealth = 100f;
	}

	[RPC_Server]
	public void RPC_Lower(BaseEntity.RPCMessage msg)
	{
		if (!base.HasFlag(BaseEntity.Flags.On) || !this.CanToggle())
		{
			return;
		}
		this.lastToggleTime = UnityEngine.Time.realtimeSinceStartup;
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	[RPC_Server]
	public void RPC_Reset(BaseEntity.RPCMessage msg)
	{
		this.ResetTarget();
	}
}