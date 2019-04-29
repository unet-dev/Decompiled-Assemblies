using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class PlacePowerlineObjects : ProceduralComponent
{
	public PathList.BasicObject[] Start;

	public PathList.BasicObject[] End;

	public PathList.SideObject[] Side;

	[FormerlySerializedAs("PowerlineObjects")]
	public PathList.PathObject[] Path;

	public PlacePowerlineObjects()
	{
	}

	public override void Process(uint seed)
	{
		int i;
		foreach (PathList powerline in TerrainMeta.Path.Powerlines)
		{
			PathList.BasicObject[] start = this.Start;
			for (i = 0; i < (int)start.Length; i++)
			{
				powerline.TrimStart(start[i]);
			}
			start = this.End;
			for (i = 0; i < (int)start.Length; i++)
			{
				powerline.TrimEnd(start[i]);
			}
			start = this.Start;
			for (i = 0; i < (int)start.Length; i++)
			{
				powerline.SpawnStart(ref seed, start[i]);
			}
			start = this.End;
			for (i = 0; i < (int)start.Length; i++)
			{
				powerline.SpawnEnd(ref seed, start[i]);
			}
			PathList.PathObject[] path = this.Path;
			for (i = 0; i < (int)path.Length; i++)
			{
				powerline.SpawnAlong(ref seed, path[i]);
			}
			PathList.SideObject[] side = this.Side;
			for (i = 0; i < (int)side.Length; i++)
			{
				powerline.SpawnSide(ref seed, side[i]);
			}
			powerline.ResetTrims();
		}
	}
}