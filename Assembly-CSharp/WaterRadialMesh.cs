using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaterRadialMesh
{
	private const float AlignmentGranularity = 1f;

	private const float MaxHorizontalDisplacement = 1f;

	private Mesh[] meshes;

	private bool initialized;

	public bool IsInitialized
	{
		get
		{
			return this.initialized;
		}
	}

	public Mesh[] Meshes
	{
		get
		{
			return this.meshes;
		}
	}

	public WaterRadialMesh()
	{
	}

	public Matrix4x4 ComputeLocalToWorldMatrix(Camera camera, float oceanWaterLevel)
	{
		if (camera == null)
		{
			return Matrix4x4.identity;
		}
		Matrix4x4 matrix4x4 = camera.worldToCameraMatrix;
		Vector3 vector3 = matrix4x4.MultiplyVector(Vector3.up);
		matrix4x4 = camera.worldToCameraMatrix;
		Vector3 vector31 = matrix4x4.MultiplyVector(Vector3.Cross(camera.transform.forward, Vector3.up));
		Vector3 vector32 = new Vector3(vector3.x, vector3.y, 0f);
		vector3 = (vector32.normalized * 0.5f) + new Vector3(0.5f, 0f, 0.5f);
		vector32 = new Vector3(vector31.x, vector31.y, 0f);
		vector31 = vector32.normalized * 0.5f;
		Vector3 vector33 = this.RaycastPlane(camera, oceanWaterLevel, vector3 - vector31);
		Vector3 vector34 = this.RaycastPlane(camera, oceanWaterLevel, vector3 + vector31);
		float single = Mathf.Min(camera.farClipPlane, 5000f);
		Vector3 vector35 = camera.transform.position;
		Vector3 vector36 = new Vector3()
		{
			x = single * Mathf.Tan(camera.fieldOfView * 0.5f * 0.0174532924f) * camera.aspect + 2f,
			y = single,
			z = single
		};
		float single1 = Mathf.Abs(vector34.x - vector33.x);
		float single2 = Mathf.Min(vector33.z, vector34.z) - (single1 + 2f) * vector36.z / vector36.x;
		Vector3 vector37 = camera.transform.forward;
		vector37.y = 0f;
		vector37.Normalize();
		vector36.z -= single2;
		vector35 = new Vector3(vector35.x, oceanWaterLevel, vector35.z) + (vector37 * single2);
		Quaternion quaternion = Quaternion.AngleAxis(Mathf.Atan2(vector37.x, vector37.z) * 57.29578f, Vector3.up);
		return Matrix4x4.TRS(vector35, quaternion, vector36);
	}

	private Mesh CreateMesh(string name, Vector3[] vertices, int[] indices)
	{
		Mesh mesh = new Mesh()
		{
			hideFlags = HideFlags.DontSave,
			name = name,
			vertices = vertices
		};
		mesh.SetIndices(indices, MeshTopology.Quads, 0);
		mesh.RecalculateBounds();
		mesh.UploadMeshData(true);
		return mesh;
	}

	public void Destroy()
	{
		if (this.initialized)
		{
			Mesh[] meshArray = this.meshes;
			for (int i = 0; i < (int)meshArray.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(meshArray[i]);
			}
			this.meshes = null;
			this.initialized = false;
		}
	}

	private Mesh[] GenerateMeshes(int vertexCount, bool volume = false)
	{
		int num = Mathf.RoundToInt((float)Mathf.RoundToInt(Mathf.Sqrt((float)vertexCount)) * 0.4f);
		int num1 = Mathf.RoundToInt((float)vertexCount / (float)num);
		int num2 = (volume ? num1 / 2 : num1);
		List<Mesh> meshes = new List<Mesh>();
		List<Vector3> vector3s = new List<Vector3>();
		List<int> nums = new List<int>();
		Vector2[] vector2Array = new Vector2[num];
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < num; i++)
		{
			float single = ((float)i / (float)(num - 1) * 2f - 1f) * 3.14159274f * 0.25f;
			Vector2 vector2 = new Vector2(Mathf.Sin(single), Mathf.Cos(single));
			vector2Array[i] = vector2.normalized;
		}
		for (int j = 0; j < num2; j++)
		{
			float single1 = (float)j / (float)(num1 - 1);
			single1 = 1f - Mathf.Cos(single1 * 3.14159274f * 0.5f);
			for (int k = 0; k < num; k++)
			{
				Vector2 vector21 = vector2Array[k] * single1;
				if (j < num2 - 2 || !volume)
				{
					vector3s.Add(new Vector3(vector21.x, 0f, vector21.y));
				}
				else if (j != num2 - 2)
				{
					vector3s.Add(new Vector3(vector21.x * 10f, -0.9f, vector21.y * -10f) * 0.5f);
				}
				else
				{
					vector3s.Add(new Vector3(vector21.x * 10f, -0.9f, vector21.y) * 0.5f);
				}
				if (k != 0 && j != 0 && num3 > num)
				{
					nums.Add(num3);
					nums.Add(num3 - num);
					nums.Add(num3 - num - 1);
					nums.Add(num3 - 1);
				}
				num3++;
				if (num3 >= 65000)
				{
					meshes.Add(this.CreateMesh(string.Concat(new object[] { "WaterMesh_", num, "x", num1, "_", num4 }), vector3s.ToArray(), nums.ToArray()));
					k--;
					j--;
					single1 = 1f - Mathf.Cos((float)j / (float)(num1 - 1) * 3.14159274f * 0.5f);
					num3 = 0;
					vector3s.Clear();
					nums.Clear();
					num4++;
				}
			}
		}
		if (num3 != 0)
		{
			meshes.Add(this.CreateMesh(string.Concat(new object[] { "WaterMesh_", num, "x", num1, "_", num4 }), vector3s.ToArray(), nums.ToArray()));
		}
		return meshes.ToArray();
	}

	public void Initialize(int vertexCount)
	{
		this.meshes = this.GenerateMeshes(vertexCount, false);
		this.initialized = true;
	}

	private Vector3 RaycastPlane(Camera camera, float planeHeight, Vector3 pos)
	{
		Ray ray = camera.ViewportPointToRay(pos);
		if (camera.transform.position.y > planeHeight)
		{
			if (ray.direction.y > -0.01f)
			{
				ray.direction = new Vector3(ray.direction.x, -ray.direction.y - 0.02f, ray.direction.z);
			}
		}
		else if (ray.direction.y < 0.01f)
		{
			ray.direction = new Vector3(ray.direction.x, -ray.direction.y + 0.02f, ray.direction.z);
		}
		float single = -(ray.origin.y - planeHeight) / ray.direction.y;
		return Quaternion.AngleAxis(-camera.transform.eulerAngles.y, Vector3.up) * ray.direction * single;
	}
}