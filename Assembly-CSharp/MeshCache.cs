using System;
using System.Collections.Generic;
using UnityEngine;

public static class MeshCache
{
	public static Dictionary<Mesh, MeshCache.Data> dictionary;

	static MeshCache()
	{
		MeshCache.dictionary = new Dictionary<Mesh, MeshCache.Data>();
	}

	public static MeshCache.Data Get(Mesh mesh)
	{
		MeshCache.Data datum;
		if (!MeshCache.dictionary.TryGetValue(mesh, out datum))
		{
			datum = new MeshCache.Data()
			{
				mesh = mesh,
				vertices = mesh.vertices,
				normals = mesh.normals,
				tangents = mesh.tangents,
				colors32 = mesh.colors32,
				triangles = mesh.triangles,
				uv = mesh.uv,
				uv2 = mesh.uv2,
				uv3 = mesh.uv3,
				uv4 = mesh.uv4
			};
			MeshCache.dictionary.Add(mesh, datum);
		}
		return datum;
	}

	[Serializable]
	public class Data
	{
		public Mesh mesh;

		public Vector3[] vertices;

		public Vector3[] normals;

		public Vector4[] tangents;

		public Color32[] colors32;

		public int[] triangles;

		public Vector2[] uv;

		public Vector2[] uv2;

		public Vector2[] uv3;

		public Vector2[] uv4;

		public Data()
		{
		}
	}
}