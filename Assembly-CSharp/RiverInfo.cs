using System;
using System.Collections.Generic;
using UnityEngine;

public class RiverInfo : MonoBehaviour
{
	public RiverInfo()
	{
	}

	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.RiverObjs.Add(this);
		}
	}
}