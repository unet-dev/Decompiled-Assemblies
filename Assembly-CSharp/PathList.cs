using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PathList
{
	private static Quaternion rot90;

	private static Quaternion rot180;

	public string Name;

	public PathInterpolator Path;

	public bool Spline;

	public bool Start;

	public bool End;

	public float Width;

	public float InnerPadding;

	public float OuterPadding;

	public float InnerFade;

	public float OuterFade;

	public float RandomScale;

	public float MeshOffset;

	public float TerrainOffset;

	public int Topology;

	public int Splat;

	public const float StepSize = 1f;

	public const float MeshStepSize = 8f;

	public const float MeshNormalSmoothing = 0.1f;

	public const int SubMeshVerts = 100;

	private static float[] placements;

	static PathList()
	{
		PathList.rot90 = Quaternion.Euler(0f, 90f, 0f);
		PathList.rot180 = Quaternion.Euler(0f, 180f, 0f);
		PathList.placements = new float[] { default(float), -1f, 1f };
	}

	public PathList(string name, Vector3[] points)
	{
		this.Name = name;
		this.Path = new PathInterpolator(points);
	}

	public void AdjustTerrainHeight()
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float single6 = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float outerFade = this.OuterFade;
		float innerFade = this.InnerFade;
		float terrainOffset = this.TerrainOffset * TerrainMeta.OneOverSize.y;
		float width = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		Vector3 vector32 = PathList.rot90 * startTangent;
		Vector3 vector33 = startPoint - (vector32 * (width + outerPadding + outerFade));
		Vector3 vector34 = startPoint + (vector32 * (width + outerPadding + outerFade));
		float length = this.Path.Length + single6;
		for (float i = 0f; i < length; i += single6)
		{
			Vector3 vector35 = (this.Spline ? this.Path.GetPointCubicHermite(i) : this.Path.GetPoint(i));
			float single7 = (startPoint - vector35).Magnitude2D();
			float single8 = (endPoint - vector35).Magnitude2D();
			float single9 = Mathf.InverseLerp(0f, width, Mathf.Min(single7, single8));
			float single10 = Mathf.Lerp(width, width * randomScale, Noise.Billow(vector35.x, vector35.z, 2, 0.005f, 1f, 2f, 0.5f));
			Vector3 vector36 = this.Path.GetTangent(i).XZ3D();
			startTangent = vector36.normalized;
			vector32 = PathList.rot90 * startTangent;
			Ray ray = new Ray(vector35, startTangent);
			Vector3 vector37 = vector35 - (vector32 * (single10 + outerPadding + outerFade));
			Vector3 vector38 = vector35 + (vector32 * (single10 + outerPadding + outerFade));
			float single11 = TerrainMeta.NormalizeY(vector35.y);
			heightMap.ForEach(vector33, vector34, vector37, vector38, (int x, int z) => {
				float single = heightMap.Coordinate(x);
				float single1 = heightMap.Coordinate(z);
				if ((topologyMap.GetTopology(single, single1) & this.Topology) != 0)
				{
					return;
				}
				Vector3 vector3 = TerrainMeta.Denormalize(new Vector3(single, single11, single1));
				Vector3 vector31 = ray.ClosestPoint(vector3);
				float single2 = (vector3 - vector31).Magnitude2D();
				float single3 = Mathf.InverseLerp(single10 + outerPadding + outerFade, single10 + outerPadding, single2);
				float single4 = Mathf.InverseLerp(single10 - innerPadding, single10 - innerPadding - innerFade, single2);
				float single5 = TerrainMeta.NormalizeY(vector31.y);
				single3 = Mathf.SmoothStep(0f, 1f, single3);
				single4 = Mathf.SmoothStep(0f, 1f, single4);
				heightMap.SetHeight(x, z, single5 + terrainOffset * single4, single9 * single3);
			});
			vector33 = vector37;
			vector34 = vector38;
		}
	}

	public void AdjustTerrainTexture()
	{
		if (this.Splat == 0)
		{
			return;
		}
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		float single4 = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float width = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		Vector3 vector31 = PathList.rot90 * startTangent;
		Vector3 vector32 = startPoint - (vector31 * (width + outerPadding));
		Vector3 vector33 = startPoint + (vector31 * (width + outerPadding));
		float length = this.Path.Length + single4;
		for (float i = 0f; i < length; i += single4)
		{
			Vector3 vector34 = (this.Spline ? this.Path.GetPointCubicHermite(i) : this.Path.GetPoint(i));
			float single5 = (startPoint - vector34).Magnitude2D();
			float single6 = (endPoint - vector34).Magnitude2D();
			float single7 = Mathf.InverseLerp(0f, width, Mathf.Min(single5, single6));
			float single8 = Mathf.Lerp(width, width * randomScale, Noise.Billow(vector34.x, vector34.z, 2, 0.005f, 1f, 2f, 0.5f));
			Vector3 vector35 = this.Path.GetTangent(i).XZ3D();
			startTangent = vector35.normalized;
			vector31 = PathList.rot90 * startTangent;
			Ray ray = new Ray(vector34, startTangent);
			Vector3 vector36 = vector34 - (vector31 * (single8 + outerPadding));
			Vector3 vector37 = vector34 + (vector31 * (single8 + outerPadding));
			float single9 = TerrainMeta.NormalizeY(vector34.y);
			splatMap.ForEach(vector32, vector33, vector36, vector37, (int x, int z) => {
				float single = splatMap.Coordinate(x);
				float single1 = splatMap.Coordinate(z);
				Vector3 vector3 = TerrainMeta.Denormalize(new Vector3(single, single9, single1));
				float single2 = (vector3 - ray.ClosestPoint(vector3)).Magnitude2D();
				float single3 = Mathf.InverseLerp(single8 + outerPadding, single8 - innerPadding, single2);
				splatMap.SetSplat(x, z, this.Splat, single3 * single7);
			});
			vector32 = vector36;
			vector33 = vector37;
		}
	}

	public void AdjustTerrainTopology()
	{
		if (this.Topology == 0)
		{
			return;
		}
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float single3 = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float width = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		Vector3 vector31 = PathList.rot90 * startTangent;
		Vector3 vector32 = startPoint - (vector31 * (width + outerPadding));
		Vector3 vector33 = startPoint + (vector31 * (width + outerPadding));
		float length = this.Path.Length + single3;
		for (float i = 0f; i < length; i += single3)
		{
			Vector3 vector34 = (this.Spline ? this.Path.GetPointCubicHermite(i) : this.Path.GetPoint(i));
			float single4 = (startPoint - vector34).Magnitude2D();
			float single5 = (endPoint - vector34).Magnitude2D();
			float single6 = Mathf.InverseLerp(0f, width, Mathf.Min(single4, single5));
			float single7 = Mathf.Lerp(width, width * randomScale, Noise.Billow(vector34.x, vector34.z, 2, 0.005f, 1f, 2f, 0.5f));
			Vector3 vector35 = this.Path.GetTangent(i).XZ3D();
			startTangent = vector35.normalized;
			vector31 = PathList.rot90 * startTangent;
			Ray ray = new Ray(vector34, startTangent);
			Vector3 vector36 = vector34 - (vector31 * (single7 + outerPadding));
			Vector3 vector37 = vector34 + (vector31 * (single7 + outerPadding));
			float single8 = TerrainMeta.NormalizeY(vector34.y);
			topologyMap.ForEach(vector32, vector33, vector36, vector37, (int x, int z) => {
				float single = topologyMap.Coordinate(x);
				float single1 = topologyMap.Coordinate(z);
				Vector3 vector3 = TerrainMeta.Denormalize(new Vector3(single, single8, single1));
				float single2 = (vector3 - ray.ClosestPoint(vector3)).Magnitude2D();
				if (Mathf.InverseLerp(single7 + outerPadding, single7 - innerPadding, single2) * single6 > 0.3f)
				{
					topologyMap.SetTopology(x, z, this.Topology);
				}
			});
			vector32 = vector36;
			vector33 = vector37;
		}
	}

	private bool CheckObjects(Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		for (int i = 0; i < (int)prefabs.Length; i++)
		{
			Prefab prefab = prefabs[i];
			Vector3 vector3 = position;
			if (!prefab.ApplyTerrainAnchors(ref vector3, rotation, prefab.Object.transform.localScale, filter))
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckObjects(Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = dir.XZ3D().normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 width = (this.Width * 0.5f + obj.Offset) * PathList.rot90 * dir;
		for (int i = 0; i < (int)PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 height = pos + (PathList.placements[i] * width);
				if (obj.HeightToTerrain)
				{
					height.y = TerrainMeta.HeightMap.GetHeight(height);
				}
				if (filter.Test(height))
				{
					if (this.CheckObjects(prefabs, height, (i == 2 ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir)), filter))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public List<Mesh> CreateMesh()
	{
		List<Mesh> meshes = new List<Mesh>();
		float single = 8f;
		float single1 = 64f;
		float randomScale = this.RandomScale;
		float meshOffset = this.MeshOffset;
		float width = this.Width * 0.5f;
		int length = (int)(this.Path.Length / single) * 2;
		int num = (int)(this.Path.Length / single) * 3;
		List<Vector3> vector3s = new List<Vector3>(length);
		List<Color> colors = new List<Color>(length);
		List<Vector2> vector2s = new List<Vector2>(length);
		List<Vector3> vector3s1 = new List<Vector3>(length);
		List<Vector4> vector4s = new List<Vector4>(length);
		List<int> nums = new List<int>(num);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector2 vector2 = new Vector2(0f, 0f);
		Vector2 vector21 = new Vector2(1f, 0f);
		Vector3 vector3 = Vector3.zero;
		Vector3 vector31 = Vector3.zero;
		Vector3 vector32 = Vector3.zero;
		Vector3 vector33 = Vector3.zero;
		int num1 = -1;
		int num2 = -1;
		float length1 = this.Path.Length + single;
		for (float i = 0f; i < length1; i += single)
		{
			Vector3 vector34 = (this.Spline ? this.Path.GetPointCubicHermite(i) : this.Path.GetPoint(i));
			float single2 = Mathf.Lerp(width, width * randomScale, Noise.Billow(vector34.x, vector34.z, 2, 0.005f, 1f, 2f, 0.5f));
			Vector3 tangent = this.Path.GetTangent(i);
			Vector3 vector35 = tangent.XZ3D().normalized;
			Vector3 vector36 = PathList.rot90 * vector35;
			Vector4 vector4 = new Vector4(vector36.x, vector36.y, vector36.z, 1f);
			Vector3 vector37 = Vector3.Slerp(Vector3.Cross(tangent, vector36), Vector3.up, 0.1f);
			Vector3 vector38 = new Vector3(vector34.x - vector36.x * single2, 0f, vector34.z - vector36.z * single2)
			{
				y = Mathf.Min(vector34.y, heightMap.GetHeight(vector38)) + meshOffset
			};
			Vector3 vector39 = new Vector3(vector34.x + vector36.x * single2, 0f, vector34.z + vector36.z * single2)
			{
				y = Mathf.Min(vector34.y, heightMap.GetHeight(vector39)) + meshOffset
			};
			if (i != 0f)
			{
				float single3 = (vector34 - vector32).Magnitude2D() / (2f * single2);
				vector2.y += single3;
				vector21.y += single3;
				if (Vector3.Dot((vector38 - vector3).XZ3D(), vector33) <= 0f)
				{
					vector38 = vector3;
				}
				if (Vector3.Dot((vector39 - vector31).XZ3D(), vector33) <= 0f)
				{
					vector39 = vector31;
				}
			}
			Color color = (i <= 0f || i + single >= length1 ? new Color(1f, 1f, 1f, 0f) : new Color(1f, 1f, 1f, 1f));
			vector2s.Add(vector2);
			colors.Add(color);
			vector3s.Add(vector38);
			vector3s1.Add(vector37);
			vector4s.Add(vector4);
			int count = vector3s.Count - 1;
			if (num1 != -1 && num2 != -1)
			{
				nums.Add(count);
				nums.Add(num2);
				nums.Add(num1);
			}
			num1 = count;
			vector3 = vector38;
			vector2s.Add(vector21);
			colors.Add(color);
			vector3s.Add(vector39);
			vector3s1.Add(vector37);
			vector4s.Add(vector4);
			int count1 = vector3s.Count - 1;
			if (num1 != -1 && num2 != -1)
			{
				nums.Add(count1);
				nums.Add(num2);
				nums.Add(num1);
			}
			num2 = count1;
			vector31 = vector39;
			vector32 = vector34;
			vector33 = vector35;
			if (vector3s.Count >= 100 && this.Path.Length - i > single1)
			{
				Mesh mesh = new Mesh();
				mesh.SetVertices(vector3s);
				mesh.SetColors(colors);
				mesh.SetUVs(0, vector2s);
				mesh.SetTriangles(nums, 0);
				mesh.SetNormals(vector3s1);
				mesh.SetTangents(vector4s);
				meshes.Add(mesh);
				vector3s.Clear();
				colors.Clear();
				vector2s.Clear();
				vector3s1.Clear();
				vector4s.Clear();
				nums.Clear();
				num1 = -1;
				num2 = -1;
				i -= single;
			}
		}
		if (nums.Count > 0)
		{
			Mesh mesh1 = new Mesh();
			mesh1.SetVertices(vector3s);
			mesh1.SetColors(colors);
			mesh1.SetUVs(0, vector2s);
			mesh1.SetTriangles(nums, 0);
			mesh1.SetNormals(vector3s1);
			mesh1.SetTangents(vector4s);
			meshes.Add(mesh1);
		}
		return meshes;
	}

	public void ResetTrims()
	{
		this.Path.MinIndex = this.Path.DefaultMinIndex;
		this.Path.MaxIndex = this.Path.DefaultMaxIndex;
	}

	public void SpawnAlong(ref uint seed, PathList.PathObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] prefabArray = Prefab.Load(string.Concat("assets/bundled/prefabs/autospawn/", obj.Folder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			Debug.LogError(string.Concat("Empty decor folder: ", obj.Folder));
			return;
		}
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float dithering = obj.Dithering;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 startPoint = this.Path.GetStartPoint();
		List<Vector3> vector3s = new List<Vector3>();
		float single = distance * 0.25f;
		float single1 = distance * 0.5f;
		float length = this.Path.Length - this.Path.EndOffset - single1;
		for (float i = this.Path.StartOffset + single1; i <= length; i += single)
		{
			Vector3 vector3 = (this.Spline ? this.Path.GetPointCubicHermite(i) : this.Path.GetPoint(i));
			if ((vector3 - startPoint).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(i);
				Vector3 vector31 = PathList.rot90 * tangent;
				Vector3 height = vector3;
				height.x += SeedRandom.Range(ref seed, -dithering, dithering);
				height.z += SeedRandom.Range(ref seed, -dithering, dithering);
				float single2 = TerrainMeta.NormalizeX(height.x);
				float single3 = TerrainMeta.NormalizeZ(height.z);
				if (filter.GetFactor(single2, single3) >= SeedRandom.Value(ref seed))
				{
					if (density >= SeedRandom.Value(ref seed))
					{
						height.y = heightMap.GetHeight(single2, single3);
						if (obj.Alignment == PathList.Alignment.None)
						{
							if (this.SpawnObject(ref seed, prefabArray, height, Quaternion.identity, filter))
							{
								goto Label1;
							}
							goto Label0;
						}
						else if (obj.Alignment == PathList.Alignment.Forward)
						{
							if (this.SpawnObject(ref seed, prefabArray, height, Quaternion.LookRotation(tangent), filter))
							{
								goto Label1;
							}
							goto Label0;
						}
						else if (obj.Alignment != PathList.Alignment.Inward)
						{
							vector3s.Add(height);
						}
						else
						{
							if (this.SpawnObject(ref seed, prefabArray, height, Quaternion.LookRotation(vector31), filter))
							{
								goto Label1;
							}
							goto Label0;
						}
					}
				Label1:
					startPoint = vector3;
				}
			}
		Label0:
		}
		if (vector3s.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, prefabArray, vector3s, filter);
		}
	}

	public void SpawnBridge(ref uint seed, PathList.BridgeObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] prefabArray = Prefab.Load(string.Concat("assets/bundled/prefabs/autospawn/", obj.Folder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			Debug.LogError(string.Concat("Empty decor folder: ", obj.Folder));
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint() - startPoint;
		float single = endPoint.magnitude;
		Vector3 vector3 = endPoint / single;
		float distance = single / obj.Distance;
		int num = Mathf.RoundToInt(distance);
		float single1 = 0.5f * (distance - (float)num);
		Vector3 distance1 = obj.Distance * vector3;
		Vector3 vector31 = startPoint + ((0.5f + single1) * distance1);
		Quaternion quaternion = Quaternion.LookRotation(vector3);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		for (int i = 0; i < num; i++)
		{
			if (vector31.y > Mathf.Max(heightMap.GetHeight(vector31), waterMap.GetHeight(vector31)) - 1f)
			{
				this.SpawnObject(ref seed, prefabArray, vector31, quaternion, null);
			}
			vector31 += distance1;
		}
	}

	public void SpawnEnd(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] prefabArray = Prefab.Load(string.Concat("assets/bundled/prefabs/autospawn/", obj.Folder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			Debug.LogError(string.Concat("Empty decor folder: ", obj.Folder));
			return;
		}
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 endTangent = -this.Path.GetEndTangent();
		this.SpawnObject(ref seed, prefabArray, endPoint, endTangent, obj);
	}

	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		Prefab random = prefabs.GetRandom<Prefab>(ref seed);
		Vector3 vector3 = position;
		Quaternion quaternion = rotation;
		Vector3 obj = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref vector3, ref quaternion, ref obj);
		if (!random.ApplyTerrainAnchors(ref vector3, quaternion, obj, filter))
		{
			return false;
		}
		random.ApplyTerrainPlacements(vector3, quaternion, obj);
		random.ApplyTerrainModifiers(vector3, quaternion, obj);
		World.AddPrefab(this.Name, random.ID, vector3, quaternion, obj);
		return true;
	}

	private void SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = dir.XZ3D().normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 width = (this.Width * 0.5f + obj.Offset) * PathList.rot90 * dir;
		for (int i = 0; i < (int)PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 height = pos + (PathList.placements[i] * width);
				if (obj.HeightToTerrain)
				{
					height.y = TerrainMeta.HeightMap.GetHeight(height);
				}
				if (filter.Test(height))
				{
					if (this.SpawnObject(ref seed, prefabs, height, (i == 2 ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir)), filter))
					{
						break;
					}
				}
			}
		}
	}

	private void SpawnObjectsNeighborAligned(ref uint seed, Prefab[] prefabs, List<Vector3> positions, SpawnFilter filter = null)
	{
		if (positions.Count < 2)
		{
			return;
		}
		for (int i = 0; i < positions.Count; i++)
		{
			int num = Mathf.Max(i - 1, 0);
			int num1 = Mathf.Min(i + 1, positions.Count - 1);
			Vector3 item = positions[i];
			Quaternion quaternion = Quaternion.LookRotation((positions[num1] - positions[num]).XZ3D());
			this.SpawnObject(ref seed, prefabs, item, quaternion, filter);
		}
	}

	public void SpawnSide(ref uint seed, PathList.SideObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] prefabArray = Prefab.Load(string.Concat("assets/bundled/prefabs/autospawn/", obj.Folder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			Debug.LogError(string.Concat("Empty decor folder: ", obj.Folder));
			return;
		}
		PathList.Side side = obj.Side;
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float width = this.Width * 0.5f + obj.Offset;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float[] singleArray = new float[] { -width, width };
		int num = 0;
		Vector3 startPoint = this.Path.GetStartPoint();
		List<Vector3> vector3s = new List<Vector3>();
		float single = distance * 0.25f;
		float single1 = distance * 0.5f;
		float length = this.Path.Length - this.Path.EndOffset - single1;
		for (float i = this.Path.StartOffset + single1; i <= length; i += single)
		{
			Vector3 vector3 = (this.Spline ? this.Path.GetPointCubicHermite(i) : this.Path.GetPoint(i));
			if ((vector3 - startPoint).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(i);
				Vector3 vector31 = PathList.rot90 * tangent;
				for (int j = 0; j < (int)singleArray.Length; j++)
				{
					int length1 = (num + j) % (int)singleArray.Length;
					if ((side != PathList.Side.Left || length1 == 0) && (side != PathList.Side.Right || length1 == 1))
					{
						float single2 = singleArray[length1];
						Vector3 height = vector3;
						ref float singlePointer = ref height.x;
						singlePointer = singlePointer + vector31.x * single2;
						ref float singlePointer1 = ref height.z;
						singlePointer1 = singlePointer1 + vector31.z * single2;
						float single3 = TerrainMeta.NormalizeX(height.x);
						float single4 = TerrainMeta.NormalizeZ(height.z);
						if (filter.GetFactor(single3, single4) >= SeedRandom.Value(ref seed))
						{
							if (density >= SeedRandom.Value(ref seed))
							{
								height.y = heightMap.GetHeight(single3, single4);
								if (obj.Alignment == PathList.Alignment.None)
								{
									if (this.SpawnObject(ref seed, prefabArray, height, Quaternion.identity, filter))
									{
										goto Label1;
									}
									goto Label0;
								}
								else if (obj.Alignment == PathList.Alignment.Forward)
								{
									if (this.SpawnObject(ref seed, prefabArray, height, Quaternion.LookRotation(tangent * single2), filter))
									{
										goto Label1;
									}
									goto Label0;
								}
								else if (obj.Alignment != PathList.Alignment.Inward)
								{
									vector3s.Add(height);
								}
								else
								{
									if (this.SpawnObject(ref seed, prefabArray, height, Quaternion.LookRotation(-vector31 * single2), filter))
									{
										goto Label1;
									}
									goto Label0;
								}
							}
						Label1:
							num = length1;
							startPoint = vector3;
							if (side == PathList.Side.Any)
							{
								break;
							}
						}
					}
				Label0:
				}
			}
		}
		if (vector3s.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, prefabArray, vector3s, filter);
		}
	}

	public void SpawnStart(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] prefabArray = Prefab.Load(string.Concat("assets/bundled/prefabs/autospawn/", obj.Folder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			Debug.LogError(string.Concat("Empty decor folder: ", obj.Folder));
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		this.SpawnObject(ref seed, prefabArray, startPoint, startTangent, obj);
	}

	public void TrimEnd(PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] prefabArray = Prefab.Load(string.Concat("assets/bundled/prefabs/autospawn/", obj.Folder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			Debug.LogError(string.Concat("Empty decor folder: ", obj.Folder));
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int length = (int)points.Length / 4;
		for (int i = 0; i < length; i++)
		{
			if (this.CheckObjects(prefabArray, points[this.Path.MaxIndex - i], -tangents[this.Path.MaxIndex - i], obj))
			{
				PathInterpolator path = this.Path;
				path.MaxIndex = path.MaxIndex - i;
				return;
			}
		}
	}

	public void TrimStart(PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] prefabArray = Prefab.Load(string.Concat("assets/bundled/prefabs/autospawn/", obj.Folder), null, null, true);
		if (prefabArray == null || prefabArray.Length == 0)
		{
			Debug.LogError(string.Concat("Empty decor folder: ", obj.Folder));
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int length = (int)points.Length / 4;
		for (int i = 0; i < length; i++)
		{
			if (this.CheckObjects(prefabArray, points[this.Path.MinIndex + i], tangents[this.Path.MinIndex + i], obj))
			{
				PathInterpolator path = this.Path;
				path.MinIndex = path.MinIndex + i;
				return;
			}
		}
	}

	public enum Alignment
	{
		None,
		Neighbor,
		Forward,
		Inward
	}

	[Serializable]
	public class BasicObject
	{
		public string Folder;

		public SpawnFilter Filter;

		public PathList.Placement Placement;

		public bool AlignToNormal;

		public bool HeightToTerrain;

		public float Offset;

		public BasicObject()
		{
		}
	}

	[Serializable]
	public class BridgeObject
	{
		public string Folder;

		public float Distance;

		public BridgeObject()
		{
		}
	}

	[Serializable]
	public class PathObject
	{
		public string Folder;

		public SpawnFilter Filter;

		public PathList.Alignment Alignment;

		public float Density;

		public float Distance;

		public float Dithering;

		public PathObject()
		{
		}
	}

	public enum Placement
	{
		Center,
		Side
	}

	public enum Side
	{
		Both,
		Left,
		Right,
		Any
	}

	[Serializable]
	public class SideObject
	{
		public string Folder;

		public SpawnFilter Filter;

		public PathList.Side Side;

		public PathList.Alignment Alignment;

		public float Density;

		public float Distance;

		public float Offset;

		public SideObject()
		{
		}
	}
}