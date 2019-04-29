using Facepunch;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;

public class WaterPurifier : LiquidContainer
{
	public GameObjectRef storagePrefab;

	public Transform storagePrefabAnchor;

	public ItemDefinition freshWater;

	public int waterToProcessPerMinute = 120;

	public int freshWaterRatio = 4;

	public LiquidContainer waterStorage;

	public float dirtyWaterProcssed;

	public float pendingFreshWater;

	public WaterPurifier()
	{
	}

	public void CheckCoolDown()
	{
		if (!base.GetParentEntity() || !base.GetParentEntity().IsOn() || !this.HasDirtyWater())
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			base.CancelInvoke(new Action(this.CheckCoolDown));
		}
	}

	public void Cook(float timeCooked)
	{
		if (this.waterStorage == null)
		{
			return;
		}
		bool flag = this.HasDirtyWater();
		if (!this.IsBoiling() & flag)
		{
			base.InvokeRepeating(new Action(this.CheckCoolDown), 2f, 2f);
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		}
		if (!this.IsBoiling())
		{
			return;
		}
		if (flag)
		{
			float single = timeCooked * ((float)this.waterToProcessPerMinute / 60f);
			this.dirtyWaterProcssed += single;
			this.pendingFreshWater = this.pendingFreshWater + single / (float)this.freshWaterRatio;
			if (this.dirtyWaterProcssed >= 1f)
			{
				int num = Mathf.FloorToInt(this.dirtyWaterProcssed);
				this.inventory.GetSlot(0).UseItem(num);
				this.dirtyWaterProcssed -= (float)num;
				base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
			if (this.pendingFreshWater >= 1f)
			{
				int num1 = Mathf.FloorToInt(this.pendingFreshWater);
				this.pendingFreshWater -= (float)num1;
				Item slot = this.waterStorage.inventory.GetSlot(0);
				if (slot != null && slot.info != this.freshWater)
				{
					slot.RemoveFromContainer();
					slot.Remove(0f);
				}
				if (slot != null)
				{
					slot.amount += num1;
					slot.amount = Mathf.Clamp(slot.amount, 0, this.waterStorage.maxStackSize);
					this.waterStorage.inventory.MarkDirty();
				}
				else
				{
					Item item = ItemManager.Create(this.freshWater, num1, (ulong)0);
					if (!item.MoveToContainer(this.waterStorage.inventory, -1, true))
					{
						item.Remove(0f);
					}
				}
				this.waterStorage.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}
	}

	public bool HasDirtyWater()
	{
		Item slot = this.inventory.GetSlot(0);
		if (slot == null || slot.info.itemType != ItemContainer.ContentsType.Liquid)
		{
			return false;
		}
		return slot.amount > 0;
	}

	public bool IsBoiling()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			if (info.msg.miningQuarry != null && this.waterStorage != null)
			{
				this.waterStorage.inventory.Load(info.msg.miningQuarry.extractor.outputContents);
			}
		}
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.waterStorage.Kill(BaseNetworkable.DestroyMode.None);
	}

	internal override void OnParentRemoved()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	public void ParentTemperatureUpdate(float temp)
	{
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.SpawnStorageEnt();
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk && this.waterStorage != null)
		{
			info.msg.miningQuarry = Pool.Get<ProtoBuf.MiningQuarry>();
			info.msg.miningQuarry.extractor = Pool.Get<ResourceExtractor>();
			info.msg.miningQuarry.extractor.outputContents = this.waterStorage.inventory.Save();
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnStorageEnt();
		}
	}

	public void SpawnStorageEnt()
	{
		this.waterStorage = GameManager.server.CreateEntity(this.storagePrefab.resourcePath, this.storagePrefabAnchor.localPosition, this.storagePrefabAnchor.localRotation, true) as LiquidContainer;
		this.waterStorage.SetParent(base.GetParentEntity(), false, false);
		this.waterStorage.Spawn();
	}

	public static class WaterPurifierFlags
	{
		public const BaseEntity.Flags Boiling = BaseEntity.Flags.Reserved1;
	}
}