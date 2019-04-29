using System;
using UnityEngine;

[AddComponentMenu("KriptoFX/Explosion_Bloom")]
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public class Explosion_Bloom : MonoBehaviour
{
	[SerializeField]
	public Explosion_Bloom.Settings settings = Explosion_Bloom.Settings.defaultSettings;

	[HideInInspector]
	[SerializeField]
	private Shader m_Shader;

	private Material m_Material;

	private const int kMaxIterations = 16;

	private RenderTexture[] m_blurBuffer1 = new RenderTexture[16];

	private RenderTexture[] m_blurBuffer2 = new RenderTexture[16];

	private int m_Threshold;

	private int m_Curve;

	private int m_PrefilterOffs;

	private int m_SampleScale;

	private int m_Intensity;

	private int m_BaseTex;

	public Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				this.m_Material = Explosion_Bloom.CheckShaderAndCreateMaterial(this.shader);
			}
			return this.m_Material;
		}
	}

	public Shader shader
	{
		get
		{
			if (this.m_Shader == null)
			{
				this.m_Shader = Shader.Find("Hidden/KriptoFX/PostEffects/Explosion_Bloom");
			}
			return this.m_Shader;
		}
	}

	public static bool supportsDX11
	{
		get
		{
			if (SystemInfo.graphicsShaderLevel >= 50)
			{
				return SystemInfo.supportsComputeShaders;
			}
			return false;
		}
	}

	public Explosion_Bloom()
	{
	}

	private void Awake()
	{
		this.m_Threshold = Shader.PropertyToID("_Threshold");
		this.m_Curve = Shader.PropertyToID("_Curve");
		this.m_PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");
		this.m_SampleScale = Shader.PropertyToID("_SampleScale");
		this.m_Intensity = Shader.PropertyToID("_Intensity");
		this.m_BaseTex = Shader.PropertyToID("_BaseTex");
	}

	public static Material CheckShaderAndCreateMaterial(Shader s)
	{
		if (s == null || !s.isSupported)
		{
			return null;
		}
		return new Material(s)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
	{
		if (s == null || !s.isSupported)
		{
			Debug.LogWarningFormat("Missing shader for image effect {0}", new object[] { effect });
			return false;
		}
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[] { effect });
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[] { effect });
			return false;
		}
		if (!needHdr || SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			return true;
		}
		Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[] { effect });
		return false;
	}

	private void OnDisable()
	{
		if (this.m_Material != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_Material);
		}
		this.m_Material = null;
	}

	private void OnEnable()
	{
		if (!Explosion_Bloom.IsSupported(this.shader, true, false, this))
		{
			base.enabled = false;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		int num;
		bool flag = Application.isMobilePlatform;
		int num1 = source.width;
		int num2 = source.height;
		if (!this.settings.highQuality)
		{
			num1 /= 2;
			num2 /= 2;
		}
		RenderTextureFormat renderTextureFormat = (flag ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR);
		float single = Mathf.Log((float)num2, 2f) + this.settings.radius - 8f;
		int num3 = (int)single;
		int num4 = Mathf.Clamp(num3, 1, 16);
		float single1 = this.settings.thresholdLinear;
		this.material.SetFloat(this.m_Threshold, single1);
		float single2 = single1 * this.settings.softKnee + 1E-05f;
		Vector3 vector3 = new Vector3(single1 - single2, single2 * 2f, 0.25f / single2);
		this.material.SetVector(this.m_Curve, vector3);
		bool flag1 = (this.settings.highQuality ? false : this.settings.antiFlicker);
		this.material.SetFloat(this.m_PrefilterOffs, (flag1 ? -0.5f : 0f));
		this.material.SetFloat(this.m_SampleScale, 0.5f + single - (float)num3);
		this.material.SetFloat(this.m_Intensity, Mathf.Max(0f, this.settings.intensity));
		RenderTexture temporary = RenderTexture.GetTemporary(num1, num2, 0, renderTextureFormat);
		Graphics.Blit(source, temporary, this.material, (this.settings.antiFlicker ? 1 : 0));
		RenderTexture mBlurBuffer1 = temporary;
		for (int i = 0; i < num4; i++)
		{
			this.m_blurBuffer1[i] = RenderTexture.GetTemporary(mBlurBuffer1.width / 2, mBlurBuffer1.height / 2, 0, renderTextureFormat);
			RenderTexture renderTexture = mBlurBuffer1;
			RenderTexture mBlurBuffer11 = this.m_blurBuffer1[i];
			Material material = this.material;
			if (i == 0)
			{
				num = (this.settings.antiFlicker ? 3 : 2);
			}
			else
			{
				num = 4;
			}
			Graphics.Blit(renderTexture, mBlurBuffer11, material, num);
			mBlurBuffer1 = this.m_blurBuffer1[i];
		}
		for (int j = num4 - 2; j >= 0; j--)
		{
			RenderTexture renderTexture1 = this.m_blurBuffer1[j];
			this.material.SetTexture(this.m_BaseTex, renderTexture1);
			this.m_blurBuffer2[j] = RenderTexture.GetTemporary(renderTexture1.width, renderTexture1.height, 0, renderTextureFormat);
			Graphics.Blit(mBlurBuffer1, this.m_blurBuffer2[j], this.material, (this.settings.highQuality ? 6 : 5));
			mBlurBuffer1 = this.m_blurBuffer2[j];
		}
		int num5 = 7 + (this.settings.highQuality ? 1 : 0);
		this.material.SetTexture(this.m_BaseTex, source);
		Graphics.Blit(mBlurBuffer1, destination, this.material, num5);
		for (int k = 0; k < 16; k++)
		{
			if (this.m_blurBuffer1[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer1[k]);
			}
			if (this.m_blurBuffer2[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer2[k]);
			}
			this.m_blurBuffer1[k] = null;
			this.m_blurBuffer2[k] = null;
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	[Serializable]
	public struct Settings
	{
		[SerializeField]
		[Tooltip("Filters out pixels under this level of brightness.")]
		public float threshold;

		[Range(0f, 1f)]
		[SerializeField]
		[Tooltip("Makes transition between under/over-threshold gradual.")]
		public float softKnee;

		[Range(1f, 7f)]
		[SerializeField]
		[Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
		public float radius;

		[SerializeField]
		[Tooltip("Blend factor of the result image.")]
		public float intensity;

		[SerializeField]
		[Tooltip("Controls filter quality and buffer resolution.")]
		public bool highQuality;

		[SerializeField]
		[Tooltip("Reduces flashing noise with an additional filter.")]
		public bool antiFlicker;

		public static Explosion_Bloom.Settings defaultSettings
		{
			get
			{
				Explosion_Bloom.Settings setting = new Explosion_Bloom.Settings()
				{
					threshold = 2f,
					softKnee = 0f,
					radius = 7f,
					intensity = 0.7f,
					highQuality = true,
					antiFlicker = true
				};
				return setting;
			}
		}

		public float thresholdGamma
		{
			get
			{
				return Mathf.Max(0f, this.threshold);
			}
			set
			{
				this.threshold = value;
			}
		}

		public float thresholdLinear
		{
			get
			{
				return Mathf.GammaToLinearSpace(this.thresholdGamma);
			}
			set
			{
				this.threshold = Mathf.LinearToGammaSpace(value);
			}
		}
	}
}