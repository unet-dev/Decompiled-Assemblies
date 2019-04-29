using ConVar;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MedicalTool : AttackEntity
{
	public float healDurationSelf = 4f;

	public float healDurationOther = 4f;

	public float maxDistanceOther = 2f;

	public bool canUseOnOther = true;

	public bool canRevive = true;

	public MedicalTool()
	{
	}

	private void GiveEffectsTo(BasePlayer player)
	{
		if (!player)
		{
			return;
		}
		ItemModConsumable component = base.GetOwnerItemDefinition().GetComponent<ItemModConsumable>();
		if (!component)
		{
			Debug.LogWarning(string.Concat("No consumable for medicaltool :", base.name));
			return;
		}
		if (Interface.CallHook("OnHealingItemUse", this, player) != null)
		{
			return;
		}
		if (player != base.GetOwnerPlayer() && player.IsWounded() && this.canRevive)
		{
			if (Interface.CallHook("OnPlayerRevive", this.GetOwnerPlayer(), player) != null)
			{
				return;
			}
			player.StopWounded();
		}
		foreach (ItemModConsumable.ConsumableEffect effect in component.effects)
		{
			if (effect.type != MetabolismAttribute.Type.Health)
			{
				player.metabolism.ApplyChange(effect.type, effect.amount, effect.time);
			}
			else
			{
				BasePlayer basePlayer = player;
				basePlayer.health = basePlayer.health + effect.amount;
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("MedicalTool.OnRpcMessage", 0.1f))
		{
			if (rpc == 789049461 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - UseOther "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("UseOther", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("UseOther", this, player))
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
							this.UseOther(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in UseOther");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -1376542826 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - UseSelf "));
				}
				using (timeWarning1 = TimeWarning.New("UseSelf", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("UseSelf", this, player))
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
							this.UseSelf(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in UseSelf");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void ServerUse()
	{
		if (base.isClient)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (!ownerPlayer.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount())
		{
			return;
		}
		this.GiveEffectsTo(ownerPlayer);
		base.UseItemAmount(1);
		base.StartAttackCooldown(this.repeatDelay);
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, null);
	}

	[IsActiveItem]
	[RPC_Server]
	private void UseOther(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!this.VerifyClientAttack(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!basePlayer.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount() || !this.canUseOnOther)
		{
			return;
		}
		BasePlayer basePlayer1 = BaseNetworkable.serverEntities.Find(msg.read.UInt32()) as BasePlayer;
		if (basePlayer1 != null && Vector3.Distance(basePlayer1.transform.position, basePlayer.transform.position) < 4f)
		{
			base.ClientRPCPlayer(null, basePlayer, "Reset");
			this.GiveEffectsTo(basePlayer1);
			base.UseItemAmount(1);
			base.StartAttackCooldown(this.repeatDelay);
		}
	}

	[IsActiveItem]
	[RPC_Server]
	private void UseSelf(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!this.VerifyClientAttack(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!basePlayer.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount())
		{
			return;
		}
		base.ClientRPCPlayer(null, basePlayer, "Reset");
		this.GiveEffectsTo(basePlayer);
		base.UseItemAmount(1);
		base.StartAttackCooldown(this.repeatDelay);
	}
}