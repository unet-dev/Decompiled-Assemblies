using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class WaterWell : LiquidContainer
{
	public Animator animator;

	private const BaseEntity.Flags Pumping = BaseEntity.Flags.Reserved2;

	private const BaseEntity.Flags WaterFlow = BaseEntity.Flags.Reserved3;

	public float caloriesPerPump = 5f;

	public float pressurePerPump = 0.2f;

	public float pressureForProduction = 1f;

	public float currentPressure;

	public int waterPerPump = 50;

	public GameObject waterLevelObj;

	public float waterLevelObjFullOffset;

	public WaterWell()
	{
	}

	public float GetWaterAmount()
	{
		if (!base.isServer)
		{
			return 0f;
		}
		Item slot = this.inventory.GetSlot(0);
		if (slot == null)
		{
			return 0f;
		}
		return (float)slot.amount;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.waterwell != null)
		{
			this.currentPressure = info.msg.waterwell.pressure;
		}
	}

	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("WaterWell.OnRpcMessage", 0.1f))
		{
			if (rpc != -1756227952 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Pump "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Pump", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Pump", this, player, 3f))
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
							this.RPC_Pump(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Pump");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void Produce()
	{
		this.inventory.AddItem(this.defaultLiquid, this.waterPerPump);
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
		this.ScheduleTapOff();
		base.SendNetworkUpdateImmediate(false);
	}

	public void ReducePressure()
	{
		float single = UnityEngine.Random.Range(0.1f, 0.2f);
		this.currentPressure = Mathf.Clamp01(this.currentPressure - single);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_Pump(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null || basePlayer.IsDead() || basePlayer.IsSleeping())
		{
			return;
		}
		if (basePlayer.metabolism.calories.@value < this.caloriesPerPump)
		{
			return;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		basePlayer.metabolism.calories.@value -= this.caloriesPerPump;
		basePlayer.metabolism.SendChangesToClient();
		this.currentPressure = Mathf.Clamp01(this.currentPressure + this.pressurePerPump);
		base.Invoke(new Action(this.StopPump), 1.8f);
		if (this.currentPressure >= 0f)
		{
			base.CancelInvoke(new Action(this.Produce));
			base.Invoke(new Action(this.Produce), 1f);
		}
		base.SendNetworkUpdateImmediate(false);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.waterwell = Facepunch.Pool.Get<ProtoBuf.WaterWell>();
		info.msg.waterwell.pressure = this.currentPressure;
		info.msg.waterwell.waterLevel = this.GetWaterAmount();
	}

	public void ScheduleTapOff()
	{
		base.CancelInvoke(new Action(this.TapOff));
		base.Invoke(new Action(this.TapOff), 1f);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
	}

	public void StopPump()
	{
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	private void TapOff()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
	}
}