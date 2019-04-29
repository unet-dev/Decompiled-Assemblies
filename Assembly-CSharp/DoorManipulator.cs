using Facepunch;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorManipulator : IOEntity
{
	public EntityRef entityRef;

	public Door targetDoor;

	public DoorManipulator.DoorEffect powerAction;

	private bool toggle = true;

	public DoorManipulator()
	{
	}

	public void DoAction()
	{
		bool flag = this.IsPowered();
		if (this.targetDoor == null)
		{
			this.DoActionDoorMissing();
		}
		if (this.targetDoor != null)
		{
			if (this.targetDoor.IsBusy())
			{
				base.Invoke(new Action(this.DoAction), 1f);
				return;
			}
			if (this.powerAction == DoorManipulator.DoorEffect.Open)
			{
				if (flag)
				{
					if (!this.targetDoor.IsOpen())
					{
						this.targetDoor.SetOpen(true);
						return;
					}
				}
				else if (this.targetDoor.IsOpen())
				{
					this.targetDoor.SetOpen(false);
					return;
				}
			}
			else if (this.powerAction != DoorManipulator.DoorEffect.Close)
			{
				if (this.powerAction == DoorManipulator.DoorEffect.Toggle)
				{
					if (flag && this.toggle)
					{
						this.targetDoor.SetOpen(!this.targetDoor.IsOpen());
						this.toggle = false;
						return;
					}
					if (!this.toggle)
					{
						this.toggle = true;
					}
				}
			}
			else if (flag)
			{
				if (this.targetDoor.IsOpen())
				{
					this.targetDoor.SetOpen(false);
					return;
				}
			}
			else if (!this.targetDoor.IsOpen())
			{
				this.targetDoor.SetOpen(true);
				return;
			}
		}
	}

	public virtual void DoActionDoorMissing()
	{
		this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
	}

	public Door FindDoor(bool allowLocked = true)
	{
		List<Door> list = Pool.GetList<Door>();
		Vis.Entities<Door>(base.transform.position, 1f, list, 2097152, QueryTriggerInteraction.Ignore);
		Door door = null;
		float single = Single.PositiveInfinity;
		foreach (Door door1 in list)
		{
			if (!door1.isServer)
			{
				continue;
			}
			if (!allowLocked)
			{
				BaseLock slot = door1.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
				if (slot != null && slot.IsLocked())
				{
					continue;
				}
			}
			float single1 = Vector3.Distance(door1.transform.position, base.transform.position);
			if (single1 >= single)
			{
				continue;
			}
			door = door1;
			single = single1;
		}
		Pool.FreeList<Door>(ref list);
		return door;
	}

	public override void Init()
	{
		base.Init();
		this.SetupInitialDoorConnection();
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		this.DoAction();
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.entityRef.uid = info.msg.ioEntity.genericEntRef1;
			this.powerAction = (DoorManipulator.DoorEffect)info.msg.ioEntity.genericInt1;
		}
	}

	public virtual bool PairWithLockedDoors()
	{
		return true;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericEntRef1 = this.entityRef.uid;
		info.msg.ioEntity.genericInt1 = (int)this.powerAction;
	}

	public virtual void SetTargetDoor(Door newTargetDoor)
	{
		Door door = this.targetDoor;
		this.targetDoor = newTargetDoor;
		base.SetFlag(BaseEntity.Flags.On, this.targetDoor != null, false, true);
		this.entityRef.Set(newTargetDoor);
		if (door != this.targetDoor && this.targetDoor != null)
		{
			this.DoAction();
		}
	}

	public virtual void SetupInitialDoorConnection()
	{
		if (this.targetDoor == null && !this.entityRef.IsValid(true))
		{
			this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
		}
		if (this.targetDoor != null && !this.entityRef.IsValid(true))
		{
			this.entityRef.Set(this.targetDoor);
		}
		if (this.entityRef.IsValid(true) && this.targetDoor == null)
		{
			this.SetTargetDoor(this.entityRef.Get(true).GetComponent<Door>());
		}
	}

	public enum DoorEffect
	{
		Close,
		Open,
		Toggle
	}
}