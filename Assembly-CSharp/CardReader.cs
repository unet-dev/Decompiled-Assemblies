using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CardReader : IOEntity
{
	public float accessDuration = 10f;

	public int accessLevel;

	public GameObjectRef accessGrantedEffect;

	public GameObjectRef accessDeniedEffect;

	public GameObjectRef swipeEffect;

	public Transform audioPosition;

	public BaseEntity.Flags AccessLevel1 = BaseEntity.Flags.Reserved1;

	public BaseEntity.Flags AccessLevel2 = BaseEntity.Flags.Reserved2;

	public BaseEntity.Flags AccessLevel3 = BaseEntity.Flags.Reserved3;

	public CardReader()
	{
	}

	public void CancelAccess()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.MarkDirty();
	}

	public void FailCard()
	{
		Effect.server.Run(this.accessDeniedEffect.resourcePath, this.audioPosition.position, Vector3.up, null, false);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	public void GrantCard()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.MarkDirty();
		Effect.server.Run(this.accessGrantedEffect.resourcePath, this.audioPosition.position, Vector3.up, null, false);
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.accessLevel = info.msg.ioEntity.genericInt1;
			this.accessDuration = info.msg.ioEntity.genericFloat1;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("CardReader.OnRpcMessage", 0.1f))
		{
			if (rpc != 979061374 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ServerCardSwiped "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("ServerCardSwiped", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("ServerCardSwiped", this, player, 3f))
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
							this.ServerCardSwiped(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in ServerCardSwiped");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void ResetIOState()
	{
		base.ResetIOState();
		base.CancelInvoke(new Action(this.GrantCard));
		base.CancelInvoke(new Action(this.CancelAccess));
		this.CancelAccess();
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.accessLevel;
		info.msg.ioEntity.genericFloat1 = this.accessDuration;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void ServerCardSwiped(BaseEntity.RPCMessage msg)
	{
		if (!this.IsPowered())
		{
			return;
		}
		if (Vector3Ex.Distance2D(msg.player.transform.position, base.transform.position) > 1f)
		{
			return;
		}
		if (base.IsInvoking(new Action(this.GrantCard)) || base.IsInvoking(new Action(this.FailCard)))
		{
			return;
		}
		uint num = msg.read.UInt32();
		Keycard keycard = BaseNetworkable.serverEntities.Find(num) as Keycard;
		Effect.server.Run(this.swipeEffect.resourcePath, this.audioPosition.position, Vector3.up, msg.player.net.connection, false);
		if (keycard != null)
		{
			Item item = keycard.GetItem();
			if (item != null && keycard.accessLevel == this.accessLevel && item.conditionNormalized > 0f)
			{
				base.Invoke(new Action(this.GrantCard), 0.5f);
				item.LoseCondition(1f);
				return;
			}
			base.Invoke(new Action(this.FailCard), 0.5f);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(this.AccessLevel1, this.accessLevel == 1, false, true);
		base.SetFlag(this.AccessLevel2, this.accessLevel == 2, false, true);
		base.SetFlag(this.AccessLevel3, this.accessLevel == 3, false, true);
	}
}