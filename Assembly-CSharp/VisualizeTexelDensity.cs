using System;
using UnityEngine;

[AddComponentMenu("Rendering/Visualize Texture Density")]
[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class VisualizeTexelDensity : MonoBehaviour
{
	public Shader shader;

	public string shaderTag = "RenderType";

	[Range(1f, 1024f)]
	public int texelsPerMeter = 256;

	[Range(0f, 1f)]
	public float overlayOpacity = 0.5f;

	public bool showHUD = true;

	private Camera mainCamera;

	private bool initialized;

	private int screenWidth;

	private int screenHeight;

	private Camera texelDensityCamera;

	private RenderTexture texelDensityRT;

	private Texture texelDensityGradTex;

	private Material texelDensityOverlayMat;

	private static VisualizeTexelDensity instance;

	public static VisualizeTexelDensity Instance
	{
		get
		{
			return VisualizeTexelDensity.instance;
		}
	}

	static VisualizeTexelDensity()
	{
	}

	public VisualizeTexelDensity()
	{
	}

	private void Awake()
	{
		VisualizeTexelDensity.instance = this;
		this.mainCamera = base.GetComponent<Camera>();
	}

	private bool CheckScreenResized(int width, int height)
	{
		if (this.screenWidth == width && this.screenHeight == height)
		{
			return false;
		}
		this.screenWidth = width;
		this.screenHeight = height;
		return true;
	}

	private void DrawGUIText(float x, float y, Vector2 size, string text, GUIStyle fontStyle)
	{
		fontStyle.normal.textColor = Color.black;
		GUI.Label(new Rect(x - 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y - 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x - 1f, y - 1f, size.x, size.y), text, fontStyle);
		fontStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(x, y, size.x, size.y), text, fontStyle);
	}

	private void LoadResources()
	{
		if (this.texelDensityGradTex == null)
		{
			this.texelDensityGradTex = Resources.Load("TexelDensityGrad") as Texture;
		}
		if (this.texelDensityOverlayMat == null)
		{
			this.texelDensityOverlayMat = new Material(Shader.Find("Hidden/TexelDensityOverlay"))
			{
				hideFlags = HideFlags.DontSave
			};
		}
	}

	private void OnDisable()
	{
		this.SafeDestroyViewTexelDensity();
		this.SafeDestroyViewTexelDensityRT();
		this.initialized = false;
	}

	private void OnEnable()
	{
		this.mainCamera = base.GetComponent<Camera>();
		this.screenWidth = Screen.width;
		this.screenHeight = Screen.height;
		this.LoadResources();
		this.initialized = true;
	}

	private void OnGUI()
	{
		if (this.initialized && this.showHUD)
		{
			string str = "Texels Per Meter";
			string str1 = "0";
			string str2 = this.texelsPerMeter.ToString();
			int num = this.texelsPerMeter << 1;
			string str3 = string.Concat(num.ToString(), "+");
			float single = (float)this.texelDensityGradTex.width;
			float single1 = (float)(this.texelDensityGradTex.height * 2);
			float single2 = (float)((Screen.width - this.texelDensityGradTex.width) / 2);
			float single3 = 32f;
			GL.PushMatrix();
			GL.LoadPixelMatrix(0f, (float)Screen.width, (float)Screen.height, 0f);
			Graphics.DrawTexture(new Rect(single2 - 2f, single3 - 2f, single + 4f, single1 + 4f), Texture2D.whiteTexture);
			Graphics.DrawTexture(new Rect(single2, single3, single, single1), this.texelDensityGradTex);
			GL.PopMatrix();
			GUIStyle gUIStyle = new GUIStyle()
			{
				fontSize = 13
			};
			Vector2 vector2 = gUIStyle.CalcSize(new GUIContent(str));
			Vector2 vector21 = gUIStyle.CalcSize(new GUIContent(str1));
			Vector2 vector22 = gUIStyle.CalcSize(new GUIContent(str2));
			Vector2 vector23 = gUIStyle.CalcSize(new GUIContent(str3));
			this.DrawGUIText(((float)Screen.width - vector2.x) / 2f, single3 - vector2.y - 5f, vector2, str, gUIStyle);
			this.DrawGUIText(single2, single3 + single1 + 6f, vector21, str1, gUIStyle);
			this.DrawGUIText(((float)Screen.width - vector22.x) / 2f, single3 + single1 + 6f, vector22, str2, gUIStyle);
			this.DrawGUIText(single2 + single - vector23.x, single3 + single1 + 6f, vector23, str3, gUIStyle);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.initialized)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.UpdateViewTexelDensity(this.CheckScreenResized(source.width, source.height));
		this.texelDensityCamera.Render();
		this.texelDensityOverlayMat.SetTexture("_TexelDensityMap", this.texelDensityRT);
		this.texelDensityOverlayMat.SetFloat("_Opacity", this.overlayOpacity);
		Graphics.Blit(source, destination, this.texelDensityOverlayMat, 0);
	}

	private void SafeDestroyViewTexelDensity()
	{
		if (this.texelDensityCamera != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texelDensityCamera.gameObject);
			this.texelDensityCamera = null;
		}
		if (this.texelDensityGradTex != null)
		{
			Resources.UnloadAsset(this.texelDensityGradTex);
			this.texelDensityGradTex = null;
		}
		if (this.texelDensityOverlayMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texelDensityOverlayMat);
			this.texelDensityOverlayMat = null;
		}
	}

	private void SafeDestroyViewTexelDensityRT()
	{
		if (this.texelDensityRT != null)
		{
			Graphics.SetRenderTarget(null);
			this.texelDensityRT.Release();
			UnityEngine.Object.DestroyImmediate(this.texelDensityRT);
			this.texelDensityRT = null;
		}
	}

	private void UpdateViewTexelDensity(bool screenResized)
	{
		if (this.texelDensityCamera == null)
		{
			GameObject gameObject = new GameObject("Texel Density Camera", new Type[] { typeof(Camera) })
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			gameObject.transform.parent = this.mainCamera.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			this.texelDensityCamera = gameObject.GetComponent<Camera>();
			this.texelDensityCamera.CopyFrom(this.mainCamera);
			this.texelDensityCamera.renderingPath = RenderingPath.Forward;
			this.texelDensityCamera.allowMSAA = false;
			this.texelDensityCamera.allowHDR = false;
			this.texelDensityCamera.clearFlags = CameraClearFlags.Skybox;
			this.texelDensityCamera.depthTextureMode = DepthTextureMode.None;
			this.texelDensityCamera.SetReplacementShader(this.shader, this.shaderTag);
			this.texelDensityCamera.enabled = false;
		}
		if ((this.texelDensityRT == null) | screenResized || !this.texelDensityRT.IsCreated())
		{
			this.texelDensityCamera.targetTexture = null;
			this.SafeDestroyViewTexelDensityRT();
			this.texelDensityRT = new RenderTexture(this.screenWidth, this.screenHeight, 24, RenderTextureFormat.ARGB32)
			{
				hideFlags = HideFlags.DontSave,
				name = "TexelDensityRT",
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp
			};
			this.texelDensityRT.Create();
		}
		if (this.texelDensityCamera.targetTexture != this.texelDensityRT)
		{
			this.texelDensityCamera.targetTexture = this.texelDensityRT;
		}
		Shader.SetGlobalFloat("global_TexelsPerMeter", (float)this.texelsPerMeter);
		Shader.SetGlobalTexture("global_TexelDensityGrad", this.texelDensityGradTex);
		this.texelDensityCamera.fieldOfView = this.mainCamera.fieldOfView;
		this.texelDensityCamera.nearClipPlane = this.mainCamera.nearClipPlane;
		this.texelDensityCamera.farClipPlane = this.mainCamera.farClipPlane;
		this.texelDensityCamera.cullingMask = this.mainCamera.cullingMask;
	}
}