using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ResourceDispenser : EntityComponent<BaseEntity>, IServerComponent
{
	public ResourceDispenser.GatherType gatherType = ResourceDispenser.GatherType.UNSET;

	public List<ItemAmount> containedItems;

	public float maxDestroyFractionForFinishBonus = 0.2f;

	public List<ItemAmount> finishBonus;

	public float fractionRemaining = 1f;

	private float categoriesRemaining;

	private float startingItemCounts;

	public ResourceDispenser()
	{
	}

	public void AssignFinishBonus(BasePlayer player, float fraction)
	{
		base.SendMessage("FinishBonusAssigned", SendMessageOptions.DontRequireReceiver);
		if (fraction <= 0f)
		{
			return;
		}
		if (this.finishBonus != null)
		{
			foreach (ItemAmount finishBonu in this.finishBonus)
			{
				Item item = ItemManager.Create(finishBonu.itemDef, Mathf.CeilToInt((float)((int)finishBonu.amount) * Mathf.Clamp01(fraction)), (ulong)0);
				if (item == null)
				{
					continue;
				}
				object obj = Interface.CallHook("OnDispenserBonus", this, player, item);
				if (obj is Item)
				{
					item = (Item)obj;
				}
				player.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
			}
		}
	}

	public void CountAllItems()
	{
		this.startingItemCounts = this.containedItems.Sum<ItemAmount>((ItemAmount x) => x.startAmount);
	}

	public void DestroyFraction(float fraction)
	{
		foreach (ItemAmount containedItem in this.containedItems)
		{
			if (containedItem.amount <= 0f)
			{
				continue;
			}
			ItemAmount itemAmount = containedItem;
			itemAmount.amount = itemAmount.amount - fraction / this.categoriesRemaining;
		}
		this.UpdateVars();
	}

	public void DoGather(HitInfo info)
	{
		BaseMelee component;
		if (!base.baseEntity.isServer)
		{
			return;
		}
		if (!info.CanGather || info.DidGather)
		{
			return;
		}
		if (this.gatherType == ResourceDispenser.GatherType.UNSET)
		{
			Debug.LogWarning(string.Concat("Object :", base.gameObject.name, ": has unset gathertype!"));
			return;
		}
		float single = 0f;
		float single1 = 0f;
		if (info.Weapon == null)
		{
			component = null;
		}
		else
		{
			component = info.Weapon.GetComponent<BaseMelee>();
		}
		BaseMelee baseMelee = component;
		if (baseMelee == null)
		{
			single = info.damageTypes.Total();
			single1 = 0.5f;
		}
		else
		{
			ResourceDispenser.GatherPropertyEntry gatherInfoFromIndex = baseMelee.GetGatherInfoFromIndex(this.gatherType);
			single = gatherInfoFromIndex.gatherDamage * info.gatherScale;
			single1 = gatherInfoFromIndex.destroyFraction;
			if (single == 0f)
			{
				return;
			}
			baseMelee.SendPunch((new Vector3(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(-0.25f, -0.5f), 0f) * -30f) * (gatherInfoFromIndex.conditionLost / 6f), 0.05f);
			baseMelee.LoseCondition(gatherInfoFromIndex.conditionLost);
			if (!baseMelee.IsValid() || baseMelee.IsBroken())
			{
				return;
			}
			info.DidGather = true;
		}
		float single2 = this.fractionRemaining;
		this.GiveResources(info.Initiator, single, single1, info.Weapon);
		this.UpdateFraction();
		float single3 = 0f;
		if (this.fractionRemaining > 0f)
		{
			single3 = (single2 - this.fractionRemaining) * base.baseEntity.MaxHealth();
		}
		else
		{
			single3 = base.baseEntity.MaxHealth();
			if (info.DidGather && single1 < this.maxDestroyFractionForFinishBonus)
			{
				this.AssignFinishBonus(info.InitiatorPlayer, 1f - single1);
			}
		}
		HitInfo hitInfo = new HitInfo(info.Initiator, base.baseEntity, DamageType.Generic, single3, base.transform.position)
		{
			gatherScale = 0f,
			PointStart = info.PointStart,
			PointEnd = info.PointEnd
		};
		base.baseEntity.OnAttacked(hitInfo);
	}

	private void GiveResourceFromItem(BaseEntity entity, ItemAmount itemAmt, float gatherDamage, float destroyFraction, AttackEntity attackWeapon)
	{
		if (itemAmt.amount == 0f)
		{
			return;
		}
		float single = Mathf.Min(gatherDamage, base.baseEntity.Health()) / base.baseEntity.MaxHealth();
		float single1 = itemAmt.startAmount / this.startingItemCounts;
		float single2 = Mathf.Clamp(itemAmt.startAmount * single / single1, 0f, itemAmt.amount);
		float single3 = single2 * destroyFraction * 2f;
		if (itemAmt.amount <= single2 + single3)
		{
			float single4 = (single2 + single3) / itemAmt.amount;
			single2 /= single4;
			single3 /= single4;
		}
		itemAmt.amount -= Mathf.Floor(single2);
		itemAmt.amount -= Mathf.Floor(single3);
		if (single2 < 1f)
		{
			single2 = (UnityEngine.Random.Range(0f, 1f) <= single2 ? 1f : 0f);
			itemAmt.amount = 0f;
		}
		if (itemAmt.amount < 0f)
		{
			itemAmt.amount = 0f;
		}
		if (single2 >= 1f)
		{
			Item item = ItemManager.CreateByItemID(itemAmt.itemid, Mathf.FloorToInt(single2), (ulong)0);
			if (item == null)
			{
				return;
			}
			if (Interface.CallHook("OnDispenserGather", this, entity, item) != null)
			{
				return;
			}
			this.OverrideOwnership(item, attackWeapon);
			entity.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
		}
	}

	private void GiveResources(BaseEntity entity, float gatherDamage, float destroyFraction, AttackEntity attackWeapon)
	{
		if (!entity.IsValid())
		{
			return;
		}
		if (gatherDamage <= 0f)
		{
			return;
		}
		ItemAmount item = null;
		int count = this.containedItems.Count;
		int num = UnityEngine.Random.Range(0, this.containedItems.Count);
		while (count > 0)
		{
			if (num >= this.containedItems.Count)
			{
				num = 0;
			}
			if (this.containedItems[num].amount <= 0f)
			{
				num++;
				count--;
			}
			else
			{
				item = this.containedItems[num];
				break;
			}
		}
		if (item == null)
		{
			return;
		}
		this.GiveResourceFromItem(entity, item, gatherDamage, destroyFraction, attackWeapon);
		this.UpdateVars();
		BasePlayer player = entity.ToPlayer();
		if (player)
		{
			Debug.Assert(attackWeapon.GetItem() != null, string.Concat("Attack Weapon ", attackWeapon, " has no Item"));
			Debug.Assert(attackWeapon.ownerItemUID != 0, string.Concat("Attack Weapon ", attackWeapon, " ownerItemUID is 0"));
			Debug.Assert(attackWeapon.GetParentEntity() != null, string.Concat("Attack Weapon ", attackWeapon, " GetParentEntity is null"));
			Debug.Assert(attackWeapon.GetParentEntity().IsValid(), string.Concat("Attack Weapon ", attackWeapon, " GetParentEntity is not valid"));
			Debug.Assert(attackWeapon.GetParentEntity().ToPlayer() != null, string.Concat("Attack Weapon ", attackWeapon, " GetParentEntity is not a player"));
			Debug.Assert(!attackWeapon.GetParentEntity().ToPlayer().IsDead(), string.Concat("Attack Weapon ", attackWeapon, " GetParentEntity is not valid"));
			BasePlayer ownerPlayer = attackWeapon.GetOwnerPlayer();
			Debug.Assert(ownerPlayer != null, string.Concat("Attack Weapon ", attackWeapon, " ownerPlayer is null"));
			Debug.Assert(ownerPlayer == player, string.Concat("Attack Weapon ", attackWeapon, " ownerPlayer is not player"));
			if (ownerPlayer != null)
			{
				Debug.Assert(ownerPlayer.inventory != null, string.Concat("Attack Weapon ", attackWeapon, " ownerPlayer inventory is null"));
				Debug.Assert(ownerPlayer.inventory.FindItemUID(attackWeapon.ownerItemUID) != null, string.Concat("Attack Weapon ", attackWeapon, " FindItemUID is null"));
			}
		}
	}

	public void Initialize()
	{
		this.UpdateFraction();
		this.UpdateRemainingCategories();
		this.CountAllItems();
	}

	public void OnAttacked(HitInfo info)
	{
		this.DoGather(info);
	}

	public virtual bool OverrideOwnership(Item item, AttackEntity weapon)
	{
		return false;
	}

	public void Start()
	{
		this.Initialize();
	}

	private void UpdateFraction()
	{
		float single = this.containedItems.Sum<ItemAmount>((ItemAmount x) => x.startAmount);
		float single1 = this.containedItems.Sum<ItemAmount>((ItemAmount x) => x.amount);
		if (single == 0f)
		{
			this.fractionRemaining = 0f;
			return;
		}
		this.fractionRemaining = single1 / single;
	}

	public void UpdateRemainingCategories()
	{
		int num = 0;
		foreach (ItemAmount containedItem in this.containedItems)
		{
			if (containedItem.amount <= 0f)
			{
				continue;
			}
			num++;
		}
		this.categoriesRemaining = (float)num;
	}

	private void UpdateVars()
	{
		this.UpdateFraction();
		this.UpdateRemainingCategories();
	}

	[Serializable]
	public class GatherProperties
	{
		public ResourceDispenser.GatherPropertyEntry Tree;

		public ResourceDispenser.GatherPropertyEntry Ore;

		public ResourceDispenser.GatherPropertyEntry Flesh;

		public GatherProperties()
		{
		}

		public bool Any()
		{
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				if (fromIndex.gatherDamage > 0f || fromIndex.conditionLost > 0f)
				{
					return true;
				}
			}
			return false;
		}

		public ResourceDispenser.GatherPropertyEntry GetFromIndex(int index)
		{
			return this.GetFromIndex((ResourceDispenser.GatherType)index);
		}

		public ResourceDispenser.GatherPropertyEntry GetFromIndex(ResourceDispenser.GatherType index)
		{
			switch (index)
			{
				case ResourceDispenser.GatherType.Tree:
				{
					return this.Tree;
				}
				case ResourceDispenser.GatherType.Ore:
				{
					return this.Ore;
				}
				case ResourceDispenser.GatherType.Flesh:
				{
					return this.Flesh;
				}
			}
			return null;
		}

		public float GetProficiency()
		{
			float single = 0f;
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				float single1 = fromIndex.gatherDamage * fromIndex.destroyFraction;
				if (single1 > 0f)
				{
					single = single + fromIndex.gatherDamage / single1;
				}
			}
			return single;
		}
	}

	[Serializable]
	public class GatherPropertyEntry
	{
		public float gatherDamage;

		public float destroyFraction;

		public float conditionLost;

		public GatherPropertyEntry()
		{
		}
	}

	public enum GatherType
	{
		Tree,
		Ore,
		Flesh,
		UNSET,
		LAST
	}
}