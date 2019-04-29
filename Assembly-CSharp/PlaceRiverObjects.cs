using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class PlaceRiverObjects : ProceduralComponent
{
	public PathList.BasicObject[] Start;

	public PathList.BasicObject[] End;

	[FormerlySerializedAs("RiversideObjects")]
	public PathList.SideObject[] Side;

	[FormerlySerializedAs("RiverObjects")]
	public PathList.PathObject[] Path;

	public PlaceRiverObjects()
	{
	}

	public override void Process(uint seed)
	{
		int i;
		foreach (PathList river in TerrainMeta.Path.Rivers)
		{
			PathList.BasicObject[] start = this.Start;
			for (i = 0; i < (int)start.Length; i++)
			{
				river.TrimStart(start[i]);
			}
			start = this.End;
			for (i = 0; i < (int)start.Length; i++)
			{
				river.TrimEnd(start[i]);
			}
			start = this.Start;
			for (i = 0; i < (int)start.Length; i++)
			{
				river.SpawnStart(ref seed, start[i]);
			}
			PathList.PathObject[] path = this.Path;
			for (i = 0; i < (int)path.Length; i++)
			{
				river.SpawnAlong(ref seed, path[i]);
			}
			PathList.SideObject[] side = this.Side;
			for (i = 0; i < (int)side.Length; i++)
			{
				river.SpawnSide(ref seed, side[i]);
			}
			start = this.End;
			for (i = 0; i < (int)start.Length; i++)
			{
				river.SpawnEnd(ref seed, start[i]);
			}
			river.ResetTrims();
		}
	}
}