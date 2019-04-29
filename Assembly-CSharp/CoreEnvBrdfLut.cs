using System;
using UnityEngine;

public class CoreEnvBrdfLut
{
	private static Texture2D runtimeEnvBrdfLut;

	static CoreEnvBrdfLut()
	{
	}

	public CoreEnvBrdfLut()
	{
	}

	public static Texture2D Generate(bool asset = false)
	{
		TextureFormat textureFormat = (asset ? TextureFormat.RGBAHalf : TextureFormat.RGHalf);
		textureFormat = (SystemInfo.SupportsTextureFormat(textureFormat) ? textureFormat : TextureFormat.ARGB32);
		int num = 128;
		int num1 = 32;
		float single = 1f / (float)num;
		float single1 = 1f / (float)num1;
		Texture2D texture2D = new Texture2D(num, num1, textureFormat, false, true)
		{
			name = "_EnvBrdfLut",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Bilinear
		};
		Color[] color = new Color[num * num1];
		float single2 = 0.0078125f;
		for (int i = 0; i < num1; i++)
		{
			float single3 = (float)((float)i + 0.5f) * single1;
			float single4 = single3 * single3;
			float single5 = single4 * single4;
			int num2 = 0;
			int num3 = i * num;
			while (num2 < num)
			{
				float single6 = (float)((float)num2 + 0.5f) * single;
				Vector3 vector3 = new Vector3(Mathf.Sqrt(1f - single6 * single6), 0f, single6);
				float single7 = 0f;
				float single8 = 0f;
				for (uint j = 0; j < 128; j++)
				{
					float single9 = (float)((float)j) * single2;
					float single10 = (float)((double)((float)CoreEnvBrdfLut.ReverseBits(j)) / 4294967296);
					float single11 = 6.28318548f * single9;
					float single12 = Mathf.Sqrt((1f - single10) / (1f + (single5 - 1f) * single10));
					float single13 = Mathf.Sqrt(1f - single12 * single12);
					Vector3 vector31 = new Vector3(single13 * Mathf.Cos(single11), single13 * Mathf.Sin(single11), single12);
					float single14 = Mathf.Max(((2f * Vector3.Dot(vector3, vector31) * vector31) - vector3).z, 0f);
					float single15 = Mathf.Max(vector31.z, 0f);
					float single16 = Mathf.Max(Vector3.Dot(vector3, vector31), 0f);
					if (single14 > 0f)
					{
						float single17 = single14 * (single6 * (1f - single4) + single4);
						float single18 = single6 * (single14 * (1f - single4) + single4);
						float single19 = 0.5f / (single17 + single18);
						float single20 = single14 * single19 * (4f * single16 / single15);
						float single21 = 1f - single16;
						single21 = single21 * (single21 * single21 * (single21 * single21));
						single7 = single7 + single20 * (1f - single21);
						single8 = single8 + single20 * single21;
					}
				}
				single7 = Mathf.Clamp(single7 * single2, 0f, 1f);
				single8 = Mathf.Clamp(single8 * single2, 0f, 1f);
				int num4 = num3;
				num3 = num4 + 1;
				color[num4] = new Color(single7, single8, 0f, 0f);
				num2++;
			}
		}
		texture2D.SetPixels(color);
		texture2D.Apply(false, !asset);
		return texture2D;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnRuntimeLoad()
	{
		CoreEnvBrdfLut.PrepareTextureForRuntime();
		CoreEnvBrdfLut.UpdateReflProbe();
	}

	private static void PrepareTextureForRuntime()
	{
		if (CoreEnvBrdfLut.runtimeEnvBrdfLut == null)
		{
			CoreEnvBrdfLut.runtimeEnvBrdfLut = CoreEnvBrdfLut.Generate(false);
		}
		Shader.SetGlobalTexture("_EnvBrdfLut", CoreEnvBrdfLut.runtimeEnvBrdfLut);
	}

	private static uint ReverseBits(uint Bits)
	{
		Bits = Bits << 16 | Bits >> 16;
		Bits = (Bits & 16711935) << 8 | (Bits & -16711936) >> 8;
		Bits = (Bits & 252645135) << 4 | (Bits & -252645136) >> 4;
		Bits = (Bits & 858993459) << 2 | (Bits & -858993460) >> 2;
		Bits = (Bits & 1431655765) << 1 | (Bits & -1431655766) >> 1;
		return Bits;
	}

	private static void UpdateReflProbe()
	{
		int num = (int)Mathf.Log((float)RenderSettings.defaultReflectionResolution, 2f) - 1;
		if (Shader.GetGlobalFloat("_ReflProbeMaxMip") != (float)num)
		{
			Shader.SetGlobalFloat("_ReflProbeMaxMip", (float)num);
		}
	}
}