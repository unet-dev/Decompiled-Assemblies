using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Skin Set")]
public class SkinSet : ScriptableObject
{
	public string Label;

	public SkinSet.MeshReplace[] MeshReplacements;

	public SkinSet.MaterialReplace[] MaterialReplacements;

	public Gradient SkinColour;

	public HairSetCollection HairCollection;

	public SkinSet()
	{
	}

	internal Color GetSkinColor(float skinNumber)
	{
		return this.SkinColour.Evaluate(skinNumber);
	}

	public void Process(GameObject obj, float Seed)
	{
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		obj.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			if (skinnedMeshRenderer.sharedMesh == null || skinnedMeshRenderer.sharedMaterial == null)
			{
				continue;
			}
			string str = skinnedMeshRenderer.sharedMesh.name;
			string str1 = skinnedMeshRenderer.sharedMaterial.name;
			for (int i = 0; i < (int)this.MeshReplacements.Length; i++)
			{
				if (this.MeshReplacements[i].Test(str))
				{
					SkinnedMeshRenderer skinnedMeshRenderer1 = this.MeshReplacements[i].Get(Seed);
					skinnedMeshRenderer.sharedMesh = skinnedMeshRenderer1.sharedMesh;
					skinnedMeshRenderer.rootBone = skinnedMeshRenderer1.rootBone;
					skinnedMeshRenderer.bones = skinnedMeshRenderer1.bones;
				}
			}
			for (int j = 0; j < (int)this.MaterialReplacements.Length; j++)
			{
				if (this.MaterialReplacements[j].Test(str1))
				{
					skinnedMeshRenderer.sharedMaterial = this.MaterialReplacements[j].Get(Seed);
				}
			}
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	[Serializable]
	public class MaterialReplace
	{
		[HideInInspector]
		public string FindName;

		public Material Find;

		public Material[] Replace;

		public MaterialReplace()
		{
		}

		public Material Get(float MeshNumber)
		{
			return this.Replace[Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)((int)this.Replace.Length)), 0, (int)this.Replace.Length - 1)];
		}

		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}
	}

	[Serializable]
	public class MeshReplace
	{
		[HideInInspector]
		public string FindName;

		public Mesh Find;

		public SkinnedMeshRenderer[] Replace;

		public MeshReplace()
		{
		}

		public SkinnedMeshRenderer Get(float MeshNumber)
		{
			return this.Replace[Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)((int)this.Replace.Length)), 0, (int)this.Replace.Length - 1)];
		}

		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}
	}
}