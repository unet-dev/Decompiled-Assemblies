using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Hair Set Collection")]
public class HairSetCollection : ScriptableObject
{
	public HairSetCollection.HairSetEntry[] Head;

	public HairSetCollection.HairSetEntry[] Eyebrow;

	public HairSetCollection.HairSetEntry[] Facial;

	public HairSetCollection.HairSetEntry[] Armpit;

	public HairSetCollection.HairSetEntry[] Pubic;

	public HairSetCollection()
	{
	}

	public HairSetCollection.HairSetEntry Get(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return listByType[this.GetIndex(listByType, typeNum)];
	}

	public int GetIndex(HairSetCollection.HairSetEntry[] list, float typeNum)
	{
		return Mathf.Clamp(Mathf.FloorToInt(typeNum * (float)((int)list.Length)), 0, (int)list.Length - 1);
	}

	public int GetIndex(HairType hairType, float typeNum)
	{
		return this.GetIndex(this.GetListByType(hairType), typeNum);
	}

	public HairSetCollection.HairSetEntry[] GetListByType(HairType hairType)
	{
		switch (hairType)
		{
			case HairType.Head:
			{
				return this.Head;
			}
			case HairType.Eyebrow:
			{
				return this.Eyebrow;
			}
			case HairType.Facial:
			{
				return this.Facial;
			}
			case HairType.Armpit:
			{
				return this.Armpit;
			}
			case HairType.Pubic:
			{
				return this.Pubic;
			}
		}
		return null;
	}

	[Serializable]
	public struct HairSetEntry
	{
		public HairSet HairSet;

		public HairDyeCollection HairDyeCollection;
	}
}