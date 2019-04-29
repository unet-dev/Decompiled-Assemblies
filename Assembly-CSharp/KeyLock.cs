using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class KeyLock : BaseLock
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition keyItemType;

	public int keyCode;

	public bool firstKeyCreated;

	public KeyLock()
	{
	}

	private bool CanKeyUnlockUs(Item key)
	{
		if (key.instanceData == null)
		{
			return false;
		}
		if (key.instanceData.dataInt != this.keyCode)
		{
			return false;
		}
		return true;
	}

	public override bool HasLockPermission(BasePlayer player)
	{
		bool flag;
		if (player.IsDead())
		{
			return false;
		}
		if (player.userID == base.OwnerID)
		{
			return true;
		}
		List<Item>.Enumerator enumerator = player.inventory.FindItemIDs(this.keyItemType.itemid).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if (!this.CanKeyUnlockUs(enumerator.Current))
				{
					continue;
				}
				flag = true;
				return flag;
			}
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.keyLock != null)
		{
			this.keyCode = info.msg.keyLock.code;
		}
	}

	public void LockLock(BasePlayer player)
	{
		base.SetFlag(BaseEntity.Flags.Locked, true, false, true);
		if (player.IsValid())
		{
			player.GiveAchievement("LOCK_LOCK");
		}
	}

	public override void OnDeployed(BaseEntity parent)
	{
		base.OnDeployed(parent);
		this.keyCode = UnityEngine.Random.Range(1, 100000);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("KeyLock.OnRpcMessage", 0.1f))
		{
			if (rpc == -159552843 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_CreateKey "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_CreateKey", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_CreateKey", this, player, 3f))
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
							this.RPC_CreateKey(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_CreateKey");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 954115386 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Lock "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_Lock", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Lock", this, player, 3f))
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
							this.RPC_Lock(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_Lock");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc != 1663222372 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Unlock "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_Unlock", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Unlock", this, player, 3f))
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
							this.RPC_Unlock(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in RPC_Unlock");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override bool OnTryToClose(BasePlayer player)
	{
		object obj = Interface.CallHook("CanUseLockedEntity", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (this.HasLockPermission(player))
		{
			return true;
		}
		return !base.IsLocked();
	}

	public override bool OnTryToOpen(BasePlayer player)
	{
		object obj = Interface.CallHook("CanUseLockedEntity", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (this.HasLockPermission(player))
		{
			return true;
		}
		return !base.IsLocked();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.OwnerID == 0 && base.GetParentEntity())
		{
			base.OwnerID = base.GetParentEntity().OwnerID;
		}
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_CreateKey(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked() && !this.HasLockPermission(rpc.player))
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(this.keyItemType.itemid);
		if (itemDefinition == null)
		{
			Debug.LogWarning(string.Concat("RPC_CreateKey: Itemdef is missing! ", this.keyItemType));
			return;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(itemDefinition);
		if (!rpc.player.inventory.crafting.CanCraft(itemBlueprint, 1))
		{
			return;
		}
		ProtoBuf.Item.InstanceData instanceDatum = Facepunch.Pool.Get<ProtoBuf.Item.InstanceData>();
		instanceDatum.dataInt = this.keyCode;
		rpc.player.inventory.crafting.CraftItem(itemBlueprint, rpc.player, instanceDatum, 1, 0, null);
		if (!this.firstKeyCreated)
		{
			this.LockLock(rpc.player);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			this.firstKeyCreated = true;
		}
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_Lock(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (Interface.CallHook("CanLock", rpc.player, this) != null)
		{
			return;
		}
		if (!this.HasLockPermission(rpc.player))
		{
			return;
		}
		this.LockLock(rpc.player);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_Unlock(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (Interface.CallHook("CanUnlock", rpc.player, this) != null)
		{
			return;
		}
		if (!this.HasLockPermission(rpc.player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.keyLock = Facepunch.Pool.Get<ProtoBuf.KeyLock>();
		info.msg.keyLock.code = this.keyCode;
	}

	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}
}