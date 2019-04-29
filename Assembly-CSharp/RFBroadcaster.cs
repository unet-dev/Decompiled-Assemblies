using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RFBroadcaster : IOEntity, IRFObject
{
	public int frequency;

	public GameObjectRef frequencyPanelPrefab;

	public const BaseEntity.Flags Flag_Broadcasting = BaseEntity.Flags.Reserved3;

	public bool playerUsable = true;

	private float nextChangeTime;

	private float nextStopTime;

	public RFBroadcaster()
	{
	}

	internal override void DoServerDestroy()
	{
		RFManager.RemoveBroadcaster(this.frequency, this);
		base.DoServerDestroy();
	}

	public int GetFrequency()
	{
		return this.frequency;
	}

	public float GetMaxRange()
	{
		return 100000f;
	}

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputAmount <= 0)
		{
			base.Invoke(new Action(this.StopBroadcasting), Mathf.Clamp01(this.nextStopTime - UnityEngine.Time.time));
			return;
		}
		base.CancelInvoke(new Action(this.StopBroadcasting));
		RFManager.AddBroadcaster(this.frequency, this);
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
		this.nextStopTime = UnityEngine.Time.time + 1f;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("RFBroadcaster.OnRpcMessage", 0.1f))
		{
			if (rpc != -1516351243 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ServerSetFrequency "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("ServerSetFrequency", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("ServerSetFrequency", this, player, 3f))
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
							this.ServerSetFrequency(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in ServerSetFrequency");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void RFSignalUpdate(bool on)
	{
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		if (!this.playerUsable)
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		int num = msg.read.Int32();
		if (RFManager.IsReserved(num))
		{
			RFManager.ReserveErrorPrint(msg.player);
			return;
		}
		RFManager.ChangeFrequency(this.frequency, num, this, false, this.IsPowered());
		this.frequency = num;
		this.MarkDirty();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void StopBroadcasting()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		RFManager.RemoveBroadcaster(this.frequency, this);
	}

	public override bool WantsPower()
	{
		return true;
	}
}