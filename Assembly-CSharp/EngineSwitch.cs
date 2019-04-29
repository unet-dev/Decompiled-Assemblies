using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class EngineSwitch : BaseEntity
{
	public EngineSwitch()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("EngineSwitch.OnRpcMessage", 0.1f))
		{
			if (rpc == 1249530220 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - StartEngine "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("StartEngine", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("StartEngine", this, player, 3f))
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
							this.StartEngine(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in StartEngine");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 1739656243 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - StopEngine "));
				}
				using (timeWarning1 = TimeWarning.New("StopEngine", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("StopEngine", this, player, 3f))
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
							this.StopEngine(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in StopEngine");
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
	public void StartEngine(BaseEntity.RPCMessage msg)
	{
		MiningQuarry parentEntity = base.GetParentEntity() as MiningQuarry;
		if (parentEntity)
		{
			parentEntity.EngineSwitch(true);
		}
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void StopEngine(BaseEntity.RPCMessage msg)
	{
		MiningQuarry parentEntity = base.GetParentEntity() as MiningQuarry;
		if (parentEntity)
		{
			parentEntity.EngineSwitch(false);
		}
	}
}