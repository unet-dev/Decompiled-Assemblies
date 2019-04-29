using System;
using UnityEngine;

[RequireComponent(typeof(PlayerModelSkin))]
public class PlayerModelHairCap : MonoBehaviour
{
	[InspectorFlags]
	public HairCapMask hairCapMask;

	public PlayerModelHairCap()
	{
	}

	public void SetupHairCap(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		float single;
		float single1;
		int index = skin.GetIndex(meshNum);
		SkinSet skins = skin.Skins[index];
		if (skins != null)
		{
			for (int i = 0; i < 5; i++)
			{
				if (((int)this.hairCapMask & 1 << (i & 31)) != 0)
				{
					PlayerModelHair.GetRandomVariation(hairNum, i, index, out single, out single1);
					HairType hairType = (HairType)i;
					HairSetCollection.HairSetEntry hairSetEntry = skins.HairCollection.Get(hairType, single);
					if (hairSetEntry.HairSet != null)
					{
						HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
						if (hairDyeCollection != null)
						{
							HairDye hairDye = hairDyeCollection.Get(single1);
							if (hairDye != null)
							{
								hairDye.ApplyCap(hairDyeCollection, hairType, block);
							}
						}
					}
				}
			}
		}
	}
}