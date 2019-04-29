using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Hair Dye Collection")]
public class HairDyeCollection : ScriptableObject
{
	public Texture capMask;

	public bool applyCap;

	public HairDye[] Variations;

	public HairDyeCollection()
	{
	}

	public HairDye Get(float seed)
	{
		if (this.Variations.Length == 0)
		{
			return null;
		}
		return this.Variations[Mathf.Clamp(Mathf.FloorToInt(seed * (float)((int)this.Variations.Length)), 0, (int)this.Variations.Length - 1)];
	}
}