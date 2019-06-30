using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInventory : EntityComponent<BasePlayer>
{
	public ItemContainer containerMain;

	public ItemContainer containerBelt;

	public ItemContainer containerWear;

	public ItemCrafter crafting;

	public PlayerLoot loot;

	[ServerVar]
	public static bool forceBirthday;

	private static float nextCheckTime;

	private static bool wasBirthday;

	static PlayerInventory()
	{
	}

	public PlayerInventory()
	{
	}

	public Item[] AllItems()
	{
		List<Item> items = new List<Item>();
		if (this.containerMain != null)
		{
			items.AddRange(this.containerMain.itemList);
		}
		if (this.containerBelt != null)
		{
			items.AddRange(this.containerBelt.itemList);
		}
		if (this.containerWear != null)
		{
			items.AddRange(this.containerWear.itemList);
		}
		return items.ToArray();
	}

	public int AllItemsNoAlloc(ref List<Item> items)
	{
		items.Clear();
		if (this.containerMain != null)
		{
			items.AddRange(this.containerMain.itemList);
		}
		if (this.containerBelt != null)
		{
			items.AddRange(this.containerBelt.itemList);
		}
		if (this.containerWear != null)
		{
			items.AddRange(this.containerWear.itemList);
		}
		return items.Count;
	}

	private bool CanEquipItem(Item item, int targetSlot)
	{
		object obj = Interface.CallHook("CanEquipItem", this, item, targetSlot);
		if (obj as bool)
		{
			return (bool)obj;
		}
		ItemModContainerRestriction component = item.info.GetComponent<ItemModContainerRestriction>();
		if (component == null)
		{
			return true;
		}
		Item[] array = this.containerBelt.itemList.ToArray();
		for (int i = 0; i < (int)array.Length; i++)
		{
			Item item1 = array[i];
			if (item1 != item)
			{
				ItemModContainerRestriction itemModContainerRestriction = item1.info.GetComponent<ItemModContainerRestriction>();
				if (!(itemModContainerRestriction == null) && !component.CanExistWith(itemModContainerRestriction) && !item1.MoveToContainer(this.containerMain, -1, true))
				{
					item1.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), new Quaternion());
				}
			}
		}
		return true;
	}

	private bool CanMoveItemsFrom(BaseEntity entity, Item item)
	{
		StorageContainer storageContainer = entity as StorageContainer;
		if (storageContainer && !storageContainer.CanMoveFrom(base.baseEntity, item))
		{
			return false;
		}
		return true;
	}

	private bool CanWearItem(Item item, int targetSlot)
	{
		ItemModWearable component = item.info.GetComponent<ItemModWearable>();
		if (component == null)
		{
			return false;
		}
		object obj = Interface.CallHook("CanWearItem", this, item, targetSlot);
		if (obj as bool)
		{
			return (bool)obj;
		}
		Item[] array = this.containerWear.itemList.ToArray();
		for (int i = 0; i < (int)array.Length; i++)
		{
			Item item1 = array[i];
			if (item1 != item)
			{
				ItemModWearable itemModWearable = item1.info.GetComponent<ItemModWearable>();
				if (!(itemModWearable == null) && !component.CanExistWith(itemModWearable) && !item1.MoveToContainer(this.containerMain, -1, true))
				{
					item1.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), new Quaternion());
				}
			}
		}
		return true;
	}

	public void DoDestroy()
	{
		if (this.containerMain != null)
		{
			this.containerMain.Kill();
			this.containerMain = null;
		}
		if (this.containerBelt != null)
		{
			this.containerBelt.Kill();
			this.containerBelt = null;
		}
		if (this.containerWear != null)
		{
			this.containerWear.Kill();
			this.containerWear = null;
		}
	}

	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		if (this.containerMain != null)
		{
			this.containerMain.FindAmmo(list, ammoType);
		}
		if (this.containerBelt != null)
		{
			this.containerBelt.FindAmmo(list, ammoType);
		}
	}

	public ItemContainer FindContainer(uint id)
	{
		ItemContainer itemContainer;
		using (TimeWarning timeWarning = TimeWarning.New("FindContainer", 0.1f))
		{
			ItemContainer itemContainer1 = this.containerMain.FindContainer(id);
			if (itemContainer1 == null)
			{
				itemContainer1 = this.containerBelt.FindContainer(id);
				if (itemContainer1 == null)
				{
					itemContainer1 = this.containerWear.FindContainer(id);
					itemContainer = (itemContainer1 == null ? this.loot.FindContainer(id) : itemContainer1);
				}
				else
				{
					itemContainer = itemContainer1;
				}
			}
			else
			{
				itemContainer = itemContainer1;
			}
		}
		return itemContainer;
	}

	public Item FindItemID(string itemName)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemName);
		if (itemDefinition == null)
		{
			return null;
		}
		return this.FindItemID(itemDefinition.itemid);
	}

	public Item FindItemID(int id)
	{
		if (this.containerMain != null)
		{
			Item item = this.containerMain.FindItemByItemID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			Item item1 = this.containerBelt.FindItemByItemID(id);
			if (item1 != null && item1.IsValid())
			{
				return item1;
			}
		}
		if (this.containerWear != null)
		{
			Item item2 = this.containerWear.FindItemByItemID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		return null;
	}

	public List<Item> FindItemIDs(int id)
	{
		List<Item> items = new List<Item>();
		if (this.containerMain != null)
		{
			items.AddRange(this.containerMain.FindItemsByItemID(id));
		}
		if (this.containerBelt != null)
		{
			items.AddRange(this.containerBelt.FindItemsByItemID(id));
		}
		if (this.containerWear != null)
		{
			items.AddRange(this.containerWear.FindItemsByItemID(id));
		}
		return items;
	}

	public Item FindItemUID(uint id)
	{
		if (id == 0)
		{
			return null;
		}
		if (this.containerMain != null)
		{
			Item item = this.containerMain.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			Item item1 = this.containerBelt.FindItemByUID(id);
			if (item1 != null && item1.IsValid())
			{
				return item1;
			}
		}
		if (this.containerWear != null)
		{
			Item item2 = this.containerWear.FindItemByUID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		return this.loot.FindItem(id);
	}

	public int GetAmount(int itemid)
	{
		if (itemid == 0)
		{
			return 0;
		}
		int amount = 0;
		if (this.containerMain != null)
		{
			amount += this.containerMain.GetAmount(itemid, true);
		}
		if (this.containerBelt != null)
		{
			amount += this.containerBelt.GetAmount(itemid, true);
		}
		if (this.containerWear != null)
		{
			amount += this.containerWear.GetAmount(itemid, true);
		}
		return amount;
	}

	public ItemContainer GetContainer(PlayerInventory.Type id)
	{
		if (id == PlayerInventory.Type.Main)
		{
			return this.containerMain;
		}
		if (PlayerInventory.Type.Belt == id)
		{
			return this.containerBelt;
		}
		if (PlayerInventory.Type.Wear != id)
		{
			return null;
		}
		return this.containerWear;
	}

	protected void GetIdealPickupContainer(Item item, ref ItemContainer container, ref int position)
	{
		if (item.info.stackable > 1)
		{
			if (this.containerBelt != null && this.containerBelt.FindItemByItemID(item.info.itemid) != null)
			{
				container = this.containerBelt;
				return;
			}
			if (this.containerMain != null && this.containerMain.FindItemByItemID(item.info.itemid) != null)
			{
				container = this.containerMain;
				return;
			}
		}
		if (!item.info.isUsable || item.info.HasFlag(ItemDefinition.Flag.NotStraightToBelt))
		{
			return;
		}
		container = this.containerBelt;
	}

	public void GiveDefaultItems()
	{
		this.Strip();
		ulong property = (ulong)0;
		int infoInt = base.baseEntity.GetInfoInt("client.rockskin", 0);
		if (infoInt > 0 && base.baseEntity.blueprints.steamInventory.HasItem(infoInt))
		{
			InventoryDef inventoryDef = Steamworks.SteamInventory.FindDefinition(infoInt);
			if (inventoryDef != null)
			{
				property = inventoryDef.GetProperty<ulong>("workshopdownload");
			}
		}
		this.GiveItem(ItemManager.CreateByName("rock", 1, property), this.containerBelt);
		this.GiveItem(ItemManager.CreateByName("torch", 1, (ulong)0), this.containerBelt);
		if (PlayerInventory.IsBirthday())
		{
			this.GiveItem(ItemManager.CreateByName("cakefiveyear", 1, (ulong)0), this.containerBelt);
			this.GiveItem(ItemManager.CreateByName("partyhat", 1, (ulong)0), this.containerWear);
		}
		if (PlayerInventory.IsChristmas())
		{
			this.GiveItem(ItemManager.CreateByName("snowball", 1, (ulong)0), this.containerBelt);
			this.GiveItem(ItemManager.CreateByName("snowball", 1, (ulong)0), this.containerBelt);
			this.GiveItem(ItemManager.CreateByName("snowball", 1, (ulong)0), this.containerBelt);
		}
	}

	public bool GiveItem(Item item, ItemContainer container = null)
	{
		if (item == null)
		{
			return false;
		}
		int num = -1;
		this.GetIdealPickupContainer(item, ref container, ref num);
		if (container != null && item.MoveToContainer(container, num, true))
		{
			return true;
		}
		if (item.MoveToContainer(this.containerMain, -1, true))
		{
			return true;
		}
		if (item.MoveToContainer(this.containerBelt, -1, true))
		{
			return true;
		}
		return false;
	}

	public bool HasAmmo(AmmoTypes ammoType)
	{
		if (this.containerMain.HasAmmo(ammoType))
		{
			return true;
		}
		return this.containerBelt.HasAmmo(ammoType);
	}

	protected void Initialize()
	{
		this.containerMain = new ItemContainer();
		this.containerMain.SetFlag(ItemContainer.Flag.IsPlayer, true);
		this.containerBelt = new ItemContainer();
		this.containerBelt.SetFlag(ItemContainer.Flag.IsPlayer, true);
		this.containerBelt.SetFlag(ItemContainer.Flag.Belt, true);
		this.containerWear = new ItemContainer();
		this.containerWear.SetFlag(ItemContainer.Flag.IsPlayer, true);
		this.containerWear.SetFlag(ItemContainer.Flag.Clothing, true);
		this.crafting = base.GetComponent<ItemCrafter>();
		this.crafting.AddContainer(this.containerMain);
		this.crafting.AddContainer(this.containerBelt);
		this.loot = base.GetComponent<PlayerLoot>();
		if (!this.loot)
		{
			this.loot = base.gameObject.AddComponent<PlayerLoot>();
		}
	}

	public static bool IsBirthday()
	{
		if (PlayerInventory.forceBirthday)
		{
			return true;
		}
		if (UnityEngine.Time.time < PlayerInventory.nextCheckTime)
		{
			return PlayerInventory.wasBirthday;
		}
		PlayerInventory.nextCheckTime = UnityEngine.Time.time + 60f;
		DateTime now = DateTime.Now;
		PlayerInventory.wasBirthday = (now.Day != 11 ? false : now.Month == 12);
		return PlayerInventory.wasBirthday;
	}

	public static bool IsChristmas()
	{
		return XMas.enabled;
	}

	[FromOwner]
	[RPC_Server]
	private void ItemCmd(BaseEntity.RPCMessage msg)
	{
		Quaternion quaternion;
		uint num = msg.read.UInt32();
		string str = msg.read.String();
		Item item = this.FindItemUID(num);
		if (item == null)
		{
			return;
		}
		if (Interface.CallHook("OnItemAction", item, str, msg.player) != null)
		{
			return;
		}
		if (item.IsLocked())
		{
			return;
		}
		if (!this.CanMoveItemsFrom(item.parent.entityOwner, item))
		{
			return;
		}
		if (str != "drop")
		{
			item.ServerCommand(str, base.baseEntity);
			ItemManager.DoRemoves();
			this.ServerUpdate(0f);
			return;
		}
		int num1 = item.amount;
		if (msg.read.Unread >= 4)
		{
			num1 = msg.read.Int32();
		}
		base.baseEntity.stats.Add("item_drop", 1, Stats.Steam);
		if (num1 >= item.amount)
		{
			Vector3 dropPosition = base.baseEntity.GetDropPosition();
			Vector3 dropVelocity = base.baseEntity.GetDropVelocity();
			quaternion = new Quaternion();
			item.Drop(dropPosition, dropVelocity, quaternion);
		}
		else
		{
			Item item1 = item.SplitItem(num1);
			if (item1 != null)
			{
				Vector3 vector3 = base.baseEntity.GetDropPosition();
				Vector3 dropVelocity1 = base.baseEntity.GetDropVelocity();
				quaternion = new Quaternion();
				item1.Drop(vector3, dropVelocity1, quaternion);
			}
		}
		base.baseEntity.SignalBroadcast(BaseEntity.Signal.Gesture, "drop_item", null);
	}

	public void Load(ProtoBuf.PlayerInventory msg)
	{
		if (msg.invMain != null)
		{
			this.containerMain.Load(msg.invMain);
		}
		if (msg.invBelt != null)
		{
			this.containerBelt.Load(msg.invBelt);
		}
		if (msg.invWear != null)
		{
			this.containerWear.Load(msg.invWear);
		}
	}

	[FromOwner]
	[RPC_Server]
	private void MoveItem(BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		uint num1 = msg.read.UInt32();
		int num2 = msg.read.Int8();
		int num3 = msg.read.UInt16();
		Item item = this.FindItemUID(num);
		if (item == null)
		{
			msg.player.ChatMessage(string.Concat("Invalid item (", num, ")"));
			return;
		}
		if (Interface.CallHook("CanMoveItem", item, this, num1, num2, num3) != null)
		{
			return;
		}
		if (!this.CanMoveItemsFrom(item.parent.entityOwner, item))
		{
			msg.player.ChatMessage("Cannot move item!");
			return;
		}
		if (num3 <= 0)
		{
			num3 = item.amount;
		}
		num3 = Mathf.Clamp(num3, 1, item.MaxStackable());
		if (num1 == 0)
		{
			if (!this.GiveItem(item, null))
			{
				msg.player.ChatMessage("GiveItem failed!");
			}
			return;
		}
		ItemContainer itemContainer = this.FindContainer(num1);
		if (itemContainer == null)
		{
			msg.player.ChatMessage(string.Concat("Invalid container (", num1, ")"));
			return;
		}
		ItemContainer itemContainer1 = item.parent;
		if (itemContainer1 != null && itemContainer1.IsLocked() || itemContainer.IsLocked())
		{
			msg.player.ChatMessage("Container is locked!");
			return;
		}
		if (itemContainer.PlayerItemInputBlocked())
		{
			msg.player.ChatMessage("Container does not accept player items!");
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("Split", 0.1f))
		{
			if (item.amount > num3)
			{
				Item item1 = item.SplitItem(num3);
				if (!item1.MoveToContainer(itemContainer, num2, true))
				{
					item.amount += item1.amount;
					item1.Remove(0f);
				}
				ItemManager.DoRemoves();
				this.ServerUpdate(0f);
				return;
			}
		}
		if (!item.MoveToContainer(itemContainer, num2, true))
		{
			return;
		}
		ItemManager.DoRemoves();
		this.ServerUpdate(0f);
	}

	private void OnClothingChanged(Item item, bool bAdded)
	{
		base.baseEntity.SV_ClothingChanged();
		ItemManager.DoRemoves();
		this.ServerUpdate(0f);
	}

	private void OnContentsDirty()
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.InvalidateNetworkCache();
		}
	}

	public void OnItemAddedOrRemoved(Item item, bool bAdded)
	{
		if (item.info.isHoldable)
		{
			base.Invoke(new Action(this.UpdatedVisibleHolsteredItems), 0.1f);
		}
		if (!bAdded)
		{
			return;
		}
		BasePlayer basePlayer = base.baseEntity;
		if (!basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash) && basePlayer.IsHostileItem(item))
		{
			base.baseEntity.SetPlayerFlag(BasePlayer.PlayerFlags.DisplaySash, true);
		}
	}

	private void OnItemRemoved(Item item)
	{
		base.baseEntity.InvalidateNetworkCache();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("PlayerInventory.OnRpcMessage", 0.1f))
		{
			if (rpc == -812517836 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ItemCmd "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("ItemCmd", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("ItemCmd", this.GetBaseEntity(), player))
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
							this.ItemCmd(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in ItemCmd");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -1253874771 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - MoveItem "));
				}
				using (timeWarning1 = TimeWarning.New("MoveItem", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("MoveItem", this.GetBaseEntity(), player))
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
							this.MoveItem(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in MoveItem");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public ProtoBuf.PlayerInventory Save(bool bForDisk)
	{
		ProtoBuf.PlayerInventory playerInventory = Facepunch.Pool.Get<ProtoBuf.PlayerInventory>();
		if (bForDisk)
		{
			playerInventory.invMain = this.containerMain.Save();
		}
		playerInventory.invBelt = this.containerBelt.Save();
		playerInventory.invWear = this.containerWear.Save();
		return playerInventory;
	}

	public void SendSnapshot()
	{
		using (TimeWarning timeWarning = TimeWarning.New("PlayerInventory.SendSnapshot", 0.1f))
		{
			this.SendUpdatedInventory(PlayerInventory.Type.Main, this.containerMain, false);
			this.SendUpdatedInventory(PlayerInventory.Type.Belt, this.containerBelt, true);
			this.SendUpdatedInventory(PlayerInventory.Type.Wear, this.containerWear, true);
		}
	}

	public void SendUpdatedInventory(PlayerInventory.Type type, ItemContainer container, bool bSendInventoryToEveryone = false)
	{
		using (UpdateItemContainer updateItemContainer = Facepunch.Pool.Get<UpdateItemContainer>())
		{
			updateItemContainer.type = (int)type;
			if (container != null)
			{
				container.dirty = false;
				updateItemContainer.container = Facepunch.Pool.Get<List<ProtoBuf.ItemContainer>>();
				updateItemContainer.container.Add(container.Save());
			}
			if (!bSendInventoryToEveryone)
			{
				base.baseEntity.ClientRPCPlayer<UpdateItemContainer>(null, base.baseEntity, "UpdatedItemContainer", updateItemContainer);
			}
			else
			{
				base.baseEntity.ClientRPC<UpdateItemContainer>(null, "UpdatedItemContainer", updateItemContainer);
			}
		}
	}

	public void ServerInit(BasePlayer owner)
	{
		this.Initialize();
		this.containerMain.ServerInitialize(null, 24);
		if (this.containerMain.uid == 0)
		{
			this.containerMain.GiveUID();
		}
		this.containerBelt.ServerInitialize(null, 6);
		if (this.containerBelt.uid == 0)
		{
			this.containerBelt.GiveUID();
		}
		this.containerWear.ServerInitialize(null, 7);
		if (this.containerWear.uid == 0)
		{
			this.containerWear.GiveUID();
		}
		this.containerMain.playerOwner = owner;
		this.containerBelt.playerOwner = owner;
		this.containerWear.playerOwner = owner;
		this.containerWear.onItemAddedRemoved = new Action<Item, bool>(this.OnClothingChanged);
		this.containerWear.canAcceptItem = new Func<Item, int, bool>(this.CanWearItem);
		this.containerBelt.canAcceptItem = new Func<Item, int, bool>(this.CanEquipItem);
		this.containerMain.onPreItemRemove = new Action<Item>(this.OnItemRemoved);
		this.containerWear.onPreItemRemove = new Action<Item>(this.OnItemRemoved);
		this.containerBelt.onPreItemRemove = new Action<Item>(this.OnItemRemoved);
		this.containerMain.onDirty += new Action(this.OnContentsDirty);
		this.containerBelt.onDirty += new Action(this.OnContentsDirty);
		this.containerWear.onDirty += new Action(this.OnContentsDirty);
		this.containerBelt.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
		this.containerMain.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
	}

	public void ServerUpdate(float delta)
	{
		this.loot.Check();
		if (delta > 0f)
		{
			this.crafting.ServerUpdate(delta);
		}
		float single = base.baseEntity.currentTemperature;
		this.UpdateContainer(delta, PlayerInventory.Type.Main, this.containerMain, false, single);
		this.UpdateContainer(delta, PlayerInventory.Type.Belt, this.containerBelt, true, single);
		this.UpdateContainer(delta, PlayerInventory.Type.Wear, this.containerWear, true, single);
	}

	public void Strip()
	{
		this.containerMain.Clear();
		this.containerBelt.Clear();
		this.containerWear.Clear();
		ItemManager.DoRemoves();
	}

	public int Take(List<Item> collect, int itemid, int amount)
	{
		int num = 0;
		if (this.containerMain != null)
		{
			num += this.containerMain.Take(collect, itemid, amount);
		}
		if (amount == num)
		{
			return num;
		}
		if (this.containerBelt != null)
		{
			num += this.containerBelt.Take(collect, itemid, amount);
		}
		if (amount == num)
		{
			return num;
		}
		if (this.containerWear != null)
		{
			num += this.containerWear.Take(collect, itemid, amount);
		}
		return num;
	}

	public void UpdateContainer(float delta, PlayerInventory.Type type, ItemContainer container, bool bSendInventoryToEveryone, float temperature)
	{
		if (container == null)
		{
			return;
		}
		container.temperature = temperature;
		if (delta > 0f)
		{
			container.OnCycle(delta);
		}
		if (container.dirty)
		{
			this.SendUpdatedInventory(type, container, bSendInventoryToEveryone);
			base.baseEntity.InvalidateNetworkCache();
		}
	}

	public void UpdatedVisibleHolsteredItems()
	{
		List<HeldEntity> list = Facepunch.Pool.GetList<HeldEntity>();
		List<Item> items = Facepunch.Pool.GetList<Item>();
		this.AllItemsNoAlloc(ref items);
		foreach (Item item in items)
		{
			if (!item.info.isHoldable || item.GetHeldEntity() == null)
			{
				continue;
			}
			HeldEntity component = item.GetHeldEntity().GetComponent<HeldEntity>();
			if (component == null)
			{
				continue;
			}
			list.Add(component);
		}
		Facepunch.Pool.FreeList<Item>(ref items);
		List<HeldEntity> heldEntities = list;
		bool flag = true;
		bool flag1 = true;
		bool flag2 = true;
		foreach (HeldEntity heldEntity in 
			from x in heldEntities
			orderby x.hostileScore descending
			select x)
		{
			if (heldEntity == null || !heldEntity.holsterInfo.displayWhenHolstered)
			{
				continue;
			}
			if (flag2 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.BACK)
			{
				heldEntity.SetVisibleWhileHolstered(true);
				flag2 = false;
			}
			else if (flag1 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.RIGHT_THIGH)
			{
				heldEntity.SetVisibleWhileHolstered(true);
				flag1 = false;
			}
			else if (!flag || heldEntity.IsDeployed() || heldEntity.holsterInfo.slot != HeldEntity.HolsterInfo.HolsterSlot.LEFT_THIGH)
			{
				heldEntity.SetVisibleWhileHolstered(false);
			}
			else
			{
				heldEntity.SetVisibleWhileHolstered(true);
				flag = false;
			}
		}
		Facepunch.Pool.FreeList<HeldEntity>(ref list);
	}

	public enum Type
	{
		Main,
		Belt,
		Wear
	}
}