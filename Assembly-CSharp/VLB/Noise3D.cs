using System;
using UnityEngine;

namespace VLB
{
	public static class Noise3D
	{
		private static bool ms_IsSupportedChecked;

		private static bool ms_IsSupported;

		private static Texture3D ms_NoiseTexture;

		private const HideFlags kHideFlags = HideFlags.HideAndDontSave;

		private const int kMinShaderLevel = 35;

		public static string isNotSupportedString
		{
			get
			{
				return string.Format("3D Noise requires higher shader capabilities (Shader Model 3.5 / OpenGL ES 3.0), which are not available on the current platform: graphicsShaderLevel (current/required) = {0} / {1}", SystemInfo.graphicsShaderLevel, 35);
			}
		}

		public static bool isProperlyLoaded
		{
			get
			{
				return Noise3D.ms_NoiseTexture != null;
			}
		}

		public static bool isSupported
		{
			get
			{
				if (!Noise3D.ms_IsSupportedChecked)
				{
					Noise3D.ms_IsSupported = SystemInfo.graphicsShaderLevel >= 35;
					if (!Noise3D.ms_IsSupported)
					{
						Debug.LogWarning(Noise3D.isNotSupportedString);
					}
					Noise3D.ms_IsSupportedChecked = true;
				}
				return Noise3D.ms_IsSupported;
			}
		}

		static Noise3D()
		{
		}

		public static void LoadIfNeeded()
		{
			if (!Noise3D.isSupported)
			{
				return;
			}
			if (Noise3D.ms_NoiseTexture == null)
			{
				Noise3D.ms_NoiseTexture = Noise3D.LoadTexture3D(Config.Instance.noise3DData, Config.Instance.noise3DSize);
				if (Noise3D.ms_NoiseTexture)
				{
					Noise3D.ms_NoiseTexture.hideFlags = HideFlags.HideAndDontSave;
				}
			}
			Shader.SetGlobalTexture("_VLB_NoiseTex3D", Noise3D.ms_NoiseTexture);
			Shader.SetGlobalVector("_VLB_NoiseGlobal", Config.Instance.globalNoiseParam);
		}

		private static Texture3D LoadTexture3D(TextAsset textData, int size)
		{
			if (textData == null)
			{
				Debug.LogErrorFormat("Fail to open Noise 3D Data", Array.Empty<object>());
				return null;
			}
			byte[] numArray = textData.bytes;
			Debug.Assert(numArray != null);
			int num = Mathf.Max(0, size * size * size);
			if ((int)numArray.Length != num)
			{
				Debug.LogErrorFormat("Noise 3D Data file has not the proper size {0}x{0}x{0}", new object[] { size });
				return null;
			}
			Texture3D texture3D = new Texture3D(size, size, size, TextureFormat.Alpha8, false);
			Color[] color32 = new Color[num];
			for (int i = 0; i < num; i++)
			{
				color32[i] = new Color32(0, 0, 0, numArray[i]);
			}
			texture3D.SetPixels(color32);
			texture3D.Apply();
			return texture3D;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void OnStartUp()
		{
			Noise3D.LoadIfNeeded();
		}
	}
}