using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class FreeableLootContainer : LootContainer
{
	private const BaseEntity.Flags tiedDown = BaseEntity.Flags.Reserved8;

	public Buoyancy buoyancy;

	public GameObjectRef freedEffect;

	private Rigidbody rb;

	public uint skinOverride;

	public FreeableLootContainer()
	{
	}

	public Rigidbody GetRB()
	{
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		return this.rb;
	}

	public bool IsTiedDown()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("FreeableLootContainer.OnRpcMessage", 0.1f))
		{
			if (rpc != -2092281351 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_FreeCrate "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_FreeCrate", 0.1f))
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
							this.RPC_FreeCrate(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_FreeCrate");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[RPC_Server]
	public void RPC_FreeCrate(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTiedDown())
		{
			return;
		}
		this.GetRB().isKinematic = false;
		this.buoyancy.enabled = true;
		this.buoyancy.buoyancyScale = 1f;
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		if (this.freedEffect.isValid)
		{
			Effect.server.Run(this.freedEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
	}

	public override void ServerInit()
	{
		this.GetRB().isKinematic = true;
		this.buoyancy.buoyancyScale = 0f;
		this.buoyancy.enabled = false;
		base.ServerInit();
		if (this.skinOverride != 0)
		{
			this.skinID = (ulong)this.skinOverride;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}
}