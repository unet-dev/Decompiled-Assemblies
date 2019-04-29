using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LootableCorpse : BaseCorpse
{
	public string lootPanelName = "generic";

	[NonSerialized]
	public ulong playerSteamID;

	[NonSerialized]
	public string _playerName;

	[NonSerialized]
	public ItemContainer[] containers;

	public string playerName
	{
		get
		{
			return this._playerName;
		}
		set
		{
			this._playerName = value;
		}
	}

	public LootableCorpse()
	{
	}

	public virtual bool CanLoot()
	{
		return true;
	}

	public override bool CanRemove()
	{
		return !base.IsOpen();
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.DropItems();
		if (this.containers != null)
		{
			ItemContainer[] itemContainerArray = this.containers;
			for (int i = 0; i < (int)itemContainerArray.Length; i++)
			{
				itemContainerArray[i].Kill();
			}
		}
		this.containers = null;
	}

	public void DropItems()
	{
		if (this.containers != null)
		{
			DroppedItemContainer droppedItemContainer = ItemContainer.Drop("assets/prefabs/misc/item drop/item_drop_backpack.prefab", base.transform.position, Quaternion.identity, this.containers);
			if (droppedItemContainer != null)
			{
				droppedItemContainer.playerName = this.playerName;
				droppedItemContainer.playerSteamID = this.playerSteamID;
			}
		}
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerName = info.msg.lootableCorpse.playerName;
			this.playerSteamID = info.msg.lootableCorpse.playerID;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("LootableCorpse.OnRpcMessage", 0.1f))
		{
			if (rpc != -2016507558 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_LootCorpse "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_LootCorpse", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_LootCorpse", this, player, 3f))
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
							this.RPC_LootCorpse(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_LootCorpse");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void PlayerStoppedLooting(BasePlayer player)
	{
		Interface.CallHook("OnLootEntityEnd", player, this);
		base.ResetRemovalTime();
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void RPC_LootCorpse(BaseEntity.RPCMessage rpc)
	{
		BasePlayer basePlayer = rpc.player;
		if (!basePlayer || !basePlayer.CanInteract())
		{
			return;
		}
		if (!this.CanLoot())
		{
			return;
		}
		if (this.containers == null)
		{
			return;
		}
		if (Interface.CallHook("CanLootEntity", basePlayer, this) != null)
		{
			return;
		}
		if (basePlayer.inventory.loot.StartLootingEntity(this, true))
		{
			base.SetFlag(BaseEntity.Flags.Open, true, false, true);
			ItemContainer[] itemContainerArray = this.containers;
			for (int i = 0; i < (int)itemContainerArray.Length; i++)
			{
				ItemContainer itemContainer = itemContainerArray[i];
				basePlayer.inventory.loot.AddContainer(itemContainer);
			}
			basePlayer.inventory.loot.SendImmediate();
			base.ClientRPCPlayer(null, basePlayer, "RPC_ClientLootCorpse");
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Facepunch.Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerName = this.playerName;
		info.msg.lootableCorpse.playerID = this.playerSteamID;
	}

	public override void ServerInit()
	{
		base.ServerInit();
	}

	public void TakeFrom(params ItemContainer[] source)
	{
		Assert.IsTrue(this.containers == null, "Initializing Twice");
		using (TimeWarning timeWarning = TimeWarning.New("Corpse.TakeFrom", 0.1f))
		{
			this.containers = new ItemContainer[(int)source.Length];
			for (int i = 0; i < (int)source.Length; i++)
			{
				this.containers[i] = new ItemContainer();
				this.containers[i].ServerInitialize(null, source[i].capacity);
				this.containers[i].GiveUID();
				this.containers[i].entityOwner = this;
				Item[] array = source[i].itemList.ToArray();
				for (int j = 0; j < (int)array.Length; j++)
				{
					Item item = array[j];
					if (!item.MoveToContainer(this.containers[i], -1, true))
					{
						item.Remove(0f);
					}
				}
			}
			base.ResetRemovalTime();
		}
	}
}