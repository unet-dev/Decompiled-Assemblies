using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class WheelSwitch : IOEntity
{
	public Transform wheelObj;

	public float rotateSpeed = 90f;

	public BaseEntity.Flags BeingRotated = BaseEntity.Flags.Reserved1;

	public BaseEntity.Flags RotatingLeft = BaseEntity.Flags.Reserved2;

	public BaseEntity.Flags RotatingRight = BaseEntity.Flags.Reserved3;

	public float rotateProgress;

	public Animator animator;

	public float kineticEnergyPerSec = 1f;

	private BasePlayer rotatorPlayer;

	private float progressTickRate = 0.1f;

	public WheelSwitch()
	{
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void BeginRotate(BaseEntity.RPCMessage msg)
	{
		if (this.IsBeingRotated())
		{
			return;
		}
		base.SetFlag(this.BeingRotated, true, false, true);
		this.rotatorPlayer = msg.player;
		base.InvokeRepeating(new Action(this.RotateProgress), 0f, this.progressTickRate);
	}

	public void CancelPlayerRotation()
	{
		base.CancelInvoke(new Action(this.RotateProgress));
		base.SetFlag(this.BeingRotated, false, false, true);
		IOEntity.IOSlot[] oSlotArray = this.outputs;
		for (int i = 0; i < (int)oSlotArray.Length; i++)
		{
			IOEntity.IOSlot oSlot = oSlotArray[i];
			if (oSlot.connectedTo.Get(true) != null)
			{
				oSlot.connectedTo.Get(true).IOInput(this, this.ioType, 0f, oSlot.connectedToSlot);
			}
		}
		this.rotatorPlayer = null;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void CancelRotate(BaseEntity.RPCMessage msg)
	{
		this.CancelPlayerRotation();
	}

	public override float IOInput(IOEntity from, IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount < 0f)
		{
			this.SetRotateProgress(this.rotateProgress + inputAmount);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		if (inputType == IOEntity.IOType.Electric && slot == 1)
		{
			if (inputAmount != 0f)
			{
				base.InvokeRepeating(new Action(this.Powered), 0f, this.progressTickRate);
			}
			else
			{
				base.CancelInvoke(new Action(this.Powered));
			}
		}
		return Mathf.Clamp(inputAmount - 1f, 0f, inputAmount);
	}

	public bool IsBeingRotated()
	{
		return base.HasFlag(this.BeingRotated);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sphereEntity == null)
		{
			return;
		}
		this.rotateProgress = info.msg.sphereEntity.radius;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("WheelSwitch.OnRpcMessage", 0.1f))
		{
			if (rpc == -2071363974 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - BeginRotate "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("BeginRotate", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("BeginRotate", this, player, 3f))
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
							this.BeginRotate(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in BeginRotate");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 434251040 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - CancelRotate "));
				}
				using (timeWarning1 = TimeWarning.New("CancelRotate", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("CancelRotate", this, player, 3f))
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
							this.CancelRotate(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in CancelRotate");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void Powered()
	{
		float single = this.kineticEnergyPerSec * this.progressTickRate;
		IOEntity.IOSlot[] oSlotArray = this.outputs;
		for (int i = 0; i < (int)oSlotArray.Length; i++)
		{
			IOEntity.IOSlot oSlot = oSlotArray[i];
			if (oSlot.connectedTo.Get(true) != null)
			{
				single = oSlot.connectedTo.Get(true).IOInput(this, this.ioType, single, oSlot.connectedToSlot);
			}
		}
		this.SetRotateProgress(this.rotateProgress + 0.1f);
	}

	public override void ResetIOState()
	{
		this.CancelPlayerRotation();
	}

	public void RotateProgress()
	{
		if (!this.rotatorPlayer || this.rotatorPlayer.IsDead() || this.rotatorPlayer.IsSleeping() || Vector3Ex.Distance2D(this.rotatorPlayer.transform.position, base.transform.position) > 2f)
		{
			this.CancelPlayerRotation();
			return;
		}
		float single = this.kineticEnergyPerSec * this.progressTickRate;
		IOEntity.IOSlot[] oSlotArray = this.outputs;
		for (int i = 0; i < (int)oSlotArray.Length; i++)
		{
			IOEntity.IOSlot oSlot = oSlotArray[i];
			if (oSlot.connectedTo.Get(true) != null)
			{
				single = oSlot.connectedTo.Get(true).IOInput(this, this.ioType, single, oSlot.connectedToSlot);
			}
		}
		if (single == 0f)
		{
			this.SetRotateProgress(this.rotateProgress + 0.1f);
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Facepunch.Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.rotateProgress;
	}

	public void SetRotateProgress(float newValue)
	{
		float single = this.rotateProgress;
		this.rotateProgress = newValue;
		base.SetFlag(BaseEntity.Flags.Reserved4, single != newValue, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(this.StoppedRotatingCheck));
		base.Invoke(new Action(this.StoppedRotatingCheck), 0.25f);
	}

	public void StoppedRotatingCheck()
	{
		base.SetFlag(BaseEntity.Flags.Reserved4, false, false, true);
	}
}