using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ReflectionProbeEx : MonoBehaviour
{
	public ReflectionProbeRefreshMode refreshMode = ReflectionProbeRefreshMode.EveryFrame;

	public bool timeSlicing;

	public int resolution = 128;

	[InspectorName("HDR")]
	public bool hdr = true;

	public float shadowDistance;

	public ReflectionProbeClearFlags clearFlags = ReflectionProbeClearFlags.Skybox;

	public Color background = new Color(0.192f, 0.301f, 0.474f);

	public float nearClip = 0.3f;

	public float farClip = 1000f;

	public Transform attachToTarget;

	public Light directionalLight;

	public float textureMipBias = 2f;

	public bool highPrecision;

	public bool enableShadows;

	public ReflectionProbeEx.ConvolutionQuality convolutionQuality;

	public List<ReflectionProbeEx.RenderListEntry> staticRenderList = new List<ReflectionProbeEx.RenderListEntry>();

	public Cubemap reflectionCubemap;

	public float reflectionIntensity = 1f;

	private Mesh blitMesh;

	private Mesh skyboxMesh;

	private static float[] octaVerts;

	private readonly static ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatrices;

	private readonly static ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatricesD3D11;

	private readonly static ReflectionProbeEx.CubemapFaceMatrices[] shadowCubemapFaceMatrices;

	private ReflectionProbeEx.CubemapFaceMatrices[] platformCubemapFaceMatrices;

	private readonly static int[] tab32;

	static ReflectionProbeEx()
	{
		ReflectionProbeEx.octaVerts = new float[] { 0f, 1f, 0f, 0f, 0f, -1f, 1f, 0f, 0f, 0f, 1f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 1f, 0f, 0f, 0f, 1f, -1f, 0f, 0f, 0f, 1f, 0f, -1f, 0f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 1f, 0f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 0f, 0f, 1f, 1f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 0f, 0f, 0f, 1f, 0f, -1f, 0f, 0f, 0f, -1f, -1f, 0f, 0f };
		ReflectionProbeEx.cubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[] { new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f)) };
		ReflectionProbeEx.cubemapFaceMatricesD3D11 = new ReflectionProbeEx.CubemapFaceMatrices[] { new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), new Vector3(-1f, 0f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(1f, 0f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f)) };
		ReflectionProbeEx.shadowCubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[] { new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f)), new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f)) };
		ReflectionProbeEx.tab32 = new int[] { 0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30, 8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31 };
	}

	public ReflectionProbeEx()
	{
	}

	private static Mesh CreateBlitMesh()
	{
		Mesh mesh = new Mesh()
		{
			vertices = new Vector3[] { new Vector3(-1f, -1f, 0f), new Vector3(-1f, 1f, 0f), new Vector3(1f, 1f, 0f), new Vector3(1f, -1f, 0f) },
			uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f) },
			triangles = new int[] { 0, 1, 2, 0, 2, 3 }
		};
		return mesh;
	}

	private void CreateMeshes()
	{
		if (this.blitMesh == null)
		{
			this.blitMesh = ReflectionProbeEx.CreateBlitMesh();
		}
		if (this.skyboxMesh == null)
		{
			this.skyboxMesh = ReflectionProbeEx.CreateSkyboxMesh();
		}
	}

	private static Mesh CreateSkyboxMesh()
	{
		List<ReflectionProbeEx.CubemapSkyboxVertex> cubemapSkyboxVertices = new List<ReflectionProbeEx.CubemapSkyboxVertex>();
		for (int i = 0; i < 24; i++)
		{
			ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex = new ReflectionProbeEx.CubemapSkyboxVertex();
			Vector3 vector3 = Vector3.Normalize(new Vector3(ReflectionProbeEx.octaVerts[i * 3], ReflectionProbeEx.octaVerts[i * 3 + 1], ReflectionProbeEx.octaVerts[i * 3 + 2]));
			float single = vector3.x;
			float single1 = single;
			cubemapSkyboxVertex.tu = single;
			cubemapSkyboxVertex.x = single1;
			float single2 = vector3.y;
			single1 = single2;
			cubemapSkyboxVertex.tv = single2;
			cubemapSkyboxVertex.y = single1;
			float single3 = vector3.z;
			single1 = single3;
			cubemapSkyboxVertex.tw = single3;
			cubemapSkyboxVertex.z = single1;
			cubemapSkyboxVertex.color = Color.white;
			cubemapSkyboxVertices.Add(cubemapSkyboxVertex);
		}
		for (int j = 0; j < 3; j++)
		{
			List<ReflectionProbeEx.CubemapSkyboxVertex> cubemapSkyboxVertices1 = new List<ReflectionProbeEx.CubemapSkyboxVertex>(cubemapSkyboxVertices.Count);
			cubemapSkyboxVertices1.AddRange(cubemapSkyboxVertices);
			int count = cubemapSkyboxVertices1.Count;
			cubemapSkyboxVertices.Clear();
			cubemapSkyboxVertices.Capacity = count * 4;
			for (int k = 0; k < count; k += 3)
			{
				ReflectionProbeEx.Subdivide(cubemapSkyboxVertices, cubemapSkyboxVertices1[k], cubemapSkyboxVertices1[k + 1], cubemapSkyboxVertices1[k + 2]);
			}
		}
		for (int l = 0; l < 2; l++)
		{
			List<ReflectionProbeEx.CubemapSkyboxVertex> cubemapSkyboxVertices2 = new List<ReflectionProbeEx.CubemapSkyboxVertex>(cubemapSkyboxVertices.Count);
			cubemapSkyboxVertices2.AddRange(cubemapSkyboxVertices);
			int num = cubemapSkyboxVertices2.Count;
			float single4 = Mathf.Pow(0.5f, (float)l + 1f);
			cubemapSkyboxVertices.Clear();
			cubemapSkyboxVertices.Capacity = num * 4;
			for (int m = 0; m < num; m += 3)
			{
				if (Mathf.Max(Mathf.Max(Mathf.Abs(cubemapSkyboxVertices2[m].y), Mathf.Abs(cubemapSkyboxVertices2[m + 1].y)), Mathf.Abs(cubemapSkyboxVertices2[m + 2].y)) <= single4)
				{
					ReflectionProbeEx.SubdivideYOnly(cubemapSkyboxVertices, cubemapSkyboxVertices2[m], cubemapSkyboxVertices2[m + 1], cubemapSkyboxVertices2[m + 2]);
				}
				else
				{
					cubemapSkyboxVertices.Add(cubemapSkyboxVertices2[m]);
					cubemapSkyboxVertices.Add(cubemapSkyboxVertices2[m + 1]);
					cubemapSkyboxVertices.Add(cubemapSkyboxVertices2[m + 2]);
				}
			}
		}
		Mesh mesh = new Mesh();
		Vector3[] vector3Array = new Vector3[cubemapSkyboxVertices.Count];
		Vector2[] vector2Array = new Vector2[cubemapSkyboxVertices.Count];
		int[] numArray = new int[cubemapSkyboxVertices.Count];
		for (int n = 0; n < cubemapSkyboxVertices.Count; n++)
		{
			vector3Array[n] = new Vector3(cubemapSkyboxVertices[n].x, cubemapSkyboxVertices[n].y, cubemapSkyboxVertices[n].z);
			vector2Array[n] = new Vector3(cubemapSkyboxVertices[n].tu, cubemapSkyboxVertices[n].tv);
			numArray[n] = n;
		}
		mesh.vertices = vector3Array;
		mesh.uv = vector2Array;
		mesh.triangles = numArray;
		return mesh;
	}

	private void DestroyMeshes()
	{
		if (this.blitMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.blitMesh);
			this.blitMesh = null;
		}
		if (this.skyboxMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.skyboxMesh);
			this.skyboxMesh = null;
		}
	}

	private int FastLog2(int value)
	{
		value = value | value >> 1;
		value = value | value >> 2;
		value = value | value >> 4;
		value = value | value >> 8;
		value = value | value >> 16;
		return ReflectionProbeEx.tab32[(uint)((long)value * (long)130329821) >> 27];
	}

	private bool InitializeCubemapFaceMatrices()
	{
		GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
		if (graphicsDeviceType == GraphicsDeviceType.Direct3D11)
		{
			this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
		}
		else
		{
			switch (graphicsDeviceType)
			{
				case GraphicsDeviceType.Metal:
				{
					this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
					break;
				}
				case GraphicsDeviceType.OpenGLCore:
				{
					this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatrices;
					break;
				}
				case GraphicsDeviceType.Direct3D12:
				{
					this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
					break;
				}
				case GraphicsDeviceType.N3DS:
				case GraphicsDeviceType.Null | GraphicsDeviceType.Metal:
				{
					this.platformCubemapFaceMatrices = null;
					break;
				}
				case GraphicsDeviceType.Vulkan:
				{
					this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
					break;
				}
				default:
				{
					goto case GraphicsDeviceType.Null | GraphicsDeviceType.Metal;
				}
			}
		}
		if (this.platformCubemapFaceMatrices != null)
		{
			return true;
		}
		Debug.LogError(string.Concat("[ReflectionProbeEx] Initialization failed. No cubemap ortho basis defined for ", SystemInfo.graphicsDeviceType));
		return false;
	}

	private uint ReverseBits(uint bits)
	{
		bits = bits << 16 | bits >> 16;
		bits = (bits & 16711935) << 8 | (bits & -16711936) >> 8;
		bits = (bits & 252645135) << 4 | (bits & -252645136) >> 4;
		bits = (bits & 858993459) << 2 | (bits & -858993460) >> 2;
		bits = (bits & 1431655765) << 1 | (bits & -1431655766) >> 1;
		return bits;
	}

	private void SafeCreateCB(ref CommandBuffer cb, string name)
	{
		if (cb == null)
		{
			cb = new CommandBuffer()
			{
				name = name
			};
		}
	}

	private void SafeCreateCubeRT(ref RenderTexture rt, string name, int size, int depth, bool mips, TextureDimension dim, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite = 1)
	{
		if (rt == null || !rt.IsCreated())
		{
			this.SafeDestroy<RenderTexture>(ref rt);
			rt = new RenderTexture(size, size, depth, format, readWrite)
			{
				hideFlags = HideFlags.DontSave,
				name = name,
				dimension = dim
			};
			if (dim == TextureDimension.Tex2DArray)
			{
				rt.volumeDepth = 6;
			}
			rt.useMipMap = mips;
			rt.autoGenerateMips = false;
			rt.filterMode = filter;
			rt.anisoLevel = 0;
			rt.Create();
		}
	}

	private void SafeCreateMaterial(ref Material mat, Shader shader)
	{
		if (mat == null)
		{
			mat = new Material(shader);
		}
	}

	private void SafeCreateMaterial(ref Material mat, string shaderName)
	{
		if (mat == null)
		{
			this.SafeCreateMaterial(ref mat, Shader.Find(shaderName));
		}
	}

	private void SafeDestroy<T>(ref T obj)
	where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	private void SafeDispose<T>(ref T obj)
	where T : IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}

	private static void Subdivide(List<ReflectionProbeEx.CubemapSkyboxVertex> destArray, ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2, ReflectionProbeEx.CubemapSkyboxVertex v3)
	{
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex = ReflectionProbeEx.SubDivVert(v1, v2);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex1 = ReflectionProbeEx.SubDivVert(v2, v3);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2 = ReflectionProbeEx.SubDivVert(v1, v3);
		destArray.Add(v1);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(v2);
		destArray.Add(cubemapSkyboxVertex1);
		destArray.Add(cubemapSkyboxVertex1);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(v3);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex1);
	}

	private static void SubdivideYOnly(List<ReflectionProbeEx.CubemapSkyboxVertex> destArray, ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2, ReflectionProbeEx.CubemapSkyboxVertex v3)
	{
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex;
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex1;
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2;
		float single = Mathf.Abs(v2.y - v1.y);
		float single1 = Mathf.Abs(v2.y - v3.y);
		float single2 = Mathf.Abs(v3.y - v1.y);
		if (single < single1 && single < single2)
		{
			cubemapSkyboxVertex = v3;
			cubemapSkyboxVertex1 = v1;
			cubemapSkyboxVertex2 = v2;
		}
		else if (single1 >= single || single1 >= single2)
		{
			cubemapSkyboxVertex = v2;
			cubemapSkyboxVertex1 = v3;
			cubemapSkyboxVertex2 = v1;
		}
		else
		{
			cubemapSkyboxVertex = v1;
			cubemapSkyboxVertex1 = v2;
			cubemapSkyboxVertex2 = v3;
		}
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex3 = ReflectionProbeEx.SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex1);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex4 = ReflectionProbeEx.SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex4);
		Vector3 vector3 = new Vector3(cubemapSkyboxVertex4.x - cubemapSkyboxVertex1.x, cubemapSkyboxVertex4.y - cubemapSkyboxVertex1.y, cubemapSkyboxVertex4.z - cubemapSkyboxVertex1.z);
		Vector3 vector31 = new Vector3(cubemapSkyboxVertex3.x - cubemapSkyboxVertex2.x, cubemapSkyboxVertex3.y - cubemapSkyboxVertex2.y, cubemapSkyboxVertex3.z - cubemapSkyboxVertex2.z);
		if (vector3.x * vector3.x + vector3.y * vector3.y + vector3.z * vector3.z > vector31.x * vector31.x + vector31.y * vector31.y + vector31.z * vector31.z)
		{
			destArray.Add(cubemapSkyboxVertex3);
			destArray.Add(cubemapSkyboxVertex1);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(cubemapSkyboxVertex4);
			destArray.Add(cubemapSkyboxVertex3);
			destArray.Add(cubemapSkyboxVertex2);
			return;
		}
		destArray.Add(cubemapSkyboxVertex4);
		destArray.Add(cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex1);
		destArray.Add(cubemapSkyboxVertex4);
		destArray.Add(cubemapSkyboxVertex1);
		destArray.Add(cubemapSkyboxVertex2);
	}

	private static ReflectionProbeEx.CubemapSkyboxVertex SubDivVert(ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2)
	{
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex = new ReflectionProbeEx.CubemapSkyboxVertex();
		Vector3 vector3 = new Vector3(v1.x, v1.y, v1.z);
		Vector3 vector31 = new Vector3(v2.x, v2.y, v2.z);
		Vector3 vector32 = Vector3.Normalize(Vector3.Lerp(vector3, vector31, 0.5f));
		float single = vector32.x;
		float single1 = single;
		cubemapSkyboxVertex.tu = single;
		cubemapSkyboxVertex.x = single1;
		float single2 = vector32.y;
		single1 = single2;
		cubemapSkyboxVertex.tv = single2;
		cubemapSkyboxVertex.y = single1;
		float single3 = vector32.z;
		single1 = single3;
		cubemapSkyboxVertex.tw = single3;
		cubemapSkyboxVertex.z = single1;
		cubemapSkyboxVertex.color = Color.white;
		return cubemapSkyboxVertex;
	}

	[Serializable]
	public enum ConvolutionQuality
	{
		Lowest,
		Low,
		Medium,
		High,
		VeryHigh
	}

	private struct CubemapFaceMatrices
	{
		public Matrix4x4 worldToView;

		public Matrix4x4 viewToWorld;

		public CubemapFaceMatrices(Vector3 x, Vector3 y, Vector3 z)
		{
			this.worldToView = Matrix4x4.identity;
			this.worldToView[0, 0] = x[0];
			this.worldToView[0, 1] = x[1];
			this.worldToView[0, 2] = x[2];
			this.worldToView[1, 0] = y[0];
			this.worldToView[1, 1] = y[1];
			this.worldToView[1, 2] = y[2];
			this.worldToView[2, 0] = z[0];
			this.worldToView[2, 1] = z[1];
			this.worldToView[2, 2] = z[2];
			this.viewToWorld = this.worldToView.inverse;
		}
	}

	private struct CubemapSkyboxVertex
	{
		public float x;

		public float y;

		public float z;

		public Color color;

		public float tu;

		public float tv;

		public float tw;
	}

	[Serializable]
	public struct RenderListEntry
	{
		public Renderer renderer;

		public bool alwaysEnabled;

		public RenderListEntry(Renderer renderer, bool alwaysEnabled)
		{
			this.renderer = renderer;
			this.alwaysEnabled = alwaysEnabled;
		}
	}
}