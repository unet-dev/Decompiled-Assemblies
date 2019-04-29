using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Signage : BaseCombatEntity, ILOD
{
	public GameObjectRef changeTextDialog;

	public MeshPaintableSource paintableSource;

	[NonSerialized]
	public uint textureID;

	public Signage()
	{
	}

	public bool CanLockSign(BasePlayer player)
	{
		if (base.IsLocked())
		{
			return false;
		}
		return this.CanUpdateSign(player);
	}

	public bool CanUnlockSign(BasePlayer player)
	{
		if (!base.IsLocked())
		{
			return false;
		}
		return this.CanUpdateSign(player);
	}

	public virtual bool CanUpdateSign(BasePlayer player)
	{
		object obj = Interface.CallHook("CanUpdateSign", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (player.IsAdmin || player.IsDeveloper)
		{
			return true;
		}
		if (!player.CanBuild())
		{
			return false;
		}
		if (!base.IsLocked())
		{
			return true;
		}
		return player.userID == base.OwnerID;
	}

	public override string Categorize()
	{
		return "sign";
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sign != null && info.msg.sign.imageid != this.textureID)
		{
			this.textureID = info.msg.sign.imageid;
		}
		if (base.isServer)
		{
			if (this.textureID != 0 && FileStorage.server.Get(this.textureID, FileStorage.Type.png, this.net.ID) == null)
			{
				this.textureID = 0;
			}
			if (this.textureID == 0)
			{
				base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
			}
		}
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void LockSign(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Locked, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.OwnerID = msg.player.userID;
		Interface.CallHook("OnSignLocked", this, msg.player);
	}

	public override void OnKilled(HitInfo info)
	{
		if (this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
		this.textureID = 0;
		base.OnKilled(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("Signage.OnRpcMessage", 0.1f))
		{
			if (rpc == 1455609404 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - LockSign "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("LockSign", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("LockSign", this, player, 3f))
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
							this.LockSign(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in LockSign");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == -145063042 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - UnLockSign "));
				}
				using (timeWarning1 = TimeWarning.New("UnLockSign", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("UnLockSign", this, player, 3f))
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
							this.UnLockSign(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in UnLockSign");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc != 1255380462 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - UpdateSign "));
				}
				using (timeWarning1 = TimeWarning.New("UpdateSign", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("UpdateSign", this, player, 3f))
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
							this.UpdateSign(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in UpdateSign");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sign = Facepunch.Pool.Get<Sign>();
		info.msg.sign.imageid = this.textureID;
	}

	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void UnLockSign(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanUnlockSign(msg.player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void UpdateSign(BaseEntity.RPCMessage msg)
	{
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		byte[] numArray = msg.read.BytesWithSize();
		if (numArray == null)
		{
			return;
		}
		if (!ImageProcessing.IsValidPNG(numArray, 1024, 1024))
		{
			return;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		this.textureID = FileStorage.server.Store(numArray, FileStorage.Type.png, this.net.ID, 0);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		Interface.CallHook("OnSignUpdated", this, msg.player);
	}
}