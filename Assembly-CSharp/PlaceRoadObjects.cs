using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class PlaceRoadObjects : ProceduralComponent
{
	public PathList.BasicObject[] Start;

	public PathList.BasicObject[] End;

	[FormerlySerializedAs("RoadsideObjects")]
	public PathList.SideObject[] Side;

	[FormerlySerializedAs("RoadObjects")]
	public PathList.PathObject[] Path;

	public PlaceRoadObjects()
	{
	}

	public override void Process(uint seed)
	{
		int i;
		foreach (PathList road in TerrainMeta.Path.Roads)
		{
			PathList.BasicObject[] start = this.Start;
			for (i = 0; i < (int)start.Length; i++)
			{
				road.TrimStart(start[i]);
			}
			start = this.End;
			for (i = 0; i < (int)start.Length; i++)
			{
				road.TrimEnd(start[i]);
			}
			start = this.Start;
			for (i = 0; i < (int)start.Length; i++)
			{
				road.SpawnStart(ref seed, start[i]);
			}
			start = this.End;
			for (i = 0; i < (int)start.Length; i++)
			{
				road.SpawnEnd(ref seed, start[i]);
			}
			PathList.PathObject[] path = this.Path;
			for (i = 0; i < (int)path.Length; i++)
			{
				road.SpawnAlong(ref seed, path[i]);
			}
			PathList.SideObject[] side = this.Side;
			for (i = 0; i < (int)side.Length; i++)
			{
				road.SpawnSide(ref seed, side[i]);
			}
			road.ResetTrims();
		}
	}
}