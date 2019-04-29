using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class FogMachine : StorageContainer
{
	public const BaseEntity.Flags FogFieldOn = BaseEntity.Flags.Reserved8;

	public const BaseEntity.Flags MotionMode = BaseEntity.Flags.Reserved7;

	public const BaseEntity.Flags Emitting = BaseEntity.Flags.Reserved6;

	public const BaseEntity.Flags Flag_HasJuice = BaseEntity.Flags.Reserved5;

	public float fogLength = 60f;

	public float nozzleBlastDuration = 5f;

	public float fuelPerSec = 1f;

	private float pendingFuel;

	public FogMachine()
	{
	}

	public void CheckTrigger()
	{
		if (this.IsEmitting())
		{
			return;
		}
		if (BasePlayer.AnyPlayersVisibleToEntity(base.transform.position + (base.transform.forward * 3f), 3f, this, base.transform.position + (Vector3.up * 0.1f), true))
		{
			this.StartFogging();
		}
	}

	public void DisableNozzle()
	{
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
	}

	public virtual void EnableFogField()
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
	}

	public virtual void FinishFogging()
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
	}

	public int GetFuelAmount()
	{
		Item slot = this.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	public bool HasFuel()
	{
		return this.GetFuelAmount() >= 1;
	}

	public bool HasJuice()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	public bool IsEmitting()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved6);
	}

	public virtual bool MotionModeEnabled()
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("FogMachine.OnRpcMessage", 0.1f))
		{
			if (rpc == -1506851731 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetFogOff "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SetFogOff", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetFogOff", this, player, 3f))
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
							this.SetFogOff(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SetFogOff");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == -389135368 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetFogOn "));
				}
				using (timeWarning1 = TimeWarning.New("SetFogOn", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetFogOn", this, player, 3f))
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
							this.SetFogOn(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in SetFogOn");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc != 1773639087 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetMotionDetection "));
				}
				using (timeWarning1 = TimeWarning.New("SetMotionDetection", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetMotionDetection", this, player, 3f))
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
							this.SetMotionDetection(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in SetMotionDetection");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.SetFlag(BaseEntity.Flags.Reserved5, this.HasFuel(), false, true);
		base.PlayerStoppedLooting(player);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved5, this.HasFuel(), false, true);
		if (base.IsOn())
		{
			base.InvokeRepeating(new Action(this.StartFogging), 0f, this.fogLength - 1f);
		}
		this.UpdateMotionMode();
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetFogOff(BaseEntity.RPCMessage msg)
	{
		if (!base.IsOn())
		{
			return;
		}
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.CancelInvoke(new Action(this.StartFogging));
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetFogOn(BaseEntity.RPCMessage msg)
	{
		if (this.IsEmitting())
		{
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		if (!this.HasFuel())
		{
			return;
		}
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.InvokeRepeating(new Action(this.StartFogging), 0f, this.fogLength - 1f);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetMotionDetection(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (!msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved7, flag, false, true);
		if (flag)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
		this.UpdateMotionMode();
	}

	public void StartFogging()
	{
		if (!this.UseFuel(1f))
		{
			base.CancelInvoke(new Action(this.StartFogging));
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, true, false, true);
		FogMachine fogMachine = this;
		base.Invoke(new Action(fogMachine.EnableFogField), 1f);
		base.Invoke(new Action(this.DisableNozzle), this.nozzleBlastDuration);
		FogMachine fogMachine1 = this;
		base.Invoke(new Action(fogMachine1.FinishFogging), this.fogLength);
	}

	public void UpdateMotionMode()
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved7))
		{
			base.CancelInvoke(new Action(this.CheckTrigger));
			return;
		}
		base.InvokeRandomized(new Action(this.CheckTrigger), UnityEngine.Random.Range(0f, 0.5f), 0.5f, 0.1f);
	}

	public bool UseFuel(float seconds)
	{
		Item slot = this.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel = this.pendingFuel + seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			this.pendingFuel -= (float)num;
		}
		return true;
	}
}