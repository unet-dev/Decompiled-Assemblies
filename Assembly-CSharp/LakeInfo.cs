using System;
using System.Collections.Generic;
using UnityEngine;

public class LakeInfo : MonoBehaviour
{
	public LakeInfo()
	{
	}

	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.LakeObjs.Add(this);
		}
	}
}