using System;

public class GenerateWireMeshes : ProceduralComponent
{
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	public GenerateWireMeshes()
	{
	}

	public override void Process(uint seed)
	{
		TerrainMeta.Path.CreateWires();
	}
}