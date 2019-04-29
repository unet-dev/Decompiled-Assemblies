using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RustigeEgg : BaseCombatEntity
{
	public const BaseEntity.Flags Flag_Spin = BaseEntity.Flags.Reserved1;

	public Transform eggRotationTransform;

	public RustigeEgg()
	{
	}

	public void CloseEgg()
	{
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
	}

	public bool IsSpinning()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("RustigeEgg.OnRpcMessage", 0.1f))
		{
			if (rpc == -40772121 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Open "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Open", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Open", this, player, 3f))
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
							this.RPC_Open(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Open");
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
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Spin", this, player, 3f))
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

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_Open(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == base.IsOpen())
		{
			return;
		}
		if (!flag)
		{
			base.CancelInvoke(new Action(this.CloseEgg));
		}
		else
		{
			base.ClientRPC<Vector3>(null, "FaceEggPosition", msg.player.eyes.position);
			base.Invoke(new Action(this.CloseEgg), 60f);
		}
		base.SetFlag(BaseEntity.Flags.Open, flag, false, false);
		if (this.IsSpinning() & flag)
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, false);
		}
		base.SendNetworkUpdateImmediate(false);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_Spin(BaseEntity.RPCMessage msg)
	{
	}
}