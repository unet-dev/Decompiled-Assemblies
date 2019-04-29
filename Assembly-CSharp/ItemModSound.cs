using Rust;
using System;
using UnityEngine;

public class ItemModSound : ItemMod
{
	public GameObjectRef effect = new GameObjectRef();

	public ItemModSound.Type actionType;

	public ItemModSound()
	{
	}

	public override void OnParentChanged(Item item)
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.actionType == ItemModSound.Type.OnAttachToWeapon)
		{
			if (item.parentItem == null)
			{
				return;
			}
			if (item.parentItem.info.category != ItemCategory.Weapon)
			{
				return;
			}
			BasePlayer ownerPlayer = item.parentItem.GetOwnerPlayer();
			if (ownerPlayer == null)
			{
				return;
			}
			Effect.server.Run(this.effect.resourcePath, ownerPlayer, 0, Vector3.zero, Vector3.zero, null, false);
		}
	}

	public enum Type
	{
		OnAttachToWeapon
	}
}