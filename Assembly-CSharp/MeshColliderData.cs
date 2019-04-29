using ConVar;
using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderData
{
	public List<int> triangles;

	public List<Vector3> vertices;

	public List<Vector3> normals;

	public MeshColliderData()
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
				return;
			}
			if (this.normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping collider normals because some meshes were missing them.");
			}
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
	}

	public void Combine(MeshColliderGroup meshGroup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance item = meshGroup.data[i];
			Matrix4x4 matrix4x4 = Matrix4x4.TRS(item.position, item.rotation, item.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < (int)item.data.triangles.Length; j++)
			{
				this.triangles.Add(count + item.data.triangles[j]);
			}
			for (int k = 0; k < (int)item.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x4.MultiplyPoint3x4(item.data.vertices[k]));
			}
			for (int l = 0; l < (int)item.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x4.MultiplyVector(item.data.normals[l]));
			}
		}
	}

	public void Combine(MeshColliderGroup meshGroup, MeshColliderLookup colliderLookup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance item = meshGroup.data[i];
			Matrix4x4 matrix4x4 = Matrix4x4.TRS(item.position, item.rotation, item.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < (int)item.data.triangles.Length; j++)
			{
				this.triangles.Add(count + item.data.triangles[j]);
			}
			for (int k = 0; k < (int)item.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x4.MultiplyPoint3x4(item.data.vertices[k]));
			}
			for (int l = 0; l < (int)item.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x4.MultiplyVector(item.data.normals[l]));
			}
			colliderLookup.Add(item);
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
	}
}