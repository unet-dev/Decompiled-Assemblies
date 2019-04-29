using Rust;
using System;
using UnityEngine;

public class ItemModWearable : ItemMod
{
	public GameObjectRef entityPrefab = new GameObjectRef();

	public ProtectionProperties protectionProperties;

	public ArmorProperties armorProperties;

	public ClothingMovementProperties movementProperties;

	public UIBlackoutOverlay.blackoutType occlusionType = UIBlackoutOverlay.blackoutType.NONE;

	public bool blocksAiming;

	public bool emissive;

	public float accuracyBonus;

	public bool blocksEquipping;

	public float eggVision;

	public GameObjectRef viewmodelAddition;

	public Wearable targetWearable
	{
		get
		{
			if (!this.entityPrefab.isValid)
			{
				return null;
			}
			return this.entityPrefab.Get().GetComponent<Wearable>();
		}
	}

	public ItemModWearable()
	{
	}

	public bool CanExistWith(ItemModWearable wearable)
	{
		if (wearable == null)
		{
			return true;
		}
		Wearable wearable1 = this.targetWearable;
		Wearable wearable2 = wearable.targetWearable;
		if ((int)(wearable1.occupationOver & wearable2.occupationOver) != 0)
		{
			return false;
		}
		if ((int)(wearable1.occupationUnder & wearable2.occupationUnder) != 0)
		{
			return false;
		}
		return true;
	}

	public void CollectProtection(Item item, ProtectionProperties protection)
	{
		if (this.protectionProperties == null)
		{
			return;
		}
		protection.Add(this.protectionProperties, this.ConditionProtectionScale(item));
	}

	public float ConditionProtectionScale(Item item)
	{
		if (!item.isBroken)
		{
			return 1f;
		}
		return 0.25f;
	}

	private void DoPrepare()
	{
		if (!this.entityPrefab.isValid)
		{
			Debug.LogWarning(string.Concat("ItemModWearable: entityPrefab is null! ", base.gameObject), base.gameObject);
		}
		if (this.entityPrefab.isValid && this.targetWearable == null)
		{
			Debug.LogWarning(string.Concat("ItemModWearable: entityPrefab doesn't have a Wearable component! ", base.gameObject), this.entityPrefab.Get());
		}
	}

	internal float GetProtection(Item item, DamageType damageType)
	{
		if (this.protectionProperties == null)
		{
			return 0f;
		}
		return this.protectionProperties.Get(damageType) * this.ConditionProtectionScale(item);
	}

	public bool HasProtections()
	{
		return this.protectionProperties != null;
	}

	public bool IsFootwear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		if (component != null && (int)(component.occupationOver & (Wearable.OccupationSlots.LeftFoot | Wearable.OccupationSlots.RightFoot)) != 0)
		{
			return true;
		}
		return false;
	}

	private bool IsHeadgear()
	{
		Wearable component = this.entityPrefab.Get().GetComponent<Wearable>();
		if (component != null && (int)(component.occupationOver & (Wearable.OccupationSlots.HeadTop | Wearable.OccupationSlots.Face | Wearable.OccupationSlots.HeadBack)) != 0)
		{
			return true;
		}
		return false;
	}

	public override void ModInit()
	{
		if (string.IsNullOrEmpty(this.entityPrefab.resourcePath))
		{
			Debug.LogWarning(string.Concat(this, " - entityPrefab is null or something.. - ", this.entityPrefab.guid));
		}
	}

	public override void OnAttacked(Item item, HitInfo info)
	{
		if (!item.hasCondition)
		{
			return;
		}
		float single = 0f;
		for (int i = 0; i < 22; i++)
		{
			DamageType damageType = (DamageType)i;
			if (info.damageTypes.Has(damageType))
			{
				single += Mathf.Clamp(info.damageTypes.types[i] * this.GetProtection(item, damageType), 0f, item.condition);
				if (single >= item.condition)
				{
					break;
				}
			}
		}
		item.LoseCondition(single);
		if (item != null && item.isBroken && item.GetOwnerPlayer() && this.IsHeadgear() && info.damageTypes.Total() >= item.GetOwnerPlayer().health)
		{
			Vector3 ownerPlayer = item.GetOwnerPlayer().transform.position + new Vector3(0f, 1.8f, 0f);
			Vector3 inheritedDropVelocity = item.GetOwnerPlayer().GetInheritedDropVelocity() + (Vector3.up * 3f);
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = item.Drop(ownerPlayer, inheritedDropVelocity, quaternion);
			quaternion = UnityEngine.Random.rotation;
			baseEntity.SetAngularVelocity(quaternion.eulerAngles * 5f);
		}
	}

	public bool ProtectsArea(HitArea area)
	{
		if (this.armorProperties == null)
		{
			return false;
		}
		return this.armorProperties.Contains(area);
	}
}