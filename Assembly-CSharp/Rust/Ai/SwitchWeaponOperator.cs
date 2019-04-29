using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class SwitchWeaponOperator : BaseAction
	{
		[ApexSerialization]
		private NPCPlayerApex.WeaponTypeEnum WeaponType;

		public SwitchWeaponOperator()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			SwitchWeaponOperator.TrySwitchWeaponTo(c as NPCHumanContext, this.WeaponType);
		}

		private static Item FindBestMelee(NPCHumanContext c)
		{
			if (c.Human.GetPathStatus() != 0)
			{
				return null;
			}
			Item item = null;
			BaseMelee baseMelee = null;
			Item[] itemArray = c.Human.inventory.AllItems();
			for (int i = 0; i < (int)itemArray.Length; i++)
			{
				Item item1 = itemArray[i];
				if (item1.info.category == ItemCategory.Weapon && !item1.isBroken)
				{
					BaseMelee heldEntity = item1.GetHeldEntity() as BaseMelee;
					if (heldEntity)
					{
						if (item == null)
						{
							item = item1;
							baseMelee = heldEntity;
						}
						else if (heldEntity.hostileScore > baseMelee.hostileScore)
						{
							item = item1;
							baseMelee = heldEntity;
						}
					}
				}
			}
			return item;
		}

		private static Item FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum from, NPCPlayerApex.WeaponTypeEnum to, NPCHumanContext c)
		{
			Item item = null;
			BaseProjectile baseProjectile = null;
			Item[] itemArray = c.Human.inventory.AllItems();
			for (int i = 0; i < (int)itemArray.Length; i++)
			{
				Item item1 = itemArray[i];
				if (item1.info.category == ItemCategory.Weapon && !item1.isBroken)
				{
					BaseProjectile heldEntity = item1.GetHeldEntity() as BaseProjectile;
					if (heldEntity != null && heldEntity.effectiveRangeType <= to && heldEntity.effectiveRangeType > from)
					{
						if (item == null)
						{
							item = item1;
							baseProjectile = heldEntity;
						}
						else if (heldEntity.hostileScore > baseProjectile.hostileScore)
						{
							item = item1;
							baseProjectile = heldEntity;
						}
					}
				}
			}
			return item;
		}

		public static bool TrySwitchWeaponTo(NPCHumanContext c, NPCPlayerApex.WeaponTypeEnum WeaponType)
		{
			Item item;
			if (c != null)
			{
				if (Time.realtimeSinceStartup < c.Human.NextWeaponSwitchTime)
				{
					return false;
				}
				uint human = c.Human.svActiveItemID;
				switch (WeaponType)
				{
					case NPCPlayerApex.WeaponTypeEnum.CloseRange:
					{
						item = SwitchWeaponOperator.FindBestMelee(c);
						if (item != null)
						{
							c.Human.StoppingDistance = 1.5f;
							break;
						}
						else
						{
							item = SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.None, NPCPlayerApex.WeaponTypeEnum.CloseRange, c) ?? (SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.CloseRange, NPCPlayerApex.WeaponTypeEnum.MediumRange, c) ?? SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.MediumRange, NPCPlayerApex.WeaponTypeEnum.LongRange, c));
							c.Human.StoppingDistance = 2.5f;
							break;
						}
					}
					case NPCPlayerApex.WeaponTypeEnum.MediumRange:
					{
						item = SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.CloseRange, NPCPlayerApex.WeaponTypeEnum.MediumRange, c) ?? SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.MediumRange, NPCPlayerApex.WeaponTypeEnum.LongRange, c);
						c.Human.StoppingDistance = 0.1f;
						break;
					}
					case NPCPlayerApex.WeaponTypeEnum.LongRange:
					{
						item = SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.MediumRange, NPCPlayerApex.WeaponTypeEnum.LongRange, c) ?? SwitchWeaponOperator.FindBestProjInRange(NPCPlayerApex.WeaponTypeEnum.CloseRange, NPCPlayerApex.WeaponTypeEnum.MediumRange, c);
						c.Human.StoppingDistance = 0.1f;
						break;
					}
					default:
					{
						c.Human.UpdateActiveItem(0);
						if (human != c.Human.svActiveItemID)
						{
							c.Human.NextWeaponSwitchTime = Time.realtimeSinceStartup + c.Human.WeaponSwitchFrequency;
							c.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte)c.Human.GetCurrentWeaponTypeEnum(), true, true);
							c.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte)c.Human.GetCurrentAmmoStateEnum(), true, true);
						}
						c.Human.StoppingDistance = 1f;
						return true;
					}
				}
				if (item != null)
				{
					c.Human.UpdateActiveItem(item.uid);
					if (human != c.Human.svActiveItemID)
					{
						c.Human.NextWeaponSwitchTime = Time.realtimeSinceStartup + c.Human.WeaponSwitchFrequency;
						c.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte)c.Human.GetCurrentWeaponTypeEnum(), true, true);
						c.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte)c.Human.GetCurrentAmmoStateEnum(), true, true);
						c.SetFact(NPCPlayerApex.Facts.CurrentToolType, 0, true, true);
					}
					return true;
				}
			}
			return false;
		}
	}
}