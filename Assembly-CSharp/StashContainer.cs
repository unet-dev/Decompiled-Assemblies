using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class StashContainer : StorageContainer
{
	public Transform visuals;

	public float burriedOffset;

	public float raisedOffset;

	public GameObjectRef buryEffect;

	public float uncoverRange = 3f;

	private float lastToggleTime;

	public StashContainer()
	{
	}

	public void Decay()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public void DisableNetworking()
	{
		base.limitNetworking = true;
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
	}

	public bool IsHidden()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		BaseEntity.RPCMessage rPCMessage;
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("StashContainer.OnRpcMessage", 0.1f))
		{
			if (rpc == -164704220 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_HideStash "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_HideStash", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_HideStash(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_HideStash");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 298671803 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_WantsUnhide "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_WantsUnhide", 0.1f))
				{
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
							this.RPC_WantsUnhide(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_WantsUnhide");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public bool PlayerInRange(BasePlayer ply)
	{
		if (Vector3.Distance(base.transform.position, ply.transform.position) <= this.uncoverRange)
		{
			Vector3 vector3 = base.transform.position - ply.eyes.position;
			Vector3 vector31 = vector3.normalized;
			if (Vector3.Dot(ply.eyes.BodyForward(), vector31) > 0.95f)
			{
				return true;
			}
		}
		return false;
	}

	[RPC_Server]
	public void RPC_HideStash(BaseEntity.RPCMessage rpc)
	{
		if (Interface.CallHook("CanHideStash", rpc.player, this) != null)
		{
			return;
		}
		this.SetHidden(true);
	}

	[RPC_Server]
	public void RPC_WantsUnhide(BaseEntity.RPCMessage rpc)
	{
		if (!this.IsHidden())
		{
			return;
		}
		if (Interface.CallHook("CanSeeStash", rpc.player, this) != null)
		{
			return;
		}
		if (this.PlayerInRange(rpc.player))
		{
			this.SetHidden(false);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.SetHidden(false);
	}

	public void SetHidden(bool isHidden)
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastToggleTime < 3f)
		{
			return;
		}
		if (isHidden == base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			return;
		}
		this.lastToggleTime = UnityEngine.Time.realtimeSinceStartup;
		base.Invoke(new Action(this.Decay), 259200f);
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, isHidden, false, true);
		}
	}

	public void ToggleHidden()
	{
		this.SetHidden(!this.IsHidden());
	}

	public static class StashContainerFlags
	{
		public const BaseEntity.Flags Hidden = BaseEntity.Flags.Reserved5;
	}
}