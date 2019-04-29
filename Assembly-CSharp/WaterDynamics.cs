using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class WaterDynamics : MonoBehaviour
{
	public bool ForceFallback;

	private WaterDynamics.Target target;

	private bool useNativePath;

	private static HashSet<WaterInteraction> interactions;

	private const int maxRasterSize = 1024;

	private const int subStep = 256;

	private const int subShift = 8;

	private const int subMask = 255;

	private const float oneOverSubStep = 0.00390625f;

	private const float interp_subStep = 65536f;

	private const int interp_subShift = 16;

	private const int interp_subFracMask = 65535;

	private WaterDynamics.ImageDesc imageDesc;

	private byte[] imagePixels;

	private WaterDynamics.TargetDesc targetDesc;

	private byte[] targetPixels;

	private byte[] targetDrawTileTable;

	private SimpleList<ushort> targetDrawTileList;

	public bool ShowDebug;

	public bool IsInitialized
	{
		get;
		private set;
	}

	static WaterDynamics()
	{
		WaterDynamics.interactions = new HashSet<WaterInteraction>();
	}

	public WaterDynamics()
	{
	}

	private int EdgeFunction(WaterDynamics.Point2D a, WaterDynamics.Point2D b, WaterDynamics.Point2D c)
	{
		return (int)(((long)(b.x - a.x) * (long)(c.y - a.y) >> 8) - ((long)(b.y - a.y) * (long)(c.x - a.x) >> 8));
	}

	private float Frac(float x)
	{
		return x - (float)((int)x);
	}

	public void Initialize(Vector3 areaPosition, Vector3 areaSize)
	{
		this.target = new WaterDynamics.Target(this, areaPosition, areaSize);
		this.useNativePath = this.SupportsNativePath();
		this.IsInitialized = true;
	}

	private bool IsTopLeft(WaterDynamics.Point2D a, WaterDynamics.Point2D b)
	{
		if (a.y == b.y && a.x < b.x)
		{
			return true;
		}
		return a.y > b.y;
	}

	private int Max3(int a, int b, int c)
	{
		return Mathf.Max(a, Mathf.Max(b, c));
	}

	private int Min3(int a, int b, int c)
	{
		return Mathf.Min(a, Mathf.Min(b, c));
	}

	public void OnDisable()
	{
		this.Shutdown();
	}

	public void OnEnable()
	{
		this.TryInitialize();
	}

	private void ProcessInteractions()
	{
		foreach (WaterInteraction interaction in WaterDynamics.interactions)
		{
			if (interaction == null)
			{
				continue;
			}
			interaction.UpdateTransform();
		}
	}

	private void RasterBindImage(WaterDynamics.Image image)
	{
		this.imageDesc = image.desc;
		this.imagePixels = image.pixels;
	}

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="Water_RasterBindImage", ExactSpelling=false)]
	private static extern void RasterBindImage_Native(ref WaterDynamics.ImageDesc desc, ref byte pixels);

	private void RasterBindTarget(WaterDynamics.Target target)
	{
		this.targetDesc = target.Desc;
		this.targetPixels = target.Pixels;
		this.targetDrawTileTable = target.DrawTileTable;
		this.targetDrawTileList = target.DrawTileList;
	}

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="Water_RasterBindTarget", ExactSpelling=false)]
	private static extern void RasterBindTarget_Native(ref WaterDynamics.TargetDesc desc, ref byte pixels, ref byte drawTileTable, ref ushort drawTileList, ref int drawTileCount);

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="Water_RasterClearTile", ExactSpelling=false)]
	private static extern void RasterClearTile_Native(ref byte pixels, int offset, int stride, int width, int height);

	private void RasterInteraction(Vector2 pos, Vector2 scale, float rotation, float disp, float dist)
	{
		Vector2 raster = this.targetDesc.WorldToRaster(pos);
		float single = -rotation * 0.0174532924f;
		float single1 = Mathf.Sin(single);
		float single2 = Mathf.Cos(single);
		float single3 = Mathf.Min((float)this.imageDesc.width * scale.x, 1024f) * 0.5f;
		float single4 = Mathf.Min((float)this.imageDesc.height * scale.y, 1024f) * 0.5f;
		Vector2 vector2 = raster + this.Rotate2D(new Vector2(-single3, -single4), single1, single2);
		Vector2 vector21 = raster + this.Rotate2D(new Vector2(single3, -single4), single1, single2);
		Vector2 vector22 = raster + this.Rotate2D(new Vector2(single3, single4), single1, single2);
		Vector2 vector23 = raster + this.Rotate2D(new Vector2(-single3, single4), single1, single2);
		WaterDynamics.Point2D point2D = new WaterDynamics.Point2D(vector2.x * 256f, vector2.y * 256f);
		WaterDynamics.Point2D point2D1 = new WaterDynamics.Point2D(vector21.x * 256f, vector21.y * 256f);
		WaterDynamics.Point2D point2D2 = new WaterDynamics.Point2D(vector22.x * 256f, vector22.y * 256f);
		WaterDynamics.Point2D point2D3 = new WaterDynamics.Point2D(vector23.x * 256f, vector23.y * 256f);
		Vector2 vector24 = new Vector2(-0.5f, -0.5f);
		Vector2 vector25 = new Vector2((float)this.imageDesc.width - 0.5f, -0.5f);
		Vector2 vector26 = new Vector2((float)this.imageDesc.width - 0.5f, (float)this.imageDesc.height - 0.5f);
		Vector2 vector27 = new Vector2(-0.5f, (float)this.imageDesc.height - 0.5f);
		byte num = (byte)(disp * 255f);
		byte num1 = (byte)(dist * 255f);
		this.RasterizeTriangle(point2D, point2D1, point2D2, vector24, vector25, vector26, num, num1);
		this.RasterizeTriangle(point2D, point2D2, point2D3, vector24, vector26, vector27, num, num1);
	}

	[DllImport("RustNative", CharSet=CharSet.None, EntryPoint="Water_RasterInteraction", ExactSpelling=false)]
	private static extern void RasterInteraction_Native(Vector2 pos, Vector2 scale, float rotation, float disp, float dist);

	private void RasterizeTriangle(WaterDynamics.Point2D p0, WaterDynamics.Point2D p1, WaterDynamics.Point2D p2, Vector2 uv0, Vector2 uv1, Vector2 uv2, byte disp, byte dist)
	{
		int num;
		int num1 = this.imageDesc.width;
		int num2 = this.imageDesc.widthShift;
		int num3 = this.imageDesc.maxWidth;
		int num4 = this.imageDesc.maxHeight;
		int num5 = this.targetDesc.size;
		int num6 = this.targetDesc.tileCount;
		int num7 = Mathf.Max(this.Min3(p0.x, p1.x, p2.x), 0);
		int num8 = Mathf.Max(this.Min3(p0.y, p1.y, p2.y), 0);
		int num9 = Mathf.Min(this.Max3(p0.x, p1.x, p2.x), this.targetDesc.maxSizeSubStep);
		int num10 = Mathf.Min(this.Max3(p0.y, p1.y, p2.y), this.targetDesc.maxSizeSubStep);
		int num11 = Mathf.Max(num7 >> 8 >> (this.targetDesc.tileSizeShift & 31), 0);
		int num12 = Mathf.Min(num9 >> 8 >> (this.targetDesc.tileSizeShift & 31), this.targetDesc.tileMaxCount);
		int num13 = Mathf.Max(num8 >> 8 >> (this.targetDesc.tileSizeShift & 31), 0);
		int num14 = Mathf.Min(num10 >> 8 >> (this.targetDesc.tileSizeShift & 31), this.targetDesc.tileMaxCount);
		for (int i = num13; i <= num14; i++)
		{
			int num15 = i * num6;
			for (int j = num11; j <= num12; j++)
			{
				int num16 = num15 + j;
				if (this.targetDrawTileTable[num16] == 0)
				{
					this.targetDrawTileTable[num16] = 1;
					this.targetDrawTileList.Add((ushort)num16);
				}
			}
		}
		num7 = num7 + 255 & -256;
		num8 = num8 + 255 & -256;
		int num17 = (this.IsTopLeft(p1, p2) ? 0 : -1);
		int num18 = (this.IsTopLeft(p2, p0) ? 0 : -1);
		int num19 = (this.IsTopLeft(p0, p1) ? 0 : -1);
		WaterDynamics.Point2D point2D = new WaterDynamics.Point2D(num7, num8);
		int num20 = this.EdgeFunction(p1, p2, point2D) + num17;
		int num21 = this.EdgeFunction(p2, p0, point2D) + num18;
		int num22 = this.EdgeFunction(p0, p1, point2D) + num19;
		int num23 = p1.y - p2.y;
		int num24 = p2.y - p0.y;
		int num25 = p0.y - p1.y;
		int num26 = p2.x - p1.x;
		int num27 = p0.x - p2.x;
		int num28 = p1.x - p0.x;
		float single = 16777216f / (float)this.EdgeFunction(p0, p1, p2);
		float single1 = uv0.x * 65536f;
		float single2 = uv0.y * 65536f;
		float single3 = (uv1.x - uv0.x) * single;
		float single4 = (uv1.y - uv0.y) * single;
		float single5 = (uv2.x - uv0.x) * single;
		float single6 = (uv2.y - uv0.y) * single;
		int num29 = (int)((float)num24 * 0.00390625f * single3 + (float)num25 * 0.00390625f * single5);
		int num30 = (int)((float)num24 * 0.00390625f * single4 + (float)num25 * 0.00390625f * single6);
		for (int k = num8; k <= num10; k += 256)
		{
			int num31 = num20;
			int num32 = num21;
			int num33 = num22;
			int num34 = (int)(single1 + single3 * 0.00390625f * (float)num32 + single5 * 0.00390625f * (float)num33);
			int num35 = (int)(single2 + single4 * 0.00390625f * (float)num32 + single6 * 0.00390625f * (float)num33);
			for (int l = num7; l <= num9; l += 256)
			{
				if ((num31 | num32 | num33) >= 0)
				{
					int num36 = (num34 > 0 ? num34 : 0);
					num = (num35 > 0 ? num35 : 0);
					int num37 = num36 >> 16;
					int num38 = num >> 16;
					byte num39 = (byte)((num36 & 65535) >> 8);
					byte num40 = (byte)((num & 65535) >> 8);
					num37 = (num37 > 0 ? num37 : 0);
					num38 = (num38 > 0 ? num38 : 0);
					num37 = (num37 < num3 ? num37 : num3);
					num38 = (num38 < num4 ? num38 : num4);
					int num41 = (num37 < num3 ? 1 : 0);
					int num42 = (num38 < num4 ? num1 : 0);
					int num43 = (num38 << (num2 & 31)) + num37;
					int num44 = num43 + num41;
					int num45 = num43 + num42;
					int num46 = num45 + num41;
					byte num47 = this.imagePixels[num43];
					byte num48 = this.imagePixels[num44];
					byte num49 = this.imagePixels[num45];
					byte num50 = this.imagePixels[num46];
					int num51 = num47 + (num39 * (num48 - num47) >> 8);
					int num52 = num49 + (num39 * (num50 - num49) >> 8);
					int num53 = num51 + (num40 * (num52 - num51) >> 8);
					num53 = num53 * disp >> 8;
					int num54 = (k >> 8) * num5 + (l >> 8);
					num53 = this.targetPixels[num54] + num53;
					num53 = (num53 < 255 ? num53 : 255);
					this.targetPixels[num54] = (byte)num53;
				}
				num31 += num23;
				num32 += num24;
				num33 += num25;
				num34 += num29;
				num35 += num30;
			}
			num20 += num26;
			num21 += num27;
			num22 += num28;
		}
	}

	public static void RegisterInteraction(WaterInteraction interaction)
	{
		WaterDynamics.interactions.Add(interaction);
	}

	private Vector2 Rotate2D(Vector2 v, float s, float c)
	{
		Vector2 vector2 = new Vector2();
		vector2.x = v.x * c - v.y * s;
		vector2.y = v.y * c + v.x * s;
		return vector2;
	}

	public static void SafeDestroy<T>(ref T obj)
	where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	public static T SafeDestroy<T>(T obj)
	where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
		return default(T);
	}

	public static void SafeRelease<T>(ref T obj)
	where T : class, IDisposable
	{
		if (obj != null)
		{
			((IDisposable)(object)obj).Dispose();
			obj = default(T);
		}
	}

	public static T SafeRelease<T>(T obj)
	where T : class, IDisposable
	{
		if (obj != null)
		{
			((IDisposable)(object)obj).Dispose();
		}
		return default(T);
	}

	public float SampleHeight(Vector3 pos)
	{
		return 0f;
	}

	public void Shutdown()
	{
		if (this.target != null)
		{
			this.target.Destroy();
			this.target = null;
		}
		this.IsInitialized = false;
	}

	private bool SupportsNativePath()
	{
		bool flag = true;
		try
		{
			WaterDynamics.ImageDesc imageDesc = new WaterDynamics.ImageDesc();
			byte[] numArray = new byte[1];
			WaterDynamics.RasterBindImage_Native(ref imageDesc, ref numArray[0]);
		}
		catch (EntryPointNotFoundException entryPointNotFoundException)
		{
			Debug.Log("[WaterDynamics] Fast native path not available. Reverting to managed fallback.");
			flag = false;
		}
		return flag;
	}

	public bool TryInitialize()
	{
		if (this.IsInitialized || !(TerrainMeta.Data != null))
		{
			return false;
		}
		this.Initialize(TerrainMeta.Position, TerrainMeta.Data.size);
		return true;
	}

	public static void UnregisterInteraction(WaterInteraction interaction)
	{
		WaterDynamics.interactions.Remove(interaction);
	}

	public void Update()
	{
		if (WaterSystem.Instance != null)
		{
			if (this.IsInitialized)
			{
				return;
			}
			this.TryInitialize();
		}
	}

	public class Image
	{
		public WaterDynamics.ImageDesc desc;

		public byte[] pixels;

		public Texture2D texture
		{
			get;
			private set;
		}

		public Image(Texture2D tex)
		{
			this.desc = new WaterDynamics.ImageDesc(tex);
			this.texture = tex;
			this.pixels = this.GetDisplacementPixelsFromTexture(tex);
		}

		public void Destroy()
		{
			this.desc.Clear();
			this.texture = null;
			this.pixels = null;
		}

		private byte[] GetDisplacementPixelsFromTexture(Texture2D tex)
		{
			Color32[] pixels32 = tex.GetPixels32();
			byte[] numArray = new byte[(int)pixels32.Length];
			for (int i = 0; i < (int)pixels32.Length; i++)
			{
				numArray[i] = pixels32[i].b;
			}
			return numArray;
		}
	}

	public struct ImageDesc
	{
		public int width;

		public int height;

		public int maxWidth;

		public int maxHeight;

		public int widthShift;

		public ImageDesc(Texture2D tex)
		{
			this.width = tex.width;
			this.height = tex.height;
			this.maxWidth = tex.width - 1;
			this.maxHeight = tex.height - 1;
			this.widthShift = (int)Mathf.Log((float)tex.width, 2f);
		}

		public void Clear()
		{
			this.width = 0;
			this.height = 0;
			this.maxWidth = 0;
			this.maxHeight = 0;
			this.widthShift = 0;
		}
	}

	private struct Point2D
	{
		public int x;

		public int y;

		public Point2D(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Point2D(float x, float y)
		{
			this.x = (int)x;
			this.y = (int)y;
		}
	}

	public class Target
	{
		public WaterDynamics owner;

		public WaterDynamics.TargetDesc desc;

		private byte[] pixels;

		private byte[] clearTileTable;

		private SimpleList<ushort> clearTileList;

		private byte[] drawTileTable;

		private SimpleList<ushort> drawTileList;

		private const int MaxInteractionOffset = 100;

		private Vector3 prevCameraWorldPos;

		private Vector2i interactionOffset;

		public WaterDynamics.TargetDesc Desc
		{
			get
			{
				return this.desc;
			}
		}

		public SimpleList<ushort> DrawTileList
		{
			get
			{
				return this.drawTileList;
			}
		}

		public byte[] DrawTileTable
		{
			get
			{
				return this.drawTileTable;
			}
		}

		public byte[] Pixels
		{
			get
			{
				return this.pixels;
			}
		}

		public Target(WaterDynamics owner, Vector3 areaPosition, Vector3 areaSize)
		{
			this.owner = owner;
			this.desc = new WaterDynamics.TargetDesc(areaPosition, areaSize);
		}

		public void ClearTiles()
		{
			int num;
			int num1;
			int num2;
			for (int i = 0; i < this.clearTileList.Count; i++)
			{
				this.desc.TileOffsetToXYOffset(this.clearTileList[i], out num, out num1, out num2);
				int num3 = Mathf.Min(num + this.desc.tileSize, this.desc.size) - num;
				int num4 = Mathf.Min(num1 + this.desc.tileSize, this.desc.size) - num1;
				if (!this.owner.useNativePath)
				{
					for (int j = 0; j < num4; j++)
					{
						Array.Clear(this.pixels, num2, num3);
						num2 += this.desc.size;
					}
				}
				else
				{
					WaterDynamics.RasterClearTile_Native(ref this.pixels[0], num2, this.desc.size, num3, num4);
				}
			}
		}

		private Texture2D CreateDynamicTexture(int size)
		{
			return new Texture2D(size, size, TextureFormat.ARGB32, false, true)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
		}

		private RenderTexture CreateRenderTexture(int size)
		{
			RenderTexture renderTexture = new RenderTexture(size, size, 0, (SystemInfoEx.SupportsRenderTextureFormat(RenderTextureFormat.RHalf) ? RenderTextureFormat.RHalf : RenderTextureFormat.RFloat), RenderTextureReadWrite.Linear)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
			renderTexture.Create();
			return renderTexture;
		}

		public void Destroy()
		{
			this.desc.Clear();
		}

		public void Prepare()
		{
		}

		public void ProcessTiles()
		{
			int num;
			int num1;
			ushort num2;
			for (int i = 0; i < this.clearTileList.Count; i++)
			{
				ushort tileXYIndex = this.desc.TileOffsetToTileXYIndex(this.clearTileList[i], out num, out num1, out num2);
				this.clearTileTable[tileXYIndex] = 0;
				this.clearTileList[i] = 65535;
			}
			this.clearTileList.Clear();
			for (int j = 0; j < this.drawTileList.Count; j++)
			{
				ushort tileXYIndex1 = this.desc.TileOffsetToTileXYIndex(this.drawTileList[j], out num, out num1, out num2);
				if (this.clearTileTable[num2] == 0)
				{
					this.clearTileTable[num2] = 1;
					this.clearTileList.Add(num2);
				}
				this.drawTileTable[tileXYIndex1] = 0;
				this.drawTileList[j] = 65535;
			}
			this.drawTileList.Clear();
		}

		public void Update()
		{
		}

		public void UpdateGlobalShaderProperties()
		{
		}

		public void UpdateTiles()
		{
		}
	}

	public struct TargetDesc
	{
		public int size;

		public int maxSize;

		public int maxSizeSubStep;

		public Vector2 areaOffset;

		public Vector2 areaToMapUV;

		public Vector2 areaToMapXY;

		public int tileSize;

		public int tileSizeShift;

		public int tileCount;

		public int tileMaxCount;

		public TargetDesc(Vector3 areaPosition, Vector3 areaSize)
		{
			this.size = 512;
			this.maxSize = this.size - 1;
			this.maxSizeSubStep = this.maxSize * 256;
			this.areaOffset = new Vector2(areaPosition.x, areaPosition.z);
			this.areaToMapUV = new Vector2(1f / areaSize.x, 1f / areaSize.z);
			this.areaToMapXY = this.areaToMapUV * (float)this.size;
			this.tileSize = Mathf.NextPowerOfTwo(Mathf.Max(this.size, 4096)) / 256;
			this.tileSizeShift = (int)Mathf.Log((float)this.tileSize, 2f);
			this.tileCount = Mathf.CeilToInt((float)this.size / (float)this.tileSize);
			this.tileMaxCount = this.tileCount - 1;
		}

		public void Clear()
		{
			this.areaOffset = Vector2.zero;
			this.areaToMapUV = Vector2.zero;
			this.areaToMapXY = Vector2.zero;
			this.size = 0;
			this.maxSize = 0;
			this.maxSizeSubStep = 0;
			this.tileSize = 0;
			this.tileSizeShift = 0;
			this.tileCount = 0;
			this.tileMaxCount = 0;
		}

		public ushort TileOffsetToTileXYIndex(ushort tileOffset, out int tileX, out int tileY, out ushort tileIndex)
		{
			tileX = tileOffset % this.tileCount;
			tileY = tileOffset / this.tileCount;
			tileIndex = (ushort)(tileY * this.tileCount + tileX);
			return tileOffset;
		}

		public ushort TileOffsetToXYOffset(ushort tileOffset, out int x, out int y, out int offset)
		{
			int num = tileOffset % this.tileCount;
			int num1 = tileOffset / this.tileCount;
			x = num * this.tileSize;
			y = num1 * this.tileSize;
			offset = y * this.size + x;
			return tileOffset;
		}

		public Vector2 WorldToRaster(Vector2 pos)
		{
			Vector2 vector2 = new Vector2();
			vector2.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
			vector2.y = (pos.y - this.areaOffset.y) * this.areaToMapXY.y;
			return vector2;
		}

		public Vector3 WorldToRaster(Vector3 pos)
		{
			Vector2 vector2 = new Vector2();
			vector2.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
			vector2.y = (pos.z - this.areaOffset.y) * this.areaToMapXY.y;
			return vector2;
		}
	}
}