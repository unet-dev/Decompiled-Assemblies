using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Stocking : LootContainer
{
	public static ListHashSet<Stocking> stockings;

	public Stocking()
	{
	}

	internal override void DoServerDestroy()
	{
		Stocking.stockings.Remove(this);
		base.DoServerDestroy();
	}

	public bool IsEmpty()
	{
		if (this.inventory == null)
		{
			return false;
		}
		for (int i = this.inventory.itemList.Count - 1; i >= 0; i--)
		{
			if (this.inventory.itemList[i] != null)
			{
				return false;
			}
		}
		return true;
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (this.IsEmpty() && base.healthFraction <= 0.1f)
		{
			this.Hurt(base.health, DamageType.Generic, this, false);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (Stocking.stockings == null)
		{
			Stocking.stockings = new ListHashSet<Stocking>(8);
		}
		Stocking.stockings.Add(this);
	}

	public override void SpawnLoot()
	{
		if (this.inventory == null)
		{
			Debug.Log(string.Concat("CONTACT DEVELOPERS! Stocking::PopulateLoot has null inventory!!! ", base.name));
			return;
		}
		if (this.IsEmpty())
		{
			base.SpawnLoot();
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			this.Hurt(this.MaxHealth() * 0.1f, DamageType.Generic, null, false);
		}
	}
}