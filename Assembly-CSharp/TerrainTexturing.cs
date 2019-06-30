using Rust;
using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainTexturing : TerrainExtension
{
	public bool debugFoliageDisplacement;

	private bool initialized;

	private static TerrainTexturing instance;

	private const int ShoreVectorDownscale = 3;

	private const int ShoreVectorBlurPasses = 0;

	private float terrainSize;

	private int shoreMapSize;

	private float[] shoreDistances;

	private Vector2[] shoreVectors;

	public static TerrainTexturing Instance
	{
		get
		{
			return TerrainTexturing.instance;
		}
	}

	public TerrainTexturing()
	{
	}

	private void Awake()
	{
		this.CheckInstance();
	}

	private void CheckInstance()
	{
		TerrainTexturing.instance = (TerrainTexturing.instance != null ? TerrainTexturing.instance : this);
	}

	private void GenerateShoreVector()
	{
		using (TimeWarning timeWarning = TimeWarning.New("GenerateShoreVector", 0.5f))
		{
			this.GenerateShoreVector(out this.shoreDistances, out this.shoreVectors);
		}
	}

	private void GenerateShoreVector(out float[] distances, out Vector2[] vectors)
	{
		object obj;
		object obj1;
		float single = this.terrainSize / (float)this.shoreMapSize;
		Vector3 position = this.terrain.GetPosition();
		int layer = LayerMask.NameToLayer("Terrain");
		NativeArray<RaycastHit> raycastHits = new NativeArray<RaycastHit>(this.shoreMapSize * this.shoreMapSize, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		NativeArray<RaycastCommand> raycastCommands = new NativeArray<RaycastCommand>(this.shoreMapSize * this.shoreMapSize, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		for (int i = 0; i < this.shoreMapSize; i++)
		{
			for (int j = 0; j < this.shoreMapSize; j++)
			{
				float single1 = ((float)j + 0.5f) * single;
				float single2 = ((float)i + 0.5f) * single;
				Vector3 vector3 = new Vector3(position.x, 0f, position.z) + new Vector3(single1, 1000f, single2);
				Vector3 vector31 = Vector3.down;
				raycastCommands[i * this.shoreMapSize + j] = new RaycastCommand(vector3, vector31, Single.MaxValue, -5, 1);
			}
		}
		JobHandle jobHandle = new JobHandle();
		RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, 1, jobHandle).Complete();
		byte[] numArray = new byte[this.shoreMapSize * this.shoreMapSize];
		distances = new float[this.shoreMapSize * this.shoreMapSize];
		vectors = new Vector2[this.shoreMapSize * this.shoreMapSize];
		int num = 0;
		int num1 = 0;
		while (num < this.shoreMapSize)
		{
			int num2 = 0;
			while (num2 < this.shoreMapSize)
			{
				RaycastHit item = raycastHits[num * this.shoreMapSize + num2];
				bool flag = item.collider.gameObject.layer == layer;
				byte[] numArray1 = numArray;
				int num3 = num1;
				if (flag)
				{
					obj = 255;
				}
				else
				{
					obj = null;
				}
				numArray1[num3] = (byte)obj;
				float[] singleArray = distances;
				int num4 = num1;
				if (flag)
				{
					obj1 = 256;
				}
				else
				{
					obj1 = null;
				}
				singleArray[num4] = (float)obj1;
				num2++;
				num1++;
			}
			num++;
		}
		byte num5 = 127;
		DistanceField.Generate(ref this.shoreMapSize, ref num5, ref numArray, ref distances);
		DistanceField.ApplyGaussianBlur(this.shoreMapSize, distances, 0);
		DistanceField.GenerateVectors(ref this.shoreMapSize, ref distances, ref vectors);
		raycastHits.Dispose();
		raycastCommands.Dispose();
	}

	public float GetCoarseDistanceToShore(Vector3 pos)
	{
		Vector2 vector2 = new Vector2();
		vector2.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		vector2.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return this.GetCoarseDistanceToShore(vector2);
	}

	public float GetCoarseDistanceToShore(Vector2 uv)
	{
		int num = this.shoreMapSize;
		int num1 = num - 1;
		float single = uv.x * (float)num1;
		float single1 = uv.y * (float)num1;
		int num2 = (int)single;
		int num3 = (int)single1;
		float single2 = single - (float)num2;
		float single3 = single1 - (float)num3;
		num2 = (num2 >= 0 ? num2 : 0);
		num3 = (num3 >= 0 ? num3 : 0);
		num2 = (num2 <= num1 ? num2 : num1);
		num3 = (num3 <= num1 ? num3 : num1);
		int num4 = (single < (float)num1 ? 1 : 0);
		int num5 = (single1 < (float)num1 ? num : 0);
		int num6 = num3 * num + num2;
		int num7 = num6 + num4;
		int num8 = num6 + num5;
		int num9 = num8 + num4;
		float single4 = this.shoreDistances[num6];
		float single5 = this.shoreDistances[num7];
		float single6 = this.shoreDistances[num8];
		float single7 = (single5 - single4) * single2 + single4;
		return ((this.shoreDistances[num9] - single6) * single2 + single6 - single7) * single3 + single7;
	}

	public Vector3 GetCoarseVectorToShore(Vector3 pos)
	{
		Vector2 vector2 = new Vector2();
		vector2.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		vector2.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return this.GetCoarseVectorToShore(vector2);
	}

	public Vector3 GetCoarseVectorToShore(Vector2 uv)
	{
		Vector3 vector3 = new Vector3();
		Vector2 vector2 = new Vector2();
		Vector2 vector21 = new Vector2();
		int num = this.shoreMapSize;
		int num1 = num - 1;
		float single = uv.x * (float)num1;
		float single1 = uv.y * (float)num1;
		int num2 = (int)single;
		int num3 = (int)single1;
		float single2 = single - (float)num2;
		float single3 = single1 - (float)num3;
		num2 = (num2 >= 0 ? num2 : 0);
		num3 = (num3 >= 0 ? num3 : 0);
		num2 = (num2 <= num1 ? num2 : num1);
		num3 = (num3 <= num1 ? num3 : num1);
		int num4 = (single < (float)num1 ? 1 : 0);
		int num5 = (single1 < (float)num1 ? num : 0);
		int num6 = num3 * num + num2;
		int num7 = num6 + num4;
		int num8 = num6 + num5;
		int num9 = num8 + num4;
		Vector2 vector22 = this.shoreVectors[num6];
		Vector2 vector23 = this.shoreVectors[num7];
		Vector2 vector24 = this.shoreVectors[num8];
		Vector2 vector25 = this.shoreVectors[num9];
		vector2.x = (vector23.x - vector22.x) * single2 + vector22.x;
		vector2.y = (vector23.y - vector22.y) * single2 + vector22.y;
		vector21.x = (vector25.x - vector24.x) * single2 + vector24.x;
		vector21.y = (vector25.y - vector24.y) * single2 + vector24.y;
		vector3.x = (vector21.x - vector2.x) * single3 + vector2.x;
		vector3.y = (vector21.y - vector2.y) * single3 + vector2.y;
		float single4 = this.shoreDistances[num6];
		float single5 = this.shoreDistances[num7];
		float single6 = this.shoreDistances[num8];
		float single7 = (single5 - single4) * single2 + single4;
		float single8 = (this.shoreDistances[num9] - single6) * single2 + single6;
		vector3.z = (single8 - single7) * single3 + single7;
		return vector3;
	}

	public void GetCoarseVectorToShoreArray(Vector2[] uv, Vector3[] shore)
	{
		Debug.Assert((int)uv.Length == (int)shore.Length);
		for (int i = 0; i < (int)uv.Length; i++)
		{
			shore[i] = this.GetCoarseVectorToShore(uv[i]);
		}
	}

	private void InitializeBasePyramid()
	{
	}

	private void InitializeCoarseHeightSlope()
	{
	}

	private void InitializeShoreVector()
	{
		int num = Mathf.ClosestPowerOfTwo(this.terrain.terrainData.heightmapResolution) >> 3;
		int num1 = num * num;
		this.terrainSize = Mathf.Max(this.terrain.terrainData.size.x, this.terrain.terrainData.size.z);
		this.shoreMapSize = num;
		this.shoreDistances = new float[num * num];
		this.shoreVectors = new Vector2[num * num];
		for (int i = 0; i < num1; i++)
		{
			this.shoreDistances[i] = 10000f;
			this.shoreVectors[i] = Vector2.one;
		}
	}

	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Shutdown();
	}

	private void OnEnable()
	{
		this.CheckInstance();
	}

	public override void PostSetup()
	{
		TerrainMeta component = base.GetComponent<TerrainMeta>();
		if (component == null || component.config == null)
		{
			Debug.LogError("[TerrainTexturing] Missing TerrainMeta or TerrainConfig not assigned.");
			return;
		}
		this.Shutdown();
		this.InitializeCoarseHeightSlope();
		this.GenerateShoreVector();
		this.initialized = true;
	}

	private void ReleaseBasePyramid()
	{
	}

	private void ReleaseCoarseHeightSlope()
	{
	}

	private void ReleaseShoreVector()
	{
		this.shoreDistances = null;
		this.shoreVectors = null;
	}

	public override void Setup()
	{
		this.InitializeShoreVector();
	}

	private void Shutdown()
	{
		this.ReleaseBasePyramid();
		this.ReleaseCoarseHeightSlope();
		this.ReleaseShoreVector();
		this.initialized = false;
	}

	private void Update()
	{
		if (!this.initialized)
		{
			return;
		}
		this.UpdateBasePyramid();
		this.UpdateCoarseHeightSlope();
	}

	private void UpdateBasePyramid()
	{
	}

	private void UpdateCoarseHeightSlope()
	{
	}
}