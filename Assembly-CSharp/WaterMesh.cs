using System;
using UnityEngine;

[Serializable]
public class WaterMesh
{
	private Mesh borderMesh;

	private Mesh centerPatch;

	private int borderRingCount;

	private float borderRingSpacingFalloff;

	private int resolution;

	private Vector3[] borderVerticesLocal;

	private Vector3[] borderVerticesWorld;

	private bool initialized;

	public Mesh BorderMesh
	{
		get
		{
			return this.borderMesh;
		}
	}

	public Mesh CenterPatch
	{
		get
		{
			return this.centerPatch;
		}
	}

	public bool IsInitialized
	{
		get
		{
			return this.initialized;
		}
	}

	public WaterMesh()
	{
	}

	private Mesh CreateSortedBorderPatch(int resolution, int ringCount, float sizeInWorld)
	{
		float single = sizeInWorld / (float)resolution;
		int num = resolution * 4 * (ringCount + 1);
		Vector3[] vector3 = new Vector3[num];
		Vector3[] vector3Array = new Vector3[num];
		Color[] colorArray = new Color[num];
		int[] numArray = new int[resolution * 4 * ringCount * 6];
		for (int i = 0; i < num; i++)
		{
			vector3Array[i] = Vector3.up;
		}
		for (int j = 0; j < num; j++)
		{
			colorArray[j] = Color.clear;
		}
		int num1 = resolution;
		int num2 = resolution * 4;
		float single1 = (float)num1 * single;
		Vector3 vector31 = new Vector3(sizeInWorld, 0f, sizeInWorld) * 0.5f;
		int num3 = 0;
		int num4 = 0;
		while (num3 < ringCount + 1)
		{
			Vector3 vector32 = -vector31;
			for (int k = 0; k < num1; k++)
			{
				int num5 = num4;
				num4 = num5 + 1;
				vector3[num5] = vector32 + new Vector3((float)k * single, 0f, 0f);
			}
			for (int l = 0; l < num1; l++)
			{
				int num6 = num4;
				num4 = num6 + 1;
				vector3[num6] = vector32 + new Vector3(single1, 0f, (float)l * single);
			}
			for (int m = num1; m > 0; m--)
			{
				int num7 = num4;
				num4 = num7 + 1;
				vector3[num7] = vector32 + new Vector3((float)m * single, 0f, single1);
			}
			for (int n = num1; n > 0; n--)
			{
				int num8 = num4;
				num4 = num8 + 1;
				vector3[num8] = vector32 + new Vector3(0f, 0f, (float)n * single);
			}
			num3++;
		}
		int num9 = 0;
		int num10 = 0;
		while (num9 < ringCount)
		{
			int num11 = num9 * num2;
			int num12 = num11 + num2;
			int num13 = num12;
			int num14 = num11 + num2 * 2;
			int num15 = num11;
			int num16 = num11 + num2 + 1;
			int num17 = num11 + num2;
			int num18 = num11 + 1;
			int num19 = num11 + num2 + 1;
			int num20 = num11;
			for (int o = 0; o < num2; o++)
			{
				bool flag = o % resolution == 0;
				int num21 = num15;
				int num22 = (flag ? num16 - num2 : num16);
				int num23 = num17;
				int num24 = num18;
				int num25 = num19;
				int num26 = (flag ? num20 + num2 : num20);
				if (num22 >= num14)
				{
					num22 = num13;
				}
				if (num24 >= num12)
				{
					num24 = num11;
				}
				if (num25 >= num14)
				{
					num25 = num13;
				}
				int num27 = num10;
				num10 = num27 + 1;
				numArray[num27] = num23;
				int num28 = num10;
				num10 = num28 + 1;
				numArray[num28] = num22;
				int num29 = num10;
				num10 = num29 + 1;
				numArray[num29] = num21;
				int num30 = num10;
				num10 = num30 + 1;
				numArray[num30] = num26;
				int num31 = num10;
				num10 = num31 + 1;
				numArray[num31] = num25;
				int num32 = num10;
				num10 = num32 + 1;
				numArray[num32] = num24;
				num15++;
				num16++;
				num17++;
				num18++;
				num19++;
				num20++;
			}
			num9++;
		}
		Mesh mesh = new Mesh()
		{
			hideFlags = HideFlags.DontSave,
			vertices = vector3,
			normals = vector3Array,
			colors = colorArray,
			triangles = numArray
		};
		mesh.RecalculateBounds();
		return mesh;
	}

	private Mesh CreateSortedCenterPatch(int resolution, float sizeInWorld, bool borderOnly)
	{
		int num;
		int num1;
		Vector3 vector3 = new Vector3();
		float single = sizeInWorld / (float)resolution;
		int num2 = resolution + 1;
		if (!borderOnly)
		{
			num = num2 * num2;
			num1 = resolution * resolution * 6;
		}
		else
		{
			num = resolution * 8 - 8;
			num1 = (resolution - 1) * 24;
		}
		Vector3[] vector3Array = new Vector3[num];
		Vector3[] vector3Array1 = new Vector3[num];
		Color[] colorArray = new Color[num];
		int[] numArray = new int[num1];
		for (int i = 0; i < num; i++)
		{
			vector3Array1[i] = Vector3.up;
		}
		int num3 = resolution / 2;
		int num4 = num3 - 1;
		int num5 = resolution;
		int num6 = resolution * 4;
		Vector3 vector31 = new Vector3(sizeInWorld, 0f, sizeInWorld) * 0.5f;
		if (!borderOnly)
		{
			for (int j = 0; j < num; j++)
			{
				if (j < num6)
				{
					colorArray[j] = Color.clear;
				}
				else
				{
					colorArray[j] = Color.white;
				}
			}
		}
		else
		{
			for (int k = 0; k < num; k++)
			{
				colorArray[k] = Color.clear;
			}
		}
		int num7 = 0;
		int num8 = 0;
		while (num7 < num3 + 1)
		{
			vector3.x = (float)num7 * single;
			vector3.y = 0f;
			vector3.z = vector3.x;
			vector3 -= vector31;
			float single1 = (float)num5 * single;
			if (num7 > num4)
			{
				int num9 = num8;
				num8 = num9 + 1;
				vector3Array[num9] = vector3;
			}
			else
			{
				for (int l = 0; l < num5; l++)
				{
					int num10 = num8;
					num8 = num10 + 1;
					vector3Array[num10] = vector3 + new Vector3((float)l * single, 0f, 0f);
				}
				for (int m = 0; m < num5; m++)
				{
					int num11 = num8;
					num8 = num11 + 1;
					vector3Array[num11] = vector3 + new Vector3(single1, 0f, (float)m * single);
				}
				for (int n = num5; n > 0; n--)
				{
					int num12 = num8;
					num8 = num12 + 1;
					vector3Array[num12] = vector3 + new Vector3((float)n * single, 0f, single1);
				}
				for (int o = num5; o > 0; o--)
				{
					int num13 = num8;
					num8 = num13 + 1;
					vector3Array[num13] = vector3 + new Vector3(0f, 0f, (float)o * single);
				}
			}
			num5 -= 2;
			if (borderOnly && num7 >= 1)
			{
				break;
			}
			num7++;
		}
		int num14 = resolution;
		int num15 = resolution - 2;
		int num16 = resolution * 4;
		int num17 = num16 - 8;
		int num18 = (resolution - 1) * 4;
		int num19 = num18 - 8;
		int num20 = 0;
		int num21 = num16;
		int num22 = 0;
		int num23 = 0;
		while (num22 < num3)
		{
			if (num22 >= num4)
			{
				int num24 = num21;
				int num25 = num21 - 1;
				int num26 = num20;
				int num27 = 0;
				for (int p = 0; p < num18; p++)
				{
					int num28 = num24;
					int num29 = num25;
					int num30 = num26;
					num25 = num26;
					num26++;
					int num31 = num24;
					int num32 = num25;
					int num33 = num26;
					num25++;
					num26++;
					if ((num27 & 1) != 0)
					{
						int num34 = num23;
						num23 = num34 + 1;
						numArray[num34] = num30;
						int num35 = num23;
						num23 = num35 + 1;
						numArray[num35] = num29;
						int num36 = num23;
						num23 = num36 + 1;
						numArray[num36] = num33;
						int num37 = num23;
						num23 = num37 + 1;
						numArray[num37] = num33;
						int num38 = num23;
						num23 = num38 + 1;
						numArray[num38] = num29;
						int num39 = num23;
						num23 = num39 + 1;
						numArray[num39] = num28;
					}
					else
					{
						int num40 = num23;
						num23 = num40 + 1;
						numArray[num40] = num33;
						int num41 = num23;
						num23 = num41 + 1;
						numArray[num41] = num32;
						int num42 = num23;
						num23 = num42 + 1;
						numArray[num42] = num31;
						int num43 = num23;
						num23 = num43 + 1;
						numArray[num43] = num30;
						int num44 = num23;
						num23 = num44 + 1;
						numArray[num44] = num29;
						int num45 = num23;
						num23 = num45 + 1;
						numArray[num45] = num28;
					}
					num27++;
				}
			}
			else
			{
				int num46 = num21;
				int num47 = num21 - 1;
				int num48 = num20;
				int num49 = 0;
				bool flag = true;
				for (int q = 0; q < num18; q++)
				{
					int num50 = num46;
					int num51 = num47;
					int num52 = num48;
					num47 = num48;
					num48++;
					int num53 = num46;
					int num54 = num47;
					int num55 = num48;
					bool flag1 = (num49 & 1) == 0;
					if (flag1 || borderOnly & flag && !flag1)
					{
						int num56 = num23;
						num23 = num56 + 1;
						numArray[num56] = num55;
						int num57 = num23;
						num23 = num57 + 1;
						numArray[num57] = num54;
						int num58 = num23;
						num23 = num58 + 1;
						numArray[num58] = num53;
						int num59 = num23;
						num23 = num59 + 1;
						numArray[num59] = num52;
						int num60 = num23;
						num23 = num60 + 1;
						numArray[num60] = num51;
						int num61 = num23;
						num23 = num61 + 1;
						numArray[num61] = num50;
					}
					else
					{
						int num62 = num23;
						num23 = num62 + 1;
						numArray[num62] = num52;
						int num63 = num23;
						num23 = num63 + 1;
						numArray[num63] = num51;
						int num64 = num23;
						num23 = num64 + 1;
						numArray[num64] = num55;
						int num65 = num23;
						num23 = num65 + 1;
						numArray[num65] = num55;
						int num66 = num23;
						num23 = num66 + 1;
						numArray[num66] = num51;
						int num67 = num23;
						num23 = num67 + 1;
						numArray[num67] = num50;
					}
					flag = (q + 1) % (num14 - 1) == 0;
					if (!flag)
					{
						num47 = num46;
						num46 = (num46 + 1 < num21 + num17 ? num46 + 1 : num21);
					}
					else
					{
						num47++;
						num48++;
						num49++;
					}
				}
				num18 -= 8;
				num19 -= 8;
				num16 -= 8;
				num17 -= 8;
				num14 -= 2;
				num15 -= 2;
				num20 = num21;
				num21 += num16;
			}
			if (borderOnly)
			{
				break;
			}
			num22++;
		}
		Mesh mesh = new Mesh()
		{
			hideFlags = HideFlags.DontSave,
			vertices = vector3Array,
			normals = vector3Array1,
			colors = colorArray,
			triangles = numArray
		};
		mesh.RecalculateBounds();
		return mesh;
	}

	public void Destroy()
	{
		if (this.initialized)
		{
			UnityEngine.Object.DestroyImmediate(this.borderMesh);
			UnityEngine.Object.DestroyImmediate(this.centerPatch);
			this.initialized = false;
		}
	}

	public void Initialize(int patchResolution, float patchSizeInWorld, int borderRingCount, float borderRingSpacingFalloff)
	{
		if (!Mathf.IsPowerOfTwo(patchResolution))
		{
			Debug.LogError("[Water] Patch resolution must be a power-of-two number.");
			return;
		}
		this.borderRingCount = borderRingCount;
		this.borderRingSpacingFalloff = borderRingSpacingFalloff;
		this.borderMesh = this.CreateSortedBorderPatch(patchResolution, borderRingCount, patchSizeInWorld);
		this.centerPatch = this.CreateSortedCenterPatch(patchResolution, patchSizeInWorld, false);
		this.resolution = patchResolution;
		this.borderVerticesLocal = new Vector3[this.borderMesh.vertexCount];
		this.borderVerticesWorld = new Vector3[this.borderMesh.vertexCount];
		Array.Copy(this.borderMesh.vertices, this.borderVerticesLocal, this.borderMesh.vertexCount);
		this.initialized = true;
	}

	public void UpdateBorderMesh(Matrix4x4 centerLocalToWorld, Matrix4x4 borderLocalToWorld, bool collapseCenter)
	{
		int num = this.resolution * 4;
		int num1 = 0;
		int num2 = num;
		int num3 = this.borderMesh.vertexCount - num;
		int num4 = this.borderMesh.vertexCount;
		Vector3 vector3 = new Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue);
		Vector3 vector31 = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
		Bounds bound = new Bounds();
		for (int i = num1; i < num2; i++)
		{
			Vector3 vector32 = borderLocalToWorld.MultiplyPoint3x4(this.borderVerticesLocal[i]);
			vector3 = Vector3.Min(vector3, vector32);
			vector31 = Vector3.Max(vector31, vector32);
			this.borderVerticesWorld[i] = vector32;
		}
		bound.SetMinMax(vector3, vector31);
		if (collapseCenter)
		{
			for (int j = num3; j < num4; j++)
			{
				this.borderVerticesWorld[j] = centerLocalToWorld.MultiplyPoint3x4(Vector3.zero);
			}
		}
		else
		{
			for (int k = num3; k < num4; k++)
			{
				this.borderVerticesWorld[k] = centerLocalToWorld.MultiplyPoint3x4(this.borderVerticesLocal[k]);
			}
		}
		int num5 = 1;
		int num6 = num2;
		while (num5 < this.borderRingCount)
		{
			float single = (float)num5 / (float)this.borderRingCount;
			single = Mathf.Clamp01(Mathf.Pow(single, this.borderRingSpacingFalloff));
			int num7 = 0;
			while (num7 < num)
			{
				Vector3 vector33 = this.borderVerticesWorld[num1 + num7];
				Vector3 vector34 = this.borderVerticesWorld[num3 + num7];
				this.borderVerticesWorld[num6].x = vector33.x + (vector34.x - vector33.x) * single;
				this.borderVerticesWorld[num6].y = vector33.y + (vector34.y - vector33.y) * single;
				this.borderVerticesWorld[num6].z = vector33.z + (vector34.z - vector33.z) * single;
				num7++;
				num6++;
			}
			num5++;
		}
		this.borderMesh.vertices = this.borderVerticesWorld;
		this.borderMesh.bounds = bound;
	}
}