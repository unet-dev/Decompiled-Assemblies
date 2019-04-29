using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GenerateRiverLayout : ProceduralComponent
{
	public const float Width = 24f;

	public const float InnerPadding = 0.5f;

	public const float OuterPadding = 0.5f;

	public const float InnerFade = 8f;

	public const float OuterFade = 16f;

	public const float RandomScale = 0.75f;

	public const float MeshOffset = -0.4f;

	public const float TerrainOffset = -2f;

	public GenerateRiverLayout()
	{
	}

	public override void Process(uint seed)
	{
		List<PathList> pathLists = new List<PathList>();
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		List<Vector3> vector3s = new List<Vector3>();
		for (float i = TerrainMeta.Position.z; i < TerrainMeta.Position.z + TerrainMeta.Size.z; i += 50f)
		{
			for (float j = TerrainMeta.Position.x; j < TerrainMeta.Position.x + TerrainMeta.Size.x; j += 50f)
			{
				Vector3 vector3 = new Vector3(j, 0f, i);
				float height = heightMap.GetHeight(vector3);
				float single = height;
				vector3.y = height;
				float single1 = single;
				if (vector3.y > 5f)
				{
					Vector3 normal = heightMap.GetNormal(vector3);
					if (normal.y > 0.01f)
					{
						Vector2 vector2 = new Vector2(normal.x, normal.z);
						Vector2 vector21 = vector2.normalized;
						vector3s.Add(vector3);
						float single2 = 12f;
						int num = 12;
						for (int k = 0; k < 10000; k++)
						{
							vector3.x += vector21.x;
							vector3.z += vector21.y;
							if (heightMap.GetSlope(vector3) > 30f)
							{
								break;
							}
							float height1 = heightMap.GetHeight(vector3);
							if (height1 > single1 + 10f)
							{
								break;
							}
							vector3.y = Mathf.Min(height1, single1);
							vector3s.Add(vector3);
							int topology = topologyMap.GetTopology(vector3, single2);
							int topology1 = topologyMap.GetTopology(vector3);
							int num1 = 2694148;
							int num2 = 128;
							if ((topology & num1) != 0)
							{
								break;
							}
							if ((topology1 & num2) != 0)
							{
								int num3 = num - 1;
								num = num3;
								if (num3 <= 0)
								{
									if (vector3s.Count < 300)
									{
										break;
									}
									PathList pathList = new PathList(string.Concat("River ", pathLists.Count), vector3s.ToArray())
									{
										Width = 24f,
										InnerPadding = 0.5f,
										OuterPadding = 0.5f,
										InnerFade = 8f,
										OuterFade = 16f,
										RandomScale = 0.75f,
										MeshOffset = -0.4f,
										TerrainOffset = -2f,
										Topology = 16384,
										Splat = 64,
										Start = true,
										End = true
									};
									pathLists.Add(pathList);
									break;
								}
							}
							normal = heightMap.GetNormal(vector3);
							vector2 = new Vector2(vector21.x + 0.15f * normal.x, vector21.y + 0.15f * normal.z);
							vector21 = vector2.normalized;
							single1 = vector3.y;
						}
						vector3s.Clear();
					}
				}
			}
		}
		pathLists.Sort((PathList a, PathList b) => ((int)b.Path.Points.Length).CompareTo((int)a.Path.Points.Length));
		int num4 = Mathf.RoundToInt(10f * TerrainMeta.Size.x * TerrainMeta.Size.z * 1E-06f);
		int num5 = Mathf.NextPowerOfTwo((int)((float)((float)World.Size) / 24f));
		bool[,] flagArray = new bool[num5, num5];
		for (int l = 0; l < pathLists.Count; l++)
		{
			if (l < num4)
			{
				PathList item = pathLists[l];
				for (int m = 0; m < l; m++)
				{
					if (Vector3.Distance(pathLists[m].Path.GetStartPoint(), item.Path.GetStartPoint()) < 100f)
					{
						int num6 = l;
						l = num6 - 1;
						pathLists.RemoveUnordered<PathList>(num6);
					}
				}
				int num7 = -1;
				int num8 = -1;
				for (int n = 0; n < (int)item.Path.Points.Length; n++)
				{
					Vector3 points = item.Path.Points[n];
					int num9 = Mathf.Clamp((int)(TerrainMeta.NormalizeX(points.x) * (float)num5), 0, num5 - 1);
					int num10 = Mathf.Clamp((int)(TerrainMeta.NormalizeZ(points.z) * (float)num5), 0, num5 - 1);
					if (num7 != num9 || num8 != num10)
					{
						if (!flagArray[num10, num9])
						{
							num7 = num9;
							num8 = num10;
							flagArray[num10, num9] = true;
						}
						else
						{
							int num11 = l;
							l = num11 - 1;
							pathLists.RemoveUnordered<PathList>(num11);
							break;
						}
					}
				}
			}
			else
			{
				int num12 = l;
				l = num12 - 1;
				pathLists.RemoveUnordered<PathList>(num12);
			}
		}
		TerrainMeta.Path.Rivers.AddRange(pathLists);
	}
}