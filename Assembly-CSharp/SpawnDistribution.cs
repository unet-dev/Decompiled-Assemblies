using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDistribution
{
	internal SpawnHandler Handler;

	public float Density;

	internal int Count;

	private WorldSpaceGrid<int> grid;

	private Dictionary<uint, int> dict = new Dictionary<uint, int>();

	private ByteQuadtree quadtree = new ByteQuadtree();

	private Vector3 origin;

	private Vector3 area;

	public SpawnDistribution(SpawnHandler handler, byte[] baseValues, Vector3 origin, Vector3 area)
	{
		this.Handler = handler;
		this.quadtree.UpdateValues(baseValues);
		this.origin = origin;
		float single = 0f;
		for (int i = 0; i < (int)baseValues.Length; i++)
		{
			single += (float)baseValues[i];
		}
		this.Density = single / (float)(255 * (int)baseValues.Length);
		this.Count = 0;
		this.area = new Vector3(area.x / (float)this.quadtree.Size, area.y, area.z / (float)this.quadtree.Size);
		this.grid = new WorldSpaceGrid<int>(area.x, 20f);
	}

	public void AddInstance(Spawnable spawnable)
	{
		this.UpdateCount(spawnable, 1);
	}

	public int GetCount(uint prefabID)
	{
		int num;
		this.dict.TryGetValue(prefabID, out num);
		return num;
	}

	public int GetCount(Vector3 position)
	{
		return this.grid[position];
	}

	public float GetGridCellArea()
	{
		return this.grid.CellArea;
	}

	public void RemoveInstance(Spawnable spawnable)
	{
		this.UpdateCount(spawnable, -1);
	}

	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, bool alignToNormal = false, float dithering = 0f)
	{
		return this.Sample(out spawnPos, out spawnRot, this.SampleNode(), alignToNormal, dithering);
	}

	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, ByteQuadtree.Element node, bool alignToNormal = false, float dithering = 0f)
	{
		RaycastHit raycastHit;
		if (this.Handler == null || TerrainMeta.HeightMap == null)
		{
			spawnPos = Vector3.zero;
			spawnRot = Quaternion.identity;
			return false;
		}
		LayerMask placementMask = this.Handler.PlacementMask;
		LayerMask placementCheckMask = this.Handler.PlacementCheckMask;
		float placementCheckHeight = this.Handler.PlacementCheckHeight;
		LayerMask radiusCheckMask = this.Handler.RadiusCheckMask;
		float radiusCheckDistance = this.Handler.RadiusCheckDistance;
		for (int i = 0; i < 15; i++)
		{
			spawnPos = this.origin;
			ref float coords = ref spawnPos.x;
			coords = coords + node.Coords.x * this.area.x;
			ref float singlePointer = ref spawnPos.z;
			singlePointer = singlePointer + node.Coords.y * this.area.z;
			ref float singlePointer1 = ref spawnPos.x;
			singlePointer1 = singlePointer1 + UnityEngine.Random.@value * this.area.x;
			ref float singlePointer2 = ref spawnPos.z;
			singlePointer2 = singlePointer2 + UnityEngine.Random.@value * this.area.z;
			spawnPos.x += UnityEngine.Random.Range(-dithering, dithering);
			spawnPos.z += UnityEngine.Random.Range(-dithering, dithering);
			Vector3 vector3 = new Vector3(spawnPos.x, TerrainMeta.HeightMap.GetHeight(spawnPos), spawnPos.z);
			if (vector3.y > spawnPos.y)
			{
				if (placementCheckMask != 0 && Physics.Raycast(vector3 + (Vector3.up * placementCheckHeight), Vector3.down, out raycastHit, placementCheckHeight, placementCheckMask))
				{
					if ((1 << (raycastHit.transform.gameObject.layer & 31) & placementMask) == 0)
					{
						goto Label0;
					}
					vector3.y = raycastHit.point.y;
				}
				if (radiusCheckMask == 0 || !Physics.CheckSphere(vector3, radiusCheckDistance, radiusCheckMask))
				{
					spawnPos.y = vector3.y;
					spawnRot = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
					if (alignToNormal)
					{
						Vector3 normal = TerrainMeta.HeightMap.GetNormal(spawnPos);
						spawnRot = QuaternionEx.LookRotationForcedUp(spawnRot * Vector3.forward, normal);
					}
					return true;
				}
			}
		Label0:
		}
		spawnPos = Vector3.zero;
		spawnRot = Quaternion.identity;
		return false;
	}

	public ByteQuadtree.Element SampleNode()
	{
		ByteQuadtree.Element root = this.quadtree.Root;
		while (!root.IsLeaf)
		{
			root = root.RandChild;
		}
		return root;
	}

	private void UpdateCount(Spawnable spawnable, int delta)
	{
		int num;
		this.Count += delta;
		WorldSpaceGrid<int> item = this.grid;
		Vector3 spawnPosition = spawnable.SpawnPosition;
		item[spawnPosition] = item[spawnPosition] + delta;
		BaseEntity component = spawnable.GetComponent<BaseEntity>();
		if (component)
		{
			if (this.dict.TryGetValue(component.prefabID, out num))
			{
				this.dict[component.prefabID] = num + delta;
				return;
			}
			num = delta;
			this.dict.Add(component.prefabID, num);
		}
	}
}