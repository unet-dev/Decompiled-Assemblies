using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CustomDoorManipulator : DoorManipulator
{
	public CustomDoorManipulator()
	{
	}

	public bool CanPlayerAdmin(BasePlayer player)
	{
		if (!(player != null) || !player.CanBuild())
		{
			return false;
		}
		return !base.IsOn();
	}

	public override void DoActionDoorMissing()
	{
		this.SetTargetDoor(null);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void DoPair(BaseEntity.RPCMessage msg)
	{
		Door door = this.targetDoor;
		Door door1 = base.FindDoor(this.PairWithLockedDoors());
		if (door1 != door)
		{
			this.SetTargetDoor(door1);
		}
	}

	public bool IsPaired()
	{
		return this.targetDoor != null;
	}

	private void OnPhysicsNeighbourChanged()
	{
		this.SetTargetDoor(this.targetDoor);
		base.Invoke(new Action(this.RefreshDoor), 0.1f);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("CustomDoorManipulator.OnRpcMessage", 0.1f))
		{
			if (rpc == 1224330484 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoPair "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoPair", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("DoPair", this, player, 3f))
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
							this.DoPair(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoPair");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -494240324 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ServerActionChange "));
				}
				using (timeWarning1 = TimeWarning.New("ServerActionChange", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("ServerActionChange", this, player, 3f))
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
							this.ServerActionChange(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in ServerActionChange");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override bool PairWithLockedDoors()
	{
		return false;
	}

	public void RefreshDoor()
	{
		this.SetTargetDoor(this.targetDoor);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void ServerActionChange(BaseEntity.RPCMessage msg)
	{
	}

	public override void SetupInitialDoorConnection()
	{
		if (this.entityRef.IsValid(true) && this.targetDoor == null)
		{
			this.SetTargetDoor(this.entityRef.Get(true).GetComponent<Door>());
		}
	}
}