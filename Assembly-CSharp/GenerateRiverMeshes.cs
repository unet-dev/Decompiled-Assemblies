using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRiverMeshes : ProceduralComponent
{
	public Material RiverMaterial;

	public PhysicMaterial RiverPhysicMaterial;

	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	public GenerateRiverMeshes()
	{
	}

	public override void Process(uint seed)
	{
		foreach (PathList river in TerrainMeta.Path.Rivers)
		{
			foreach (Mesh mesh in river.CreateMesh())
			{
				GameObject gameObject = new GameObject("River Mesh");
				MeshCollider riverPhysicMaterial = gameObject.AddComponent<MeshCollider>();
				riverPhysicMaterial.sharedMaterial = this.RiverPhysicMaterial;
				riverPhysicMaterial.sharedMesh = mesh;
				gameObject.AddComponent<RiverInfo>();
				gameObject.AddComponent<WaterBody>();
				gameObject.AddComponent<AddToWaterMap>();
				gameObject.tag = "River";
				gameObject.layer = 4;
				gameObject.SetHierarchyGroup(river.Name, true, false);
			}
		}
	}
}