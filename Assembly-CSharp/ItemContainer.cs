using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class ItemContainer
{
	public ItemContainer.Flag flags;

	public ItemContainer.ContentsType allowedContents;

	public ItemDefinition onlyAllowedItem;

	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	public int capacity = 2;

	public uint uid;

	public bool dirty;

	public List<Item> itemList = new List<Item>();

	public float temperature = 15f;

	public Item parent;

	public BasePlayer playerOwner;

	public BaseEntity entityOwner;

	public bool isServer;

	public int maxStackSize;

	public Func<Item, int, bool> canAcceptItem;

	public Action<Item, bool> onItemAddedRemoved;

	public Action<Item> onPreItemRemove;

	public Vector3 dropPosition
	{
		get
		{
			if (this.playerOwner)
			{
				return this.playerOwner.GetDropPosition();
			}
			if (this.entityOwner)
			{
				return this.entityOwner.GetDropPosition();
			}
			if (this.parent != null)
			{
				BaseEntity worldEntity = this.parent.GetWorldEntity();
				if (worldEntity != null)
				{
					return worldEntity.GetDropPosition();
				}
			}
			Debug.LogWarning("ItemContainer.dropPosition dropped through");
			return Vector3.zero;
		}
	}

	public Vector3 dropVelocity
	{
		get
		{
			if (this.playerOwner)
			{
				return this.playerOwner.GetDropVelocity();
			}
			if (this.entityOwner)
			{
				return this.entityOwner.GetDropVelocity();
			}
			if (this.parent != null)
			{
				BaseEntity worldEntity = this.parent.GetWorldEntity();
				if (worldEntity != null)
				{
					return worldEntity.GetDropVelocity();
				}
			}
			Debug.LogWarning("ItemContainer.dropVelocity dropped through");
			return Vector3.zero;
		}
	}

	public ItemContainer()
	{
	}

	public void AddItem(ItemDefinition itemToCreate, int p)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (p == 0)
			{
				return;
			}
			if (this.itemList[i].info == itemToCreate)
			{
				int num = this.itemList[i].MaxStackable();
				if (num > this.itemList[i].amount)
				{
					this.MarkDirty();
					this.itemList[i].amount += p;
					p -= p;
					if (this.itemList[i].amount > num)
					{
						p = this.itemList[i].amount - num;
						if (p > 0)
						{
							this.itemList[i].amount -= p;
						}
					}
				}
			}
		}
		if (p == 0)
		{
			return;
		}
		Item item = ItemManager.Create(itemToCreate, p, (ulong)0);
		if (!item.MoveToContainer(this, -1, true))
		{
			item.Remove(0f);
		}
	}

	public ItemContainer.CanAcceptResult CanAcceptItem(Item item, int targetPos)
	{
		if (this.canAcceptItem != null && !this.canAcceptItem(item, targetPos))
		{
			return ItemContainer.CanAcceptResult.CannotAccept;
		}
		if ((this.allowedContents & item.info.itemType) != item.info.itemType)
		{
			return ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.onlyAllowedItem != null && this.onlyAllowedItem != item.info)
		{
			return ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.availableSlots != null && this.availableSlots.Count > 0)
		{
			if ((int)item.info.occupySlots == 0 || item.info.occupySlots == ItemSlot.None)
			{
				return ItemContainer.CanAcceptResult.CannotAccept;
			}
			int[] numArray = new int[32];
			foreach (ItemSlot availableSlot in this.availableSlots)
			{
				numArray[(int)Mathf.Log((float)availableSlot, 2f)]++;
			}
			foreach (Item item1 in this.itemList)
			{
				for (int i = 0; i < 32; i++)
				{
					if (((int)item1.info.occupySlots & 1 << (i & 31)) != 0)
					{
						numArray[i]--;
					}
				}
			}
			for (int j = 0; j < 32; j++)
			{
				if (((int)item.info.occupySlots & 1 << (j & 31)) != 0 && numArray[j] <= 0)
				{
					return ItemContainer.CanAcceptResult.CannotAcceptRightNow;
				}
			}
		}
		object obj = Interface.CallHook("CanAcceptItem", this, item, targetPos);
		if (!(obj is ItemContainer.CanAcceptResult))
		{
			return ItemContainer.CanAcceptResult.CanAccept;
		}
		return (ItemContainer.CanAcceptResult)obj;
	}

	public bool CanTake(Item item)
	{
		if (this.IsFull())
		{
			return false;
		}
		return true;
	}

	public void Clear()
	{
		Item[] array = this.itemList.ToArray();
		for (int i = 0; i < (int)array.Length; i++)
		{
			array[i].Remove(0f);
		}
	}

	public uint ContentsHash()
	{
		uint num = 0;
		for (int i = 0; i < this.capacity; i++)
		{
			Item slot = this.GetSlot(i);
			if (slot != null)
			{
				num = CRC.Compute32(num, slot.info.itemid);
				num = CRC.Compute32(num, slot.skin);
			}
		}
		return num;
	}

	public DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot)
	{
		if ((this.itemList != null ? !(bool)this.itemList.Count : true))
		{
			return null;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, pos, rot, true);
		if (baseEntity == null)
		{
			return null;
		}
		DroppedItemContainer droppedItemContainer = baseEntity as DroppedItemContainer;
		if (droppedItemContainer != null)
		{
			droppedItemContainer.TakeFrom(new ItemContainer[] { this });
		}
		droppedItemContainer.Spawn();
		return droppedItemContainer;
	}

	public static DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot, params ItemContainer[] containers)
	{
		int num = 0;
		for (int i = 0; i < (int)containers.Length; i++)
		{
			ItemContainer itemContainer = containers[i];
			num = num + (itemContainer.itemList != null ? itemContainer.itemList.Count : 0);
		}
		if (num == 0)
		{
			return null;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, pos, rot, true);
		if (baseEntity == null)
		{
			return null;
		}
		DroppedItemContainer droppedItemContainer = baseEntity as DroppedItemContainer;
		if (droppedItemContainer != null)
		{
			droppedItemContainer.TakeFrom(containers);
		}
		droppedItemContainer.Spawn();
		return droppedItemContainer;
	}

	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].FindAmmo(list, ammoType);
		}
	}

	public ItemContainer FindContainer(uint id)
	{
		if (id == this.uid)
		{
			return this;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			Item item = this.itemList[i];
			if (item.contents != null)
			{
				ItemContainer itemContainer = item.contents.FindContainer(id);
				if (itemContainer != null)
				{
					return itemContainer;
				}
			}
		}
		return null;
	}

	public Item FindItemByItemID(int itemid)
	{
		return this.itemList.FirstOrDefault<Item>((Item x) => x.info.itemid == itemid);
	}

	public Item FindItemByUID(uint iUID)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			Item item = this.itemList[i];
			if (item.IsValid())
			{
				Item item1 = item.FindItem(iUID);
				if (item1 != null)
				{
					return item1;
				}
			}
		}
		return null;
	}

	public List<Item> FindItemsByItemID(int itemid)
	{
		return this.itemList.FindAll((Item x) => x.info.itemid == itemid);
	}

	public Item FindItemsByItemName(string name)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(name);
		if (itemDefinition == null)
		{
			return null;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].info == itemDefinition)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	public bool FindPosition(Item item)
	{
		int num = item.position;
		item.position = -1;
		if (num >= 0 && !this.SlotTaken(num))
		{
			item.position = num;
			return true;
		}
		for (int i = 0; i < this.capacity; i++)
		{
			if (!this.SlotTaken(i))
			{
				item.position = i;
				return true;
			}
		}
		return false;
	}

	public int GetAmount(int itemid, bool onlyUsableAmounts)
	{
		int num = 0;
		foreach (Item item in this.itemList)
		{
			if (item.info.itemid != itemid || onlyUsableAmounts && item.IsBusy())
			{
				continue;
			}
			num += item.amount;
		}
		return num;
	}

	public BasePlayer GetOwnerPlayer()
	{
		return this.playerOwner;
	}

	public Item GetSlot(int slot)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].position == slot)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	public void GiveUID()
	{
		Assert.IsTrue(this.uid == 0, "Calling GiveUID - but already has a uid!");
		this.uid = Net.sv.TakeUID();
	}

	public bool HasAmmo(AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].HasAmmo(ammoType))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasFlag(ItemContainer.Flag f)
	{
		return (this.flags & f) == f;
	}

	public bool Insert(Item item)
	{
		if (this.itemList.Contains(item))
		{
			return false;
		}
		if (this.IsFull())
		{
			return false;
		}
		this.itemList.Add(item);
		item.parent = this;
		if (!this.FindPosition(item))
		{
			return false;
		}
		this.MarkDirty();
		if (this.onItemAddedRemoved != null)
		{
			this.onItemAddedRemoved(item, true);
		}
		Interface.CallHook("OnItemAddedToContainer", this, item);
		return true;
	}

	public bool IsFull()
	{
		return this.itemList.Count >= this.capacity;
	}

	public bool IsLocked()
	{
		return this.HasFlag(ItemContainer.Flag.IsLocked);
	}

	public void Kill()
	{
		this.onDirty = null;
		this.canAcceptItem = null;
		this.onItemAddedRemoved = null;
		if (Net.sv != null)
		{
			Net.sv.ReturnUID(this.uid);
			this.uid = 0;
		}
		foreach (Item list in this.itemList.ToList<Item>())
		{
			list.Remove(0f);
		}
		this.itemList.Clear();
	}

	public void Load(ProtoBuf.ItemContainer container)
	{
		ItemContainer.ContentsType contentsType;
		ItemDefinition itemDefinition;
		using (TimeWarning timeWarning = TimeWarning.New("ItemContainer.Load", 0.1f))
		{
			this.uid = container.UID;
			this.capacity = container.slots;
			List<Item> items = this.itemList;
			this.itemList = Pool.GetList<Item>();
			this.temperature = container.temperature;
			this.flags = (ItemContainer.Flag)container.flags;
			if (container.allowedContents == 0)
			{
				contentsType = ItemContainer.ContentsType.Generic;
			}
			else
			{
				contentsType = (ItemContainer.ContentsType)container.allowedContents;
			}
			this.allowedContents = contentsType;
			if (container.allowedItem != 0)
			{
				itemDefinition = ItemManager.FindItemDefinition(container.allowedItem);
			}
			else
			{
				itemDefinition = null;
			}
			this.onlyAllowedItem = itemDefinition;
			this.maxStackSize = container.maxStackSize;
			this.availableSlots.Clear();
			for (int i = 0; i < container.availableSlots.Count; i++)
			{
				this.availableSlots.Add((ItemSlot)container.availableSlots[i]);
			}
			using (TimeWarning timeWarning1 = TimeWarning.New("container.contents", 0.1f))
			{
				foreach (ProtoBuf.Item content in container.contents)
				{
					Item item = null;
					foreach (Item item1 in items)
					{
						if (item1.uid != content.UID)
						{
							continue;
						}
						item = item1;
						goto Label0;
					}
				Label0:
					item = ItemManager.Load(content, item, this.isServer);
					if (item == null)
					{
						continue;
					}
					item.parent = this;
					item.position = content.slot;
					this.Insert(item);
				}
			}
			using (timeWarning1 = TimeWarning.New("Delete old items", 0.1f))
			{
				foreach (Item item2 in items)
				{
					if (this.itemList.Contains(item2))
					{
						continue;
					}
					item2.Remove(0f);
				}
			}
			this.dirty = true;
			Pool.FreeList<Item>(ref items);
		}
	}

	public void MarkDirty()
	{
		this.dirty = true;
		if (this.parent != null)
		{
			this.parent.MarkDirty();
		}
		if (this.onDirty != null)
		{
			this.onDirty();
		}
	}

	public void OnChanged()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnChanged();
		}
	}

	public void OnCycle(float delta)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].IsValid())
			{
				this.itemList[i].OnCycle(delta);
			}
		}
	}

	public void OnMovedToWorld()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnMovedToWorld();
		}
	}

	public void OnRemovedFromWorld()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnRemovedFromWorld();
		}
	}

	public bool PlayerItemInputBlocked()
	{
		return this.HasFlag(ItemContainer.Flag.NoItemInput);
	}

	public bool Remove(Item item)
	{
		if (!this.itemList.Contains(item))
		{
			return false;
		}
		if (this.onPreItemRemove != null)
		{
			this.onPreItemRemove(item);
		}
		this.itemList.Remove(item);
		item.parent = null;
		this.MarkDirty();
		if (this.onItemAddedRemoved != null)
		{
			this.onItemAddedRemoved(item, false);
		}
		Interface.CallHook("OnItemRemovedFromContainer", this, item);
		return true;
	}

	public ProtoBuf.ItemContainer Save()
	{
		ProtoBuf.ItemContainer list = Pool.Get<ProtoBuf.ItemContainer>();
		list.contents = Pool.GetList<ProtoBuf.Item>();
		list.UID = this.uid;
		list.slots = this.capacity;
		list.temperature = this.temperature;
		list.allowedContents = (int)this.allowedContents;
		list.allowedItem = (this.onlyAllowedItem != null ? this.onlyAllowedItem.itemid : 0);
		list.flags = (int)this.flags;
		list.maxStackSize = this.maxStackSize;
		if (this.availableSlots != null && this.availableSlots.Count > 0)
		{
			list.availableSlots = Pool.GetList<int>();
			for (int i = 0; i < this.availableSlots.Count; i++)
			{
				list.availableSlots.Add(this.availableSlots[i]);
			}
		}
		for (int j = 0; j < this.itemList.Count; j++)
		{
			Item item = this.itemList[j];
			if (item.IsValid())
			{
				list.contents.Add(item.Save(true, true));
			}
		}
		return list;
	}

	public void ServerInitialize(Item parentItem, int iMaxCapacity)
	{
		this.parent = parentItem;
		this.capacity = iMaxCapacity;
		this.uid = 0;
		this.isServer = true;
		if ((int)this.allowedContents == 0)
		{
			this.allowedContents = ItemContainer.ContentsType.Generic;
		}
		this.MarkDirty();
	}

	public void SetFlag(ItemContainer.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	public void SetLocked(bool isLocked)
	{
		this.SetFlag(ItemContainer.Flag.IsLocked, isLocked);
		this.MarkDirty();
	}

	public bool SlotTaken(int i)
	{
		return this.GetSlot(i) != null;
	}

	public int Take(List<Item> collect, int itemid, int iAmount)
	{
		int num = 0;
		if (iAmount == 0)
		{
			return num;
		}
		List<Item> list = Pool.GetList<Item>();
		List<Item>.Enumerator enumerator = this.itemList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Item current = enumerator.Current;
				if (current.info.itemid != itemid)
				{
					continue;
				}
				int num1 = iAmount - num;
				if (num1 <= 0)
				{
					continue;
				}
				if (current.amount <= num1)
				{
					if (current.amount <= num1)
					{
						num += current.amount;
						list.Add(current);
						if (collect != null)
						{
							collect.Add(current);
						}
					}
					if (num != iAmount)
					{
						continue;
					}
					enumerator = list.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							current.RemoveFromContainer();
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					Pool.FreeList<Item>(ref list);
					return num;
				}
				else
				{
					current.MarkDirty();
					current.amount -= num1;
					num += num1;
					Item item = ItemManager.CreateByItemID(itemid, 1, (ulong)0);
					item.amount = num1;
					item.CollectedForCrafting(this.playerOwner);
					if (collect == null)
					{
						break;
					}
					collect.Add(item);
					enumerator = list.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							current.RemoveFromContainer();
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					Pool.FreeList<Item>(ref list);
					return num;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		foreach (Item item1 in list)
		{
			item1.RemoveFromContainer();
		}
		Pool.FreeList<Item>(ref list);
		return num;
	}

	public event Action onDirty;

	public enum CanAcceptResult
	{
		CanAccept,
		CannotAccept,
		CannotAcceptRightNow
	}

	[Flags]
	public enum ContentsType
	{
		Generic = 1,
		Liquid = 2
	}

	[Flags]
	public enum Flag
	{
		IsPlayer = 1,
		Clothing = 2,
		Belt = 4,
		SingleType = 8,
		IsLocked = 16,
		ShowSlotsOnIcon = 32,
		NoBrokenItems = 64,
		NoItemInput = 128
	}
}