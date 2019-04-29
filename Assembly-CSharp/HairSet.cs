using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Hair Set")]
public class HairSet : ScriptableObject
{
	public HairSet.MeshReplace[] MeshReplacements;

	public HairSet.MaterialReplace[] MaterialReplacements;

	public HairSet()
	{
	}

	public void Process(PlayerModelHair playerModelHair, HairDyeCollection dyeCollection, HairDye dye, MaterialPropertyBlock block)
	{
		PlayerModelHair.RendererMaterials replace;
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		playerModelHair.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer bones in list)
		{
			if (bones.sharedMesh == null || bones.sharedMaterial == null)
			{
				continue;
			}
			string str = bones.sharedMesh.name;
			string str1 = bones.sharedMaterial.name;
			if (!bones.gameObject.activeSelf)
			{
				bones.gameObject.SetActive(true);
			}
			for (int i = 0; i < (int)this.MeshReplacements.Length; i++)
			{
				if (this.MeshReplacements[i].Test(str))
				{
					SkinnedMeshRenderer skinnedMeshRenderer = this.MeshReplacements[i].Replace;
					if (skinnedMeshRenderer != null)
					{
						bones.sharedMesh = skinnedMeshRenderer.sharedMesh;
						bones.rootBone = skinnedMeshRenderer.rootBone;
						bones.bones = this.MeshReplacements[i].GetBones();
					}
					else
					{
						bones.gameObject.SetActive(false);
					}
				}
			}
			if (!playerModelHair.Materials.TryGetValue(bones, out replace))
			{
				Debug.LogWarning(string.Concat("[HairSet.Process] Missing cached renderer materials in ", playerModelHair.name));
			}
			else
			{
				Array.Copy(replace.original, replace.replacement, (int)replace.original.Length);
				for (int j = 0; j < (int)replace.original.Length; j++)
				{
					for (int k = 0; k < (int)this.MaterialReplacements.Length; k++)
					{
						if (this.MaterialReplacements[k].Test(replace.names[j]))
						{
							replace.replacement[j] = this.MaterialReplacements[k].Replace;
						}
					}
				}
				bones.sharedMaterials = replace.replacement;
			}
			if (dye == null || !bones.gameObject.activeSelf)
			{
				continue;
			}
			dye.Apply(dyeCollection, block);
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	public void ProcessMorphs(GameObject obj, int blendShapeIndex = -1)
	{
	}

	[Serializable]
	public class MaterialReplace
	{
		[HideInInspector]
		public string FindName;

		public Material Find;

		public Material Replace;

		public MaterialReplace()
		{
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

		public SkinnedMeshRenderer Replace;

		private Transform[] bones;

		public MeshReplace()
		{
		}

		public Transform[] GetBones()
		{
			if (this.bones == null)
			{
				this.bones = this.Replace.bones;
			}
			return this.bones;
		}

		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}
	}
}