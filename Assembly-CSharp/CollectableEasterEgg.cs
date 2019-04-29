using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CollectableEasterEgg : BaseEntity
{
	public Transform artwork;

	public float bounceRange = 0.2f;

	public float bounceSpeed = 1f;

	public GameObjectRef pickupEffect;

	public ItemDefinition itemToGive;

	private float lastPickupStartTime;

	public CollectableEasterEgg()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("CollectableEasterEgg.OnRpcMessage", 0.1f))
		{
			if (rpc == -1858148972 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_PickUp "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_PickUp", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_PickUp", this, player, 3f))
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
							this.RPC_PickUp(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_PickUp");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -2051878907 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_StartPickUp "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_StartPickUp", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_StartPickUp", this, player, 3f))
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
							this.RPC_StartPickUp(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_StartPickUp");
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
	public void RPC_PickUp(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		float single = UnityEngine.Time.realtimeSinceStartup - this.lastPickupStartTime;
		if (!msg.player.GetHeldEntity() as EasterBasket)
		{
			if (single > 2f)
			{
				return;
			}
			if (single < 0.8f)
			{
				return;
			}
		}
		if (EggHuntEvent.serverEvent)
		{
			if (!EggHuntEvent.serverEvent.IsEventActive())
			{
				return;
			}
			EggHuntEvent.serverEvent.EggCollected(msg.player);
			int num = 1;
			msg.player.GiveItem(ItemManager.Create(this.itemToGive, num, (ulong)0), BaseEntity.GiveItemReason.Generic);
		}
		Effect.server.Run(this.pickupEffect.resourcePath, base.transform.position + (Vector3.up * 0.3f), Vector3.up, null, false);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_StartPickUp(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		this.lastPickupStartTime = UnityEngine.Time.realtimeSinceStartup;
	}

	public override void ServerInit()
	{
		int num = UnityEngine.Random.Range(0, 3);
		base.SetFlag(BaseEntity.Flags.Reserved1, num == 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, num == 1, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, num == 2, false, false);
		base.ServerInit();
	}
}