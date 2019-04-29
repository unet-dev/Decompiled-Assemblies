using Facepunch;
using Rust.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ViewmodelClothing : MonoBehaviour
{
	public SkeletonSkin[] SkeletonSkins;

	public ViewmodelClothing()
	{
	}

	internal void CopyToSkeleton(Skeleton skeleton, GameObject parent, Item item)
	{
		Func<ItemSkinDirectory.Skin, bool> func = null;
		SkeletonSkin[] skeletonSkins = this.SkeletonSkins;
		for (int i = 0; i < (int)skeletonSkins.Length; i++)
		{
			SkeletonSkin skeletonSkin = skeletonSkins[i];
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = parent.transform;
			skeletonSkin.DuplicateAndRetarget(gameObject, skeleton).updateWhenOffscreen = true;
			if (item != null && item.skin > (long)0)
			{
				ItemSkinDirectory.Skin[] skinArray = item.info.skins;
				Func<ItemSkinDirectory.Skin, bool> func1 = func;
				if (func1 == null)
				{
					Func<ItemSkinDirectory.Skin, bool> func2 = (ItemSkinDirectory.Skin x) => (long)x.id == item.skin;
					Func<ItemSkinDirectory.Skin, bool> func3 = func2;
					func = func2;
					func1 = func3;
				}
				ItemSkinDirectory.Skin skin = ((IEnumerable<ItemSkinDirectory.Skin>)skinArray).FirstOrDefault<ItemSkinDirectory.Skin>(func1);
				if (skin.id == 0 && item.skin > (long)0)
				{
					Rust.Workshop.WorkshopSkin.Apply(gameObject, item.skin, null);
					return;
				}
				if ((long)skin.id != item.skin)
				{
					return;
				}
				ItemSkin itemSkin = skin.invItem as ItemSkin;
				if (itemSkin == null)
				{
					return;
				}
				itemSkin.ApplySkin(gameObject);
			}
		}
	}
}