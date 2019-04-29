using System;

public class GenerateTerrainMesh : ProceduralComponent
{
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	public GenerateTerrainMesh()
	{
	}

	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("terrain", TerrainMeta.HeightMap.ToByteArray());
		}
		TerrainMeta.HeightMap.ApplyToTerrain();
		if (World.Cached)
		{
			TerrainMeta.HeightMap.FromByteArray(World.GetMap("height"));
		}
	}
}