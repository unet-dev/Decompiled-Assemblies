using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Detonator : HeldEntity, IRFObject
{
	public int frequency = 55;

	private float timeSinceDeploy;

	public GameObjectRef frequencyPanelPrefab;

	public GameObjectRef attackEffect;

	public GameObjectRef unAttackEffect;

	private float nextChangeTime;

	public Detonator()
	{
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

	internal void InternalSetPressed(bool pressed)
	{
		base.SetFlag(BaseEntity.Flags.On, pressed, false, true);
		if (pressed)
		{
			RFManager.AddBroadcaster(this.frequency, this);
			return;
		}
		RFManager.RemoveBroadcaster(this.frequency, this);
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
		BaseEntity.RPCMessage rPCMessage;
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Detonator.OnRpcMessage", 0.1f))
		{
			if (rpc == -1516351243 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ServerSetFrequency "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("ServerSetFrequency", 0.1f))
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
			else if (rpc != 1106698135 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetPressed "));
				}
				using (timeWarning1 = TimeWarning.New("SetPressed", 0.1f))
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
							this.SetPressed(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in SetPressed");
						Debug.LogException(exception1);
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
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	[RPC_Server]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		if (base.GetOwnerPlayer() != msg.player)
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
		RFManager.ChangeFrequency(this.frequency, num, this, false, base.IsOn());
		this.frequency = num;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		Item item = this.GetItem();
		if (item != null)
		{
			item.MarkDirty();
		}
	}

	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.InternalSetPressed(false);
		}
		base.SetHeld(bHeld);
	}

	[RPC_Server]
	public void SetPressed(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || msg.player != base.GetOwnerPlayer())
		{
			return;
		}
		bool flag = base.HasFlag(BaseEntity.Flags.On);
		bool flag1 = msg.read.Bit();
		this.InternalSetPressed(flag1);
		if (flag != flag1)
		{
			Effect.server.Run((flag1 ? this.attackEffect.resourcePath : this.unAttackEffect.resourcePath), this, 0, Vector3.zero, Vector3.zero, null, false);
		}
	}
}