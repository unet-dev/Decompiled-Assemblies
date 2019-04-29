using ConVar;
using Network;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class TorchWeapon : BaseMelee
{
	[NonSerialized]
	public float fuelTickAmount = 0.166666672f;

	[Header("TorchWeapon")]
	public AnimatorOverrideController LitHoldAnimationOverride;

	public TorchWeapon()
	{
	}

	[IsActiveItem]
	[RPC_Server]
	private void Extinguish(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.CancelInvoke(new Action(this.UseFuel));
	}

	public override void GetAttackStats(HitInfo info)
	{
		base.GetAttackStats(info);
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			info.damageTypes.Add(DamageType.Heat, 1f);
		}
	}

	public override float GetConditionLoss()
	{
		return base.GetConditionLoss() + (base.HasFlag(BaseEntity.Flags.On) ? 6f : 0f);
	}

	[IsActiveItem]
	[RPC_Server]
	private void Ignite(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.InvokeRepeating(new Action(this.UseFuel), 1f, 1f);
	}

	public override void OnHeldChanged()
	{
		if (base.IsDisabled())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			base.CancelInvoke(new Action(this.UseFuel));
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("TorchWeapon.OnRpcMessage", 0.1f))
		{
			if (rpc == -2059475731 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Extinguish "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("Extinguish", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("Extinguish", this, player))
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
							this.Extinguish(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in Extinguish");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -1284382553 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Ignite "));
				}
				using (timeWarning1 = TimeWarning.New("Ignite", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("Ignite", this, player))
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
							this.Ignite(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in Ignite");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void UseFuel()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(this.fuelTickAmount);
	}
}