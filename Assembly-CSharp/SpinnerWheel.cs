using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SpinnerWheel : Signage
{
	public Transform wheel;

	public float velocity;

	public Quaternion targetRotation = Quaternion.identity;

	[Header("Sound")]
	public SoundDefinition spinLoopSoundDef;

	public SoundDefinition spinStartSoundDef;

	public SoundDefinition spinAccentSoundDef;

	public SoundDefinition spinStopSoundDef;

	public float minTimeBetweenSpinAccentSounds = 0.3f;

	public float spinAccentAngleDelta = 180f;

	private Sound spinSound;

	private SoundModulation.Modulator spinSoundGain;

	public SpinnerWheel()
	{
	}

	public virtual bool AllowPlayerSpins()
	{
		return true;
	}

	public bool AnyoneSpin()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	public virtual float GetMaxSpinSpeed()
	{
		return 720f;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spinnerWheel != null)
		{
			Quaternion quaternion = Quaternion.Euler(info.msg.spinnerWheel.spin);
			if (base.isServer)
			{
				this.wheel.transform.rotation = quaternion;
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("SpinnerWheel.OnRpcMessage", 0.1f))
		{
			if (rpc == -1275292189 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_AnyoneSpin "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_AnyoneSpin", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_AnyoneSpin", this, player, 3f))
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
							this.RPC_AnyoneSpin(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_AnyoneSpin");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 1455840454 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Spin "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_Spin", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Spin", this, player, 3f))
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
							this.RPC_Spin(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_Spin");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_AnyoneSpin(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, rpc.read.Bit(), false, true);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_Spin(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.AllowPlayerSpins())
		{
			return;
		}
		if (this.AnyoneSpin() || rpc.player.CanBuild())
		{
			Interface.CallHook("OnSpinWheel", rpc.player, this);
			if (this.velocity > 15f)
			{
				return;
			}
			this.velocity += UnityEngine.Random.Range(4f, 7f);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.spinnerWheel = Facepunch.Pool.Get<ProtoBuf.SpinnerWheel>();
		info.msg.spinnerWheel.spin = this.wheel.rotation.eulerAngles;
	}

	public void Update()
	{
		if (base.isClient)
		{
			this.Update_Client();
		}
		if (base.isServer)
		{
			this.Update_Server();
		}
	}

	public void Update_Client()
	{
	}

	public virtual void Update_Server()
	{
		if (this.velocity > 0f)
		{
			float single = Mathf.Clamp(this.GetMaxSpinSpeed() * this.velocity, 0f, this.GetMaxSpinSpeed());
			this.velocity = this.velocity - UnityEngine.Time.deltaTime * Mathf.Clamp(this.velocity / 2f, 0.1f, 1f);
			if (this.velocity < 0f)
			{
				this.velocity = 0f;
			}
			this.wheel.Rotate(Vector3.up, single * UnityEngine.Time.deltaTime, Space.Self);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}
}