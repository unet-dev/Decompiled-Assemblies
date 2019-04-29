using ConVar;
using Network;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class StrobeLight : BaseCombatEntity
{
	public float frequency;

	public MeshRenderer lightMesh;

	public Light strobeLight;

	private float speedSlow = 10f;

	private float speedMed = 20f;

	private float speedFast = 40f;

	public float burnRate = 10f;

	public float lifeTimeSeconds = 21600f;

	public const BaseEntity.Flags Flag_Slow = BaseEntity.Flags.Reserved6;

	public const BaseEntity.Flags Flag_Med = BaseEntity.Flags.Reserved7;

	public const BaseEntity.Flags Flag_Fast = BaseEntity.Flags.Reserved8;

	private int currentSpeed = 1;

	public StrobeLight()
	{
	}

	public float GetFrequency()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved6))
		{
			return this.speedSlow;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved7))
		{
			return this.speedMed;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved8))
		{
			return this.speedFast;
		}
		return this.speedSlow;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("StrobeLight.OnRpcMessage", 0.1f))
		{
			if (rpc == 1433326740 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetStrobe "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SetStrobe", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetStrobe", this, player, 3f))
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
							this.SetStrobe(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SetStrobe");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 1814332702 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetStrobeSpeed "));
				}
				using (timeWarning1 = TimeWarning.New("SetStrobeSpeed", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetStrobeSpeed", this, player, 3f))
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
							this.SetStrobeSpeed(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in SetStrobeSpeed");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void SelfDamage()
	{
		float single = this.burnRate / this.lifeTimeSeconds;
		this.Hurt(single * this.MaxHealth(), DamageType.Decay, this, false);
	}

	public void ServerEnableStrobing(bool wantsOn)
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.UpdateSpeedFlags();
		if (!wantsOn)
		{
			base.CancelInvoke(new Action(this.SelfDamage));
			return;
		}
		base.InvokeRandomized(new Action(this.SelfDamage), 0f, 10f, 0.1f);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetStrobe(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		this.ServerEnableStrobing(flag);
		if (flag)
		{
			this.UpdateSpeedFlags();
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetStrobeSpeed(BaseEntity.RPCMessage msg)
	{
		this.currentSpeed = msg.read.Int32();
		this.UpdateSpeedFlags();
	}

	public void UpdateSpeedFlags()
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, this.currentSpeed == 1, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved7, this.currentSpeed == 2, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.currentSpeed == 3, false, true);
	}
}