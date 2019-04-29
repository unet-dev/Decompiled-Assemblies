using ConVar;
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

public class Item
{
	public float _condition;

	public float _maxCondition = 100f;

	public ItemDefinition info;

	public uint uid;

	public bool dirty;

	public int amount = 1;

	public int position;

	public float busyTime;

	public float removeTime;

	public float fuel;

	public bool isServer;

	public ProtoBuf.Item.InstanceData instanceData;

	public ulong skin;

	public string name;

	public string text;

	public Item.Flag flags;

	public ItemContainer contents;

	public ItemContainer parent;

	private EntityRef worldEnt;

	private EntityRef heldEntity;

	public int blueprintAmount
	{
		get
		{
			return this.amount;
		}
		set
		{
			this.amount = value;
		}
	}

	public int blueprintTarget
	{
		get
		{
			if (this.instanceData == null)
			{
				return 0;
			}
			return this.instanceData.blueprintTarget;
		}
		set
		{
			if (this.instanceData == null)
			{
				this.instanceData = new ProtoBuf.Item.InstanceData();
			}
			this.instanceData.ShouldPool = false;
			this.instanceData.blueprintTarget = value;
		}
	}

	public ItemDefinition blueprintTargetDef
	{
		get
		{
			if (!this.IsBlueprint())
			{
				return null;
			}
			return ItemManager.FindItemDefinition(this.blueprintTarget);
		}
	}

	public float condition
	{
		get
		{
			return this._condition;
		}
		set
		{
			float single = this._condition;
			this._condition = Mathf.Clamp(value, 0f, this.maxCondition);
			if (this.isServer && Mathf.Ceil(value) != Mathf.Ceil(single))
			{
				this.MarkDirty();
			}
		}
	}

	public float conditionNormalized
	{
		get
		{
			if (!this.hasCondition)
			{
				return 1f;
			}
			return this.condition / this.maxCondition;
		}
		set
		{
			if (!this.hasCondition)
			{
				return;
			}
			this.condition = value * this.maxCondition;
		}
	}

	public int despawnMultiplier
	{
		get
		{
			if (this.info == null)
			{
				return 1;
			}
			return Mathf.Clamp(((int)this.info.rarity - (int)Rarity.Common) * (int)Rarity.VeryRare, 1, 100);
		}
	}

	public bool hasCondition
	{
		get
		{
			if (!(this.info != null) || !this.info.condition.enabled)
			{
				return false;
			}
			return this.info.condition.max > 0f;
		}
	}

	public bool isBroken
	{
		get
		{
			if (!this.hasCondition)
			{
				return false;
			}
			return this.condition <= 0f;
		}
	}

	public float maxCondition
	{
		get
		{
			return this._maxCondition;
		}
		set
		{
			this._maxCondition = Mathf.Clamp(value, 0f, this.info.condition.max);
			if (this.isServer)
			{
				this.MarkDirty();
			}
		}
	}

	public float maxConditionNormalized
	{
		get
		{
			return this._maxCondition / this.info.condition.max;
		}
	}

	public Item parentItem
	{
		get
		{
			if (this.parent == null)
			{
				return null;
			}
			return this.parent.parent;
		}
	}

	public float temperature
	{
		get
		{
			if (this.parent == null)
			{
				return 15f;
			}
			return this.parent.temperature;
		}
	}

	public BaseEntity.TraitFlag Traits
	{
		get
		{
			return this.info.Traits;
		}
	}

	public Item()
	{
	}

	public void BusyFor(float fTime)
	{
		this.busyTime = UnityEngine.Time.time + fTime;
	}

	public bool CanBeHeld()
	{
		if (this.isBroken)
		{
			return false;
		}
		return true;
	}

	public bool CanMoveTo(ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true)
	{
		if (this.IsChildContainer(newcontainer))
		{
			return false;
		}
		if (newcontainer.CanAcceptItem(this, iTargetPos) != ItemContainer.CanAcceptResult.CanAccept)
		{
			return false;
		}
		if (iTargetPos >= newcontainer.capacity)
		{
			return false;
		}
		if (this.parent != null && newcontainer == this.parent && iTargetPos == this.position)
		{
			return false;
		}
		return true;
	}

	public bool CanStack(Item item)
	{
		object obj = Interface.CallHook("CanStackItem", this, item);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (item == this)
		{
			return false;
		}
		if (this.info.stackable <= 1)
		{
			return false;
		}
		if (item.info.stackable <= 1)
		{
			return false;
		}
		if (item.info.itemid != this.info.itemid)
		{
			return false;
		}
		if (this.hasCondition && this.condition != this.maxCondition)
		{
			return false;
		}
		if (item.hasCondition && item.condition != item.maxCondition)
		{
			return false;
		}
		if (!this.IsValid())
		{
			return false;
		}
		if (this.IsBlueprint() && this.blueprintTarget != item.blueprintTarget)
		{
			return false;
		}
		if (item.skin != this.skin)
		{
			return false;
		}
		return true;
	}

	public void CollectedForCrafting(BasePlayer crafter)
	{
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].CollectedForCrafting(this, crafter);
		}
	}

	public BaseEntity CreateWorldObject(Vector3 pos, Quaternion rotation = null, BaseEntity parentEnt = null, uint parentBone = 0)
	{
		BaseEntity worldEntity = this.GetWorldEntity();
		if (worldEntity != null)
		{
			return worldEntity;
		}
		worldEntity = GameManager.server.CreateEntity("assets/prefabs/misc/burlap sack/generic_world.prefab", pos, rotation, true);
		if (worldEntity == null)
		{
			Debug.LogWarning("Couldn't create world object for prefab: items/generic_world");
			return null;
		}
		WorldItem worldItem = worldEntity as WorldItem;
		if (worldItem != null)
		{
			worldItem.InitializeItem(this);
		}
		if (parentEnt != null)
		{
			worldEntity.SetParent(parentEnt, parentBone, false, false);
		}
		worldEntity.Spawn();
		this.SetWorldEntity(worldEntity);
		return this.GetWorldEntity();
	}

	public void DoRemove()
	{
		this.OnDirty = null;
		this.onCycle = null;
		if (this.isServer && this.uid > 0 && Network.Net.sv != null)
		{
			Network.Net.sv.ReturnUID(this.uid);
			this.uid = 0;
		}
		if (this.contents != null)
		{
			this.contents.Kill();
			this.contents = null;
		}
		if (this.isServer)
		{
			this.RemoveFromWorld();
			this.RemoveFromContainer();
		}
		BaseEntity heldEntity = this.GetHeldEntity();
		if (heldEntity.IsValid())
		{
			Debug.LogWarning(string.Concat(new object[] { "Item's Held Entity not removed!", this.info.displayName.english, " -> ", heldEntity }), heldEntity);
		}
	}

	public void DoRepair(float maxLossFraction)
	{
		if (!this.hasCondition)
		{
			return;
		}
		if (this.info.condition.maintainMaxCondition)
		{
			maxLossFraction = 0f;
		}
		float single = 1f - this.condition / this.maxCondition;
		maxLossFraction = Mathf.Clamp(maxLossFraction, 0f, this.info.condition.max);
		this.maxCondition = this.maxCondition * (1f - maxLossFraction * single);
		this.condition = this.maxCondition;
		BaseEntity heldEntity = this.GetHeldEntity();
		if (heldEntity != null)
		{
			heldEntity.SetFlag(BaseEntity.Flags.Broken, false, false, true);
		}
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[] { this.info.shortname, " was repaired! new cond is: ", this.condition, "/", this.maxCondition }));
		}
	}

	public BaseEntity Drop(Vector3 vPos, Vector3 vVelocity, Quaternion rotation = null)
	{
		this.RemoveFromWorld();
		BaseEntity baseEntity = null;
		if (!(vPos != Vector3.zero) || this.info.HasFlag(ItemDefinition.Flag.NoDropping))
		{
			this.Remove(0f);
		}
		else
		{
			baseEntity = this.CreateWorldObject(vPos, rotation, null, 0);
			if (baseEntity)
			{
				baseEntity.SetVelocity(vVelocity);
			}
		}
		Interface.CallHook("OnItemDropped", this, baseEntity);
		this.RemoveFromContainer();
		return baseEntity;
	}

	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		ItemModProjectile component = this.info.GetComponent<ItemModProjectile>();
		if (component && component.IsAmmo(ammoType))
		{
			list.Add(this);
			return;
		}
		if (this.contents != null)
		{
			this.contents.FindAmmo(list, ammoType);
		}
	}

	public Item FindItem(uint iUID)
	{
		if (this.uid == iUID)
		{
			return this;
		}
		if (this.contents == null)
		{
			return null;
		}
		return this.contents.FindItemByUID(iUID);
	}

	public BaseEntity GetHeldEntity()
	{
		return this.heldEntity.Get(this.isServer);
	}

	public BasePlayer GetOwnerPlayer()
	{
		if (this.parent == null)
		{
			return null;
		}
		return this.parent.GetOwnerPlayer();
	}

	public ItemContainer GetRootContainer()
	{
		int i;
		ItemContainer itemContainer = this.parent;
		for (i = 0; itemContainer != null && i <= 8 && itemContainer.parent != null && itemContainer.parent.parent != null; i++)
		{
			itemContainer = itemContainer.parent.parent;
		}
		if (i == 8)
		{
			Debug.LogWarning("GetRootContainer failed with 8 iterations");
		}
		return itemContainer;
	}

	public BaseEntity GetWorldEntity()
	{
		return this.worldEnt.Get(this.isServer);
	}

	public bool HasAmmo(AmmoTypes ammoType)
	{
		ItemModProjectile component = this.info.GetComponent<ItemModProjectile>();
		if (component && component.IsAmmo(ammoType))
		{
			return true;
		}
		if (this.contents == null)
		{
			return false;
		}
		return this.contents.HasAmmo(ammoType);
	}

	public bool HasFlag(Item.Flag f)
	{
		return (this.flags & f) == f;
	}

	public void Initialize(ItemDefinition template)
	{
		this.uid = Network.Net.sv.TakeUID();
		float single = this.info.condition.max;
		float single1 = single;
		this.maxCondition = single;
		this.condition = single1;
		this.OnItemCreated();
	}

	public bool IsBlueprint()
	{
		return this.blueprintTarget != 0;
	}

	public bool IsBusy()
	{
		if (this.busyTime > UnityEngine.Time.time)
		{
			return true;
		}
		return false;
	}

	public bool IsChildContainer(ItemContainer c)
	{
		bool flag;
		if (this.contents == null)
		{
			return false;
		}
		if (this.contents == c)
		{
			return true;
		}
		List<Item>.Enumerator enumerator = this.contents.itemList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.IsChildContainer(c))
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

	public bool IsCooking()
	{
		return this.HasFlag(Item.Flag.Cooking);
	}

	public bool IsLocked()
	{
		if (this.HasFlag(Item.Flag.IsLocked))
		{
			return true;
		}
		if (this.parent == null)
		{
			return false;
		}
		return this.parent.IsLocked();
	}

	public bool IsOn()
	{
		return this.HasFlag(Item.Flag.IsOn);
	}

	public bool IsOnFire()
	{
		return this.HasFlag(Item.Flag.OnFire);
	}

	public bool IsValid()
	{
		if (this.removeTime > 0f)
		{
			return false;
		}
		return true;
	}

	public virtual void Load(ProtoBuf.Item load)
	{
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		this.uid = load.UID;
		this.name = load.name;
		this.text = load.text;
		this.amount = load.amount;
		this.position = load.slot;
		this.busyTime = load.locktime;
		this.removeTime = load.removetime;
		this.flags = (Item.Flag)load.flags;
		this.worldEnt.uid = load.worldEntity;
		this.heldEntity.uid = load.heldEntity;
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = true;
			this.instanceData.ResetToPool();
			this.instanceData = null;
		}
		this.instanceData = load.instanceData;
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = false;
		}
		this.skin = load.skinid;
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		if (this.info == null)
		{
			return;
		}
		this._condition = 0f;
		this._maxCondition = 0f;
		if (load.conditionData != null)
		{
			this._condition = load.conditionData.condition;
			this._maxCondition = load.conditionData.maxCondition;
		}
		else if (this.info.condition.enabled)
		{
			this._condition = this.info.condition.max;
			this._maxCondition = this.info.condition.max;
		}
		if (load.contents != null)
		{
			if (this.contents == null)
			{
				this.contents = new ItemContainer();
				if (this.isServer)
				{
					this.contents.ServerInitialize(this, load.contents.slots);
				}
			}
			this.contents.Load(load.contents);
		}
		if (this.isServer)
		{
			this.removeTime = 0f;
			this.OnItemCreated();
		}
	}

	public void LockUnlock(bool bNewState, BasePlayer player)
	{
		if (this.HasFlag(Item.Flag.IsLocked) == bNewState)
		{
			return;
		}
		this.SetFlag(Item.Flag.IsLocked, bNewState);
		this.MarkDirty();
	}

	public void LoseCondition(float amount)
	{
		if (!this.hasCondition)
		{
			return;
		}
		if (Debugging.disablecondition)
		{
			return;
		}
		if (Interface.CallHook("IOnLoseCondition", this, amount) != null)
		{
			return;
		}
		float single = this.condition;
		this.condition = this.condition - amount;
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[] { this.info.shortname, " was damaged by: ", amount, "cond is: ", this.condition, "/", this.maxCondition }));
		}
		if (this.condition <= 0f && this.condition < single)
		{
			this.OnBroken();
		}
	}

	public void MarkDirty()
	{
		this.OnChanged();
		this.dirty = true;
		if (this.parent != null)
		{
			this.parent.MarkDirty();
		}
		if (this.OnDirty != null)
		{
			this.OnDirty(this);
		}
	}

	public int MaxStackable()
	{
		int num = this.info.stackable;
		if (this.parent != null && this.parent.maxStackSize > 0)
		{
			num = Mathf.Min(this.parent.maxStackSize, num);
		}
		object obj = Interface.CallHook("OnMaxStackable", this);
		if (!(obj is int))
		{
			return num;
		}
		return (int)obj;
	}

	public bool MoveToContainer(ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true)
	{
		bool container;
		Quaternion quaternion;
		using (TimeWarning timeWarning = TimeWarning.New("MoveToContainer", 0.1f))
		{
			ItemContainer itemContainer = this.parent;
			if (!this.CanMoveTo(newcontainer, iTargetPos, allowStack))
			{
				container = false;
			}
			else if (iTargetPos >= 0 && newcontainer.SlotTaken(iTargetPos))
			{
				Item slot = newcontainer.GetSlot(iTargetPos);
				if (allowStack)
				{
					int num = slot.MaxStackable();
					if (slot.CanStack(this))
					{
						if (slot.amount < num)
						{
							slot.amount += this.amount;
							slot.MarkDirty();
							this.RemoveFromWorld();
							this.RemoveFromContainer();
							this.Remove(0f);
							int num1 = slot.amount - num;
							if (num1 > 0)
							{
								Item item = slot.SplitItem(num1);
								if (item != null && !item.MoveToContainer(newcontainer, -1, false) && (itemContainer == null || !item.MoveToContainer(itemContainer, -1, true)))
								{
									Vector3 vector3 = newcontainer.dropPosition;
									Vector3 vector31 = newcontainer.dropVelocity;
									quaternion = new Quaternion();
									item.Drop(vector3, vector31, quaternion);
								}
								slot.amount = num;
							}
							container = true;
							return container;
						}
						else
						{
							container = false;
							return container;
						}
					}
				}
				if (this.parent == null)
				{
					container = false;
				}
				else
				{
					ItemContainer itemContainer1 = this.parent;
					int num2 = this.position;
					if (slot.CanMoveTo(itemContainer1, num2, true))
					{
						this.RemoveFromContainer();
						slot.RemoveFromContainer();
						slot.MoveToContainer(itemContainer1, num2, true);
						container = this.MoveToContainer(newcontainer, iTargetPos, true);
					}
					else
					{
						container = false;
					}
				}
			}
			else if (this.parent != newcontainer)
			{
				if (iTargetPos == -1 & allowStack && this.info.stackable > 1)
				{
					Item item1 = (
						from x in newcontainer.FindItemsByItemID(this.info.itemid)
						orderby x.amount
						select x).FirstOrDefault<Item>();
					if (item1 != null && item1.CanStack(this))
					{
						int num3 = item1.MaxStackable();
						if (item1.amount < num3)
						{
							item1.amount += this.amount;
							item1.MarkDirty();
							int num4 = item1.amount - num3;
							if (num4 > 0)
							{
								this.amount = num4;
								this.MarkDirty();
								item1.amount = num3;
								container = this.MoveToContainer(newcontainer, iTargetPos, allowStack);
								return container;
							}
							else
							{
								this.RemoveFromWorld();
								this.RemoveFromContainer();
								this.Remove(0f);
								container = true;
								return container;
							}
						}
					}
				}
				if (newcontainer.maxStackSize > 0 && newcontainer.maxStackSize < this.amount)
				{
					Item item2 = this.SplitItem(newcontainer.maxStackSize);
					if (item2 != null && !item2.MoveToContainer(newcontainer, iTargetPos, false) && (itemContainer == null || !item2.MoveToContainer(itemContainer, -1, true)))
					{
						Vector3 vector32 = newcontainer.dropPosition;
						Vector3 vector33 = newcontainer.dropVelocity;
						quaternion = new Quaternion();
						item2.Drop(vector32, vector33, quaternion);
					}
					container = true;
				}
				else if (newcontainer.CanTake(this))
				{
					this.RemoveFromContainer();
					this.RemoveFromWorld();
					this.position = iTargetPos;
					this.SetParent(newcontainer);
					container = true;
				}
				else
				{
					container = false;
				}
			}
			else if (iTargetPos < 0 || iTargetPos == this.position || this.parent.SlotTaken(iTargetPos))
			{
				container = false;
			}
			else
			{
				this.position = iTargetPos;
				this.MarkDirty();
				container = true;
			}
		}
		return container;
	}

	public void OnAttacked(HitInfo hitInfo)
	{
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].OnAttacked(this, hitInfo);
		}
	}

	public virtual void OnBroken()
	{
		if (!this.hasCondition)
		{
			return;
		}
		BaseEntity heldEntity = this.GetHeldEntity();
		if (heldEntity != null)
		{
			heldEntity.SetFlag(BaseEntity.Flags.Broken, true, false, true);
		}
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer && ownerPlayer.GetActiveItem() == this)
		{
			Effect.server.Run("assets/bundled/prefabs/fx/item_break.prefab", ownerPlayer, 0, Vector3.zero, Vector3.zero, null, false);
			ownerPlayer.ChatMessage("Your active item was broken!");
		}
		if (!this.info.condition.repairable || this.maxCondition <= 5f)
		{
			this.Remove(0f);
		}
		else if (this.parent != null && this.parent.HasFlag(ItemContainer.Flag.NoBrokenItems))
		{
			ItemContainer rootContainer = this.GetRootContainer();
			if (!rootContainer.HasFlag(ItemContainer.Flag.NoBrokenItems))
			{
				BasePlayer basePlayer = rootContainer.playerOwner;
				if (basePlayer != null && !this.MoveToContainer(basePlayer.inventory.containerMain, -1, true))
				{
					this.Drop(basePlayer.transform.position, basePlayer.eyes.BodyForward() * 1.5f, new Quaternion());
				}
			}
			else
			{
				this.Remove(0f);
			}
		}
		this.MarkDirty();
	}

	public void OnChanged()
	{
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].OnChanged(this);
		}
		if (this.contents != null)
		{
			this.contents.OnChanged();
		}
	}

	public void OnCycle(float delta)
	{
		if (this.onCycle != null)
		{
			this.onCycle(this, delta);
		}
	}

	public void OnItemCreated()
	{
		this.onCycle = null;
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].OnItemCreated(this);
		}
	}

	public void OnMovedToWorld()
	{
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].OnMovedToWorld(this);
		}
	}

	public void OnRemovedFromWorld()
	{
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].OnRemovedFromWorld(this);
		}
	}

	public void OnVirginSpawn()
	{
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].OnVirginItem(this);
		}
	}

	public void Remove(float fTime = 0f)
	{
		if (this.removeTime > 0f)
		{
			return;
		}
		if (Interface.CallHook("OnItemRemove", this) != null)
		{
			return;
		}
		if (this.isServer)
		{
			ItemMod[] itemModArray = this.info.itemMods;
			for (int i = 0; i < (int)itemModArray.Length; i++)
			{
				itemModArray[i].OnRemove(this);
			}
		}
		this.onCycle = null;
		this.removeTime = UnityEngine.Time.time + fTime;
		this.OnDirty = null;
		this.position = -1;
		if (this.isServer)
		{
			ItemManager.RemoveItem(this, fTime);
		}
	}

	public void RemoveFromContainer()
	{
		if (this.parent == null)
		{
			return;
		}
		this.SetParent(null);
	}

	public void RemoveFromWorld()
	{
		BaseEntity worldEntity = this.GetWorldEntity();
		if (worldEntity == null)
		{
			return;
		}
		this.SetWorldEntity(null);
		this.OnRemovedFromWorld();
		if (this.contents != null)
		{
			this.contents.OnRemovedFromWorld();
		}
		if (!worldEntity.IsValid())
		{
			return;
		}
		worldEntity.Kill(BaseNetworkable.DestroyMode.None);
	}

	public void RepairCondition(float amount)
	{
		if (!this.hasCondition)
		{
			return;
		}
		this.condition = this.condition + amount;
	}

	public void ReturnedFromCancelledCraft(BasePlayer crafter)
	{
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].ReturnedFromCancelledCraft(this, crafter);
		}
	}

	public virtual ProtoBuf.Item Save(bool bIncludeContainer = false, bool bIncludeOwners = true)
	{
		this.dirty = false;
		ProtoBuf.Item item = Facepunch.Pool.Get<ProtoBuf.Item>();
		item.UID = this.uid;
		item.itemid = this.info.itemid;
		item.slot = this.position;
		item.amount = this.amount;
		item.flags = (int)this.flags;
		item.removetime = this.removeTime;
		item.locktime = this.busyTime;
		item.instanceData = this.instanceData;
		item.worldEntity = this.worldEnt.uid;
		item.heldEntity = this.heldEntity.uid;
		item.skinid = this.skin;
		item.name = this.name;
		item.text = this.text;
		if (this.hasCondition)
		{
			item.conditionData = Facepunch.Pool.Get<ProtoBuf.Item.ConditionData>();
			item.conditionData.maxCondition = this._maxCondition;
			item.conditionData.condition = this._condition;
		}
		if (this.contents != null & bIncludeContainer)
		{
			item.contents = this.contents.Save();
		}
		return item;
	}

	public void ServerCommand(string command, BasePlayer player)
	{
		HeldEntity heldEntity = this.GetHeldEntity() as HeldEntity;
		if (heldEntity != null)
		{
			heldEntity.ServerCommand(this, command, player);
		}
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].ServerCommand(this, command, player);
		}
	}

	public void SetFlag(Item.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	public void SetHeldEntity(BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.heldEntity.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.heldEntity.uid == ent.net.ID)
		{
			return;
		}
		this.heldEntity.Set(ent);
		this.MarkDirty();
		if (ent.IsValid())
		{
			HeldEntity heldEntity = ent as HeldEntity;
			if (heldEntity != null)
			{
				heldEntity.SetupHeldEntity(this);
			}
		}
	}

	public void SetParent(ItemContainer target)
	{
		if (target == this.parent)
		{
			return;
		}
		if (this.parent != null)
		{
			this.parent.Remove(this);
			this.parent = null;
		}
		if (target != null)
		{
			this.parent = target;
			if (!this.parent.Insert(this))
			{
				this.Remove(0f);
				Debug.LogError("Item.SetParent caused remove - this shouldn't ever happen");
			}
		}
		else
		{
			this.position = 0;
		}
		this.MarkDirty();
		ItemMod[] itemModArray = this.info.itemMods;
		for (int i = 0; i < (int)itemModArray.Length; i++)
		{
			itemModArray[i].OnParentChanged(this);
		}
	}

	public void SetWorldEntity(BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.worldEnt.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.worldEnt.uid == ent.net.ID)
		{
			return;
		}
		this.worldEnt.Set(ent);
		this.MarkDirty();
		this.OnMovedToWorld();
		if (this.contents != null)
		{
			this.contents.OnMovedToWorld();
		}
	}

	public Item SplitItem(int split_Amount)
	{
		Assert.IsTrue(split_Amount > 0, "split_Amount <= 0");
		if (split_Amount <= 0)
		{
			return null;
		}
		if (split_Amount >= this.amount)
		{
			return null;
		}
		object obj = Interface.CallHook("OnItemSplit", this, split_Amount);
		if (obj is Item)
		{
			return (Item)obj;
		}
		this.amount -= split_Amount;
		Item splitAmount = ItemManager.CreateByItemID(this.info.itemid, 1, (ulong)0);
		splitAmount.amount = split_Amount;
		if (this.IsBlueprint())
		{
			splitAmount.blueprintTarget = this.blueprintTarget;
		}
		this.MarkDirty();
		return splitAmount;
	}

	public void SwitchOnOff(bool bNewState, BasePlayer player)
	{
		if (this.HasFlag(Item.Flag.IsOn) == bNewState)
		{
			return;
		}
		this.SetFlag(Item.Flag.IsOn, bNewState);
		this.MarkDirty();
	}

	public override string ToString()
	{
		return string.Concat(new object[] { "Item.", this.info.shortname, "x", this.amount, ".", this.uid });
	}

	public void UseItem(int amountToConsume = 1)
	{
		if (amountToConsume <= 0)
		{
			return;
		}
		Interface.CallHook("OnItemUse", this, amountToConsume);
		this.amount -= amountToConsume;
		if (this.amount > 0)
		{
			this.MarkDirty();
			return;
		}
		this.amount = 0;
		this.Remove(0f);
	}

	public event Action<Item, float> onCycle;

	public event Action<Item> OnDirty;

	[Flags]
	public enum Flag
	{
		None = 0,
		Placeholder = 1,
		IsOn = 2,
		OnFire = 4,
		IsLocked = 8,
		Cooking = 16
	}
}