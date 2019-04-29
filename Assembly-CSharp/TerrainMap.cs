using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class TerrainMap : TerrainExtension
{
	internal int res;

	protected TerrainMap()
	{
	}

	public void ApplyFilter(float normX, float normZ, float radius, float fade, Action<int, int, float> action)
	{
		object obj;
		float oneOverSize = TerrainMeta.OneOverSize.x * radius;
		float single = TerrainMeta.OneOverSize.x * fade;
		float single1 = (float)this.res * (oneOverSize - single);
		float single2 = (float)this.res * oneOverSize;
		float single3 = normX * (float)this.res;
		float single4 = normZ * (float)this.res;
		int num = this.Index(normX - oneOverSize);
		int num1 = this.Index(normX + oneOverSize);
		int num2 = this.Index(normZ - oneOverSize);
		int num3 = this.Index(normZ + oneOverSize);
		if (single1 != single2)
		{
			for (int i = num2; i <= num3; i++)
			{
				for (int j = num; j <= num1; j++)
				{
					Vector2 vector2 = new Vector2((float)j + 0.5f - single3, (float)i + 0.5f - single4);
					float single5 = Mathf.InverseLerp(single2, single1, vector2.magnitude);
					action(j, i, single5);
				}
			}
			return;
		}
		for (int k = num2; k <= num3; k++)
		{
			for (int l = num; l <= num1; l++)
			{
				if ((new Vector2((float)l + 0.5f - single3, (float)k + 0.5f - single4)).magnitude < single2)
				{
					obj = 1;
				}
				else
				{
					obj = null;
				}
				action(l, k, (float)obj);
			}
		}
	}

	public float Coordinate(int index)
	{
		return ((float)index + 0.5f) / (float)this.res;
	}

	public void ForEach(Vector3 worldPos, float radius, Action<int, int> action)
	{
		int num = this.Index(TerrainMeta.NormalizeX(worldPos.x - radius));
		int num1 = this.Index(TerrainMeta.NormalizeX(worldPos.x + radius));
		int num2 = this.Index(TerrainMeta.NormalizeZ(worldPos.z - radius));
		int num3 = this.Index(TerrainMeta.NormalizeZ(worldPos.z + radius));
		for (int i = num2; i <= num3; i++)
		{
			for (int j = num; j <= num1; j++)
			{
				action(j, i);
			}
		}
	}

	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		Vector2i vector2i = new Vector2i(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i vector2i1 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i vector2i2 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		this.ForEach(vector2i, vector2i1, vector2i2, action);
	}

	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		Vector2i vector2i = new Vector2i(-2147483648, -2147483648);
		Vector2i vector2i1 = new Vector2i(2147483647, 2147483647);
		this.ForEachInternal(v0, v1, v2, action, vector2i, vector2i1);
	}

	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		Vector2i vector2i = new Vector2i(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i vector2i1 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i vector2i2 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i vector2i3 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v3.x)), this.Index(TerrainMeta.NormalizeZ(v3.z)));
		this.ForEach(vector2i, vector2i1, vector2i2, vector2i3, action);
	}

	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		Vector2i vector2i = new Vector2i(-2147483648, -2147483648);
		Vector2i vector2i1 = new Vector2i(2147483647, 2147483647);
		this.ForEachInternal(v0, v1, v2, v3, action, vector2i, vector2i1);
	}

	public void ForEach(int x_min, int x_max, int z_min, int z_max, Action<int, int> action)
	{
		for (int i = z_min; i <= z_max; i++)
		{
			for (int j = x_min; j <= x_max; j++)
			{
				action(j, i);
			}
		}
	}

	public void ForEach(Action<int, int> action)
	{
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				action(j, i);
			}
		}
	}

	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action, Vector2i min, Vector2i max)
	{
		int num = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x));
		int num1 = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x));
		int num2 = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y));
		int num3 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y));
		int num4 = v0.y - v1.y;
		int num5 = v1.x - v0.x;
		int num6 = v1.y - v2.y;
		int num7 = v2.x - v1.x;
		int num8 = v2.y - v0.y;
		int num9 = v0.x - v2.x;
		Vector2i vector2i = new Vector2i(num, num2);
		int num10 = (v2.x - v1.x) * (vector2i.y - v1.y) - (v2.y - v1.y) * (vector2i.x - v1.x);
		int num11 = (v0.x - v2.x) * (vector2i.y - v2.y) - (v0.y - v2.y) * (vector2i.x - v2.x);
		int num12 = (v1.x - v0.x) * (vector2i.y - v0.y) - (v1.y - v0.y) * (vector2i.x - v0.x);
		vector2i.y = num2;
		while (vector2i.y <= num3)
		{
			int num13 = num10;
			int num14 = num11;
			int num15 = num12;
			vector2i.x = num;
			while (vector2i.x <= num1)
			{
				if ((num13 | num14 | num15) >= 0)
				{
					action(vector2i.x, vector2i.y);
				}
				num13 += num6;
				num14 += num8;
				num15 += num4;
				vector2i.x++;
			}
			num10 += num7;
			num11 += num9;
			num12 += num5;
			vector2i.y++;
		}
	}

	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action, Vector2i min, Vector2i max)
	{
		int num = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x, v3.x));
		int num1 = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x, v3.x));
		int num2 = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y, v3.y));
		int num3 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y, v3.y));
		int num4 = v0.y - v1.y;
		int num5 = v1.x - v0.x;
		int num6 = v1.y - v2.y;
		int num7 = v2.x - v1.x;
		int num8 = v2.y - v0.y;
		int num9 = v0.x - v2.x;
		int num10 = v3.y - v2.y;
		int num11 = v2.x - v3.x;
		int num12 = v2.y - v1.y;
		int num13 = v1.x - v2.x;
		int num14 = v1.y - v3.y;
		int num15 = v3.x - v1.x;
		Vector2i vector2i = new Vector2i(num, num2);
		int num16 = (v2.x - v1.x) * (vector2i.y - v1.y) - (v2.y - v1.y) * (vector2i.x - v1.x);
		int num17 = (v0.x - v2.x) * (vector2i.y - v2.y) - (v0.y - v2.y) * (vector2i.x - v2.x);
		int num18 = (v1.x - v0.x) * (vector2i.y - v0.y) - (v1.y - v0.y) * (vector2i.x - v0.x);
		int num19 = (v1.x - v2.x) * (vector2i.y - v2.y) - (v1.y - v2.y) * (vector2i.x - v2.x);
		int num20 = (v3.x - v1.x) * (vector2i.y - v1.y) - (v3.y - v1.y) * (vector2i.x - v1.x);
		int num21 = (v2.x - v3.x) * (vector2i.y - v3.y) - (v2.y - v3.y) * (vector2i.x - v3.x);
		vector2i.y = num2;
		while (vector2i.y <= num3)
		{
			int num22 = num16;
			int num23 = num17;
			int num24 = num18;
			int num25 = num19;
			int num26 = num20;
			int num27 = num21;
			vector2i.x = num;
			while (vector2i.x <= num1)
			{
				if ((num22 | num23 | num24) >= 0 || (num25 | num26 | num27) >= 0)
				{
					action(vector2i.x, vector2i.y);
				}
				num22 += num6;
				num23 += num8;
				num24 += num4;
				num25 += num12;
				num26 += num14;
				num27 += num10;
				vector2i.x++;
			}
			num16 += num7;
			num17 += num9;
			num18 += num5;
			num19 += num13;
			num20 += num15;
			num21 += num11;
			vector2i.y++;
		}
	}

	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		Vector2i vector2i = new Vector2i(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i vector2i1 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i vector2i2 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		this.ForEachParallel(vector2i, vector2i1, vector2i2, action);
	}

	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		int num = Mathx.Min(v0.x, v1.x, v2.x);
		int num1 = Mathx.Max(v0.x, v1.x, v2.x);
		int num2 = Mathx.Min(v0.y, v1.y, v2.y);
		int num3 = Mathx.Max(v0.y, v1.y, v2.y);
		Vector2i vector2i1 = new Vector2i(num, num2);
		Vector2i vector2i2 = new Vector2i(num1, num3);
		Vector2i vector2i3 = (vector2i2 - vector2i1) + Vector2i.one;
		Parallel.Call((int thread_id, int thread_count) => {
			Vector2i baseMin = vector2i1 + ((vector2i3 * thread_id) / thread_count);
			Vector2i vector2i = (vector2i1 + ((vector2i3 * (thread_id + 1)) / thread_count)) - Vector2i.one;
			this.ForEachInternal(v0, v1, v2, action, baseMin, vector2i);
		});
	}

	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		Vector2i vector2i = new Vector2i(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i vector2i1 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i vector2i2 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i vector2i3 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v3.x)), this.Index(TerrainMeta.NormalizeZ(v3.z)));
		this.ForEachParallel(vector2i, vector2i1, vector2i2, vector2i3, action);
	}

	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		int num = Mathx.Min(v0.x, v1.x, v2.x, v3.x);
		int num1 = Mathx.Max(v0.x, v1.x, v2.x, v3.x);
		int num2 = Mathx.Min(v0.y, v1.y, v2.y, v3.y);
		int num3 = Mathx.Max(v0.y, v1.y, v2.y, v3.y);
		Vector2i vector2i1 = new Vector2i(num, num2);
		Vector2i vector2i2 = (new Vector2i(num1, num3) - vector2i1) + Vector2i.one;
		Vector2i vector2i3 = new Vector2i(vector2i2.x, 0);
		Vector2i vector2i4 = new Vector2i(0, vector2i2.y);
		Parallel.Call((int thread_id, int thread_count) => {
			Vector2i baseMin = vector2i1 + ((vector2i4 * thread_id) / thread_count);
			Vector2i vector2i = ((vector2i1 + ((vector2i4 * (thread_id + 1)) / thread_count)) + vector2i3) - Vector2i.one;
			this.ForEachInternal(v0, v1, v2, v3, action, baseMin, vector2i);
		});
	}

	public int Index(float normalized)
	{
		int num = (int)(normalized * (float)this.res);
		if (num < 0)
		{
			return 0;
		}
		if (num <= this.res - 1)
		{
			return num;
		}
		return this.res - 1;
	}
}