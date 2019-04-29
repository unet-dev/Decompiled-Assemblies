using ConVar;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseOven : StorageContainer, ISplashable
{
	public BaseOven.TemperatureType temperature;

	public BaseEntity.Menu.Option switchOnMenu;

	public BaseEntity.Menu.Option switchOffMenu;

	public ItemAmount[] startupContents;

	public bool allowByproductCreation = true;

	public ItemDefinition fuelType;

	public bool canModFire;

	public bool disabledBySplash = true;

	private const float UpdateRate = 0.5f;

	public float cookingTemperature
	{
		get
		{
			switch (this.temperature)
			{
				case BaseOven.TemperatureType.Warming:
				{
					return 50f;
				}
				case BaseOven.TemperatureType.Cooking:
				{
					return 200f;
				}
				case BaseOven.TemperatureType.Smelting:
				{
					return 1000f;
				}
				case BaseOven.TemperatureType.Fractioning:
				{
					return 1500f;
				}
			}
			return 15f;
		}
	}

	public BaseOven()
	{
	}

	private void ConsumeFuel(Item fuel, ItemModBurnable burnable)
	{
		Interface.CallHook("OnConsumeFuel", this, fuel, burnable);
		if (this.allowByproductCreation && burnable.byproductItem != null && UnityEngine.Random.Range(0f, 1f) > burnable.byproductChance)
		{
			Item item = ItemManager.Create(burnable.byproductItem, burnable.byproductAmount, (ulong)0);
			if (!item.MoveToContainer(this.inventory, -1, true))
			{
				this.OvenFull();
				item.Drop(this.inventory.dropPosition, this.inventory.dropVelocity, new Quaternion());
			}
		}
		if (fuel.amount <= 1)
		{
			fuel.Remove(0f);
			return;
		}
		fuel.amount--;
		fuel.fuel = burnable.fuelAmount;
		fuel.MarkDirty();
	}

	public void Cook()
	{
		Item item = this.FindBurnable();
		if (item == null)
		{
			this.StopCooking();
			return;
		}
		this.inventory.OnCycle(0.5f);
		BaseEntity slot = base.GetSlot(BaseEntity.Slot.FireMod);
		if (slot)
		{
			slot.SendMessage("Cook", 0.5f, SendMessageOptions.DontRequireReceiver);
		}
		ItemModBurnable component = item.info.GetComponent<ItemModBurnable>();
		Item item1 = item;
		item1.fuel = item1.fuel - 0.5f * (this.cookingTemperature / 200f);
		if (!item.HasFlag(Item.Flag.OnFire))
		{
			item.SetFlag(Item.Flag.OnFire, true);
			item.MarkDirty();
		}
		if (item.fuel <= 0f)
		{
			this.ConsumeFuel(item, component);
		}
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.StopCooking();
		return Mathf.Min(200, amount);
	}

	private Item FindBurnable()
	{
		Item item;
		object obj = Interface.CallHook("OnFindBurnable", this);
		if (obj is Item)
		{
			return (Item)obj;
		}
		if (this.inventory == null)
		{
			return null;
		}
		List<Item>.Enumerator enumerator = this.inventory.itemList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Item current = enumerator.Current;
				if (!current.info.GetComponent<ItemModBurnable>() || !(this.fuelType == null) && !(current.info == this.fuelType))
				{
					continue;
				}
				item = current;
				return item;
			}
			return null;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return item;
	}

	public override bool HasSlot(BaseEntity.Slot slot)
	{
		if (this.canModFire && slot == BaseEntity.Slot.FireMod)
		{
			return true;
		}
		return base.HasSlot(slot);
	}

	public override void OnInventoryFirstCreated(ItemContainer container)
	{
		base.OnInventoryFirstCreated(container);
		if (this.startupContents == null)
		{
			return;
		}
		ItemAmount[] itemAmountArray = this.startupContents;
		for (int i = 0; i < (int)itemAmountArray.Length; i++)
		{
			ItemAmount itemAmount = itemAmountArray[i];
			ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, (ulong)0).MoveToContainer(container, -1, true);
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		if (item != null && item.HasFlag(Item.Flag.OnFire))
		{
			item.SetFlag(Item.Flag.OnFire, false);
			item.MarkDirty();
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("BaseOven.OnRpcMessage", 0.1f))
		{
			if (rpc != -127127424 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SVSwitch "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SVSwitch", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("SVSwitch", this, player, 3f))
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
							this.SVSwitch(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SVSwitch");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void OvenFull()
	{
		this.StopCooking();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.IsOn())
		{
			this.StartCooking();
		}
	}

	public virtual void StartCooking()
	{
		if (this.FindBurnable() == null)
		{
			return;
		}
		this.inventory.temperature = this.cookingTemperature;
		this.UpdateAttachmentTemperature();
		base.InvokeRepeating(new Action(this.Cook), 0.5f, 0.5f);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
	}

	public virtual void StopCooking()
	{
		this.UpdateAttachmentTemperature();
		if (this.inventory != null)
		{
			this.inventory.temperature = 15f;
			foreach (Item item in this.inventory.itemList)
			{
				if (!item.HasFlag(Item.Flag.OnFire))
				{
					continue;
				}
				item.SetFlag(Item.Flag.OnFire, false);
				item.MarkDirty();
			}
		}
		base.CancelInvoke(new Action(this.Cook));
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void SVSwitch(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (Interface.CallHook("OnOvenToggle", this, msg.player) != null)
		{
			return;
		}
		if (flag == base.IsOn())
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
		{
			return;
		}
		if (flag)
		{
			this.StartCooking();
			return;
		}
		this.StopCooking();
	}

	public void UpdateAttachmentTemperature()
	{
		BaseEntity slot = base.GetSlot(BaseEntity.Slot.FireMod);
		if (slot)
		{
			slot.SendMessage("ParentTemperatureUpdate", this.inventory.temperature, SendMessageOptions.DontRequireReceiver);
		}
	}

	public bool wantsSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsOn())
		{
			return false;
		}
		return this.disabledBySplash;
	}

	public enum TemperatureType
	{
		Normal,
		Warming,
		Cooking,
		Smelting,
		Fractioning
	}
}