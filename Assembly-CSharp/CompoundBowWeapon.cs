using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CompoundBowWeapon : BowWeapon
{
	public float stringHoldDurationMax = 3f;

	public float stringBonusDamage = 1f;

	public float stringBonusDistance = 0.5f;

	public float stringBonusVelocity = 1f;

	public float movementPenaltyRampUpTime = 0.5f;

	public SoundDefinition chargeUpSoundDef;

	public SoundDefinition stringHeldSoundDef;

	public SoundDefinition drawFinishSoundDef;

	private Sound chargeUpSound;

	private Sound stringHeldSound;

	protected float movementPenalty;

	internal float stringHoldTimeStart;

	protected float serverMovementCheckTickRate = 0.1f;

	public CompoundBowWeapon()
	{
	}

	public override void DidAttackServerside()
	{
		base.DidAttackServerside();
		this.stringHoldTimeStart = 0f;
	}

	public override float GetDamageScale(bool getMax = false)
	{
		float single;
		single = (getMax ? 1f : this.GetStringBonusScale());
		return this.damageScale + this.stringBonusDamage * single;
	}

	public override float GetDistanceScale(bool getMax = false)
	{
		float single;
		single = (getMax ? 1f : this.GetStringBonusScale());
		return this.distanceScale + this.stringBonusDistance * single;
	}

	public float GetLastPlayerMovementTime()
	{
		bool flag = base.isServer;
		return 0f;
	}

	public override float GetProjectileVelocityScale(bool getMax = false)
	{
		float single;
		single = (getMax ? 1f : this.GetStringBonusScale());
		return this.projectileVelocityScale + this.stringBonusVelocity * single;
	}

	public float GetStringBonusScale()
	{
		if (this.stringHoldTimeStart == 0f)
		{
			return 0f;
		}
		return Mathf.Clamp01(Mathf.Clamp01((UnityEngine.Time.time - this.stringHoldTimeStart) / this.stringHoldDurationMax) - this.movementPenalty);
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			base.CancelInvoke(new Action(this.ServerMovementCheck));
			return;
		}
		base.InvokeRepeating(new Action(this.ServerMovementCheck), 0f, this.serverMovementCheckTickRate);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("CompoundBowWeapon.OnRpcMessage", 0.1f))
		{
			if (rpc != 618693016 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_StringHoldStatus "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_StringHoldStatus", 0.1f))
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
							this.RPC_StringHoldStatus(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_StringHoldStatus");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[RPC_Server]
	public void RPC_StringHoldStatus(BaseEntity.RPCMessage msg)
	{
		if (msg.read.Bit())
		{
			this.stringHoldTimeStart = UnityEngine.Time.time;
			return;
		}
		this.stringHoldTimeStart = 0f;
	}

	public void ServerMovementCheck()
	{
		this.UpdateMovementPenalty(this.serverMovementCheckTickRate);
	}

	public void UpdateMovementPenalty(float delta)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		bool flag = false;
		if (base.isServer)
		{
			if (ownerPlayer == null)
			{
				return;
			}
			flag = ownerPlayer.estimatedSpeed > 0.1f;
		}
		if (!flag)
		{
			this.movementPenalty = this.movementPenalty - delta * (1f / this.stringHoldDurationMax);
		}
		else
		{
			this.movementPenalty = this.movementPenalty + delta * (1f / this.movementPenaltyRampUpTime);
		}
		this.movementPenalty = Mathf.Clamp01(this.movementPenalty);
	}
}