using System;
using System.Collections.Generic;

public class GeneratePowerlineTopology : ProceduralComponent
{
	public GeneratePowerlineTopology()
	{
	}

	public override void Process(uint seed)
	{
		foreach (PathList powerline in TerrainMeta.Path.Powerlines)
		{
			powerline.Path.RecalculateTangents();
		}
	}
}