using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AsyncTerrainNavMeshBake : CustomYieldInstruction
{
	private List<int> indices;

	private List<Vector3> vertices;

	private List<Vector3> normals;

	private List<int> triangles;

	private Vector3 pivot;

	private int width;

	private int height;

	private bool normal;

	private bool alpha;

	private Action worker;

	public bool isDone
	{
		get
		{
			return this.worker == null;
		}
	}

	public override bool keepWaiting
	{
		get
		{
			return this.worker != null;
		}
	}

	public Mesh mesh
	{
		get
		{
			Mesh mesh = new Mesh();
			if (this.vertices != null)
			{
				mesh.SetVertices(this.vertices);
				Pool.FreeList<Vector3>(ref this.vertices);
			}
			if (this.normals != null)
			{
				mesh.SetNormals(this.normals);
				Pool.FreeList<Vector3>(ref this.normals);
			}
			if (this.triangles != null)
			{
				mesh.SetTriangles(this.triangles, 0);
				Pool.FreeList<int>(ref this.triangles);
			}
			if (this.indices != null)
			{
				Pool.FreeList<int>(ref this.indices);
			}
			return mesh;
		}
	}

	public AsyncTerrainNavMeshBake(Vector3 pivot, int width, int height, bool normal, bool alpha)
	{
		List<Vector3> list;
		this.pivot = pivot;
		this.width = width;
		this.height = height;
		this.normal = normal;
		this.alpha = alpha;
		this.indices = Pool.GetList<int>();
		this.vertices = Pool.GetList<Vector3>();
		if (normal)
		{
			list = Pool.GetList<Vector3>();
		}
		else
		{
			list = null;
		}
		this.normals = list;
		this.triangles = Pool.GetList<int>();
		this.Invoke();
	}

	private void Callback(IAsyncResult result)
	{
		this.worker.EndInvoke(result);
		this.worker = null;
	}

	public NavMeshBuildSource CreateNavMeshBuildSource(bool addSourceObject)
	{
		NavMeshBuildSource navMeshBuildSource = new NavMeshBuildSource()
		{
			transform = Matrix4x4.TRS(this.pivot, Quaternion.identity, Vector3.one),
			shape = NavMeshBuildSourceShape.Mesh
		};
		if (addSourceObject)
		{
			navMeshBuildSource.sourceObject = this.mesh;
		}
		return navMeshBuildSource;
	}

	private void DoWork()
	{
		Vector3 vector3 = new Vector3((float)(this.width / 2), 0f, (float)(this.height / 2));
		Vector3 vector31 = new Vector3(this.pivot.x - vector3.x, 0f, this.pivot.z - vector3.z);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainAlphaMap alphaMap = TerrainMeta.AlphaMap;
		int num = 0;
		for (int i = 0; i <= this.height; i++)
		{
			int num1 = 0;
			while (num1 <= this.width)
			{
				Vector3 vector32 = new Vector3((float)num1, 0f, (float)i) + vector31;
				Vector3 vector33 = new Vector3((float)num1, 0f, (float)i) - vector3;
				float height = heightMap.GetHeight(vector32);
				if (height < -1f)
				{
					this.indices.Add(-1);
				}
				else if (!this.alpha || alphaMap.GetAlpha(vector32) >= 0.1f)
				{
					if (this.normal)
					{
						Vector3 normal = heightMap.GetNormal(vector32);
						this.normals.Add(normal);
					}
					float single = height - this.pivot.y;
					float single1 = single;
					vector33.y = single;
					vector32.y = single1;
					this.indices.Add(this.vertices.Count);
					this.vertices.Add(vector33);
				}
				else
				{
					this.indices.Add(-1);
				}
				num1++;
				num++;
			}
		}
		int num2 = 0;
		int num3 = 0;
		while (num3 < this.height)
		{
			int num4 = 0;
			while (num4 < this.width)
			{
				int item = this.indices[num2];
				int item1 = this.indices[num2 + this.width + 1];
				int item2 = this.indices[num2 + 1];
				int item3 = this.indices[num2 + 1];
				int item4 = this.indices[num2 + this.width + 1];
				int item5 = this.indices[num2 + this.width + 2];
				if (item != -1 && item1 != -1 && item2 != -1)
				{
					this.triangles.Add(item);
					this.triangles.Add(item1);
					this.triangles.Add(item2);
				}
				if (item3 != -1 && item4 != -1 && item5 != -1)
				{
					this.triangles.Add(item3);
					this.triangles.Add(item4);
					this.triangles.Add(item5);
				}
				num4++;
				num2++;
			}
			num3++;
			num2++;
		}
	}

	private void Invoke()
	{
		this.worker = new Action(this.DoWork);
		this.worker.BeginInvoke(new AsyncCallback(this.Callback), null);
	}
}