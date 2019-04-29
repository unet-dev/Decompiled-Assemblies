using System;
using System.Collections.Generic;
using UnityEngine;

public class MapMarker : BaseEntity
{
	public static List<MapMarker> mapMarkers;

	public GameObject markerObj;

	static MapMarker()
	{
		MapMarker.mapMarkers = new List<MapMarker>();
	}

	public MapMarker()
	{
	}
}