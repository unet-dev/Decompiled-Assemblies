using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DroppedItemContainer : BaseCombatEntity
{
	public string lootPanelName = "generic";

	[NonSerialized]
	public ulong playerSteamID;

	[NonSerialized]
	public string _playerName;

	public ItemContainer inventory;

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

	public DroppedItemContainer()
	{
	}

	public float CalculateRemovalTime()
	{
		int num = 1;
		if (this.inventory != null)
		{
			foreach (Item item in this.inventory.itemList)
			{
				num = Mathf.Max(num, item.despawnMultiplier);
			}
		}
		return (float)num * ConVar.Server.itemdespawn;
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.inventory != null)
		{
			this.inventory.Kill();
			this.inventory = null;
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
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				return;
			}
			Debug.LogWarning(string.Concat("Dropped item container without inventory: ", this.ToString()));
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("DroppedItemContainer.OnRpcMessage", 0.1f))
		{
			if (rpc != 331989034 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_OpenLoot "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_OpenLoot", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_OpenLoot", this, player, 3f))
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
							this.RPC_OpenLoot(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_OpenLoot");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (baseEntity.InSafeZone() && baseEntity.userID != this.playerSteamID)
		{
			return false;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public void PlayerStoppedLooting(BasePlayer player)
	{
		if (this.inventory == null || this.inventory.itemList == null || this.inventory.itemList.Count == 0)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		this.ResetRemovalTime();
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.inventory = new ItemContainer()
		{
			entityOwner = this
		};
		this.inventory.ServerInitialize(null, 0);
		this.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
	}

	public void RemoveMe()
	{
		if (base.IsOpen())
		{
			this.ResetRemovalTime();
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning timeWarning = TimeWarning.New("ResetRemovalTime", 0.1f))
		{
			base.Invoke(new Action(this.RemoveMe), dur);
		}
	}

	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.CalculateRemovalTime());
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void RPC_OpenLoot(BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		BasePlayer basePlayer = rpc.player;
		if (!basePlayer || !basePlayer.CanInteract())
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
			basePlayer.inventory.loot.AddContainer(this.inventory);
			basePlayer.inventory.loot.SendImmediate();
			basePlayer.ClientRPCPlayer<string>(null, basePlayer, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Facepunch.Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerName = this.playerName;
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning(string.Concat("Dropped item container without inventory: ", this.ToString()));
		}
	}

	public override void ServerInit()
	{
		this.ResetRemovalTime();
		base.ServerInit();
	}

	public void TakeFrom(params ItemContainer[] source)
	{
		int i;
		Assert.IsTrue(this.inventory == null, "Initializing Twice");
		using (TimeWarning timeWarning = TimeWarning.New("DroppedItemContainer.TakeFrom", 0.1f))
		{
			int count = 0;
			ItemContainer[] itemContainerArray = source;
			for (i = 0; i < (int)itemContainerArray.Length; i++)
			{
				count += itemContainerArray[i].itemList.Count;
			}
			this.inventory = new ItemContainer();
			this.inventory.ServerInitialize(null, count);
			this.inventory.GiveUID();
			this.inventory.entityOwner = this;
			this.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
			itemContainerArray = source;
			for (i = 0; i < (int)itemContainerArray.Length; i++)
			{
				Item[] array = itemContainerArray[i].itemList.ToArray();
				for (int j = 0; j < (int)array.Length; j++)
				{
					Item item = array[j];
					if (!item.MoveToContainer(this.inventory, -1, true))
					{
						item.Remove(0f);
					}
				}
			}
			this.ResetRemovalTime();
		}
	}
}