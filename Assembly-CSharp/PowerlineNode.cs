using System;
using UnityEngine;

public class PowerlineNode : MonoBehaviour
{
	public Material WireMaterial;

	public float MaxDistance = 50f;

	public PowerlineNode()
	{
	}

	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.AddWire(this);
		}
	}
}