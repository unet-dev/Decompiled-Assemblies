using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HBHFSensor : BaseDetector
{
	public GameObjectRef detectUp;

	public GameObjectRef detectDown;

	public const BaseEntity.Flags Flag_IncludeOthers = BaseEntity.Flags.Reserved2;

	public const BaseEntity.Flags Flag_IncludeAuthed = BaseEntity.Flags.Reserved3;

	private int detectedPlayers;

	public HBHFSensor()
	{
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Min(this.detectedPlayers, this.GetCurrentEnergy());
	}

	public override void OnEmpty()
	{
		base.OnEmpty();
		this.UpdatePassthroughAmount();
		base.CancelInvoke(new Action(this.UpdatePassthroughAmount));
	}

	public override void OnObjects()
	{
		base.OnObjects();
		this.UpdatePassthroughAmount();
		base.InvokeRandomized(new Action(this.UpdatePassthroughAmount), 0f, 1f, 0.1f);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("HBHFSensor.OnRpcMessage", 0.1f))
		{
			if (rpc == -1088081576 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetIncludeAuth "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SetIncludeAuth", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetIncludeAuth", this, player, 3f))
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
							this.SetIncludeAuth(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SetIncludeAuth");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -2071763921 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetIncludeOthers "));
				}
				using (timeWarning1 = TimeWarning.New("SetIncludeOthers", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetIncludeOthers", this, player, 3f))
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
							this.SetIncludeOthers(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in SetIncludeOthers");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetIncludeAuth(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (msg.player.CanBuild() && this.IsPowered())
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, flag, false, true);
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetIncludeOthers(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (msg.player.CanBuild() && this.IsPowered())
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, flag, false, true);
		}
	}

	public bool ShouldIncludeAuthorized()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	public bool ShouldIncludeOthers()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	public void UpdatePassthroughAmount()
	{
		if (base.isClient)
		{
			return;
		}
		int num = this.detectedPlayers;
		this.detectedPlayers = 0;
		if (this.myTrigger.entityContents != null)
		{
			foreach (BaseEntity entityContent in this.myTrigger.entityContents)
			{
				if (entityContent == null || !entityContent.IsVisible(base.transform.position + (base.transform.forward * 0.1f), 10f))
				{
					continue;
				}
				BasePlayer component = entityContent.GetComponent<BasePlayer>();
				bool flag = component.CanBuild();
				if (flag && !this.ShouldIncludeAuthorized() || !flag && !this.ShouldIncludeOthers() || !(component != null) || !component.IsAlive() || component.IsSleeping() || !component.isServer)
				{
					continue;
				}
				this.detectedPlayers++;
			}
		}
		if (num != this.detectedPlayers && this.IsPowered())
		{
			this.MarkDirty();
			if (this.detectedPlayers > num)
			{
				Effect.server.Run(this.detectUp.resourcePath, base.transform.position, Vector3.up, null, false);
				return;
			}
			if (this.detectedPlayers < num)
			{
				Effect.server.Run(this.detectDown.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
	}
}