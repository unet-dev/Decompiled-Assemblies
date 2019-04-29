using System;
using UnityEngine;

public class LockedByEntCrate : LootContainer
{
	public GameObject lockingEnt;

	public LockedByEntCrate()
	{
	}

	public void SetLocked(bool isLocked)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, isLocked, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, isLocked, false, true);
	}

	public void SetLockingEnt(GameObject ent)
	{
		base.CancelInvoke(new Action(this.Think));
		this.SetLocked(false);
		this.lockingEnt = ent;
		if (this.lockingEnt != null)
		{
			base.InvokeRepeating(new Action(this.Think), UnityEngine.Random.Range(0f, 1f), 1f);
			this.SetLocked(true);
		}
	}

	public void Think()
	{
		if (this.lockingEnt == null && base.IsLocked())
		{
			this.SetLockingEnt(null);
		}
	}
}