using System;
using UnityEngine;

public class FXAAPostEffectsBase : MonoBehaviour
{
	protected bool supportHDRTextures = true;

	protected bool isSupported = true;

	public FXAAPostEffectsBase()
	{
	}

	private bool CheckResources()
	{
		Debug.LogWarning(string.Concat("CheckResources () for ", this.ToString(), " should be overwritten."));
		return this.isSupported;
	}

	private bool CheckShader(Shader s)
	{
		Debug.Log(string.Concat(new string[] { "The shader ", s.ToString(), " on effect ", this.ToString(), " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package." }));
		if (s.isSupported)
		{
			return false;
		}
		this.NotSupported();
		return false;
	}

	public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log(string.Concat("Missing shader in ", this.ToString()));
			base.enabled = false;
			return null;
		}
		if (s.isSupported && m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (s.isSupported)
		{
			m2Create = new Material(s)
			{
				hideFlags = HideFlags.DontSave
			};
			if (m2Create)
			{
				return m2Create;
			}
			return null;
		}
		this.NotSupported();
		Debug.LogError(string.Concat(new string[] { "The shader ", s.ToString(), " on effect ", this.ToString(), " is not supported on this platform!" }));
		return null;
	}

	private bool CheckSupport()
	{
		return this.CheckSupport(false);
	}

	public bool CheckSupport(bool needDepth)
	{
		this.isSupported = true;
		this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.NotSupported();
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			this.NotSupported();
			return false;
		}
		if (needDepth)
		{
			Camera component = base.GetComponent<Camera>();
			component.depthTextureMode = component.depthTextureMode | DepthTextureMode.Depth;
		}
		return true;
	}

	private bool CheckSupport(bool needDepth, bool needHdr)
	{
		if (!this.CheckSupport(needDepth))
		{
			return false;
		}
		if (!needHdr || this.supportHDRTextures)
		{
			return true;
		}
		this.NotSupported();
		return false;
	}

	private Material CreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log(string.Concat("Missing shader in ", this.ToString()));
			return null;
		}
		if (m2Create && m2Create.shader == s && s.isSupported)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			return null;
		}
		m2Create = new Material(s)
		{
			hideFlags = HideFlags.DontSave
		};
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}

	private void DrawBorder(RenderTexture dest, Material material)
	{
		float single;
		float single1;
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			if (!flag)
			{
				single = 0f;
				single1 = 1f;
			}
			else
			{
				single = 1f;
				single1 = 0f;
			}
			float single2 = 0f;
			float single3 = 0f + 1f / ((float)dest.width * 1f);
			float single4 = 0f;
			float single5 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0f, single);
			GL.Vertex3(single2, single4, 0.1f);
			GL.TexCoord2(1f, single);
			GL.Vertex3(single3, single4, 0.1f);
			GL.TexCoord2(1f, single1);
			GL.Vertex3(single3, single5, 0.1f);
			GL.TexCoord2(0f, single1);
			GL.Vertex3(single2, single5, 0.1f);
			float single6 = 1f - 1f / ((float)dest.width * 1f);
			single3 = 1f;
			single4 = 0f;
			single5 = 1f;
			GL.TexCoord2(0f, single);
			GL.Vertex3(single6, single4, 0.1f);
			GL.TexCoord2(1f, single);
			GL.Vertex3(single3, single4, 0.1f);
			GL.TexCoord2(1f, single1);
			GL.Vertex3(single3, single5, 0.1f);
			GL.TexCoord2(0f, single1);
			GL.Vertex3(single6, single5, 0.1f);
			float single7 = 0f;
			single3 = 1f;
			single4 = 0f;
			single5 = 0f + 1f / ((float)dest.height * 1f);
			GL.TexCoord2(0f, single);
			GL.Vertex3(single7, single4, 0.1f);
			GL.TexCoord2(1f, single);
			GL.Vertex3(single3, single4, 0.1f);
			GL.TexCoord2(1f, single1);
			GL.Vertex3(single3, single5, 0.1f);
			GL.TexCoord2(0f, single1);
			GL.Vertex3(single7, single5, 0.1f);
			float single8 = 0f;
			single3 = 1f;
			single4 = 1f - 1f / ((float)dest.height * 1f);
			single5 = 1f;
			GL.TexCoord2(0f, single);
			GL.Vertex3(single8, single4, 0.1f);
			GL.TexCoord2(1f, single);
			GL.Vertex3(single3, single4, 0.1f);
			GL.TexCoord2(1f, single1);
			GL.Vertex3(single3, single5, 0.1f);
			GL.TexCoord2(0f, single1);
			GL.Vertex3(single8, single5, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	private void NotSupported()
	{
		base.enabled = false;
		this.isSupported = false;
	}

	private void OnEnable()
	{
		this.isSupported = true;
	}

	private void ReportAutoDisable()
	{
		Debug.LogWarning(string.Concat("The image effect ", this.ToString(), " has been disabled as it's not supported on the current platform."));
	}

	private void Start()
	{
		this.CheckResources();
	}
}