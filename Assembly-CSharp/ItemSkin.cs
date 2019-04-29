using Rust.Workshop;
using System;
using UnityEngine;

public class ItemSkin : SteamInventoryItem
{
	public Skinnable Skinnable;

	public Material[] Materials;

	public ItemSkin()
	{
	}

	public void ApplySkin(GameObject obj)
	{
		if (this.Skinnable == null)
		{
			return;
		}
		Skin.Apply(obj, this.Skinnable, this.Materials);
	}
}