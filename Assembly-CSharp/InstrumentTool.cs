using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class InstrumentTool : HeldEntity
{
	public GameObjectRef[] soundEffect = new GameObjectRef[2];

	public InstrumentTool()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("InstrumentTool.OnRpcMessage", 0.1f))
		{
			if (rpc != -542161330 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SVPlayNote "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SVPlayNote", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("SVPlayNote", this, player))
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
							this.SVPlayNote(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SVPlayNote");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[IsActiveItem]
	[RPC_Server]
	private void SVPlayNote(BaseEntity.RPCMessage msg)
	{
		byte num = msg.read.UInt8();
		float single = msg.read.Float();
		EffectNetwork.Send(new Effect(this.soundEffect[num].resourcePath, this, 0, Vector3.zero, Vector3.forward, msg.connection)
		{
			scale = single
		});
	}
}