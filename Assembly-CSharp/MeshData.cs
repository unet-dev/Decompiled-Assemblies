using ConVar;
using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public List<int> triangles;

	public List<Vector3> vertices;

	public List<Vector3> normals;

	public List<Vector4> tangents;

	public List<Color32> colors32;

	public List<Vector2> uv;

	public List<Vector2> uv2;

	public List<Vector4> positions;

	public MeshData()
	{
	}

	public void Alloc()
	{
		if (this.triangles == null)
		{
			this.triangles = Facepunch.Pool.GetList<int>();
		}
		if (this.vertices == null)
		{
			this.vertices = Facepunch.Pool.GetList<Vector3>();
		}
		if (this.normals == null)
		{
			this.normals = Facepunch.Pool.GetList<Vector3>();
		}
		if (this.tangents == null)
		{
			this.tangents = Facepunch.Pool.GetList<Vector4>();
		}
		if (this.colors32 == null)
		{
			this.colors32 = Facepunch.Pool.GetList<Color32>();
		}
		if (this.uv == null)
		{
			this.uv = Facepunch.Pool.GetList<Vector2>();
		}
		if (this.uv2 == null)
		{
			this.uv2 = Facepunch.Pool.GetList<Vector2>();
		}
		if (this.positions == null)
		{
			this.positions = Facepunch.Pool.GetList<Vector4>();
		}
	}

	public void Apply(UnityEngine.Mesh mesh)
	{
		mesh.Clear();
		if (this.vertices != null)
		{
			mesh.SetVertices(this.vertices);
		}
		if (this.triangles != null)
		{
			mesh.SetTriangles(this.triangles, 0);
		}
		if (this.normals != null)
		{
			if (this.normals.Count == this.vertices.Count)
			{
				mesh.SetNormals(this.normals);
			}
			else if (this.normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh normals because some meshes were missing them.");
			}
		}
		if (this.tangents != null)
		{
			if (this.tangents.Count == this.vertices.Count)
			{
				mesh.SetTangents(this.tangents);
			}
			else if (this.tangents.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh tangents because some meshes were missing them.");
			}
		}
		if (this.colors32 != null)
		{
			if (this.colors32.Count == this.vertices.Count)
			{
				mesh.SetColors(this.colors32);
			}
			else if (this.colors32.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh colors because some meshes were missing them.");
			}
		}
		if (this.uv != null)
		{
			if (this.uv.Count == this.vertices.Count)
			{
				mesh.SetUVs(0, this.uv);
			}
			else if (this.uv.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh uvs because some meshes were missing them.");
			}
		}
		if (this.uv2 != null)
		{
			if (this.uv2.Count == this.vertices.Count)
			{
				mesh.SetUVs(1, this.uv2);
			}
			else if (this.uv2.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping mesh uv2s because some meshes were missing them.");
			}
		}
		if (this.positions != null)
		{
			mesh.SetUVs(2, this.positions);
		}
	}

	public void Clear()
	{
		if (this.triangles != null)
		{
			this.triangles.Clear();
		}
		if (this.vertices != null)
		{
			this.vertices.Clear();
		}
		if (this.normals != null)
		{
			this.normals.Clear();
		}
		if (this.tangents != null)
		{
			this.tangents.Clear();
		}
		if (this.colors32 != null)
		{
			this.colors32.Clear();
		}
		if (this.uv != null)
		{
			this.uv.Clear();
		}
		if (this.uv2 != null)
		{
			this.uv2.Clear();
		}
		if (this.positions != null)
		{
			this.positions.Clear();
		}
	}

	public void Combine(MeshGroup meshGroup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshInstance item = meshGroup.data[i];
			Matrix4x4 matrix4x4 = Matrix4x4.TRS(item.position, item.rotation, item.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < (int)item.data.triangles.Length; j++)
			{
				this.triangles.Add(count + item.data.triangles[j]);
			}
			for (int k = 0; k < (int)item.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x4.MultiplyPoint3x4(item.data.vertices[k]));
				this.positions.Add(item.position);
			}
			for (int l = 0; l < (int)item.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x4.MultiplyVector(item.data.normals[l]));
			}
			for (int m = 0; m < (int)item.data.tangents.Length; m++)
			{
				Vector4 vector4 = item.data.tangents[m];
				Vector3 vector3 = new Vector3(vector4.x, vector4.y, vector4.z);
				Vector3 vector31 = matrix4x4.MultiplyVector(vector3);
				this.tangents.Add(new Vector4(vector31.x, vector31.y, vector31.z, vector4.w));
			}
			for (int n = 0; n < (int)item.data.colors32.Length; n++)
			{
				this.colors32.Add(item.data.colors32[n]);
			}
			for (int o = 0; o < (int)item.data.uv.Length; o++)
			{
				this.uv.Add(item.data.uv[o]);
			}
			for (int p = 0; p < (int)item.data.uv2.Length; p++)
			{
				this.uv2.Add(item.data.uv2[p]);
			}
		}
	}

	public void Free()
	{
		if (this.triangles != null)
		{
			Facepunch.Pool.FreeList<int>(ref this.triangles);
		}
		if (this.vertices != null)
		{
			Facepunch.Pool.FreeList<Vector3>(ref this.vertices);
		}
		if (this.normals != null)
		{
			Facepunch.Pool.FreeList<Vector3>(ref this.normals);
		}
		if (this.tangents != null)
		{
			Facepunch.Pool.FreeList<Vector4>(ref this.tangents);
		}
		if (this.colors32 != null)
		{
			Facepunch.Pool.FreeList<Color32>(ref this.colors32);
		}
		if (this.uv != null)
		{
			Facepunch.Pool.FreeList<Vector2>(ref this.uv);
		}
		if (this.uv2 != null)
		{
			Facepunch.Pool.FreeList<Vector2>(ref this.uv2);
		}
		if (this.positions != null)
		{
			Facepunch.Pool.FreeList<Vector4>(ref this.positions);
		}
	}
}