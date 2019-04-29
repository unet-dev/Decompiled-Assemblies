using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Skin Set Collection")]
public class SkinSetCollection : ScriptableObject
{
	public SkinSet[] Skins;

	public SkinSetCollection()
	{
	}

	public SkinSet Get(float MeshNumber)
	{
		return this.Skins[this.GetIndex(MeshNumber)];
	}

	public int GetIndex(float MeshNumber)
	{
		return Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)((int)this.Skins.Length)), 0, (int)this.Skins.Length - 1);
	}
}