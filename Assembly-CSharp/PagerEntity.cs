using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class PagerEntity : BaseEntity, IRFObject
{
	public static BaseEntity.Flags Flag_Silent;

	private int frequency = 55;

	public float beepRepeat = 2f;

	public GameObjectRef pagerEffect;

	public GameObjectRef silentEffect;

	private float nextChangeTime;

	static PagerEntity()
	{
		PagerEntity.Flag_Silent = BaseEntity.Flags.Reserved1;
	}

	public PagerEntity()
	{
	}

	public void ChangeFrequency(int newFreq)
	{
		RFManager.ChangeFrequency(this.frequency, newFreq, this, true, true);
		this.frequency = newFreq;
	}

	internal override void DoServerDestroy()
	{
		RFManager.RemoveListener(this.frequency, this);
		base.DoServerDestroy();
	}

	public int GetFrequency()
	{
		return this.frequency;
	}

	public float GetMaxRange()
	{
		return Single.PositiveInfinity;
	}

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
		if (base.isServer && info.fromDisk)
		{
			this.ChangeFrequency(this.frequency);
		}
	}

	public void OnParentDestroying()
	{
		if (base.isServer)
		{
			base.transform.parent = null;
		}
	}

	internal override void OnParentRemoved()
	{
		base.SetParent(null, false, true);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("PagerEntity.OnRpcMessage", 0.1f))
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
		if (base.IsDestroyed)
		{
			return;
		}
		if (on != base.IsOn())
		{
			base.SetFlag(BaseEntity.Flags.On, on, false, true);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericInt1 = this.frequency;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		RFManager.AddListener(this.frequency, this);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void ServerSetFrequency(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 2f;
		int num = msg.read.Int32();
		RFManager.ChangeFrequency(this.frequency, num, this, true, true);
		this.frequency = num;
		base.SendNetworkUpdateImmediate(false);
	}

	public void SetOff()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	public void SetSilentMode(bool wantsSilent)
	{
		base.SetFlag(PagerEntity.Flag_Silent, wantsSilent, false, true);
	}

	public override void SwitchParent(BaseEntity ent)
	{
		base.SetParent(ent, false, true);
	}
}