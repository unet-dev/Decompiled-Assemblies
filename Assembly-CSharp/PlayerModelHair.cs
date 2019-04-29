using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelHair : MonoBehaviour
{
	public HairType type;

	private Dictionary<Renderer, PlayerModelHair.RendererMaterials> materials;

	public Dictionary<Renderer, PlayerModelHair.RendererMaterials> Materials
	{
		get
		{
			return this.materials;
		}
	}

	public PlayerModelHair()
	{
	}

	private void CacheOriginalMaterials()
	{
		if (this.materials == null)
		{
			this.materials = new Dictionary<Renderer, PlayerModelHair.RendererMaterials>();
			List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
			base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
			this.materials.Clear();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
			{
				this.materials.Add(skinnedMeshRenderer, new PlayerModelHair.RendererMaterials(skinnedMeshRenderer));
			}
			Pool.FreeList<SkinnedMeshRenderer>(ref list);
		}
	}

	public static void GetRandomVariation(float hairNum, int typeIndex, int meshIndex, out float typeNum, out float dyeNum)
	{
		int num = Mathf.FloorToInt(hairNum * 100000f);
		UnityEngine.Random.InitState(num + typeIndex);
		typeNum = UnityEngine.Random.Range(0f, 1f);
		UnityEngine.Random.InitState(num + meshIndex);
		dyeNum = UnityEngine.Random.Range(0f, 1f);
	}

	private void Setup(HairType type, HairSetCollection hair, int meshIndex, float typeNum, float dyeNum, MaterialPropertyBlock block)
	{
		this.CacheOriginalMaterials();
		HairSetCollection.HairSetEntry hairSetEntry = hair.Get(type, typeNum);
		if (hairSetEntry.HairSet == null)
		{
			Debug.LogWarning("Hair.Get returned a NULL hair");
			return;
		}
		int num = -1;
		if (type == HairType.Facial || type == HairType.Eyebrow)
		{
			num = meshIndex;
		}
		HairDye hairDye = null;
		HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
		if (hairDyeCollection != null)
		{
			hairDye = hairDyeCollection.Get(dyeNum);
		}
		hairSetEntry.HairSet.Process(this, hairDyeCollection, hairDye, block);
		hairSetEntry.HairSet.ProcessMorphs(base.gameObject, num);
	}

	public void Setup(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		float single;
		float single1;
		int index = skin.GetIndex(meshNum);
		SkinSet skins = skin.Skins[index];
		if (skins == null)
		{
			Debug.LogError("Skin.Get returned a NULL skin");
			return;
		}
		int num = (int)this.type;
		PlayerModelHair.GetRandomVariation(hairNum, num, index, out single, out single1);
		this.Setup(this.type, skins.HairCollection, index, single, single1, block);
	}

	public struct RendererMaterials
	{
		public string[] names;

		public Material[] original;

		public Material[] replacement;

		public RendererMaterials(Renderer r)
		{
			this.original = r.sharedMaterials;
			this.replacement = this.original.Clone() as Material[];
			this.names = new string[(int)this.original.Length];
			for (int i = 0; i < (int)this.original.Length; i++)
			{
				this.names[i] = this.original[i].name;
			}
		}
	}
}