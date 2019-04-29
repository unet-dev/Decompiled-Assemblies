using RustNative;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class OcclusionCulling : MonoBehaviour
{
	public ComputeShader computeShader;

	public bool usePixelShaderFallback = true;

	public bool useAsyncReadAPI;

	private Camera camera;

	private const int ComputeThreadsPerGroup = 64;

	private const int InputBufferStride = 16;

	private const int ResultBufferStride = 4;

	private const int OccludeeMaxSlotsPerPool = 1048576;

	private const int OccludeePoolGranularity = 2048;

	private const int StateBufferGranularity = 2048;

	private const int GridBufferGranularity = 256;

	private static Queue<OccludeeState> statePool;

	private static OcclusionCulling.SimpleList<OccludeeState> staticOccludees;

	private static OcclusionCulling.SimpleList<OccludeeState.State> staticStates;

	private static OcclusionCulling.SimpleList<int> staticVisibilityChanged;

	private static OcclusionCulling.SimpleList<OccludeeState> dynamicOccludees;

	private static OcclusionCulling.SimpleList<OccludeeState.State> dynamicStates;

	private static OcclusionCulling.SimpleList<int> dynamicVisibilityChanged;

	private static List<int> staticChanged;

	private static Queue<int> staticRecycled;

	private static List<int> dynamicChanged;

	private static Queue<int> dynamicRecycled;

	private static OcclusionCulling.BufferSet staticSet;

	private static OcclusionCulling.BufferSet dynamicSet;

	private static OcclusionCulling.BufferSet gridSet;

	private Vector4[] frustumPlanes = new Vector4[6];

	private string[] frustumPropNames = new string[6];

	private float[] matrixToFloatTemp = new float[16];

	private Material fallbackMat;

	private Material depthCopyMat;

	private Matrix4x4 viewMatrix;

	private Matrix4x4 projMatrix;

	private Matrix4x4 viewProjMatrix;

	private Matrix4x4 prevViewProjMatrix;

	private Matrix4x4 invViewProjMatrix;

	private bool useNativePath = true;

	private static OcclusionCulling instance;

	private static GraphicsDeviceType[] supportedDeviceTypes;

	private static bool _enabled;

	private static bool _safeMode;

	private static OcclusionCulling.DebugFilter _debugShow;

	public OcclusionCulling.DebugSettings debugSettings = new OcclusionCulling.DebugSettings();

	private Material debugMipMat;

	private const float debugDrawDuration = 0.0334f;

	private Material downscaleMat;

	private Material blitCopyMat;

	private int hiZLevelCount;

	private int hiZWidth;

	private int hiZHeight;

	private RenderTexture depthTexture;

	private RenderTexture hiZTexture;

	private RenderTexture[] hiZLevels;

	private const int GridCellsPerAxis = 2097152;

	private const int GridHalfCellsPerAxis = 1048576;

	private const int GridMinHalfCellsPerAxis = -1048575;

	private const int GridMaxHalfCellsPerAxis = 1048575;

	private const float GridCellSize = 100f;

	private const float GridHalfCellSize = 50f;

	private const float GridRcpCellSize = 0.01f;

	private const int GridPoolCapacity = 16384;

	private const int GridPoolGranularity = 4096;

	private static OcclusionCulling.HashedPool<OcclusionCulling.Cell> grid;

	private static Queue<OcclusionCulling.Cell> gridChanged;

	public static OcclusionCulling.DebugFilter DebugShow
	{
		get
		{
			return OcclusionCulling._debugShow;
		}
		set
		{
			OcclusionCulling._debugShow = value;
		}
	}

	public static bool Enabled
	{
		get
		{
			return OcclusionCulling._enabled;
		}
		set
		{
			OcclusionCulling._enabled = value;
			if (OcclusionCulling.instance != null)
			{
				OcclusionCulling.instance.enabled = value;
			}
		}
	}

	public bool HiZReady
	{
		get
		{
			if (!(this.hiZTexture != null) || this.hiZWidth <= 0)
			{
				return false;
			}
			return this.hiZHeight > 0;
		}
	}

	public static OcclusionCulling Instance
	{
		get
		{
			return OcclusionCulling.instance;
		}
	}

	public static bool SafeMode
	{
		get
		{
			return OcclusionCulling._safeMode;
		}
		set
		{
			OcclusionCulling._safeMode = value;
		}
	}

	public static bool Supported
	{
		get
		{
			return OcclusionCulling.supportedDeviceTypes.Contains<GraphicsDeviceType>(SystemInfo.graphicsDeviceType);
		}
	}

	static OcclusionCulling()
	{
		OcclusionCulling.statePool = new Queue<OccludeeState>();
		OcclusionCulling.staticOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);
		OcclusionCulling.staticStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);
		OcclusionCulling.staticVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);
		OcclusionCulling.dynamicOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);
		OcclusionCulling.dynamicStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);
		OcclusionCulling.dynamicVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);
		OcclusionCulling.staticChanged = new List<int>(256);
		OcclusionCulling.staticRecycled = new Queue<int>();
		OcclusionCulling.dynamicChanged = new List<int>(1024);
		OcclusionCulling.dynamicRecycled = new Queue<int>();
		OcclusionCulling.staticSet = new OcclusionCulling.BufferSet();
		OcclusionCulling.dynamicSet = new OcclusionCulling.BufferSet();
		OcclusionCulling.gridSet = new OcclusionCulling.BufferSet();
		OcclusionCulling.supportedDeviceTypes = new GraphicsDeviceType[] { typeof(<PrivateImplementationDetails>).GetField("82BEEBBCC8BD8731B58FAE2B7BF8B77906BBF7D2").FieldHandle };
		OcclusionCulling._enabled = false;
		OcclusionCulling._safeMode = false;
		OcclusionCulling._debugShow = OcclusionCulling.DebugFilter.Off;
		OcclusionCulling.grid = new OcclusionCulling.HashedPool<OcclusionCulling.Cell>(16384, 4096);
		OcclusionCulling.gridChanged = new Queue<OcclusionCulling.Cell>();
	}

	public OcclusionCulling()
	{
	}

	private static OccludeeState Allocate()
	{
		if (OcclusionCulling.statePool.Count == 0)
		{
			OcclusionCulling.GrowStatePool();
		}
		return OcclusionCulling.statePool.Dequeue();
	}

	private void ApplyVisibility_Fast(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool flag = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell item = OcclusionCulling.grid[i];
			if (item != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag1 = OcclusionCulling.FrustumCull(this.frustumPlanes, item.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 & flag1;
				if (item.isVisible | flag2)
				{
					int num = 0;
					int num1 = 0;
					if (ready && item.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.staticStates.array, item.staticBucket.Slots, item.staticBucket.Size, OcclusionCulling.staticSet.resultData, (int)OcclusionCulling.staticSet.resultData.Length, OcclusionCulling.staticVisibilityChanged.array, ref OcclusionCulling.staticVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					if (flag && item.dynamicBucket.Count > 0)
					{
						num1 = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.dynamicStates.array, item.dynamicBucket.Slots, item.dynamicBucket.Size, OcclusionCulling.dynamicSet.resultData, (int)OcclusionCulling.dynamicSet.resultData.Length, OcclusionCulling.dynamicVisibilityChanged.array, ref OcclusionCulling.dynamicVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					item.isVisible = (flag2 || num < item.staticBucket.Count ? true : num1 < item.dynamicBucket.Count);
				}
			}
		}
	}

	private void ApplyVisibility_Native(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool flag = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell item = OcclusionCulling.grid[i];
			if (item != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag1 = OcclusionCulling.FrustumCull(this.frustumPlanes, item.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 & flag1;
				if (item.isVisible | flag2)
				{
					int num = 0;
					int num1 = 0;
					if (ready && item.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.staticStates.array[0], ref item.staticBucket.Slots[0], item.staticBucket.Size, ref OcclusionCulling.staticSet.resultData[0], (int)OcclusionCulling.staticSet.resultData.Length, ref OcclusionCulling.staticVisibilityChanged.array[0], ref OcclusionCulling.staticVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					if (flag && item.dynamicBucket.Count > 0)
					{
						num1 = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.dynamicStates.array[0], ref item.dynamicBucket.Slots[0], item.dynamicBucket.Size, ref OcclusionCulling.dynamicSet.resultData[0], (int)OcclusionCulling.dynamicSet.resultData.Length, ref OcclusionCulling.dynamicVisibilityChanged.array[0], ref OcclusionCulling.dynamicVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					item.isVisible = (flag2 || num < item.staticBucket.Count ? true : num1 < item.dynamicBucket.Count);
				}
			}
		}
	}

	private void ApplyVisibility_Safe(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool flag = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell item = OcclusionCulling.grid[i];
			if (item != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag1 = OcclusionCulling.FrustumCull(this.frustumPlanes, item.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 & flag1;
				if (item.isVisible | flag2)
				{
					int num = 0;
					int num1 = 0;
					if (ready && item.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.staticStates, item.staticBucket, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticVisibilityChanged, this.frustumPlanes, time, frame);
					}
					if (flag && item.dynamicBucket.Count > 0)
					{
						num1 = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.dynamicStates, item.dynamicBucket, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicVisibilityChanged, this.frustumPlanes, time, frame);
					}
					item.isVisible = (flag2 || num < item.staticBucket.Count ? true : num1 < item.dynamicBucket.Count);
				}
			}
		}
	}

	private void Awake()
	{
		OcclusionCulling.instance = this;
		this.camera = base.GetComponent<Camera>();
		for (int i = 0; i < 6; i++)
		{
			this.frustumPropNames[i] = string.Concat("_FrustumPlane", i);
		}
	}

	public void CheckResizeHiZMap()
	{
		int num = this.camera.pixelWidth;
		int num1 = this.camera.pixelHeight;
		if (num > 0 && num1 > 0)
		{
			int num2 = num / 4;
			int num3 = num1 / 4;
			if (this.hiZLevels == null || this.hiZWidth != num2 || this.hiZHeight != num3)
			{
				this.InitializeHiZMap(num2, num3);
				this.hiZWidth = num2;
				this.hiZHeight = num3;
				if (this.debugSettings.log)
				{
					Debug.Log(string.Concat(new object[] { "[OcclusionCulling] Resized HiZ Map to ", this.hiZWidth, " x ", this.hiZHeight }));
				}
			}
		}
	}

	private RenderTexture CreateDepthTexture(string name, int width, int height, bool mips = false)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear)
		{
			name = name,
			useMipMap = mips,
			autoGenerateMips = false,
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		renderTexture.Create();
		return renderTexture;
	}

	private RenderTexture CreateDepthTextureMip(string name, int width, int height, int mip)
	{
		int num = width >> (mip & 31);
		int num1 = height >> (mip & 31);
		RenderTexture renderTexture = new RenderTexture(num, num1, (mip == 0 ? 24 : 0), RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear)
		{
			name = name,
			useMipMap = false,
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		renderTexture.Create();
		return renderTexture;
	}

	private void DebugDraw()
	{
	}

	private void DebugDrawGizmos()
	{
		Camera component = base.GetComponent<Camera>();
		Gizmos.color = new Color(0.75f, 0.75f, 0f, 0.5f);
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		Gizmos.DrawFrustum(Vector3.zero, component.fieldOfView, component.farClipPlane, component.nearClipPlane, component.aspect);
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.identity;
		Matrix4x4 matrix4x4 = component.worldToCameraMatrix;
		Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(component.projectionMatrix, false) * matrix4x4;
		Vector4[] vector4Array = new Vector4[6];
		OcclusionCulling.ExtractFrustum(gPUProjectionMatrix, ref vector4Array);
		for (int i = 0; i < (int)vector4Array.Length; i++)
		{
			Vector3 vector3 = -new Vector3(vector4Array[i].x, vector4Array[i].y, vector4Array[i].z) * vector4Array[i].w;
			Gizmos.DrawLine(vector3, vector3 * 2f);
		}
	}

	public static bool DebugFilterIsDynamic(int filter)
	{
		if (filter == 1)
		{
			return true;
		}
		return filter == 4;
	}

	public static bool DebugFilterIsGrid(int filter)
	{
		if (filter == 3)
		{
			return true;
		}
		return filter == 4;
	}

	public static bool DebugFilterIsStatic(int filter)
	{
		if (filter == 2)
		{
			return true;
		}
		return filter == 4;
	}

	private void DebugInitialize()
	{
		this.debugMipMat = new Material(Shader.Find("Hidden/OcclusionCulling/DebugMip"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	private void DebugShutdown()
	{
		if (this.debugMipMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.debugMipMat);
			this.debugMipMat = null;
		}
	}

	private void DebugUpdate()
	{
		if (this.HiZReady)
		{
			this.debugSettings.showMainLod = Mathf.Clamp(this.debugSettings.showMainLod, 0, (int)this.hiZLevels.Length - 1);
		}
	}

	private void DestroyHiZMap()
	{
		if (this.depthTexture != null)
		{
			RenderTexture.active = null;
			UnityEngine.Object.DestroyImmediate(this.depthTexture);
			this.depthTexture = null;
		}
		if (this.hiZTexture != null)
		{
			RenderTexture.active = null;
			UnityEngine.Object.DestroyImmediate(this.hiZTexture);
			this.hiZTexture = null;
		}
		if (this.hiZLevels != null)
		{
			for (int i = 0; i < (int)this.hiZLevels.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(this.hiZLevels[i]);
			}
			this.hiZLevels = null;
		}
	}

	public static void ExtractFrustum(Matrix4x4 viewProjMatrix, ref Vector4[] planes)
	{
		planes[0].x = viewProjMatrix.m30 + viewProjMatrix.m00;
		planes[0].y = viewProjMatrix.m31 + viewProjMatrix.m01;
		planes[0].z = viewProjMatrix.m32 + viewProjMatrix.m02;
		planes[0].w = viewProjMatrix.m33 + viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[0]);
		planes[1].x = viewProjMatrix.m30 - viewProjMatrix.m00;
		planes[1].y = viewProjMatrix.m31 - viewProjMatrix.m01;
		planes[1].z = viewProjMatrix.m32 - viewProjMatrix.m02;
		planes[1].w = viewProjMatrix.m33 - viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[1]);
		planes[2].x = viewProjMatrix.m30 - viewProjMatrix.m10;
		planes[2].y = viewProjMatrix.m31 - viewProjMatrix.m11;
		planes[2].z = viewProjMatrix.m32 - viewProjMatrix.m12;
		planes[2].w = viewProjMatrix.m33 - viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[2]);
		planes[3].x = viewProjMatrix.m30 + viewProjMatrix.m10;
		planes[3].y = viewProjMatrix.m31 + viewProjMatrix.m11;
		planes[3].z = viewProjMatrix.m32 + viewProjMatrix.m12;
		planes[3].w = viewProjMatrix.m33 + viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[3]);
		planes[4].x = viewProjMatrix.m20;
		planes[4].y = viewProjMatrix.m21;
		planes[4].z = viewProjMatrix.m22;
		planes[4].w = viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[4]);
		planes[5].x = viewProjMatrix.m30 - viewProjMatrix.m20;
		planes[5].y = viewProjMatrix.m31 - viewProjMatrix.m21;
		planes[5].z = viewProjMatrix.m32 - viewProjMatrix.m22;
		planes[5].w = viewProjMatrix.m33 - viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[5]);
	}

	private void FinalizeHiZMap()
	{
		this.DestroyHiZMap();
		if (this.downscaleMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.downscaleMat);
			this.downscaleMat = null;
		}
		if (this.blitCopyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.blitCopyMat);
			this.blitCopyMat = null;
		}
	}

	private static int FindFreeSlot(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled)
	{
		int count;
		if (recycled.Count <= 0)
		{
			if (occludees.Count == occludees.Capacity)
			{
				int num = Mathf.Min(occludees.Capacity + 2048, 1048576);
				if (num > 0)
				{
					occludees.Capacity = num;
					states.Capacity = num;
				}
			}
			if (occludees.Count >= occludees.Capacity)
			{
				count = -1;
			}
			else
			{
				count = occludees.Count;
				occludees.Add(null);
				states.Add(new OccludeeState.State());
			}
		}
		else
		{
			count = recycled.Dequeue();
		}
		return count;
	}

	private static int floor(float x)
	{
		int num = (int)x;
		if (x >= (float)num)
		{
			return num;
		}
		return num - 1;
	}

	private static bool FrustumCull(Vector4[] planes, Vector4 testSphere)
	{
		for (int i = 0; i < 6; i++)
		{
			if (planes[i].x * testSphere.x + planes[i].y * testSphere.y + planes[i].z * testSphere.z + planes[i].w < -testSphere.w)
			{
				return false;
			}
		}
		return true;
	}

	public void GenerateHiZMipChain()
	{
		if (this.HiZReady)
		{
			bool flag = true;
			this.depthCopyMat.SetMatrix("_CameraReprojection", this.prevViewProjMatrix * this.invViewProjMatrix);
			this.depthCopyMat.SetFloat("_FrustumNoDataDepth", (flag ? 1f : 0f));
			UnityEngine.Graphics.Blit(this.depthTexture, this.hiZLevels[0], this.depthCopyMat, 1);
			for (int i = 1; i < (int)this.hiZLevels.Length; i++)
			{
				RenderTexture renderTexture = this.hiZLevels[i - 1];
				RenderTexture renderTexture1 = this.hiZLevels[i];
				int num = ((renderTexture.width & 1) != 0 || (renderTexture.height & 1) != 0 ? 1 : 0);
				this.downscaleMat.SetTexture("_MainTex", renderTexture);
				UnityEngine.Graphics.Blit(renderTexture, renderTexture1, this.downscaleMat, num);
			}
			for (int j = 0; j < (int)this.hiZLevels.Length; j++)
			{
				UnityEngine.Graphics.SetRenderTarget(this.hiZTexture, j);
				UnityEngine.Graphics.Blit(this.hiZLevels[j], this.blitCopyMat);
			}
		}
	}

	public static OccludeeState GetStateById(int id)
	{
		if (id < 0 || id >= 2097152)
		{
			return null;
		}
		bool flag = id < 1048576;
		int num = (flag ? id : id - 1048576);
		if (flag)
		{
			return OcclusionCulling.staticOccludees[num];
		}
		return OcclusionCulling.dynamicOccludees[num];
	}

	public void GrabDepthTexture()
	{
		if (this.depthTexture != null)
		{
			UnityEngine.Graphics.Blit(null, this.depthTexture, this.depthCopyMat, 0);
		}
	}

	private static void GrowStatePool()
	{
		for (int i = 0; i < 2048; i++)
		{
			OcclusionCulling.statePool.Enqueue(new OccludeeState());
		}
	}

	private void InitializeHiZMap()
	{
		Shader shader = Shader.Find("Hidden/OcclusionCulling/DepthDownscale");
		Shader shader1 = Shader.Find("Hidden/OcclusionCulling/BlitCopy");
		this.downscaleMat = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.blitCopyMat = new Material(shader1)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.CheckResizeHiZMap();
	}

	private void InitializeHiZMap(int width, int height)
	{
		this.DestroyHiZMap();
		width = Mathf.Clamp(width, 1, 65536);
		height = Mathf.Clamp(height, 1, 65536);
		int num = Mathf.Min(width, height);
		this.hiZLevelCount = (int)(Mathf.Log((float)num, 2f) + 1f);
		this.hiZLevels = new RenderTexture[this.hiZLevelCount];
		this.depthTexture = this.CreateDepthTexture("DepthTex", width, height, false);
		this.hiZTexture = this.CreateDepthTexture("HiZMapTex", width, height, true);
		for (int i = 0; i < this.hiZLevelCount; i++)
		{
			this.hiZLevels[i] = this.CreateDepthTextureMip(string.Concat("HiZMap", i), width, height, i);
		}
	}

	private void IssueRead()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.IssueRead();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.IssueRead();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.IssueRead();
		}
		GL.IssuePluginEvent(RustNative.Graphics.GetRenderEventFunc(), 2);
	}

	public static void MakeAllVisible()
	{
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			if (OcclusionCulling.staticOccludees[i] != null)
			{
				OcclusionCulling.staticOccludees[i].MakeVisible();
			}
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			if (OcclusionCulling.dynamicOccludees[j] != null)
			{
				OcclusionCulling.dynamicOccludees[j].MakeVisible();
			}
		}
	}

	private float[] MatrixToFloatArray(Matrix4x4 m)
	{
		int num = 0;
		int num1 = 0;
		while (num < 4)
		{
			for (int i = 0; i < 4; i++)
			{
				int num2 = num1;
				num1 = num2 + 1;
				this.matrixToFloatTemp[num2] = m[i, num];
			}
			num++;
		}
		return this.matrixToFloatTemp;
	}

	public static void NormalizePlane(ref Vector4 plane)
	{
		float single = Mathf.Sqrt(plane.x * plane.x + plane.y * plane.y + plane.z * plane.z);
		plane.x /= single;
		plane.y /= single;
		plane.z /= single;
		plane.w /= single;
	}

	private void OnDisable()
	{
		if (this.fallbackMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.fallbackMat);
			this.fallbackMat = null;
		}
		if (this.depthCopyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.depthCopyMat);
			this.depthCopyMat = null;
		}
		OcclusionCulling.staticSet.Dispose(true);
		OcclusionCulling.dynamicSet.Dispose(true);
		OcclusionCulling.gridSet.Dispose(true);
		this.FinalizeHiZMap();
	}

	private void OnEnable()
	{
		if (!OcclusionCulling.Enabled)
		{
			OcclusionCulling.Enabled = false;
			return;
		}
		if (!OcclusionCulling.Supported)
		{
			Debug.LogWarning(string.Concat("[OcclusionCulling] Disabled due to graphics device type ", SystemInfo.graphicsDeviceType, " not supported."));
			OcclusionCulling.Enabled = false;
			return;
		}
		this.usePixelShaderFallback = (this.usePixelShaderFallback || !SystemInfo.supportsComputeShaders || this.computeShader == null ? true : !this.computeShader.HasKernel("compute_cull"));
		this.useNativePath = (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Direct3D11 ? false : this.SupportsNativePath());
		this.useAsyncReadAPI = (this.useNativePath ? false : SystemInfo.supportsAsyncGPUReadback);
		if (!this.useNativePath && !this.useAsyncReadAPI)
		{
			Debug.LogWarning(string.Concat("[OcclusionCulling] Disabled due to unsupported Async GPU Reads on device ", SystemInfo.graphicsDeviceType));
			OcclusionCulling.Enabled = false;
			return;
		}
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			OcclusionCulling.staticChanged.Add(i);
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			OcclusionCulling.dynamicChanged.Add(j);
		}
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat = new Material(Shader.Find("Hidden/OcclusionCulling/Culling"))
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
		OcclusionCulling.staticSet.Attach(this);
		OcclusionCulling.dynamicSet.Attach(this);
		OcclusionCulling.gridSet.Attach(this);
		this.depthCopyMat = new Material(Shader.Find("Hidden/OcclusionCulling/DepthCopy"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.InitializeHiZMap();
		this.UpdateCameraMatrices(true);
	}

	private void OnPostRender()
	{
		bool flag = GL.sRGBWrite;
		RenderBuffer renderBuffer = UnityEngine.Graphics.activeColorBuffer;
		RenderBuffer renderBuffer1 = UnityEngine.Graphics.activeDepthBuffer;
		this.GrabDepthTexture();
		UnityEngine.Graphics.SetRenderTarget(renderBuffer, renderBuffer1);
		GL.sRGBWrite = flag;
	}

	private void OnPreCull()
	{
		this.UpdateCameraMatrices(false);
		this.GenerateHiZMipChain();
		this.PrepareAndDispatch();
		this.IssueRead();
		if (OcclusionCulling.grid.Size <= (int)OcclusionCulling.gridSet.resultData.Length)
		{
			this.RetrieveAndApplyVisibility();
			return;
		}
		Debug.LogWarning(string.Concat(new object[] { "[OcclusionCulling] Grid size and result capacity are out of sync: ", OcclusionCulling.grid.Size, ", ", (int)OcclusionCulling.gridSet.resultData.Length }));
	}

	private void PrepareAndDispatch()
	{
		Vector2 vector2 = new Vector2((float)this.hiZWidth, (float)this.hiZHeight);
		OcclusionCulling.ExtractFrustum(this.viewProjMatrix, ref this.frustumPlanes);
		bool flag = true;
		if (!this.usePixelShaderFallback)
		{
			this.computeShader.SetTexture(0, "_HiZMap", this.hiZTexture);
			this.computeShader.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.computeShader.SetFloats("_ViewMatrix", this.MatrixToFloatArray(this.viewMatrix));
			this.computeShader.SetFloats("_ProjMatrix", this.MatrixToFloatArray(this.projMatrix));
			this.computeShader.SetFloats("_ViewProjMatrix", this.MatrixToFloatArray(this.viewProjMatrix));
			this.computeShader.SetVector("_CameraWorldPos", base.transform.position);
			this.computeShader.SetVector("_ViewportSize", vector2);
			this.computeShader.SetFloat("_FrustumCull", (flag ? 0f : 1f));
			for (int i = 0; i < 6; i++)
			{
				this.computeShader.SetVector(this.frustumPropNames[i], this.frustumPlanes[i]);
			}
		}
		else
		{
			this.fallbackMat.SetTexture("_HiZMap", this.hiZTexture);
			this.fallbackMat.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.fallbackMat.SetMatrix("_ViewMatrix", this.viewMatrix);
			this.fallbackMat.SetMatrix("_ProjMatrix", this.projMatrix);
			this.fallbackMat.SetMatrix("_ViewProjMatrix", this.viewProjMatrix);
			this.fallbackMat.SetVector("_CameraWorldPos", base.transform.position);
			this.fallbackMat.SetVector("_ViewportSize", vector2);
			this.fallbackMat.SetFloat("_FrustumCull", (flag ? 0f : 1f));
			for (int j = 0; j < 6; j++)
			{
				this.fallbackMat.SetVector(this.frustumPropNames[j], this.frustumPlanes[j]);
			}
		}
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticSet, OcclusionCulling.staticChanged, true);
			OcclusionCulling.staticSet.Dispatch(OcclusionCulling.staticOccludees.Count);
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicChanged, false);
			OcclusionCulling.dynamicSet.Dispatch(OcclusionCulling.dynamicOccludees.Count);
		}
		this.UpdateGridBuffers();
		OcclusionCulling.gridSet.Dispatch(OcclusionCulling.grid.Size);
	}

	private void ProcessCallbacks(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SimpleList<int> changed)
	{
		for (int i = 0; i < changed.Count; i++)
		{
			int item = changed[i];
			OccludeeState occludeeState = occludees[item];
			if (occludeeState != null)
			{
				bool flag = states.array[item].isVisible == 0;
				OcclusionCulling.OnVisibilityChanged onVisibilityChanged = occludeeState.onVisibilityChanged;
				if (onVisibilityChanged != null && (UnityEngine.Object)onVisibilityChanged.Target != null)
				{
					onVisibilityChanged(flag);
				}
				if (occludeeState.slot >= 0)
				{
					states.array[occludeeState.slot].isVisible = (byte)((flag ? 1 : 0));
				}
			}
		}
		changed.Clear();
	}

	private static int ProcessOccludees_Fast(OccludeeState.State[] states, int[] bucket, int bucketCount, Color32[] results, int resultCount, int[] changed, ref int changedCount, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucketCount; i++)
		{
			int num1 = bucket[i];
			if (num1 >= 0 && num1 < resultCount && states[num1].active != 0)
			{
				OccludeeState.State state = states[num1];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag1 = results[num1].r > 0 & flag;
				if (flag1 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag1)
				{
					flag1 = time < state.waitTime;
				}
				if (flag1 != state.isVisible != 0)
				{
					if (state.callback == 0)
					{
						state.isVisible = (byte)((flag1 ? 1 : 0));
					}
					else
					{
						int num2 = changedCount;
						changedCount = num2 + 1;
						changed[num2] = num1;
					}
				}
				states[num1] = state;
				num = num + (flag1 ? 0 : 1);
			}
		}
		return num;
	}

	[DllImport("Renderer", CharSet=CharSet.None, EntryPoint="CULL_ProcessOccludees", ExactSpelling=false)]
	private static extern int ProcessOccludees_Native(ref OccludeeState.State states, ref int bucket, int bucketCount, ref Color32 results, int resultCount, ref int changed, ref int changedCount, ref Vector4 frustumPlanes, float time, uint frame);

	private static int ProcessOccludees_Safe(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SmartList bucket, Color32[] results, OcclusionCulling.SimpleList<int> changed, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState item = bucket[i];
			if (item != null && item.slot < (int)results.Length)
			{
				int num1 = item.slot;
				OccludeeState.State state = states[num1];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag1 = results[num1].r > 0 & flag;
				if (flag1 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag1)
				{
					flag1 = time < state.waitTime;
				}
				if (flag1 != state.isVisible != 0)
				{
					if (state.callback == 0)
					{
						state.isVisible = (byte)((flag1 ? 1 : 0));
					}
					else
					{
						changed.Add(num1);
					}
				}
				states[num1] = state;
				num += state.isVisible;
			}
		}
		return num;
	}

	public static void RecursiveAddOccludees<T>(Transform transform, float minTimeVisible = 0.1f, bool isStatic = true, bool stickyGizmos = false)
	where T : Occludee
	{
		Renderer component = transform.GetComponent<Renderer>();
		Collider collider = transform.GetComponent<Collider>();
		if (component != null && collider != null)
		{
			T t = component.gameObject.GetComponent<T>();
			t = (t == null ? component.gameObject.AddComponent<T>() : t);
			t.minTimeVisible = minTimeVisible;
			t.isStatic = isStatic;
			t.stickyGizmos = stickyGizmos;
			t.Register();
		}
		foreach (object obj in transform)
		{
			OcclusionCulling.RecursiveAddOccludees<T>((Transform)obj, minTimeVisible, isStatic, stickyGizmos);
		}
	}

	public static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged = null)
	{
		int num = -1;
		num = (!isStatic ? OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicVisibilityChanged) : OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged, OcclusionCulling.staticSet, OcclusionCulling.staticVisibilityChanged));
		if (num < 0 | isStatic)
		{
			return num;
		}
		return num + 1048576;
	}

	private static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged, OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled, List<int> changed, OcclusionCulling.BufferSet set, OcclusionCulling.SimpleList<int> visibilityChanged)
	{
		int num = OcclusionCulling.FindFreeSlot(occludees, states, recycled);
		if (num >= 0)
		{
			Vector4 vector4 = new Vector4(center.x, center.y, center.z, radius);
			OccludeeState grid = OcclusionCulling.Allocate().Initialize(states, set, num, vector4, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged);
			grid.cell = OcclusionCulling.RegisterToGrid(grid);
			occludees[num] = grid;
			changed.Add(num);
			if (states.array[num].isVisible != 0 != grid.cell.isVisible)
			{
				visibilityChanged.Add(num);
			}
		}
		return num;
	}

	public static OcclusionCulling.Cell RegisterToGrid(OccludeeState occludee)
	{
		OcclusionCulling.Cell cell;
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num1 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		int num3 = Mathf.Clamp(num, -1048575, 1048575);
		int num4 = Mathf.Clamp(num1, -1048575, 1048575);
		int num5 = Mathf.Clamp(num2, -1048575, 1048575);
		long num6 = (long)((num3 >= 0 ? num3 : num3 + 1048575));
		ulong num7 = (ulong)((num4 >= 0 ? num4 : num4 + 1048575));
		ulong num8 = (ulong)((num5 >= 0 ? num5 : num5 + 1048575));
		ulong num9 = num6 << 42 | num7 << 21 | num8;
		bool flag = OcclusionCulling.grid.TryGetValue(num9, out cell);
		if (!flag)
		{
			Vector3 vector3 = new Vector3()
			{
				x = (float)num * 100f + 50f,
				y = (float)num1 * 100f + 50f,
				z = (float)num2 * 100f + 50f
			};
			Vector3 vector31 = new Vector3(100f, 100f, 100f);
			cell = OcclusionCulling.grid.Add(num9, 16).Initialize(num, num1, num2, new Bounds(vector3, vector31));
		}
		OcclusionCulling.SmartList smartList = (occludee.isStatic ? cell.staticBucket : cell.dynamicBucket);
		if (!flag || !smartList.Contains(occludee))
		{
			occludee.cell = cell;
			smartList.Add(occludee, 16);
			OcclusionCulling.gridChanged.Enqueue(cell);
		}
		return cell;
	}

	private static void Release(OccludeeState state)
	{
		OcclusionCulling.statePool.Enqueue(state);
	}

	public void ResetTiming(OcclusionCulling.SmartList bucket)
	{
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState item = bucket[i];
			if (item != null)
			{
				item.states.array[item.slot].waitTime = 0f;
			}
		}
	}

	public void ResetTiming()
	{
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell item = OcclusionCulling.grid[i];
			if (item != null)
			{
				this.ResetTiming(item.staticBucket);
				this.ResetTiming(item.dynamicBucket);
			}
		}
	}

	public void RetrieveAndApplyVisibility()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.GetResults();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.GetResults();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.GetResults();
		}
		if (this.debugSettings.showAllVisible)
		{
			for (int i = 0; i < (int)OcclusionCulling.staticSet.resultData.Length; i++)
			{
				OcclusionCulling.staticSet.resultData[i].r = 1;
			}
			for (int j = 0; j < (int)OcclusionCulling.dynamicSet.resultData.Length; j++)
			{
				OcclusionCulling.dynamicSet.resultData[j].r = 1;
			}
			for (int k = 0; k < (int)OcclusionCulling.gridSet.resultData.Length; k++)
			{
				OcclusionCulling.gridSet.resultData[k].r = 1;
			}
		}
		OcclusionCulling.staticVisibilityChanged.EnsureCapacity(OcclusionCulling.staticOccludees.Count);
		OcclusionCulling.dynamicVisibilityChanged.EnsureCapacity(OcclusionCulling.dynamicOccludees.Count);
		float single = Time.time;
		uint num = (uint)Time.frameCount;
		if (!this.useNativePath)
		{
			this.ApplyVisibility_Fast(single, num);
		}
		else
		{
			this.ApplyVisibility_Native(single, num);
		}
		this.ProcessCallbacks(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticVisibilityChanged);
		this.ProcessCallbacks(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicVisibilityChanged);
	}

	private bool SupportsNativePath()
	{
		bool flag = true;
		try
		{
			OccludeeState.State state = new OccludeeState.State();
			Color32 color32 = new Color32(0, 0, 0, 0);
			Vector4 vector4 = Vector4.zero;
			int num = 0;
			int num1 = 0;
			int num2 = 0;
			OcclusionCulling.ProcessOccludees_Native(ref state, ref num, 0, ref color32, 0, ref num1, ref num2, ref vector4, 0f, 0);
		}
		catch (EntryPointNotFoundException entryPointNotFoundException)
		{
			Debug.Log("[OcclusionCulling] Fast native path not available. Reverting to managed fallback.");
			flag = false;
		}
		return flag;
	}

	public static void UnregisterFromGrid(OccludeeState occludee)
	{
		OcclusionCulling.SmartList smartList;
		OcclusionCulling.Cell cell = occludee.cell;
		smartList = (occludee.isStatic ? cell.staticBucket : cell.dynamicBucket);
		OcclusionCulling.gridChanged.Enqueue(cell);
		smartList.Remove(occludee);
		if (cell.staticBucket.Count == 0 && cell.dynamicBucket.Count == 0)
		{
			OcclusionCulling.grid.Remove(cell);
			cell.Reset();
		}
		occludee.cell = null;
	}

	public static void UnregisterOccludee(int id)
	{
		if (id >= 0 && id < 2097152)
		{
			bool flag = id < 1048576;
			int num = (flag ? id : id - 1048576);
			if (flag)
			{
				OcclusionCulling.UnregisterOccludee(num, OcclusionCulling.staticOccludees, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged);
				return;
			}
			OcclusionCulling.UnregisterOccludee(num, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged);
		}
	}

	private static void UnregisterOccludee(int slot, OcclusionCulling.SimpleList<OccludeeState> occludees, Queue<int> recycled, List<int> changed)
	{
		OccludeeState item = occludees[slot];
		OcclusionCulling.UnregisterFromGrid(item);
		recycled.Enqueue(slot);
		changed.Add(slot);
		OcclusionCulling.Release(item);
		occludees[slot] = null;
		item.Invalidate();
	}

	private void Update()
	{
		if (!OcclusionCulling.Enabled)
		{
			base.enabled = false;
			return;
		}
		this.CheckResizeHiZMap();
		this.DebugUpdate();
		this.DebugDraw();
	}

	private void UpdateBuffers(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, List<int> changed, bool isStatic)
	{
		int count = occludees.Count;
		bool flag = changed.Count > 0;
		set.CheckResize(count, 2048);
		for (int i = 0; i < changed.Count; i++)
		{
			int item = changed[i];
			OccludeeState occludeeState = occludees[item];
			if (occludeeState == null)
			{
				set.inputData[item] = Vector4.zero;
			}
			else
			{
				if (!isStatic)
				{
					OcclusionCulling.UpdateInGrid(occludeeState);
				}
				set.inputData[item] = states[item].sphereBounds;
			}
		}
		changed.Clear();
		if (flag)
		{
			set.UploadData();
		}
	}

	private void UpdateCameraMatrices(bool starting = false)
	{
		if (!starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
		Matrix4x4 matrix4x4 = Matrix4x4.Perspective(this.camera.fieldOfView, this.camera.aspect, this.camera.nearClipPlane, this.camera.farClipPlane);
		this.viewMatrix = this.camera.worldToCameraMatrix;
		this.projMatrix = GL.GetGPUProjectionMatrix(matrix4x4, false);
		this.viewProjMatrix = this.projMatrix * this.viewMatrix;
		this.invViewProjMatrix = Matrix4x4.Inverse(this.viewProjMatrix);
		if (starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
	}

	public static void UpdateDynamicOccludee(int id, Vector3 center, float radius)
	{
		int num = id - 1048576;
		if (num >= 0 && num < 1048576)
		{
			OcclusionCulling.dynamicStates.array[num].sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			OcclusionCulling.dynamicChanged.Add(num);
		}
	}

	public void UpdateGridBuffers()
	{
		if (OcclusionCulling.gridSet.CheckResize(OcclusionCulling.grid.Size, 256))
		{
			if (this.debugSettings.log)
			{
				Debug.Log(string.Concat("[OcclusionCulling] Resized grid to ", OcclusionCulling.grid.Size));
			}
			for (int i = 0; i < OcclusionCulling.grid.Size; i++)
			{
				if (OcclusionCulling.grid[i] != null)
				{
					OcclusionCulling.gridChanged.Enqueue(OcclusionCulling.grid[i]);
				}
			}
		}
		bool count = OcclusionCulling.gridChanged.Count > 0;
		while (OcclusionCulling.gridChanged.Count > 0)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.gridChanged.Dequeue();
			OcclusionCulling.gridSet.inputData[cell.hashedPoolIndex] = cell.sphereBounds;
		}
		if (count)
		{
			OcclusionCulling.gridSet.UploadData();
		}
	}

	public static void UpdateInGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num1 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		if (num != occludee.cell.x || num1 != occludee.cell.y || num2 != occludee.cell.z)
		{
			OcclusionCulling.UnregisterFromGrid(occludee);
			OcclusionCulling.RegisterToGrid(occludee);
		}
	}

	public class BufferSet
	{
		public ComputeBuffer inputBuffer;

		public ComputeBuffer resultBuffer;

		public int width;

		public int height;

		public int capacity;

		public int count;

		public Texture2D inputTexture;

		public RenderTexture resultTexture;

		public Texture2D resultReadTexture;

		public Color[] inputData;

		public Color32[] resultData;

		private OcclusionCulling culling;

		private const int MaxAsyncGPUReadbackRequests = 10;

		private Queue<AsyncGPUReadbackRequest> asyncRequests;

		public IntPtr readbackInst;

		public bool Ready
		{
			get
			{
				return this.resultData.Length != 0;
			}
		}

		public BufferSet()
		{
		}

		private int AlignDispatchSize(int dispatchSize)
		{
			return (dispatchSize + 63) / 64;
		}

		public void Attach(OcclusionCulling culling)
		{
			this.culling = culling;
		}

		public bool CheckResize(int count, int granularity)
		{
			if (count <= this.capacity && (!this.culling.usePixelShaderFallback || !(this.resultTexture != null) || this.resultTexture.IsCreated()))
			{
				return false;
			}
			this.Dispose(false);
			int num = this.capacity;
			int num1 = count / granularity * granularity + granularity;
			if (!this.culling.usePixelShaderFallback)
			{
				this.inputBuffer = new ComputeBuffer(num1, 16);
				this.resultBuffer = new ComputeBuffer(num1, 4);
				if (!this.culling.useAsyncReadAPI)
				{
					uint num2 = (uint)(this.capacity * 4);
					this.readbackInst = RustNative.Graphics.BufferReadback.CreateForBuffer(this.resultBuffer.GetNativeBufferPtr(), num2);
				}
				this.capacity = num1;
			}
			else
			{
				this.width = Mathf.CeilToInt(Mathf.Sqrt((float)num1));
				this.height = Mathf.CeilToInt((float)num1 / (float)this.width);
				this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true)
				{
					name = "_Input",
					filterMode = FilterMode.Point,
					wrapMode = TextureWrapMode.Clamp
				};
				this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
				{
					name = "_Result",
					filterMode = FilterMode.Point,
					wrapMode = TextureWrapMode.Clamp,
					useMipMap = false
				};
				this.resultTexture.Create();
				this.resultReadTexture = new Texture2D(this.width, this.height, TextureFormat.ARGB32, false, true)
				{
					name = "_ResultRead",
					filterMode = FilterMode.Point,
					wrapMode = TextureWrapMode.Clamp
				};
				if (!this.culling.useAsyncReadAPI)
				{
					this.readbackInst = RustNative.Graphics.BufferReadback.CreateForTexture(this.resultTexture.GetNativeTexturePtr(), (uint)this.width, (uint)this.height, (uint)this.resultTexture.format);
				}
				this.capacity = this.width * this.height;
			}
			Array.Resize<Color>(ref this.inputData, this.capacity);
			Array.Resize<Color32>(ref this.resultData, this.capacity);
			Color32 color32 = new Color32(255, 255, 255, 255);
			for (int i = num; i < this.capacity; i++)
			{
				this.resultData[i] = color32;
			}
			this.count = count;
			return true;
		}

		public void Dispatch(int count)
		{
			if (this.culling.usePixelShaderFallback)
			{
				RenderBuffer renderBuffer = UnityEngine.Graphics.activeColorBuffer;
				RenderBuffer renderBuffer1 = UnityEngine.Graphics.activeDepthBuffer;
				this.culling.fallbackMat.SetTexture("_Input", this.inputTexture);
				UnityEngine.Graphics.Blit(this.inputTexture, this.resultTexture, this.culling.fallbackMat, 0);
				UnityEngine.Graphics.SetRenderTarget(renderBuffer, renderBuffer1);
				return;
			}
			this.culling.computeShader.SetBuffer(0, "_Input", this.inputBuffer);
			this.culling.computeShader.SetBuffer(0, "_Result", this.resultBuffer);
			this.culling.computeShader.Dispatch(0, this.AlignDispatchSize(count), 1, 1);
		}

		public void Dispose(bool data = true)
		{
			if (this.inputBuffer != null)
			{
				this.inputBuffer.Dispose();
				this.inputBuffer = null;
			}
			if (this.resultBuffer != null)
			{
				this.resultBuffer.Dispose();
				this.resultBuffer = null;
			}
			if (this.inputTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				UnityEngine.Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (this.resultReadTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.resultReadTexture);
				this.resultReadTexture = null;
			}
			if (this.readbackInst != IntPtr.Zero)
			{
				RustNative.Graphics.BufferReadback.Destroy(this.readbackInst);
				this.readbackInst = IntPtr.Zero;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
				this.capacity = 0;
				this.count = 0;
			}
		}

		public void GetResults()
		{
			if (this.resultData != null && this.resultData.Length != 0)
			{
				if (OcclusionCulling.SafeMode)
				{
					if (this.culling.usePixelShaderFallback)
					{
						RenderTexture.active = this.resultTexture;
						this.resultReadTexture.ReadPixels(new Rect(0f, 0f, (float)this.width, (float)this.height), 0, 0);
						this.resultReadTexture.Apply();
						Array.Copy(this.resultReadTexture.GetPixels32(), this.resultData, (int)this.resultData.Length);
						return;
					}
					this.resultBuffer.GetData(this.resultData);
				}
				else if (this.culling.useAsyncReadAPI)
				{
					while (this.asyncRequests.Count > 0)
					{
						AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
						if (!asyncGPUReadbackRequest.hasError)
						{
							if (!asyncGPUReadbackRequest.done)
							{
								return;
							}
							NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
							for (int i = 0; i < data.Length; i++)
							{
								this.resultData[i] = data[i];
							}
							this.asyncRequests.Dequeue();
						}
						else
						{
							this.asyncRequests.Dequeue();
						}
					}
					return;
				}
				else if (this.readbackInst != IntPtr.Zero)
				{
					RustNative.Graphics.BufferReadback.GetData(this.readbackInst, ref this.resultData[0]);
					return;
				}
			}
		}

		public void IssueRead()
		{
			AsyncGPUReadbackRequest asyncGPUReadbackRequest;
			if (!OcclusionCulling.SafeMode)
			{
				if (this.culling.useAsyncReadAPI)
				{
					if (this.asyncRequests.Count < 10)
					{
						asyncGPUReadbackRequest = (!this.culling.usePixelShaderFallback ? AsyncGPUReadback.Request(this.resultBuffer, null) : AsyncGPUReadback.Request(this.resultTexture, 0, null));
						this.asyncRequests.Enqueue(asyncGPUReadbackRequest);
						return;
					}
				}
				else if (this.readbackInst != IntPtr.Zero)
				{
					RustNative.Graphics.BufferReadback.IssueRead(this.readbackInst);
				}
			}
		}

		public void UploadData()
		{
			if (!this.culling.usePixelShaderFallback)
			{
				this.inputBuffer.SetData(this.inputData);
				return;
			}
			this.inputTexture.SetPixels(this.inputData);
			this.inputTexture.Apply();
		}
	}

	[Serializable]
	public class Cell : OcclusionCulling.HashedPoolValue
	{
		public int x;

		public int y;

		public int z;

		public Bounds bounds;

		public Vector4 sphereBounds;

		public bool isVisible;

		public OcclusionCulling.SmartList staticBucket;

		public OcclusionCulling.SmartList dynamicBucket;

		public Cell()
		{
		}

		public OcclusionCulling.Cell Initialize(int x, int y, int z, Bounds bounds)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.bounds = bounds;
			float single = bounds.center.x;
			float single1 = bounds.center.y;
			float single2 = bounds.center.z;
			Vector3 vector3 = bounds.extents;
			this.sphereBounds = new Vector4(single, single1, single2, vector3.magnitude);
			this.isVisible = true;
			this.staticBucket = new OcclusionCulling.SmartList(32);
			this.dynamicBucket = new OcclusionCulling.SmartList(32);
			return this;
		}

		public void Reset()
		{
			int num = 0;
			int num1 = num;
			this.z = num;
			int num2 = num1;
			num1 = num2;
			this.y = num2;
			this.x = num1;
			this.bounds = new Bounds();
			this.sphereBounds = Vector4.zero;
			this.isVisible = true;
			this.staticBucket = null;
			this.dynamicBucket = null;
		}
	}

	public enum DebugFilter
	{
		Off,
		Dynamic,
		Static,
		Grid,
		All
	}

	[Flags]
	public enum DebugMask
	{
		Off = 0,
		Dynamic = 1,
		Static = 2,
		Grid = 4,
		All = 7
	}

	[Serializable]
	public class DebugSettings
	{
		public bool log;

		public bool showAllVisible;

		public bool showMipChain;

		public bool showMain;

		public int showMainLod;

		public bool showFallback;

		public bool showStats;

		public bool showScreenBounds;

		public OcclusionCulling.DebugMask showMask;

		public LayerMask layerFilter;

		public DebugSettings()
		{
		}
	}

	public class HashedPool<ValueType>
	where ValueType : OcclusionCulling.HashedPoolValue, new()
	{
		private int granularity;

		private Dictionary<ulong, ValueType> dict;

		private List<ValueType> pool;

		private List<ValueType> list;

		private Queue<ValueType> recycled;

		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		public ValueType this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		public int Size
		{
			get
			{
				return this.list.Count;
			}
		}

		public HashedPool(int capacity, int granularity)
		{
			this.granularity = granularity;
			this.dict = new Dictionary<ulong, ValueType>(capacity);
			this.pool = new List<ValueType>(capacity);
			this.list = new List<ValueType>(capacity);
			this.recycled = new Queue<ValueType>();
		}

		public ValueType Add(ulong key, int capacityGranularity = 16)
		{
			ValueType valueType;
			if (this.recycled.Count <= 0)
			{
				int count = this.pool.Count;
				if (count == this.pool.Capacity)
				{
					List<ValueType> capacity = this.pool;
					capacity.Capacity = capacity.Capacity + this.granularity;
				}
				valueType = Activator.CreateInstance<ValueType>();
				valueType.hashedPoolIndex = count;
				this.pool.Add(valueType);
				this.list.Add(valueType);
			}
			else
			{
				valueType = this.recycled.Dequeue();
				this.list[valueType.hashedPoolIndex] = valueType;
			}
			valueType.hashedPoolKey = key;
			this.dict.Add(key, valueType);
			return valueType;
		}

		public void Clear()
		{
			this.dict.Clear();
			this.pool.Clear();
			this.list.Clear();
			this.recycled.Clear();
		}

		public bool ContainsKey(ulong key)
		{
			return this.dict.ContainsKey(key);
		}

		public void Remove(ValueType value)
		{
			this.dict.Remove(value.hashedPoolKey);
			this.list[value.hashedPoolIndex] = default(ValueType);
			this.recycled.Enqueue(value);
			value.hashedPoolKey = (ulong)-1;
		}

		public bool TryGetValue(ulong key, out ValueType value)
		{
			return this.dict.TryGetValue(key, out value);
		}
	}

	public class HashedPoolValue
	{
		public ulong hashedPoolKey;

		public int hashedPoolIndex;

		public HashedPoolValue()
		{
		}
	}

	public delegate void OnVisibilityChanged(bool visible);

	public class SimpleList<T>
	{
		private const int defaultCapacity = 16;

		private readonly static T[] emptyArray;

		public T[] array;

		public int count;

		public int Capacity
		{
			get
			{
				return (int)this.array.Length;
			}
			set
			{
				if (value != (int)this.array.Length)
				{
					if (value > 0)
					{
						T[] tArray = new T[value];
						if (this.count > 0)
						{
							Array.Copy(this.array, 0, tArray, 0, this.count);
						}
						this.array = tArray;
						return;
					}
					this.array = OcclusionCulling.SimpleList<T>.emptyArray;
				}
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		static SimpleList()
		{
			OcclusionCulling.SimpleList<T>.emptyArray = new T[0];
		}

		public SimpleList()
		{
			this.array = OcclusionCulling.SimpleList<T>.emptyArray;
		}

		public SimpleList(int capacity)
		{
			this.array = (capacity == 0 ? OcclusionCulling.SimpleList<T>.emptyArray : new T[capacity]);
		}

		public void Add(T item)
		{
			if (this.count == (int)this.array.Length)
			{
				this.EnsureCapacity(this.count + 1);
			}
			T[] tArray = this.array;
			int num = this.count;
			this.count = num + 1;
			tArray[num] = item;
		}

		public void Clear()
		{
			if (this.count > 0)
			{
				Array.Clear(this.array, 0, this.count);
				this.count = 0;
			}
		}

		public bool Contains(T item)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.array[i].Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(T[] array)
		{
			Array.Copy(this.array, 0, array, 0, this.count);
		}

		public void EnsureCapacity(int min)
		{
			if ((int)this.array.Length < min)
			{
				int num = (this.array.Length == 0 ? 16 : (int)this.array.Length * 2);
				num = (num < min ? min : num);
				this.Capacity = num;
			}
		}
	}

	public class SmartList
	{
		private const int defaultCapacity = 16;

		private readonly static OccludeeState[] emptyList;

		private readonly static int[] emptySlots;

		private OccludeeState[] list;

		private int[] slots;

		private Queue<int> recycled;

		private int count;

		public int Capacity
		{
			get
			{
				return (int)this.list.Length;
			}
			set
			{
				if (value != (int)this.list.Length)
				{
					if (value > 0)
					{
						OccludeeState[] occludeeStateArray = new OccludeeState[value];
						int[] numArray = new int[value];
						if (this.count > 0)
						{
							Array.Copy(this.list, occludeeStateArray, this.count);
							Array.Copy(this.slots, numArray, this.count);
						}
						this.list = occludeeStateArray;
						this.slots = numArray;
						return;
					}
					this.list = OcclusionCulling.SmartList.emptyList;
					this.slots = OcclusionCulling.SmartList.emptySlots;
				}
			}
		}

		public int Count
		{
			get
			{
				return this.count - this.recycled.Count;
			}
		}

		public OccludeeState this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		public OccludeeState[] List
		{
			get
			{
				return this.list;
			}
		}

		public int Size
		{
			get
			{
				return this.count;
			}
		}

		public int[] Slots
		{
			get
			{
				return this.slots;
			}
		}

		static SmartList()
		{
			OcclusionCulling.SmartList.emptyList = new OccludeeState[0];
			OcclusionCulling.SmartList.emptySlots = new int[0];
		}

		public SmartList(int capacity)
		{
			this.list = new OccludeeState[capacity];
			this.slots = new int[capacity];
			this.recycled = new Queue<int>();
			this.count = 0;
		}

		public void Add(OccludeeState value, int capacityGranularity = 16)
		{
			int num;
			if (this.recycled.Count <= 0)
			{
				num = this.count;
				if (num == (int)this.list.Length)
				{
					this.EnsureCapacity(this.count + 1);
				}
				this.list[num] = value;
				this.slots[num] = value.slot;
				this.count++;
			}
			else
			{
				num = this.recycled.Dequeue();
				this.list[num] = value;
				this.slots[num] = value.slot;
			}
			value.hashedListIndex = num;
		}

		public bool Contains(OccludeeState value)
		{
			int num = value.hashedListIndex;
			if (num < 0)
			{
				return false;
			}
			return this.list[num] != null;
		}

		public void EnsureCapacity(int min)
		{
			if ((int)this.list.Length < min)
			{
				int num = (this.list.Length == 0 ? 16 : (int)this.list.Length * 2);
				num = (num < min ? min : num);
				this.Capacity = num;
			}
		}

		public void Remove(OccludeeState value)
		{
			int num = value.hashedListIndex;
			this.list[num] = null;
			this.slots[num] = -1;
			this.recycled.Enqueue(num);
			value.hashedListIndex = -1;
		}
	}

	public class SmartListValue
	{
		public int hashedListIndex;

		public SmartListValue()
		{
		}
	}

	public struct Sphere
	{
		public Vector3 position;

		public float radius;

		public Sphere(Vector3 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}

		public bool IsValid()
		{
			return this.radius > 0f;
		}
	}
}