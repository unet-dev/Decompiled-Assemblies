using System;
using UnityEngine;

public class TerrainPhysics : TerrainExtension
{
	private TerrainSplatMap splat;

	private PhysicMaterial[] materials;

	public TerrainPhysics()
	{
	}

	public PhysicMaterial GetMaterial(Vector3 worldPos)
	{
		return this.materials[this.splat.GetSplatMaxIndex(worldPos, -1)];
	}

	public override void Setup()
	{
		this.splat = this.terrain.GetComponent<TerrainSplatMap>();
		this.materials = this.config.GetPhysicMaterials();
	}
}