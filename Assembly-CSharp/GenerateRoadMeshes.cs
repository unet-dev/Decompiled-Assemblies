using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoadMeshes : ProceduralComponent
{
	public Material RoadMaterial;

	public PhysicMaterial RoadPhysicMaterial;

	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	public GenerateRoadMeshes()
	{
	}

	public override void Process(uint seed)
	{
		foreach (PathList road in TerrainMeta.Path.Roads)
		{
			foreach (Mesh mesh in road.CreateMesh())
			{
				GameObject gameObject = new GameObject("Road Mesh");
				MeshCollider roadPhysicMaterial = gameObject.AddComponent<MeshCollider>();
				roadPhysicMaterial.sharedMaterial = this.RoadPhysicMaterial;
				roadPhysicMaterial.sharedMesh = mesh;
				gameObject.AddComponent<AddToHeightMap>();
				gameObject.layer = 16;
				gameObject.SetHierarchyGroup(road.Name, true, false);
			}
		}
	}
}