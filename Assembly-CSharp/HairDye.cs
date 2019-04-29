using System;
using UnityEngine;

[Serializable]
public class HairDye
{
	[ColorUsage(false, true)]
	public Color capBaseColor;

	public Material sourceMaterial;

	[InspectorFlags]
	public HairDye.CopyPropertyMask copyProperties;

	private static MaterialPropertyDesc[] transferableProps;

	private static int _HairBaseColorUV1;

	private static int _HairBaseColorUV2;

	private static int _HairPackedMapUV1;

	private static int _HairPackedMapUV2;

	static HairDye()
	{
		HairDye.transferableProps = new MaterialPropertyDesc[] { new MaterialPropertyDesc("_DyeColor", typeof(Color)), new MaterialPropertyDesc("_RootColor", typeof(Color)), new MaterialPropertyDesc("_TipColor", typeof(Color)), new MaterialPropertyDesc("_Brightness", typeof(float)), new MaterialPropertyDesc("_DyeRoughness", typeof(float)), new MaterialPropertyDesc("_DyeScatter", typeof(float)), new MaterialPropertyDesc("_HairSpecular", typeof(float)), new MaterialPropertyDesc("_HairRoughness", typeof(float)) };
		HairDye._HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");
		HairDye._HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");
		HairDye._HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");
		HairDye._HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");
	}

	public HairDye()
	{
	}

	public void Apply(HairDyeCollection collection, MaterialPropertyBlock block)
	{
		if (this.sourceMaterial != null)
		{
			for (int i = 0; i < 8; i++)
			{
				if (((int)this.copyProperties & 1 << (i & 31)) != 0)
				{
					MaterialPropertyDesc materialPropertyDesc = HairDye.transferableProps[i];
					if (this.sourceMaterial.HasProperty(materialPropertyDesc.nameID))
					{
						if (materialPropertyDesc.type == typeof(Color))
						{
							block.SetColor(materialPropertyDesc.nameID, this.sourceMaterial.GetColor(materialPropertyDesc.nameID));
						}
						else if (materialPropertyDesc.type == typeof(float))
						{
							block.SetFloat(materialPropertyDesc.nameID, this.sourceMaterial.GetFloat(materialPropertyDesc.nameID));
						}
					}
				}
			}
		}
	}

	public void ApplyCap(HairDyeCollection collection, HairType type, MaterialPropertyBlock block)
	{
		Texture texture;
		Texture texture1;
		if (collection.applyCap)
		{
			if (type == HairType.Head || type == HairType.Armpit || type == HairType.Pubic)
			{
				block.SetColor(HairDye._HairBaseColorUV1, this.capBaseColor.gamma);
				MaterialPropertyBlock materialPropertyBlock = block;
				int num = HairDye._HairPackedMapUV1;
				if (collection.capMask != null)
				{
					texture = collection.capMask;
				}
				else
				{
					texture = Texture2D.blackTexture;
				}
				materialPropertyBlock.SetTexture(num, texture);
				return;
			}
			if (type == HairType.Facial)
			{
				block.SetColor(HairDye._HairBaseColorUV2, this.capBaseColor.gamma);
				MaterialPropertyBlock materialPropertyBlock1 = block;
				int num1 = HairDye._HairPackedMapUV2;
				if (collection.capMask != null)
				{
					texture1 = collection.capMask;
				}
				else
				{
					texture1 = Texture2D.blackTexture;
				}
				materialPropertyBlock1.SetTexture(num1, texture1);
			}
		}
	}

	public enum CopyProperty
	{
		DyeColor,
		RootColor,
		TipColor,
		Brightness,
		DyeRoughness,
		DyeScatter,
		Specular,
		Roughness,
		Count
	}

	[Flags]
	public enum CopyPropertyMask
	{
		DyeColor = 1,
		RootColor = 2,
		TipColor = 4,
		Brightness = 8,
		DyeRoughness = 16,
		DyeScatter = 32,
		Specular = 64,
		Roughness = 128
	}
}