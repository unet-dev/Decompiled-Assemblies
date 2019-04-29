using ConVar;
using Network;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Candle : BaseCombatEntity
{
	private float lifeTimeSeconds = 7200f;

	private float burnRate = 10f;

	public Candle()
	{
	}

	public void Burn()
	{
		float single = this.burnRate / this.lifeTimeSeconds;
		this.Hurt(single * this.MaxHealth(), DamageType.Decay, this, false);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Candle.OnRpcMessage", 0.1f))
		{
			if (rpc != -1771073851 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetWantsOn "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SetWantsOn", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetWantsOn", this, player, 3f))
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
							this.SetWantsOn(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SetWantsOn");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateInvokes();
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetWantsOn(BaseEntity.RPCMessage msg)
	{
		base.SetFlag(BaseEntity.Flags.On, msg.read.Bit(), false, true);
		this.UpdateInvokes();
	}

	public void UpdateInvokes()
	{
		if (base.IsOn())
		{
			base.InvokeRandomized(new Action(this.Burn), this.burnRate, this.burnRate, 1f);
		}
	}
}